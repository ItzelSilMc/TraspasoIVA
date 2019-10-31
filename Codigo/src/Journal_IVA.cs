using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CTECH.Log;
using CTECH.Acceso_a_Datos;

namespace VMXTRASPIVA
{
    public class Journal_IVA
    {
        private GJ _GJ;
        AdminGJ oAdmGJ = new AdminGJ();
        AdminGeneralJournal _oDAOGJ;
        private List<MonedaRastreo> _lstCurrencyTracking;
        public DataTable _dtVMX_IVATRASTEMP { get; set; }
        public DataTable _dtDetalla_IVA { get; set; }
        public bool esCXC;
        List<VMXCONTROLPOLIZA> lstVMXPoliza = new List<VMXCONTROLPOLIZA>();
        List<VMX_CONTOLPOLIZA_LINE> lstVMXPoliza_line = new List<VMX_CONTOLPOLIZA_LINE>();
        private string _sCuentaRedondeo;
        private bool _PG_Intercarmbiada = false;
        
        public Journal_IVA(GJ pGJ,string siteid, bool pg_intercambiada)
        {
            _GJ = pGJ;
            _PG_Intercarmbiada = pg_intercambiada;
            _GJ.CURRENCY_NATIVE = AdminFuncionalCurrency.getFunctionalCurrency(siteid);
            if (_GJ.CURRENCY_ID != _GJ.CURRENCY_NATIVE)
            {
                //Si no es moneda nativa toma el sell rate y buy rate para insertar en moneda nativa
                DataTable tc = AdminFuncionalCurrency.getFunctionalCurrencyExchange(_GJ.CURRENCY_ID, DateTime.Parse(_GJ.GJ_DATE).ToString("MM/dd/yyyy"), _GJ.SITE_ID);
                _GJ.SELL_RATE = double.Parse(tc.Rows[0]["SELL_RATE"].ToString());
                _GJ.BUY_RATE = double.Parse(tc.Rows[0]["BUY_RATE"].ToString());
                _GJ.EFFECTIVE_DATE_EXCHANGE = DateTime.Parse(tc.Rows[0]["EFFECTIVE_DATE"].ToString());
            }
            setListCurrencyTraking(siteid);
        }

        private void setListCurrencyTraking(string siteid)
        {
            AdminCurrencyTracking oCurrencyTracking = new AdminCurrencyTracking();
            _lstCurrencyTracking = oCurrencyTracking.getCurrencyTracking(_GJ.GJ_DATE, siteid);

            if (_GJ.CURRENCY_NATIVE != _GJ.CURRENCY_ID)
            {
                MonedaRastreo nativa = new MonedaRastreo();
                nativa.ID = _GJ.CURRENCY_NATIVE;
                nativa.SELL_RATE = _GJ.SELL_RATE;
                nativa.BUY_RATE = _GJ.BUY_RATE;
                nativa.EFFECTIVE_DATE = _GJ.EFFECTIVE_DATE_EXCHANGE;

                _lstCurrencyTracking.Add(nativa);
            }
        }

        public void transferTaxes()
        {
            try
            {
                _oDAOGJ = new AdminGeneralJournal();
                _oDAOGJ.crearConexiones();

                _sCuentaRedondeo = _oDAOGJ.getRoundedAccount(_GJ.SITE_ID);

                // a) GJ - General Journal
                _oDAOGJ.createGJ(_GJ);

                // b) GJ_CURR - General Journal Currecncy
                saveGJ_Curr();

                // c) GJ_DIST - General Journal Distribution
                saveGJ_Dist();

                // d) GJ_LINE - General Journal Line
                saveGJ_Line();

                // e) VMXCONTROLPOLIZA
                saveVMXCONTROLPOLIZA();

                // f) VMX_CONTOLPOLIZA_LINE
                lstVMXPoliza_line = getListVMX_CONTOLPOLIZA_LINE();

                int i = 0;
                if (esCXC)
                {
                    
                    foreach (VMX_CONTOLPOLIZA_LINE oVMXPoliza in lstVMXPoliza_line)
                    {
                        _oDAOGJ.createVMX_CONTOLPOLIZA_LINE_CXC(oVMXPoliza);
                        i++;
                    }
                }
                else
                {
                    foreach (VMX_CONTOLPOLIZA_LINE oVMXPoliza in lstVMXPoliza_line)
                    {
                        _oDAOGJ.createVMX_CONTOLPOLIZA_LINE_CXP(oVMXPoliza);
                    }
                }

                // Actualizar el nuevo número de póliza
                //oAdmGJ.setNext_GJ_ID(_GJ.SITE_ID); //Busca el ultimo id en NEXT_NUMBER_GEN de GJ y almacena el siguiente en la misma.
                _oDAOGJ.commitTransaction();
            }
            catch (Exception ex)
            {
                _oDAOGJ.rollBackTransaction();
                throw new Exception("Error al generar Poliza Contable. Detalle: " + ex.Message);
            }
            finally
            {
                _oDAOGJ.cerrarConexiones();
            }
        }

       
        #region b) GJ_CURR - General Journal Currecncy
        private void saveGJ_Curr()
        {
            List<GJ_CURR> lstGJ_CURR = new List<GJ_CURR>();

            lstGJ_CURR = getGJ_CURR();

            foreach (GJ_CURR oGJ_CURR in lstGJ_CURR)
            {
                _oDAOGJ.createGJ_CURR(oGJ_CURR);
            }
        }

        public List<GJ_CURR> getGJ_CURR()
        {
            List<GJ_CURR> lstGJ_CURR = new List<GJ_CURR>();
            GJ_CURR oGJ_CURR = new GJ_CURR();

            oGJ_CURR.GJ_ID = _GJ.ID;
            oGJ_CURR.CURRENCY_ID = _GJ.CURRENCY_ID;
            oGJ_CURR.AMOUNT = _GJ.TOTAL_DR_AMOUNT;
            oGJ_CURR.SELL_RATE = _GJ.SELL_RATE;
            oGJ_CURR.BUY_RATE = _GJ.BUY_RATE;

            lstGJ_CURR.Add(oGJ_CURR);

            if (_lstCurrencyTracking.Count > 0)
            {
                foreach (MonedaRastreo currency in _lstCurrencyTracking)
                {
                    if(currency.ID != _GJ.CURRENCY_ID)
                    {
                        GJ_CURR oGJ_CURR_Trace = new GJ_CURR();

                        oGJ_CURR_Trace.GJ_ID = _GJ.ID;
                        oGJ_CURR_Trace.CURRENCY_ID = currency.ID;
                        oGJ_CURR_Trace.AMOUNT = Math.Round((_GJ.TOTAL_DR_AMOUNT / (currency.ID!= _GJ.CURRENCY_NATIVE ? currency.SELL_RATE : currency.BUY_RATE)), 2);
                        oGJ_CURR_Trace.SELL_RATE = currency.ID != _GJ.CURRENCY_NATIVE ? currency.SELL_RATE : 1;
                        oGJ_CURR_Trace.BUY_RATE = currency.ID != _GJ.CURRENCY_NATIVE ? currency.BUY_RATE : 1;

                        lstGJ_CURR.Add(oGJ_CURR_Trace);
                    }                   
                }
            }

            return lstGJ_CURR;
        }
        #endregion

        #region c) GJ_DIST - General Journal Distribution
        private void saveGJ_Dist()
        {
            List<GJ_DIST> lstGJ_DIST = new List<GJ_DIST>();
            List<GJ_DIST> lstGJ_DIST_Agrupada = new List<GJ_DIST>();

            if (esCXC)
            {
                lstGJ_DIST = getListGJ_DIST();
            }
            else
            {
                lstGJ_DIST = getListGJ_DIST_CXP();
            }
            
            var prodQuery = from prod in lstGJ_DIST
                            group prod by new
                            {
                                prod.GL_ACCOUNT_ID,
                                prod.CURRENCY_ID,
                                prod.AMOUNT_TYPE //se agregó para que no sumarice por cuenta, si no tambien por tipo de transacción
                            }
                                into grouping
                                select new { grouping.Key, grouping };
            int entry = 0;

            foreach (var grp in prodQuery)
            {
                double amount = 0;
                GJ_DIST oGJ_Dist = new GJ_DIST();

                foreach (GJ_DIST item in grp.grouping)
                {
                    oGJ_Dist = item;
                    amount += item.AMOUNT;
                }

                if (oGJ_Dist.AMOUNT > 0)
                {
                    oGJ_Dist.ENTRY_NO = string.Format("{0}", entry);
                    oGJ_Dist.AMOUNT = amount;

                    lstGJ_DIST_Agrupada.Add(oGJ_Dist);
                    entry++;
                }
            }

            foreach (GJ_DIST oGJ_DIST in lstGJ_DIST_Agrupada)
            {
                _oDAOGJ.createGJ_DIST(oGJ_DIST);
            }
        }

        public List<GJ_DIST> getListGJ_DIST_CXP()
        {
            List<GJ_DIST> lstGJ_DIST = new List<GJ_DIST>();
            int iEntry_No = 0;
            double dMontoFactura = 0;
            double dMontoPago = 0;
            double dMonto_tipoCambio = 0;
            double dIVA_Pagar = 0.0;
            double dIVA_Pagado = 0.0;
            double dMontoRastreableFactura = 0.0;
            double dMontoRastreablePago = 0.0;
            double dPerdida = 0.0;
            double dGanancia = 0.0;
            double dRedondeo = 0;
            
            string retencion = "";
            double dRET_Pagar = 0.0;
            double dRET_Pagado = 0.0;

            if (Global.TipoCambioPagos.Equals("FACTURA"))
            {
                //no tenemos clientes que usen este método
                foreach (DataRow item in _dtVMX_IVATRASTEMP.Rows)
                {
                    GJ_DIST oGJ_DIST = new GJ_DIST();

                    dMontoFactura = double.Parse(item["MONTO"].ToString());
                    retencion = item["RETENCION"].ToString();

                    //if (_PG_Intercarmbiada)
                    {
                        //Si es IVA la cuenta de ORIGEN se va a crédito, si es retención se va a débito
                        oGJ_DIST = !bool.Parse(retencion) ? getGJ_DIST(iEntry_No++, dMontoFactura, "CR", item["CUENTA_ORIGEN"].ToString(), _GJ.CURRENCY_ID, "Y") : getGJ_DIST(iEntry_No++, dMontoFactura, "DR", item["CUENTA_ORIGEN"].ToString(), _GJ.CURRENCY_ID, "Y");
                        //Si es IVA la cuenta DESTINO se va a débito, si es retención se va a crédito
                        oGJ_DIST = !bool.Parse(retencion) ? getGJ_DIST(iEntry_No++, dMontoFactura, "DR", item["CUENTA_DESTINO"].ToString(), _GJ.CURRENCY_ID, "Y") : getGJ_DIST(iEntry_No++, dMontoFactura, "CR", item["CUENTA_DESTINO"].ToString(), _GJ.CURRENCY_ID, "Y");
                    }
                    //else
                    //{
                    //    //Si es IVA la cuenta de ORIGEN se va a débito, si es retención se va a crédito
                    //    oGJ_DIST = !bool.Parse(retencion) ? getGJ_DIST(iEntry_No++, dMontoFactura, "DR", item["CUENTA_ORIGEN"].ToString(), _GJ.CURRENCY_ID, "Y") : getGJ_DIST(iEntry_No++, dMontoFactura, "CR", item["CUENTA_ORIGEN"].ToString(), _GJ.CURRENCY_ID, "Y");
                    //    //Si es IVA la cuenta DESTINO se va a crédito, si es retención se va a débito
                    //    oGJ_DIST = !bool.Parse(retencion) ? getGJ_DIST(iEntry_No++, dMontoFactura, "CR", item["CUENTA_DESTINO"].ToString(), _GJ.CURRENCY_ID, "Y") : getGJ_DIST(iEntry_No++, dMontoFactura, "DR", item["CUENTA_DESTINO"].ToString(), _GJ.CURRENCY_ID, "Y");
                    //}
                    //oGJ_DIST = getGJ_DIST(iEntry_No++, dMontoFactura, "CR", item["CUENTA_ORIGEN"].ToString(), _GJ.CURRENCY_ID, "Y");
                    //oGJ_DIST = getGJ_DIST(iEntry_No++, dMontoFactura, "DR", item["CUENTA_DESTINO"].ToString(), _GJ.CURRENCY_ID, "Y");

                    lstGJ_DIST.Add(oGJ_DIST);
                    lstGJ_DIST.Add(oGJ_DIST);
                }

                // Monedas rastreables
                foreach (DataRow item in _dtVMX_IVATRASTEMP.Rows)
                {
                    foreach (MonedaRastreo moneda in _lstCurrencyTracking)
                    {
                        GJ_DIST oGJ_DIST_TRACK = new GJ_DIST();

                        dMontoFactura = double.Parse(item["MONTO"].ToString());
                        dMonto_tipoCambio = Math.Round((dMontoFactura / moneda.SELL_RATE), 2);
                        retencion = item["RETENCION"].ToString();

                        //if (_PG_Intercarmbiada)
                        {
                            //Si es IVA la cuenta de ORIGEN se va a crédito, si es retención se va a débito
                            oGJ_DIST_TRACK = !bool.Parse(retencion) ? getGJ_DIST(iEntry_No++, dMonto_tipoCambio, "CR", item["CUENTA_ORIGEN"].ToString(), moneda.ID, "N") : getGJ_DIST(iEntry_No++, dMonto_tipoCambio, "DR", item["CUENTA_ORIGEN"].ToString(), moneda.ID, "N");
                            lstGJ_DIST.Add(oGJ_DIST_TRACK);
                            //Si es IVA la cuenta DESTINO se va a débito, si es retención se va a crédito
                            oGJ_DIST_TRACK = !bool.Parse(retencion) ? getGJ_DIST(iEntry_No++, dMonto_tipoCambio, "DR", item["CUENTA_DESTINO"].ToString(), moneda.ID, "N") : getGJ_DIST(iEntry_No++, dMonto_tipoCambio, "CR", item["CUENTA_DESTINO"].ToString(), moneda.ID, "N");
                            lstGJ_DIST.Add(oGJ_DIST_TRACK);
                        }
                        //else
                        //{
                        //    //Si es IVA la cuenta de ORIGEN se va a débito, si es retención se va a crédito
                        //    oGJ_DIST_TRACK = !bool.Parse(retencion) ? getGJ_DIST(iEntry_No++, dMonto_tipoCambio, "DR", item["CUENTA_ORIGEN"].ToString(), moneda.ID, "N") : getGJ_DIST(iEntry_No++, dMonto_tipoCambio, "CR", item["CUENTA_ORIGEN"].ToString(), moneda.ID, "N");
                        //    lstGJ_DIST.Add(oGJ_DIST_TRACK);
                        //    //Si es IVA la cuenta DESTINO se va a crédito, si es retención se va a débito
                        //    oGJ_DIST_TRACK = !bool.Parse(retencion) ? getGJ_DIST(iEntry_No++, dMonto_tipoCambio, "CR", item["CUENTA_DESTINO"].ToString(), moneda.ID, "N") : getGJ_DIST(iEntry_No++, dMonto_tipoCambio, "DR", item["CUENTA_DESTINO"].ToString(), moneda.ID, "N");
                        //    lstGJ_DIST.Add(oGJ_DIST_TRACK);
                        //}
                       
                        //oGJ_DIST_TRACK = getGJ_DIST(iEntry_No++, dMonto_tipoCambio, "CR", item["CUENTA_ORIGEN"].ToString(), moneda.ID, "N");
                        //oGJ_DIST_TRACK = getGJ_DIST(iEntry_No++, dMonto_tipoCambio, "DR", item["CUENTA_DESTINO"].ToString(), moneda.ID, "N");
                        
                    }
                }
            }
            else
            {
                // 1 - Moneda del sistema / elegida
                foreach (DataRow item in _dtVMX_IVATRASTEMP.Rows)
                {
                    GJ_DIST oGJ_DIST = new GJ_DIST();

                    dMontoFactura = double.Parse(item["MONTO"].ToString());
                    dMontoPago = double.Parse(item["MONTO_IVA_DEPOSITO"].ToString());
                    retencion = item["RETENCION"].ToString().Trim();

                    //Si es IVA la cuenta de ORIGEN se va a crédito, si es retención se va a débito
                    oGJ_DIST = !bool.Parse(retencion) ? getGJ_DIST(iEntry_No++, dMontoFactura, "CR", item["CUENTA_ORIGEN"].ToString(), _GJ.CURRENCY_ID, "Y") : getGJ_DIST(iEntry_No++, dMontoFactura, "DR", item["CUENTA_ORIGEN"].ToString(), _GJ.CURRENCY_ID, "Y");
                    //oGJ_DIST = getGJ_DIST(iEntry_No++, dMontoFactura, "CR", item["CUENTA_ORIGEN"].ToString(), _GJ.CURRENCY_ID, "Y"); //previo a retenciones
                    lstGJ_DIST.Add(oGJ_DIST);

                    //Si es IVA la cuenta DESTINO se va a débito, si es retención se va a crédito
                    oGJ_DIST = !bool.Parse(retencion) ? getGJ_DIST(iEntry_No++, dMontoPago, "DR", item["CUENTA_DESTINO"].ToString(), _GJ.CURRENCY_ID, "Y") : getGJ_DIST(iEntry_No++, dMontoPago, "CR", item["CUENTA_DESTINO"].ToString(), _GJ.CURRENCY_ID, "Y");
                    //oGJ_DIST = getGJ_DIST(iEntry_No++, dMontoPago, "DR", item["CUENTA_DESTINO"].ToString(), _GJ.CURRENCY_ID, "Y"); //previo a retenciones
                    lstGJ_DIST.Add(oGJ_DIST);


                    // Perdida Ganancia
                    PerdidaGanancia oPG = new PerdidaGanancia();
                    string sCta_Origen = item["CUENTA_ORIGEN"].ToString();
                    string sCta_Destino = item["CUENTA_DESTINO"].ToString();

                    oPG = oPG.getPerdidaGanancia(_dtDetalla_IVA, sCta_Origen, sCta_Destino);

                    if (oPG.Ganancia > 0)
                    {
                        //si entra en ganancia es positivo, un positivo se traduce como pérdida en retención aunque la variable sea la de ganancia

                        if (_PG_Intercarmbiada)
                        {
                            //Si es IVA la ganancia va a crédito, si es retencion la ganancia va a débito (v12)
                            oGJ_DIST = !bool.Parse(retencion) ? getGJ_DIST(iEntry_No++, oPG.Ganancia, "CR", oPG.CuentaGanancia, _GJ.CURRENCY_ID, "Y") : getGJ_DIST(iEntry_No++, oPG.Ganancia, "DR", oPG.CuentaGanancia, _GJ.CURRENCY_ID, "Y");
                        }
                        else
                        {
                            // Si es IVA la ganancia va a débito, si es retencion la ganancia va a credito (v11)
                            oGJ_DIST = !bool.Parse(retencion) ? getGJ_DIST(iEntry_No++, oPG.Ganancia, "DR", oPG.CuentaGanancia, _GJ.CURRENCY_ID, "Y") : getGJ_DIST(iEntry_No++, oPG.Ganancia, "CR", oPG.CuentaGanancia, _GJ.CURRENCY_ID, "Y");
                        }

                         if(oGJ_DIST!=null) lstGJ_DIST.Add(oGJ_DIST);
                    }

                    if (oPG.Perdida > 0)
                    {
                        //si entra en pérdida es negativo, un negativo se traduce como ganancia en retención aunque la variable sea la de pérdida
                                                
                        if (_PG_Intercarmbiada)
                        {
                            //Si es IVA la pérdida va a débito, si es retencion va a crédito (v12)
                            oGJ_DIST = !bool.Parse(retencion) ? getGJ_DIST(iEntry_No++, oPG.Perdida, "DR", oPG.CuentaPerdida, _GJ.CURRENCY_ID, "Y") : getGJ_DIST(iEntry_No++, oPG.Perdida, "CR", oPG.CuentaPerdida, _GJ.CURRENCY_ID, "Y");
                        }
                        else
                        {
                            //Si es IVA la pérdida va a crédito, si es retencion va a débito (v11)
                            oGJ_DIST = !bool.Parse(retencion) ? getGJ_DIST(iEntry_No++, oPG.Perdida, "CR", oPG.CuentaPerdida, _GJ.CURRENCY_ID, "Y") : getGJ_DIST(iEntry_No++, oPG.Perdida, "DR", oPG.CuentaPerdida, _GJ.CURRENCY_ID, "Y");
                        }
                        if (oGJ_DIST != null)  lstGJ_DIST.Add(oGJ_DIST);
                    }
                }

                double totalDebitos = 0.0;
                double totalCreditos = 0.0;
                string TIPO = string.Empty;

                foreach (GJ_DIST item in lstGJ_DIST)
                {
                    TIPO = item.AMOUNT_TYPE;

                    if (TIPO == "CR")
                    {
                        totalCreditos = Math.Round(totalCreditos + item.AMOUNT, 2);
                    }
                    else
                    {
                        totalDebitos = Math.Round(totalDebitos + item.AMOUNT, 2);
                    }   
                }


                // Cuadrar totalCreditos & totalDebitos por menos de 1 peso de diferencia
                if (totalCreditos != totalDebitos)
                {

                    // Se hace la asignación vacia de la variable que va a cuadrar las diferencias
                    double Diferencia_CD = 0.00;

                    // Se crea el objeto GJ_DIST()
                    GJ_DIST oGJ_LINE = new GJ_DIST();

                    // Se obtiene la diferencia entre las variables totalCreditos & totalDebitos
                    if (totalCreditos > totalDebitos) 
                    {
                        Diferencia_CD = Math.Round(totalCreditos - totalDebitos, 2);
                    }
                    else 
                    {
                        Diferencia_CD = Math.Round(totalDebitos - totalCreditos, 2);
                    }

                    // Se cuadra la diferencia
                    if (Diferencia_CD <= 1) 
                    {
                        if (totalCreditos > totalDebitos)
                        {
                            //
                            oGJ_LINE = getGJ_DIST(iEntry_No++, Diferencia_CD, "DR", _sCuentaRedondeo, _GJ.CURRENCY_ID, "Y");
                            lstGJ_DIST.Add(oGJ_LINE);
                        }
                        else 
                        {
                            //
                            oGJ_LINE = getGJ_DIST(iEntry_No++, Diferencia_CD, "CR", _sCuentaRedondeo, _GJ.CURRENCY_ID, "Y");
                            lstGJ_DIST.Add(oGJ_LINE);
                        }
                    }
                    else 
                    {
                        throw new Exception("La diferencia de las polizas es mayor al limite establecido. Diferencia: " + Diferencia_CD);
                    }
                }
              
                // 2- Monedas rastreables
                GJ_DIST oGJ_DIST_TRACK;

                foreach (MonedaRastreo moneda in _lstCurrencyTracking)
                {
                    if (moneda.ID != _GJ.CURRENCY_ID) //Si la moneda rastreable no es la elegida
                    {

                        dIVA_Pagar = 0;
                        dIVA_Pagado = 0;
                        dRET_Pagar = 0;
                        dRET_Pagado = 0;

                        foreach (DataRow item in _dtVMX_IVATRASTEMP.Rows)
                        {
                            oGJ_DIST_TRACK = new GJ_DIST();
                            dGanancia = 0;
                            dPerdida = 0;
                            dMontoFactura = double.Parse(item["MONTO"].ToString());
                            dMontoPago = double.Parse(item["MONTO_IVA_DEPOSITO"].ToString());
                            retencion = item["RETENCION"].ToString().Trim();

                            dMontoRastreableFactura = Math.Round((dMontoFactura / (moneda.ID != _GJ.CURRENCY_NATIVE ? moneda.SELL_RATE : moneda.BUY_RATE)), 2);

                            //Si es IVA la cuenta de ORIGEN se va a crédito, si es retención se va a débito
                            oGJ_DIST_TRACK = !bool.Parse(retencion) ? getGJ_DIST(iEntry_No++, dMontoRastreableFactura, "CR", item["CUENTA_ORIGEN"].ToString(), moneda.ID, "N") : getGJ_DIST(iEntry_No++, dMontoRastreableFactura, "DR", item["CUENTA_ORIGEN"].ToString(), moneda.ID, "N"); //antes CR
                            //oGJ_DIST_TRACK = getGJ_DIST(iEntry_No++, dMontoRastreableFactura, "CR", item["CUENTA_ORIGEN"].ToString(), moneda.ID, "N");

                            lstGJ_DIST.Add(oGJ_DIST_TRACK);

                            dMontoRastreablePago = Math.Round((dMontoPago / (moneda.ID != _GJ.CURRENCY_NATIVE ? moneda.SELL_RATE : moneda.BUY_RATE)), 2);

                            //Si es IVA la cuenta DESTINO se va a débito, si es retención se va a crédito
                            oGJ_DIST_TRACK = !bool.Parse(retencion) ? getGJ_DIST(iEntry_No++, dMontoRastreablePago, "DR", item["CUENTA_DESTINO"].ToString(), moneda.ID, "N") : getGJ_DIST(iEntry_No++, dMontoRastreablePago, "CR", item["CUENTA_DESTINO"].ToString(), moneda.ID, "N"); //antes DR
                            //oGJ_DIST_TRACK = getGJ_DIST(iEntry_No++, dMontoRastreablePago, "DR", item["CUENTA_DESTINO"].ToString(), moneda.ID, "N");

                            lstGJ_DIST.Add(oGJ_DIST_TRACK);

                            // Perdida Ganancia
                            PerdidaGanancia oPG = new PerdidaGanancia();
                            string sCta_Origen = item["CUENTA_ORIGEN"].ToString();
                            string sCta_Destino = item["CUENTA_DESTINO"].ToString();

                            oPG = oPG.getPerdidaGanancia(_dtDetalla_IVA, sCta_Origen, sCta_Destino);

                            if (oPG.Ganancia > 0)
                            {
                                //si entra en ganancia es positivo, un positivo se traduce como pérdida en retención aunque la variable sea la de ganancia
                                //la retención siempre va al movimiento inverso del iva

                                dGanancia = Math.Round((oPG.Ganancia / (moneda.ID != _GJ.CURRENCY_NATIVE ? moneda.SELL_RATE : moneda.BUY_RATE)), 2);

                                if (_PG_Intercarmbiada)
                                    //Si es IVA la ganancia va a crédito (v12)
                                    oGJ_DIST_TRACK = !bool.Parse(retencion) ? getGJ_DIST(iEntry_No++, dGanancia, "CR", oPG.CuentaGanancia, moneda.ID, "N") : getGJ_DIST(iEntry_No++, dGanancia, "DR", oPG.CuentaGanancia, moneda.ID, "N");
                                else 
                                    //Si es IVA la ganancia va a débito, si es retención a crédito (v11)
                                    oGJ_DIST_TRACK = !bool.Parse(retencion) ? getGJ_DIST(iEntry_No++, dGanancia, "DR", oPG.CuentaGanancia, moneda.ID, "N") : getGJ_DIST(iEntry_No++, dGanancia, "CR", oPG.CuentaGanancia, moneda.ID, "N");

                                
                                lstGJ_DIST.Add(oGJ_DIST_TRACK);
                            }

                            if (oPG.Perdida > 0)
                            {
                                //si entra en pérdida es negativo, un negativo se traduce como ganancia en retención aunque la variable sea la de pérdida

                                dPerdida = Math.Round((oPG.Perdida / (moneda.ID != _GJ.CURRENCY_NATIVE ? moneda.SELL_RATE : moneda.BUY_RATE)), 2);

                                if (_PG_Intercarmbiada)
                                    //Si es IVA la pérdida va a débito (v12)
                                    oGJ_DIST_TRACK = !bool.Parse(retencion) ? getGJ_DIST(iEntry_No++, dPerdida, "DR", oPG.CuentaPerdida, moneda.ID, "N") : getGJ_DIST(iEntry_No++, dPerdida, "CR", oPG.CuentaPerdida, moneda.ID, "N");

                                else
                                    //Si es IVA la pérdida va a crédito, retención a débito (v11)
                                    oGJ_DIST_TRACK = !bool.Parse(retencion) ? getGJ_DIST(iEntry_No++, dPerdida, "CR", oPG.CuentaPerdida, moneda.ID, "N"): getGJ_DIST(iEntry_No++, dPerdida, "DR", oPG.CuentaPerdida, moneda.ID, "N");


                                lstGJ_DIST.Add(oGJ_DIST_TRACK);
                            }

                            if (!bool.Parse(retencion))
                            {
                                // IVA PAGADO = Total IVA Factura + Perdida
                                dIVA_Pagado += (dMontoRastreableFactura +(_PG_Intercarmbiada ? dGanancia : dPerdida)); //CR antes -> //DR ahora

                                // IVA POR PAGAR = Total IVA Pago + Ganancia
                                dIVA_Pagar += (dMontoRastreablePago + (_PG_Intercarmbiada ? dPerdida : dGanancia)); //DR antes -> //CR ahora
                            }
                            else
                            {
                                // RETENCION PAGADA
                                dRET_Pagado += (dMontoRastreableFactura + (_PG_Intercarmbiada ? dGanancia : dPerdida)); //DR antes -> CR ahora, no genera la poliza

                                //RETENCION POR PAGAR
                                dRET_Pagar += (dMontoRastreablePago + (_PG_Intercarmbiada ? dPerdida : dGanancia)); //CR antes -> DR ahora, no genera la poliza
                            }

                        }

                        //REDONDEO
                    
                        //Variables que suman el redondeo de IVA y retenciones
                        double dCR = 0.0;
                        double dDR = 0.0;

                        //IVA
                        if (dIVA_Pagado > dIVA_Pagar) // CR > DR ->
                        {
                            dRedondeo = Math.Round(Math.Abs(dIVA_Pagado - dIVA_Pagar), 2);
                            if (dRedondeo > 0)
                            {
                                dDR = dRedondeo;
                                //oGJ_DIST_TRACK = getGJ_DIST(iEntry_No++, dRedondeo, "DR", _sCuentaRedondeo, moneda.ID, "N");
                                //lstGJ_DIST.Add(oGJ_DIST_TRACK);
                            }
                        }
                        else if (dIVA_Pagado < dIVA_Pagar) // CR < DR ->
                        {
                            dRedondeo = Math.Round(Math.Abs(dIVA_Pagar - dIVA_Pagado), 2);
                            if (dRedondeo > 0)
                            {
                                dCR = dRedondeo;
                                //oGJ_DIST_TRACK = getGJ_DIST(iEntry_No++, dRedondeo, "CR", _sCuentaRedondeo, moneda.ID, "N");
                                //lstGJ_DIST.Add(oGJ_DIST_TRACK);
                            }
                        }

                        //RETENCION
                        if(dRET_Pagado > dRET_Pagar) // DR > CR ->
                        {
                            dRedondeo = Math.Round(Math.Abs(dRET_Pagado - dRET_Pagar), 2);
                            if (dRedondeo > 0) dCR += dRedondeo;
                        }
                        else if (dRET_Pagado < dRET_Pagar) // DR < CR ->
                        {
                            dRedondeo = Math.Round(Math.Abs(dRET_Pagar - dRET_Pagado), 2);
                            if (dRedondeo > 0) dDR += dRedondeo;
                        }

                        if (dDR > 0)
                        {
                            oGJ_DIST_TRACK = getGJ_DIST(iEntry_No++, dDR, "DR", _sCuentaRedondeo, moneda.ID, "N"); //->
                            lstGJ_DIST.Add(oGJ_DIST_TRACK);
                        }

                        if (dCR > 0)
                        {
                            oGJ_DIST_TRACK = getGJ_DIST(iEntry_No++, dCR, "CR", _sCuentaRedondeo, moneda.ID, "N"); //->
                            lstGJ_DIST.Add(oGJ_DIST_TRACK);
                        }
                    }
                }
            }

            return lstGJ_DIST;
        }


        public List<GJ_DIST> getListGJ_DIST()
        {
            List<GJ_DIST> lstGJ_DIST = new List<GJ_DIST>();
            int iEntry_No = 0;
            double dMontoFactura = 0;
            double dMontoPago = 0;
            double dMonto_tipoCambio = 0;
            double dIVA_Pagar = 0.0;
            double dIVA_Pagado = 0.0;
            double dMontoRastreableFactura = 0.0;
            double dMontoRastreablePago = 0.0;
            double dPerdida = 0.0;
            double dGanancia = 0.0;
            double dRedondeo = 0.0;

            if (Global.TipoCambioPagos.Equals("FACTURA"))
            {
                foreach (DataRow item in _dtVMX_IVATRASTEMP.Rows)
                {
                    GJ_DIST oGJ_DIST = new GJ_DIST();

                    dMontoFactura = double.Parse(item["MONTO"].ToString());

                    oGJ_DIST = getGJ_DIST(iEntry_No++, dMontoFactura, "DR", item["CUENTA_ORIGEN"].ToString(), _GJ.CURRENCY_ID, "Y");
                    lstGJ_DIST.Add(oGJ_DIST);

                    oGJ_DIST = getGJ_DIST(iEntry_No++, dMontoFactura, "CR", item["CUENTA_DESTINO"].ToString(), _GJ.CURRENCY_ID, "Y");
                    lstGJ_DIST.Add(oGJ_DIST);
                }

                // Monedas rastreables
                foreach (DataRow item in _dtVMX_IVATRASTEMP.Rows)
                {
                    foreach (MonedaRastreo moneda in _lstCurrencyTracking)
                    {
                        GJ_DIST oGJ_DIST_TRACK = new GJ_DIST();

                        dMontoFactura = double.Parse(item["MONTO"].ToString());
                        dMonto_tipoCambio = Math.Round((dMontoFactura / moneda.SELL_RATE), 2);

                        oGJ_DIST_TRACK = getGJ_DIST(iEntry_No++, dMonto_tipoCambio, "DR", item["CUENTA_ORIGEN"].ToString(), moneda.ID, "N");
                        lstGJ_DIST.Add(oGJ_DIST_TRACK);

                        oGJ_DIST_TRACK = getGJ_DIST(iEntry_No++, dMonto_tipoCambio, "CR", item["CUENTA_DESTINO"].ToString(), moneda.ID, "N");
                        lstGJ_DIST.Add(oGJ_DIST_TRACK);
                    }
                }
            }
            else
            {
                // 1 - Moneda del sistema / elegida
                foreach (DataRow item in _dtVMX_IVATRASTEMP.Rows)
                {
                    GJ_DIST oGJ_DIST = new GJ_DIST();

                    dMontoFactura = double.Parse(item["MONTO"].ToString()); 
                    dMontoPago = double.Parse(item["MONTO_IVA_DEPOSITO"].ToString());

                    // Cuadrar dMontoFactura & dMontoPago por menos de 1 peso de diferencia
                    if (dMontoFactura != dMontoPago) 
                    {
                        // Se hace la asignación vacia de la variable que va a cuadrar las diferencias
                        double Diferencia_FP = 0.00;
                                             
                        // Se obtiene la diferencia entre las variables dMontoFactura & dMontoPagos
                        if (dMontoFactura > dMontoPago) 
                        {
                            Diferencia_FP = Math.Round(dMontoFactura - dMontoPago, 2);
                        }
                        else 
                        {
                            Diferencia_FP = Math.Round(dMontoPago - dMontoFactura, 2);
                        }

                        // Se cuadra la diferencia
                        if (Diferencia_FP <= 1) 
                        {
                            if (dMontoFactura > dMontoPago) 
                            {
                                dMontoPago = dMontoPago + Diferencia_FP;  
                            }
                            else 
                            {
                                dMontoFactura = dMontoFactura + Diferencia_FP;
                            }
                        }
                        //else {
                        //    throw new Exception("La diferencia entre MontoFactura & MontoPago es mayor al limite establecido. Diferencia: " + Diferencia_FP);
                        //}
                    }

                    
                    oGJ_DIST = getGJ_DIST(iEntry_No++, dMontoFactura, "DR", item["CUENTA_ORIGEN"].ToString(), _GJ.CURRENCY_ID, "Y");
                    lstGJ_DIST.Add(oGJ_DIST);

                    oGJ_DIST = getGJ_DIST(iEntry_No++, dMontoPago, "CR", item["CUENTA_DESTINO"].ToString(), _GJ.CURRENCY_ID, "Y");
                    lstGJ_DIST.Add(oGJ_DIST);

                    // Perdida Ganancia
                    PerdidaGanancia oPG = new PerdidaGanancia();
                    string sCta_Origen = item["CUENTA_ORIGEN"].ToString();
                    string sCta_Destino = item["CUENTA_DESTINO"].ToString();

                    oPG = oPG.getPerdidaGanancia(_dtDetalla_IVA, sCta_Origen, sCta_Destino);

                    if (oPG.Ganancia > 0)
                    {
                        oGJ_DIST = getGJ_DIST(iEntry_No++, oPG.Ganancia, "DR", oPG.CuentaGanancia, _GJ.CURRENCY_ID, "Y");
                        lstGJ_DIST.Add(oGJ_DIST);
                    }

                    if (oPG.Perdida > 0)
                    {
                        oGJ_DIST = getGJ_DIST(iEntry_No++, oPG.Perdida, "CR", oPG.CuentaPerdida, _GJ.CURRENCY_ID, "Y");
                        lstGJ_DIST.Add(oGJ_DIST);
                    }
                }

                // 2- Monedas rastreables 
                GJ_DIST oGJ_DIST_TRACK;

                foreach (MonedaRastreo moneda in _lstCurrencyTracking)
                {
                    if(moneda.ID != _GJ.CURRENCY_ID) //Si la moneda rastreable no es la elegida
                    {
                        dIVA_Pagar = 0;
                        dIVA_Pagado = 0;

                        foreach (DataRow item in _dtVMX_IVATRASTEMP.Rows)
                        {
                            oGJ_DIST_TRACK = new GJ_DIST();

                            dMontoFactura = double.Parse(item["MONTO"].ToString());
                            dMontoPago = double.Parse(item["MONTO_IVA_DEPOSITO"].ToString());
                            dGanancia = 0;
                            dPerdida = 0;

                            // Cuadrar dMontoFactura & dMontoPago por menos de 1 peso de diferencia
                            if (dMontoFactura != dMontoPago)
                            {
                                // Se hace la asignación vacia de la variable que va a cuadrar las diferencias
                                double Diferencia_FP = 0.00;

                                // Se obtiene la diferencia entre las variables dMontoFactura & dMontoPagos
                                if (dMontoFactura > dMontoPago)
                                {
                                    Diferencia_FP = Math.Round(dMontoFactura - dMontoPago, 2);
                                }
                                else
                                {
                                    Diferencia_FP = Math.Round(dMontoPago - dMontoFactura, 2);
                                }

                                // Se cuadra la diferencia
                                if (Diferencia_FP <= 1)
                                {
                                    if (dMontoFactura > dMontoPago)
                                    {
                                        dMontoPago = dMontoPago + Diferencia_FP;
                                    }
                                    else
                                    {
                                        dMontoFactura = dMontoFactura + Diferencia_FP;
                                    }
                                }
                                //else {
                                //    throw new Exception("La diferencia entre MontoFactura & MontoPago es mayor al limite establecido. Diferencia: " + Diferencia_FP);
                                //}
                            }

                            if (!(dMontoFactura > 0) && !(dMontoPago > 0))
                            {
                                continue;
                            }

                            dMontoRastreableFactura = Math.Round((dMontoFactura / (moneda.ID != _GJ.CURRENCY_NATIVE ? moneda.SELL_RATE : moneda.BUY_RATE)), 2);
                            oGJ_DIST_TRACK = getGJ_DIST(iEntry_No++, dMontoRastreableFactura, "DR", item["CUENTA_ORIGEN"].ToString(), moneda.ID, "N");
                            lstGJ_DIST.Add(oGJ_DIST_TRACK);

                            dMontoRastreablePago = Math.Round((dMontoPago / (moneda.ID != _GJ.CURRENCY_NATIVE ? moneda.SELL_RATE : moneda.BUY_RATE)), 2);
                            oGJ_DIST_TRACK = getGJ_DIST(iEntry_No++, dMontoRastreablePago, "CR", item["CUENTA_DESTINO"].ToString(), moneda.ID, "N");
                            lstGJ_DIST.Add(oGJ_DIST_TRACK);

                            // Perdida Ganancia
                            PerdidaGanancia oPG = new PerdidaGanancia();
                            string sCta_Origen = item["CUENTA_ORIGEN"].ToString();
                            string sCta_Destino = item["CUENTA_DESTINO"].ToString();

                            oPG = oPG.getPerdidaGanancia(_dtDetalla_IVA, sCta_Origen, sCta_Destino);

                            if (oPG.Ganancia > 0)
                            {
                                dGanancia = Math.Round((oPG.Ganancia / (moneda.ID != _GJ.CURRENCY_NATIVE ? moneda.SELL_RATE : moneda.BUY_RATE)), 2);
                                oGJ_DIST_TRACK = getGJ_DIST(iEntry_No++, dGanancia, "DR", oPG.CuentaGanancia, moneda.ID, "N");
                                lstGJ_DIST.Add(oGJ_DIST_TRACK);
                            }

                            if (oPG.Perdida > 0)
                            {
                                dPerdida = Math.Round((oPG.Perdida / (moneda.ID != _GJ.CURRENCY_NATIVE ? moneda.SELL_RATE : moneda.BUY_RATE)), 2);
                                oGJ_DIST_TRACK = getGJ_DIST(iEntry_No++, dPerdida, "CR", oPG.CuentaPerdida, moneda.ID, "N");
                                lstGJ_DIST.Add(oGJ_DIST_TRACK);
                            }

                            // IVA POR PAGAR = Total IVA Factura + Perdida
                            dIVA_Pagar += (dMontoRastreableFactura + dGanancia);

                            // IVA PAGADO = Total IVA Pago + Ganancia
                            dIVA_Pagado += (dMontoRastreablePago + dPerdida);
                        }

                        if (dIVA_Pagado > dIVA_Pagar)
                        {
                            dRedondeo = Math.Round(Math.Abs(dIVA_Pagado - dIVA_Pagar), 2);

                            if (dRedondeo > 0)
                            {
                                oGJ_DIST_TRACK = getGJ_DIST(iEntry_No++, dRedondeo, "DR", _sCuentaRedondeo, moneda.ID, "N");
                                lstGJ_DIST.Add(oGJ_DIST_TRACK);
                            }
                        }
                        else if (dIVA_Pagado < dIVA_Pagar)
                        {
                            dRedondeo = Math.Round(Math.Abs(dIVA_Pagar - dIVA_Pagado), 2);

                            if (dRedondeo > 0)
                            {
                                oGJ_DIST_TRACK = getGJ_DIST(iEntry_No++, dRedondeo, "CR", _sCuentaRedondeo, moneda.ID, "N");
                                lstGJ_DIST.Add(oGJ_DIST_TRACK);
                            }
                        }
                    }
                }
            }

            return lstGJ_DIST;
        }

        private GJ_DIST getGJ_DIST(int pEntry_No, double pAmount, string pAmountType, string sAccount, string pCurrency, string pNative)
        {
            GJ_DIST oGJ_DIST_TRACK = new GJ_DIST();

            oGJ_DIST_TRACK.GJ_ID = _GJ.ID;
            oGJ_DIST_TRACK.DIST_NO = "1";
            oGJ_DIST_TRACK.ENTRY_NO = pEntry_No.ToString();
            oGJ_DIST_TRACK.AMOUNT = pAmount;
            oGJ_DIST_TRACK.AMOUNT_TYPE = pAmountType;
            oGJ_DIST_TRACK.GL_ACCOUNT_ID = sAccount;
            oGJ_DIST_TRACK.NATIVE_AMOUNT = "0";
            oGJ_DIST_TRACK.POSTING_DATE = _GJ.POSTING_DATE;
            oGJ_DIST_TRACK.POSTING_STATUS = "U";
            oGJ_DIST_TRACK.CREATE_DATE = _GJ.CREATE_DATE;
            oGJ_DIST_TRACK.SITE_ID = _GJ.SITE_ID;
            oGJ_DIST_TRACK.CURRENCY_ID = pCurrency;
            oGJ_DIST_TRACK.NATIVE = pNative;

            return oGJ_DIST_TRACK;
        }
        #endregion

        #region d) GJ_LINE - General Journal Line
        private void saveGJ_Line()
        {
            List<GJ_LINE> lstGJ_LINE = new List<GJ_LINE>();

            if (esCXC)
            {
                lstGJ_LINE = getListGJ_LINE();
            }
            else
            {
                lstGJ_LINE = getListGJ_LINE_CXP();
            }

            foreach (GJ_LINE oGJ_LINE in lstGJ_LINE)
            {
                _oDAOGJ.createGJ_LINE(oGJ_LINE);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>_dtDetalla_IVA
        /// <!-- http://csharpindepth.com/articles/general/FloatingPoint.aspx -->
        public List<GJ_LINE> getListGJ_LINE()
        {
            List<GJ_LINE> lstGJ_LINE = new List<GJ_LINE>();
            int iEntry_No = 1;
            double dMontoFactura = 0;
            double dMontoPago = 0;
            string sCta_Origen = string.Empty;
            string sCta_Destino = string.Empty;
            double dPerdidaGanancia = 0;
            double dPerdida = 0;
            double dGanancia = 0;
            string sCtaPerdida = string.Empty;
            string sCtaGanancia = string.Empty;
            double dTotFactura = 0.0;
            double dTotPago = 0.0;
            string retencion = "";

            foreach (DataRow item in _dtVMX_IVATRASTEMP.Rows)
            {
                GJ_LINE oGJ_LINE = new GJ_LINE();

                sCta_Origen = item["CUENTA_ORIGEN"].ToString();
                sCta_Destino = item["CUENTA_DESTINO"].ToString();
                retencion = item["RETENCION"].ToString();

                dMontoFactura = double.Parse(item["MONTO"].ToString());
                dMontoPago = double.Parse(item["MONTO_IVA_DEPOSITO"].ToString());

                // Cuadrar dMontoFactura & dMontoPago por menos de 1 peso de diferencia (menos de una unidad de moneda nativa)
                if (dMontoFactura != dMontoPago && _GJ.CURRENCY_ID == _GJ.CURRENCY_NATIVE) 
                {
                    // Se hace la asignación vacia de la variable que va a cuadrar las diferencias
                    double Diferencia_FP = 0.00;

                    // Se obtiene la diferencia entre las variables dMontoFactura & dMontoPagos
                    if (dMontoFactura > dMontoPago) 
                    {
                        Diferencia_FP = Math.Round(dMontoFactura - dMontoPago, 2);
                    }
                    else 
                    {
                        Diferencia_FP = Math.Round(dMontoPago - dMontoFactura, 2);
                    }

                    // Se cuadra la diferencia
                    if (Diferencia_FP <= 1) 
                    {
                        if (dMontoFactura > dMontoPago) 
                        {
                            dMontoPago = dMontoPago + Diferencia_FP;
                        }
                        else 
                        {
                            dMontoFactura = dMontoFactura + Diferencia_FP;
                        }
                    }
                    //else {
                    //    throw new Exception("La diferencia entre MontoFactura & MontoPago es mayor al limite establecido. Diferencia: " + Diferencia_FP);
                    //}
                }

                //Si es IVA la cuenta ORIGEN va a débito
                oGJ_LINE = getGJ_LINE(iEntry_No++, sCta_Origen, dMontoFactura, 0); //Débito DR
                lstGJ_LINE.Add(oGJ_LINE);

                //Si es IVA la cuenta DESTINO va a crédito
                if (Global.TipoCambioPagos.Equals("PAGO"))
                    oGJ_LINE = getGJ_LINE(iEntry_No++, sCta_Destino, 0, dMontoPago); //Crédito CR
                else
                    oGJ_LINE = getGJ_LINE(iEntry_No++, sCta_Destino, 0, dMontoFactura); //Crédito CR
                lstGJ_LINE.Add(oGJ_LINE);


                // Perdida Ganancia   
                if (((dMontoFactura != dMontoPago) || _GJ.CURRENCY_ID != _GJ.CURRENCY_NATIVE) && Global.TipoCambioPagos.Equals("PAGO"))
                {
                    DataRow[] result = _dtDetalla_IVA.Select("VAT_GL_ACCT_ID = '" + sCta_Origen + "' AND TRASLADO = '" + sCta_Destino + "'");
                    dPerdida = 0;
                    dGanancia = 0;

                    foreach (DataRow rowDetalle in result)
                    {
                        dPerdidaGanancia = double.Parse(rowDetalle["PERDIDA_GANANCIA"].ToString());

                        if (rowDetalle["VAT_GL_ACCT_ID"].ToString().Equals(sCta_Origen) && rowDetalle["TRASLADO"].ToString().Equals(sCta_Destino) && dPerdidaGanancia < 0)
                        {
                            dPerdida += Math.Abs(dPerdidaGanancia);
                            sCtaPerdida = rowDetalle["CUENTA_PER_GANANCIA"].ToString();
                        }
                        else
                        {
                            dGanancia += dPerdidaGanancia;
                            sCtaGanancia = rowDetalle["CUENTA_PER_GANANCIA"].ToString();
                        }
                    }

                    if (dGanancia > 0)
                    {
                        oGJ_LINE = getGJ_LINE(iEntry_No++, sCtaGanancia, dGanancia, 0);//debito
                        lstGJ_LINE.Add(oGJ_LINE);
                    }

                    if (dPerdida > 0)
                    {
                        oGJ_LINE = getGJ_LINE(iEntry_No++, sCtaPerdida, 0, dPerdida); //credito
                        lstGJ_LINE.Add(oGJ_LINE);
                    }

                    // Se obtiene dTotPago & dTotFactura
                    dTotFactura = dMontoFactura + dGanancia;
                    dTotPago = dMontoPago + dPerdida;
                    string smensaje = string.Empty;

                    // Cuadrar dTotPago & dTotFactura por menos de 1 peso de diferencia
                    if ((decimal)dTotPago != (decimal)dTotFactura) 
                    {
                        // Se hace la asignación vacia de la variable que va a cuadrar las diferencias
                        double Diferencia_PG = 0.00;

                        // Se obtiene la diferencia entre las variables dTotPago & dTotFactura
                        if (dTotPago > dTotFactura) 
                        {
                            Diferencia_PG = Math.Round(dTotPago - dTotFactura, 2);
                        }
                        else 
                        {
                            Diferencia_PG = Math.Round(dTotFactura - dTotPago, 2);
                        }

                        // Se cuadra la diferencia
                        if (Diferencia_PG <= 1) 
                        {
                            if (dTotPago > dTotFactura) 
                            {
                                dTotFactura = dTotFactura + Diferencia_PG;
                            }
                            else 
                            {
                                dTotPago = dTotPago + Diferencia_PG;
                            }
                        }
                        else 
                        {
                            smensaje = string.Format("Linea de póliza. dTotPago:{0} -- dTotFactura: {1}", dTotPago, dTotFactura);
                            throw new Exception(smensaje);
                        }
                    }
                    else 
                    {
                        dTotFactura = 0;
                        dTotPago = 0;
                    }
                    
                }
            }

            return lstGJ_LINE;
        }

        public List<GJ_LINE> getListGJ_LINE_CXP()
        {
            List<GJ_LINE> lstGJ_LINE = new List<GJ_LINE>();
            int iEntry_No = 1;
            double dMontoFactura = 0;
            double dMontoPago = 0;
            string sCta_Origen = string.Empty;
            string sCta_Destino = string.Empty;
            double dPerdidaGanancia = 0;
            double dPerdida = 0;
            double dGanancia = 0;
            string sCtaPerdida = string.Empty;
            string sCtaGanancia = string.Empty;
            double dTotFactura = 0.0;
            double dTotPago = 0.0;
            string retencion = "";

            foreach (DataRow item in _dtVMX_IVATRASTEMP.Rows)
            {
                GJ_LINE oGJ_LINE = new GJ_LINE();

                sCta_Origen = item["CUENTA_ORIGEN"].ToString();
                sCta_Destino = item["CUENTA_DESTINO"].ToString();
                retencion = item["RETENCION"].ToString();

                dMontoFactura = double.Parse(item["MONTO"].ToString());
                dMontoPago = double.Parse(item["MONTO_IVA_DEPOSITO"].ToString());

                // Cuadrar dMontoFactura & dMontoPago por menos de 1 peso de diferencia
                //if (dMontoFactura != dMontoPago) 
                if (((dMontoFactura != dMontoPago) || _GJ.CURRENCY_ID != _GJ.CURRENCY_NATIVE) && Global.TipoCambioPagos.Equals("PAGO"))
                {
                    // Se hace la asignación vacia de la variable que va a cuadrar las diferencias
                    double Diferencia_FP = 0.00;

                    // Se obtiene la diferencia entre las variables dMontoFactura & dMontoPagos
                    if (dMontoFactura > dMontoPago) 
                    {
                        Diferencia_FP = Math.Round(dMontoFactura - dMontoPago, 2);
                    }
                    else 
                    {
                        Diferencia_FP = Math.Round(dMontoPago - dMontoFactura, 2);
                    }

                    // Se cuadra la diferencia
                    if (Diferencia_FP <= 1) 
                    {
                        if (dMontoFactura > dMontoPago) 
                        {
                            dMontoPago = dMontoPago + Diferencia_FP;
                        }
                        else 
                        {
                            dMontoFactura = dMontoFactura + Diferencia_FP;
                        }
                    }
                   // else {
                   //     throw new Exception("La diferencia entre MontoFactura & MontoPago es mayor al limite establecido. Diferencia: " + Diferencia_FP);
                   // }
                }


                //Si es IVA la cuenta de ORIGEN se va a crédito, si es retención se va a débito
                oGJ_LINE = !bool.Parse(retencion) ? getGJ_LINE(iEntry_No++, sCta_Origen, 0, dMontoFactura) : getGJ_LINE(iEntry_No++, sCta_Origen, dMontoFactura, 0); //Crédito CR vs Débito DR
                lstGJ_LINE.Add(oGJ_LINE);

                //Si es IVA la cuenta DESTINO se va a débito, si es retención se va a crédito
                if (Global.TipoCambioPagos.Equals("PAGO"))
                    oGJ_LINE = !bool.Parse(retencion)?getGJ_LINE(iEntry_No++, sCta_Destino, dMontoPago, 0):getGJ_LINE(iEntry_No++, sCta_Destino, 0, dMontoPago); //Débito DR vs Crédito CR
                else
                    oGJ_LINE = !bool.Parse(retencion)?getGJ_LINE(iEntry_No++, sCta_Destino, dMontoFactura,0):getGJ_LINE(iEntry_No++, sCta_Destino, 0, dMontoFactura); //Débito DR vs Crédito CR
                lstGJ_LINE.Add(oGJ_LINE);


                // Perdida Ganancia   
                if (Global.TipoCambioPagos.Equals("PAGO"))
                {
                    DataRow[] result = _dtDetalla_IVA.Select("VAT_GL_ACCT_ID = '" + sCta_Origen + "' AND TRASLADO = '" + sCta_Destino + "'");
                    dPerdida = 0;
                    dGanancia = 0;

                    foreach (DataRow rowDetalle in result)
                    {
                        dPerdidaGanancia = double.Parse(rowDetalle["PERDIDA_GANANCIA"].ToString());

                        if (dPerdidaGanancia != 0)
                        {
                            if (rowDetalle["VAT_GL_ACCT_ID"].ToString().Equals(sCta_Origen) && rowDetalle["TRASLADO"].ToString().Equals(sCta_Destino) && dPerdidaGanancia < 0)
                            {
                                //if(!bool.Parse(retencion))
                                    dPerdida += Math.Abs(dPerdidaGanancia);
                                //else dGanancia += Math.Abs(dPerdidaGanancia);

                                sCtaPerdida = rowDetalle["CUENTA_PER_GANANCIA"].ToString();
                            }
                            else
                            {
                                //if (!bool.Parse(retencion))
                                    dGanancia += Math.Abs(dPerdidaGanancia);
                                //else dPerdida += Math.Abs(dPerdidaGanancia);
                                sCtaGanancia = rowDetalle["CUENTA_PER_GANANCIA"].ToString();
                            }
                        }
                    }

                    if (dGanancia > 0)
                    {
                        //oGJ_LINE = getGJ_LINE(iEntry_No++, sCtaGanancia, 0, dGanancia);

                        if (!bool.Parse(retencion))
                            oGJ_LINE = getGJ_LINE(iEntry_No++, sCtaGanancia, (_PG_Intercarmbiada ? 0 : dGanancia), (_PG_Intercarmbiada ? dGanancia:0));
                        else oGJ_LINE = getGJ_LINE(iEntry_No++, sCtaGanancia, (_PG_Intercarmbiada ? dGanancia : 0), (_PG_Intercarmbiada ? 0 : dGanancia));

                        lstGJ_LINE.Add(oGJ_LINE);
                    }

                    if (dPerdida > 0)
                    {
                        //oGJ_LINE = getGJ_LINE(iEntry_No++, sCtaPerdida, dPerdida, 0);
                        //if (!bool.Parse(retencion))
                        //    oGJ_LINE = getGJ_LINE(iEntry_No++, sCtaPerdida, 0, dPerdida); //Credito
                        //else oGJ_LINE = getGJ_LINE(iEntry_No++, sCtaPerdida, dPerdida, 0); //Debito

                        //se cambió por nuevo esquema
                        if (!bool.Parse(retencion))
                            oGJ_LINE = getGJ_LINE(iEntry_No++, sCtaPerdida, (_PG_Intercarmbiada ?  dPerdida : 0), (_PG_Intercarmbiada ? 0 : dPerdida)); 
                        else oGJ_LINE = getGJ_LINE(iEntry_No++, sCtaPerdida, (_PG_Intercarmbiada ? 0 : dPerdida), (_PG_Intercarmbiada ? dPerdida : 0)); 

                        lstGJ_LINE.Add(oGJ_LINE);
                    }

                    // Se obtiene dTotPago & dTotFactura               
                    if (!_PG_Intercarmbiada)
                    {
                        dTotFactura = dMontoFactura + dPerdida;
                        dTotPago = dMontoPago + dGanancia;
                    }
                    else
                    {
                        dTotFactura = dMontoFactura + dGanancia;
                        dTotPago = dMontoPago + dPerdida;
                    }

                    string smensaje = string.Empty;

                    // Cuadrar dTotPago & dTotFactura por menos de 1 peso de diferencia
                    if ((decimal)dTotPago != (decimal)dTotFactura) 
                    {
                        // Se hace la asignación vacia de la variable que va a cuadrar las diferencias
                        double Diferencia_PG = 0.00;

                        // Se obtiene la diferencia entre las variables dTotPago & dTotFactura
                        if (dTotPago > dTotFactura) 
                        {
                            Diferencia_PG = Math.Round(dTotPago - dTotFactura, 2);
                        }
                        else 
                        {
                            Diferencia_PG = Math.Round(dTotFactura - dTotPago, 2);
                        }

                        // Se cuadra la diferencia
                        if (Diferencia_PG <= 1) 
                        {
                            if (dTotPago > dTotFactura) 
                            {
                                dTotFactura = dTotFactura + Diferencia_PG;
                            }
                            else 
                            {
                                dTotPago = dTotPago + Diferencia_PG;
                            }
                        }
                        else 
                        {
                            smensaje = string.Format("Linea de p&oacute;liza. dTotPago:{0} -- dTotFactura: {1}", dTotPago, dTotFactura);
                            throw new Exception(smensaje);
                        }
                    }
                    else
                    {
                        dTotFactura = 0;
                        dTotPago = 0;
                    }

                    
                }
            }//Termina FOR de lineas

            double totalDebitos = 0.0;
            double totalCreditos = 0.0;

            foreach (GJ_LINE item in lstGJ_LINE)
            {
                totalDebitos = Math.Round(totalDebitos + item.DEBIT_AMOUNT, 2);
                totalCreditos = Math.Round(totalCreditos + item.CREDIT_AMOUNT, 2);
            }


            // Cuadrar totalCreditos & totalDebitos por menos de 1 peso de diferencia
            if (totalCreditos != totalDebitos) {

                // Se hace la asignación vacia de la variable que va a cuadrar las diferencias
                double diferencia = 0.00;

                // Se crea el objeto GJ_DIST()
                GJ_LINE oGJ_LINE = new GJ_LINE();

                // Se obtiene la diferencia entre las variables totalCreditos & totalDebitos
                if (totalCreditos > totalDebitos) 
                {
                    diferencia = Math.Round(totalCreditos - totalDebitos, 2);
                }
                else 
                {
                    diferencia = Math.Round(totalDebitos - totalCreditos, 2);
                }

                // Se cuadra la diferencia
                if (diferencia <= 1) 
                {
                    if (totalCreditos > totalDebitos) 
                    {
                        //
                        oGJ_LINE = getGJ_LINE(iEntry_No++, _sCuentaRedondeo, diferencia, 0);
                        lstGJ_LINE.Add(oGJ_LINE);
                    }
                    else 
                    {
                        //
                        oGJ_LINE = getGJ_LINE(iEntry_No++, _sCuentaRedondeo, 0, diferencia);
                        lstGJ_LINE.Add(oGJ_LINE);
                    }
                }
                else 
                {
                    throw new Exception("La diferencia de las polizas es mayor al limite establecido. Diferencia: " + diferencia);
                }
            }


            return lstGJ_LINE;
        }

        private GJ_LINE getGJ_LINE(int pEntry_No, string pCuenta, double pDebit, double pCredit)
        {
            GJ_LINE oGJ_Line = new GJ_LINE();

            oGJ_Line.GJ_ID = _GJ.ID;
            oGJ_Line.LINE_NO = pEntry_No;
            oGJ_Line.GL_ACCOUNT_ID = pCuenta;
            oGJ_Line.DEBIT_AMOUNT = pDebit;
            oGJ_Line.CREDIT_AMOUNT = pCredit;

            return oGJ_Line;
        }
        #endregion

        private void saveVMXCONTROLPOLIZA()
        {
            VMXCONTROLPOLIZA oCP = new VMXCONTROLPOLIZA();

            oCP.NO_TRANSACCION = _GJ.ID;
            oCP.CUENTA = "";
            oCP.MONTO = _GJ.TOTAL_CR_AMOUNT;
            oCP.FECHA_TRANSACCION = _GJ.POSTING_DATE;
            oCP.FECHA_CREACION = _GJ.CREATE_DATE;
            oCP.FECHA_PERIODO = _GJ.POSTING_DATE;
            oCP.USUARIO = _GJ.USER_ID;
            oCP.TIPO_OPERACION = esCXC ? "CXC" : "CXP";
            oCP.SITE_ID = _GJ.SITE_ID;

            _oDAOGJ.createVMXCONTROLPOLIZA(oCP);
        }

        private List<VMX_CONTOLPOLIZA_LINE> getListVMX_CONTOLPOLIZA_LINE()
        {
            List<VMX_CONTOLPOLIZA_LINE> lstVMXPoliza = new List<VMX_CONTOLPOLIZA_LINE>();

            foreach (DataRow dr_i in _dtDetalla_IVA.Rows)
            {
                VMX_CONTOLPOLIZA_LINE oPL = new VMX_CONTOLPOLIZA_LINE();

                oPL.NO_TRANSACCION = _GJ.ID;
                oPL.BANK_ACCOUNT_ID = dr_i["BANK_ACCOUNT_ID"].ToString();
                oPL.VAT_AMOUNT = "0";
                oPL.SELL_RATE = double.Parse(dr_i["TC_FACTURA"].ToString());
                oPL.MONTO = double.Parse(dr_i["IVA_MXN_TRASLADAR_FACT"].ToString());
                oPL.CUENTA = dr_i["VAT_GL_ACCT_ID"].ToString();
                oPL.DESCRIPCION = dr_i["DESCRIPCION"].ToString();
                oPL.CUENTA_TRASLADO = dr_i["TRASLADO"].ToString();
                oPL.DESCRIPTION = dr_i["TRASLADO"].ToString();
                oPL.PERDIDA_GANANCIA = double.Parse(dr_i["PERDIDA_GANANCIA"].ToString());
                oPL.IVA_DEPOSITO = double.Parse(dr_i["IVA_MXN_TRASLADAR_DEP"].ToString());
                oPL.SITE_ID = dr_i["SITE_ID"].ToString();
                oPL.MONTO_RET = double.Parse(dr_i["RET_MXN_TRASLADAR_FACT"].ToString());
                oPL.RET_DEPOSITO = double.Parse(dr_i["RET_MXN_TRASLADAR_DEP"].ToString());

                if (esCXC)
                {
                    oPL.CUSTOMER_ID = dr_i["Cliente"].ToString();
                    oPL.CHECK_ID = dr_i["CONTROL_NO"].ToString();
                    oPL.TIPO_OPERACION = "CXC";
                }
                else
                {
                    oPL.CUSTOMER_ID = dr_i["CLIENTE"].ToString();
                    oPL.CONTROL_NO = dr_i["CONTROL_NO"].ToString();
                    oPL.TIPO_OPERACION = "CXP";
                }

                lstVMXPoliza.Add(oPL);
            }

            return lstVMXPoliza;
        }

    }
}

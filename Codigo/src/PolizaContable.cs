using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using CTECH.Acceso_a_Datos;
using CTech.Helper.Configuracion;

namespace VMXTRASPIVA
{
    /// <summary>
    /// La póliza es un documento en el que se asientan las operaciones 
    /// desarrolladas por entidad y toda la información necesaria para su identificación.
    /// </summary>
    public class PolizaContable
    {
        public string _ID;
        public string _GJ_Date;
        public string _Description;
        public string _PostingDate;
        /// <summary>
        /// Total Monto Debito
        /// </summary>
        public double _Total_DR_Amount;

        /// <summary>
        /// Total Monto Credito
        /// </summary>
        public double _Total_CR_Amount;
        public double _Sell_Rate;
        public double _Buy_Rate;
        public string _EntityID;
        public string _Create_Date;
        public string _UserID;
        public string _CurrencyID;
        public string _Post_All_Tracking;
        public string _Post_As_Native;
        public string _User_Exch_Rate;
        public string _Posting_Candidate;

        public DataTable _dtVMX_IVATRASTEMP { get; set; }
        public DataTable _dtDetalla_IVA { get; set; }
        private List<MonedaRastreo> _lstCurrencyTracking;

        /// <summary>
        /// Obtiene el siguiente ID para poliza contable
        /// </summary>
        public PolizaContable()
        {
            AdminGJ oAdmGJ = new AdminGJ();
            _ID = oAdmGJ.getNext_GJ_ID();
        }

        public void generateGeneralJournal(string sTipoTraspaso)
        {
            AdminCurrencyTracking oCurrencyTracking = new AdminCurrencyTracking();
            AdminGeneralJournal oAdminGJ = new AdminGeneralJournal();
            AdminGJ oAdmGJ = new AdminGJ();
            GJ oGJ;
            List<GJ_CURR> lstGJ_CURR = new List<GJ_CURR>();
            List<GJ_DIST> lstGJ_DIST = new List<GJ_DIST>();
            List<GJ_LINE> lstGJ_LINE = new List<GJ_LINE>();
            List<VMXCONTROLPOLIZA> lstVMXPoliza = new List<VMXCONTROLPOLIZA>();
            List<VMX_CONTOLPOLIZA_LINE> lstVMXPoliza_line = new List<VMX_CONTOLPOLIZA_LINE>();

            try
            {
                oAdminGJ.crearConexiones();

                _lstCurrencyTracking = oCurrencyTracking.getCurrencyTracking(_GJ_Date);

                // a) GJ - General Journal
                oGJ = getGJ();
                oAdminGJ.createGJ(oGJ);

                // b) GJ_CURR - General Journal Currecncy
                lstGJ_CURR = getGJ_CURR();
                foreach (GJ_CURR oGJ_CURR in lstGJ_CURR)
                {
                    oAdminGJ.createGJ_CURR(oGJ_CURR);
                }

                // c) GJ_DIST - General Journal Distribution
                lstGJ_DIST = getListGJ_DIST();
                foreach (GJ_DIST oGJ_DIST in lstGJ_DIST)
                {
                    oAdminGJ.createGJ_DIST(oGJ_DIST);
                }

                // d) GJ_LINE - General Journal Line
                lstGJ_LINE = getListGJ_LINE();
                foreach (GJ_LINE oGJ_LINE in lstGJ_LINE)
                {
                    oAdminGJ.createGJ_LINE(oGJ_LINE);
                }

                // e) VMXCONTROLPOLIZA
                lstVMXPoliza = getListVMXCONTROLPOLIZA();
                foreach (VMXCONTROLPOLIZA oVMXPoliza in lstVMXPoliza)
                {
                    oAdminGJ.createVMXCONTROLPOLIZA(oVMXPoliza);
                }

                // f) VMX_CONTOLPOLIZA_LINE
                lstVMXPoliza_line = getListVMX_CONTOLPOLIZA_LINE_CXC_TCPago();
                if (sTipoTraspaso.Equals("CXC"))
                {
                    foreach (VMX_CONTOLPOLIZA_LINE oVMXPoliza in lstVMXPoliza_line)
                    {
                        oAdminGJ.createVMX_CONTOLPOLIZA_LINE_CXC(oVMXPoliza);
                    }
                }
                else
                {
                    foreach (VMX_CONTOLPOLIZA_LINE oVMXPoliza in lstVMXPoliza_line)
                    {
                        oAdminGJ.createVMX_CONTOLPOLIZA_LINE_CXP(oVMXPoliza);
                    }
                }

                // Actualizar el nuevo número de póliza
                oAdmGJ.setNext_GJ_ID();

                oAdminGJ.commitTransaction();
            }
            catch (Exception ex)
            {
                oAdminGJ.rollBackTransaction();
                throw new Exception("Error al generar Poliza Contable. Detalle: " + ex.Message);
            }
            finally
            {
                oAdminGJ.cerrarConexiones();
            }
        }

        public GJ getGJ()
        {
            GJ oGJ = new GJ();

            oGJ.ID = _ID;
            oGJ.GJ_DATE = _GJ_Date;
            oGJ.DESCRIPTION = _Description;
            oGJ.POSTING_DATE = _PostingDate;
            oGJ.TOTAL_DR_AMOUNT = _Total_DR_Amount;
            oGJ.TOTAL_CR_AMOUNT = _Total_CR_Amount;
            oGJ.SELL_RATE = _Sell_Rate;
            oGJ.BUY_RATE = _Buy_Rate;
            oGJ.ENTITY_ID = _EntityID;
            oGJ.CREATE_DATE = _Create_Date;
            oGJ.USER_ID = _UserID;
            oGJ.CURRENCY_ID = _CurrencyID;
            oGJ.POST_ALL_TRACKING = _Post_All_Tracking;
            oGJ.POST_AS_NATIVE = _Post_As_Native;
            oGJ.USER_EXCH_RATE = _User_Exch_Rate;
            oGJ.POSTING_CANDIDATE = _Posting_Candidate;

            return oGJ;
        }

        public List<GJ_CURR> getGJ_CURR()
        {
            List<GJ_CURR> lstGJ_CURR = new List<GJ_CURR>();
            GJ_CURR oGJ_CURR = new GJ_CURR();

            oGJ_CURR.GJ_ID = _ID;
            oGJ_CURR.CURRENCY_ID = _CurrencyID;
            oGJ_CURR.AMOUNT = _Total_DR_Amount;
            oGJ_CURR.SELL_RATE = _Sell_Rate;
            oGJ_CURR.BUY_RATE = _Buy_Rate;

            lstGJ_CURR.Add(oGJ_CURR);

            if (_lstCurrencyTracking.Count > 0)
            {
                foreach (MonedaRastreo item in _lstCurrencyTracking)
                {
                    GJ_CURR oGJ_CURR_Trace = new GJ_CURR();

                    oGJ_CURR_Trace.GJ_ID = _ID;
                    oGJ_CURR_Trace.CURRENCY_ID = item.ID;
                    oGJ_CURR_Trace.AMOUNT = Math.Round((_Total_DR_Amount / item.SELL_RATE), 2);
                    oGJ_CURR_Trace.SELL_RATE = item.SELL_RATE;
                    oGJ_CURR_Trace.BUY_RATE = item.BUY_RATE;

                    lstGJ_CURR.Add(oGJ_CURR_Trace);
                }
            }

            return lstGJ_CURR;
        }

        public List<GJ_DIST> getListGJ_DIST()
        {
            List<GJ_DIST> lstGJ_DIST = new List<GJ_DIST>();
            int iEntry_No = 1;
            double dMontoFactura = 0;
            double dMontoPago = 0;
            double dMonto_tipoCambio = 0;

            if (Global.TipoCambioPagos.Equals("FACTURA"))
            {
                foreach (DataRow item in _dtVMX_IVATRASTEMP.Rows)
                {
                    GJ_DIST oGJ_DIST = new GJ_DIST();

                    dMontoFactura = double.Parse(item["MONTO"].ToString());

                    oGJ_DIST = getGJ_DIST_NATIVE(iEntry_No++, dMontoFactura, "DR", item["CUENTA_ORIGEN"].ToString());
                    lstGJ_DIST.Add(oGJ_DIST);

                    oGJ_DIST = getGJ_DIST_NATIVE(iEntry_No++, dMontoFactura, "CR", item["CUENTA_DESTINO"].ToString());
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

                        oGJ_DIST_TRACK = getGJ_DIST_TRACK(iEntry_No++, dMonto_tipoCambio, "DR", item["CUENTA_DESTINO"].ToString(), moneda.ID);
                        lstGJ_DIST.Add(oGJ_DIST_TRACK);

                        oGJ_DIST_TRACK = getGJ_DIST_TRACK(iEntry_No++, dMonto_tipoCambio, "CR", item["CUENTA_ORIGEN"].ToString(), moneda.ID);
                        lstGJ_DIST.Add(oGJ_DIST_TRACK);
                    }
                }
            }
            else
            {
                // 1 - Moneda del sistema
                foreach (DataRow item in _dtVMX_IVATRASTEMP.Rows)
                {
                    GJ_DIST oGJ_DIST = new GJ_DIST();

                    dMontoFactura = double.Parse(item["MONTO"].ToString());
                    dMontoPago = double.Parse(item["MONTO_IVA_DEPOSITO"].ToString());

                    oGJ_DIST = getGJ_DIST_NATIVE(iEntry_No++, dMontoFactura, "DR", item["CUENTA_ORIGEN"].ToString());
                    lstGJ_DIST.Add(oGJ_DIST);

                    oGJ_DIST = getGJ_DIST_NATIVE(iEntry_No++, dMontoPago, "CR", item["CUENTA_DESTINO"].ToString());
                    lstGJ_DIST.Add(oGJ_DIST);

                    // Perdida Ganancia
                    PerdidaGanancia oPG = new PerdidaGanancia();
                    string sCta_Origen = item["CUENTA_ORIGEN"].ToString();
                    string sCta_Destino = item["CUENTA_DESTINO"].ToString();

                    oPG = oPG.getPerdidaGanancia(_dtDetalla_IVA, sCta_Origen, sCta_Destino);

                    if (oPG.Ganancia > 0)
                    {
                        oGJ_DIST = getGJ_DIST_NATIVE(iEntry_No++, oPG.Ganancia, "DR", oPG.CuentaGanancia);
                        lstGJ_DIST.Add(oGJ_DIST);
                    }

                    if (oPG.Perdida > 0)
                    {
                        oGJ_DIST = getGJ_DIST_NATIVE(iEntry_No++, oPG.Perdida, "CR", oPG.CuentaPerdida);
                        lstGJ_DIST.Add(oGJ_DIST);
                    }
                }

                // 2- Monedas rastreables
                foreach (DataRow item in _dtVMX_IVATRASTEMP.Rows)
                {
                    dMontoFactura = double.Parse(item["MONTO"].ToString());
                    dMontoPago = double.Parse(item["MONTO_IVA_DEPOSITO"].ToString());

                    foreach (MonedaRastreo moneda in _lstCurrencyTracking)
                    {
                        GJ_DIST oGJ_DIST_TRACK = new GJ_DIST();

                        dMonto_tipoCambio = Math.Round((dMontoFactura / moneda.SELL_RATE), 2);
                        oGJ_DIST_TRACK = getGJ_DIST_TRACK(iEntry_No++, dMonto_tipoCambio, "DR", item["CUENTA_DESTINO"].ToString(), moneda.ID);
                        lstGJ_DIST.Add(oGJ_DIST_TRACK);

                        dMonto_tipoCambio = Math.Round((dMontoPago / moneda.SELL_RATE), 2);
                        oGJ_DIST_TRACK = getGJ_DIST_TRACK(iEntry_No++, dMonto_tipoCambio, "CR", item["CUENTA_ORIGEN"].ToString(), moneda.ID);
                        lstGJ_DIST.Add(oGJ_DIST_TRACK);

                        // Perdida Ganancia
                        PerdidaGanancia oPG = new PerdidaGanancia();
                        string sCta_Origen = item["CUENTA_ORIGEN"].ToString();
                        string sCta_Destino = item["CUENTA_DESTINO"].ToString();

                        oPG = oPG.getPerdidaGanancia(_dtDetalla_IVA, sCta_Origen, sCta_Destino);

                        if (oPG.Ganancia > 0)
                        {
                            dMonto_tipoCambio = Math.Round((oPG.Ganancia / moneda.SELL_RATE), 2);
                            oGJ_DIST_TRACK = getGJ_DIST_TRACK(iEntry_No++, dMonto_tipoCambio, "DR", oPG.CuentaGanancia, moneda.ID);
                            lstGJ_DIST.Add(oGJ_DIST_TRACK);
                        }

                        if (oPG.Perdida > 0)
                        {
                            dMonto_tipoCambio = Math.Round((oPG.Perdida / moneda.SELL_RATE), 2);
                            oGJ_DIST_TRACK = getGJ_DIST_TRACK(iEntry_No++, dMonto_tipoCambio, "CR", oPG.CuentaPerdida, moneda.ID);
                            lstGJ_DIST.Add(oGJ_DIST_TRACK);
                        }
                    }
                }
            }

            return lstGJ_DIST;
        }

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

            foreach (DataRow item in _dtVMX_IVATRASTEMP.Rows)
            {
                GJ_LINE oGJ_LINE = new GJ_LINE();

                sCta_Origen = item["CUENTA_ORIGEN"].ToString();
                sCta_Destino = item["CUENTA_DESTINO"].ToString();

                dMontoFactura = double.Parse(item["MONTO"].ToString());
                dMontoPago = double.Parse(item["MONTO_IVA_DEPOSITO"].ToString());

                oGJ_LINE = getGJ_LINE(iEntry_No++, sCta_Destino, dMontoFactura, 0);
                lstGJ_LINE.Add(oGJ_LINE);

                if (Global.TipoCambioPagos.Equals("PAGO"))
                {
                    oGJ_LINE = getGJ_LINE(iEntry_No++, sCta_Origen, 0, dMontoPago);
                }
                else
                {
                    oGJ_LINE = getGJ_LINE(iEntry_No++, sCta_Origen, 0, dMontoFactura);
                }

                lstGJ_LINE.Add(oGJ_LINE);

                // Perdida Ganancia   
                if (Global.TipoCambioPagos.Equals("PAGO"))
                {
                    DataRow[] result = _dtDetalla_IVA.Select("VAT_GL_ACCT_ID = '" + sCta_Origen + "' AND TRASLADO = '" + sCta_Destino + "'");

                    foreach (DataRow rowDetalle in _dtDetalla_IVA.Rows)
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
                        oGJ_LINE = getGJ_LINE(iEntry_No++, sCtaGanancia, dGanancia, 0);
                        lstGJ_LINE.Add(oGJ_LINE);
                    }

                    if (dPerdida > 0)
                    {
                        oGJ_LINE = getGJ_LINE(iEntry_No++, sCtaPerdida, 0, dPerdida);
                        lstGJ_LINE.Add(oGJ_LINE);
                    }
                }
            }

            return lstGJ_LINE;
        }

        private List<VMXCONTROLPOLIZA> getListVMXCONTROLPOLIZA()
        {
            // Encacbezado de poliza para control de CTECH
            List<VMXCONTROLPOLIZA> lstVMXPoliza = new List<VMXCONTROLPOLIZA>();

            foreach (DataRow drRow_Traspasos in _dtVMX_IVATRASTEMP.Rows)
            {
                VMXCONTROLPOLIZA ovmPoliza = new VMXCONTROLPOLIZA();

                ovmPoliza.NO_TRANSACCION = _ID;
                ovmPoliza.CUENTA = drRow_Traspasos["CUENTA_DESTINO"].ToString();
                ovmPoliza.MONTO = double.Parse(drRow_Traspasos["MONTO"].ToString());
                ovmPoliza.FECHA_PERIODO = _PostingDate;
                ovmPoliza.FECHA_TRANSACCION = _Create_Date;
                ovmPoliza.FECHA_CREACION = _Create_Date;
                ovmPoliza.USUARIO = _UserID;
                //ovmPoliza.TIPO_OPERACION = pTipoTraspaso;

                lstVMXPoliza.Add(ovmPoliza);
            }

            return lstVMXPoliza;
        }


        private List<VMX_CONTOLPOLIZA_LINE> getListVMX_CONTOLPOLIZA_LINE_CXC_TCFactura()
        {
            List<VMX_CONTOLPOLIZA_LINE> lstVMXPoliza = new List<VMX_CONTOLPOLIZA_LINE>();

            foreach (DataRow dr_i in _dtDetalla_IVA.Rows)
            {
                VMX_CONTOLPOLIZA_LINE oPL = new VMX_CONTOLPOLIZA_LINE();

                oPL.NO_TRANSACCION = _ID;
                oPL.BANK_ACCOUNT_ID = dr_i["BANK_ACCOUNT_ID"].ToString();
                oPL.VAT_AMOUNT = "0";
                oPL.SELL_RATE = double.Parse(dr_i["TC_FACTURA"].ToString());
                oPL.MONTO = double.Parse(dr_i["IVA_MXN_TRASLADAR_FACT"].ToString());
                oPL.CUENTA = dr_i["VAT_GL_ACCT_ID"].ToString();
                oPL.DESCRIPCION = dr_i["DESCRIPCION"].ToString();
                oPL.CUENTA_TRASLADO = dr_i["TRASLADO"].ToString();
                oPL.DESCRIPTION = dr_i["TRASLADO"].ToString();
                oPL.CHECK_ID = dr_i["CONTROL_NO"].ToString();
                oPL.TIPO_OPERACION = "CXC";
                oPL.CUSTOMER_ID = dr_i["Cliente"].ToString();

                lstVMXPoliza.Add(oPL);
            }

            return lstVMXPoliza;
        }

        private List<VMX_CONTOLPOLIZA_LINE> getListVMX_CONTOLPOLIZA_LINE_CXC_TCPago()
        {
            List<VMX_CONTOLPOLIZA_LINE> lstVMXPoliza = new List<VMX_CONTOLPOLIZA_LINE>();

            foreach (DataRow dr_i in _dtDetalla_IVA.Rows)
            {
                VMX_CONTOLPOLIZA_LINE oPL = new VMX_CONTOLPOLIZA_LINE();

                oPL.NO_TRANSACCION = _ID;
                oPL.BANK_ACCOUNT_ID = dr_i["BANK_ACCOUNT_ID"].ToString();
                oPL.VAT_AMOUNT = "0";
                oPL.SELL_RATE = double.Parse(dr_i["TC_FACTURA"].ToString());
                oPL.MONTO = double.Parse(dr_i["IVA_MXN_TRASLADAR_FACT"].ToString());
                oPL.CUENTA = dr_i["VAT_GL_ACCT_ID"].ToString();
                oPL.DESCRIPCION = dr_i["DESCRIPCION"].ToString();
                oPL.CUENTA_TRASLADO = dr_i["TRASLADO"].ToString();
                oPL.DESCRIPTION = dr_i["TRASLADO"].ToString();
                oPL.CHECK_ID = dr_i["CONTROL_NO"].ToString();
                oPL.TIPO_OPERACION = "CXC";
                oPL.CUSTOMER_ID = dr_i["Cliente"].ToString();

                lstVMXPoliza.Add(oPL);
            }

            return lstVMXPoliza;
        }



        private List<VMX_CONTOLPOLIZA_LINE> getListVMX_CONTOLPOLIZA_LINE_CXP_TCPago()
        {
            List<VMX_CONTOLPOLIZA_LINE> lstVMXPoliza = new List<VMX_CONTOLPOLIZA_LINE>();

            foreach (DataRow dr_i in _dtDetalla_IVA.Rows)
            {
                VMX_CONTOLPOLIZA_LINE oPL = new VMX_CONTOLPOLIZA_LINE();

                oPL.NO_TRANSACCION = _ID;
                oPL.BANK_ACCOUNT_ID = dr_i["BANK_ACCOUNT_ID"].ToString();
                oPL.VAT_AMOUNT = "0";
                oPL.SELL_RATE = double.Parse(dr_i["TC_FACTURA"].ToString());
                oPL.MONTO = double.Parse(dr_i["IVA_MXN_TRASLADAR_FACT"].ToString());
                oPL.CUENTA = dr_i["VAT_GL_ACCT_ID"].ToString();
                oPL.DESCRIPCION = dr_i["DESCRIPCION"].ToString();
                oPL.CUENTA_TRASLADO = dr_i["TRASLADO"].ToString();
                oPL.DESCRIPTION = dr_i["TRASLADO"].ToString();
                oPL.CONTROL_NO = dr_i["CONTROL_NO"].ToString();
                oPL.TIPO_OPERACION = "CXP";

                lstVMXPoliza.Add(oPL);
            }

            return lstVMXPoliza;
        }

        private List<VMX_CONTOLPOLIZA_LINE> getListVMX_CONTOLPOLIZA_LINE_CXP_TCFactura()
        {
            List<VMX_CONTOLPOLIZA_LINE> lstVMXPoliza = new List<VMX_CONTOLPOLIZA_LINE>();

            foreach (DataRow dr_i in _dtDetalla_IVA.Rows)
            {
                VMX_CONTOLPOLIZA_LINE oPL = new VMX_CONTOLPOLIZA_LINE();

                oPL.NO_TRANSACCION = _ID;
                oPL.BANK_ACCOUNT_ID = dr_i["BANK_ACCOUNT_ID"].ToString();
                oPL.VAT_AMOUNT = "0";
                oPL.SELL_RATE = double.Parse(dr_i["TC_FACTURA"].ToString());
                oPL.MONTO = double.Parse(dr_i["IVA_MXN_TRASLADAR_FACT"].ToString());
                oPL.CUENTA = dr_i["VAT_GL_ACCT_ID"].ToString();
                oPL.DESCRIPCION = dr_i["DESCRIPCION"].ToString();
                oPL.CUENTA_TRASLADO = dr_i["TRASLADO"].ToString();
                oPL.DESCRIPTION = dr_i["TRASLADO"].ToString();
                oPL.CONTROL_NO = dr_i["CONTROL_NO"].ToString();
                oPL.TIPO_OPERACION = "CXP";

                lstVMXPoliza.Add(oPL);
            }

            return lstVMXPoliza;
        }



        private GJ_DIST getGJ_DIST_NATIVE(int pEntry_No, double pAmount, string pAmountType, string sAccount)
        {
            GJ_DIST oGJ_DIST = new GJ_DIST();

            oGJ_DIST.GJ_ID = _ID;
            oGJ_DIST.DIST_NO = "1";
            oGJ_DIST.ENTRY_NO = pEntry_No.ToString();
            oGJ_DIST.AMOUNT = pAmount;
            oGJ_DIST.AMOUNT_TYPE = pAmountType;
            oGJ_DIST.GL_ACCOUNT_ID = sAccount;
            oGJ_DIST.NATIVE_AMOUNT = "0";
            oGJ_DIST.POSTING_DATE = _PostingDate;
            oGJ_DIST.POSTING_STATUS = "U";
            oGJ_DIST.CREATE_DATE = _Create_Date;
            oGJ_DIST.ENTITY_ID = _EntityID;
            oGJ_DIST.CURRENCY_ID = _CurrencyID;
            oGJ_DIST.NATIVE = "Y";

            return oGJ_DIST;
        }

        private GJ_DIST getGJ_DIST_TRACK(int pEntry_No, double pAmount, string pAmountType, string sAccount, string pCurrency)
        {
            GJ_DIST oGJ_DIST_TRACK = new GJ_DIST();

            oGJ_DIST_TRACK.GJ_ID = _ID;
            oGJ_DIST_TRACK.DIST_NO = "1";
            oGJ_DIST_TRACK.ENTRY_NO = pEntry_No.ToString();
            oGJ_DIST_TRACK.AMOUNT = pAmount;
            oGJ_DIST_TRACK.AMOUNT_TYPE = pAmountType;
            oGJ_DIST_TRACK.GL_ACCOUNT_ID = sAccount;
            oGJ_DIST_TRACK.NATIVE_AMOUNT = "0";
            oGJ_DIST_TRACK.POSTING_DATE = _PostingDate;
            oGJ_DIST_TRACK.POSTING_STATUS = "U";
            oGJ_DIST_TRACK.CREATE_DATE = _Create_Date;
            oGJ_DIST_TRACK.ENTITY_ID = _EntityID;
            oGJ_DIST_TRACK.CURRENCY_ID = pCurrency;
            oGJ_DIST_TRACK.NATIVE = "N";

            return oGJ_DIST_TRACK;
        }

        private GJ_LINE getGJ_LINE(int pEntry_No, string pCuenta, double pDebit, double pCredit)
        {
            GJ_LINE oGJ_Line = new GJ_LINE();

            oGJ_Line.GJ_ID = _ID;
            oGJ_Line.LINE_NO = pEntry_No;
            oGJ_Line.GL_ACCOUNT_ID = pCuenta;
            oGJ_Line.DEBIT_AMOUNT = pDebit;
            oGJ_Line.CREDIT_AMOUNT = pCredit;

            return oGJ_Line;
        }


        public void generateGJ_CXC_TC_Factura()
        {
            // basica
            AdminCurrencyTracking oCurrencyTracking = new AdminCurrencyTracking();
            AdminGeneralJournal oAdminGJ = new AdminGeneralJournal();
            AdminGJ oAdmGJ = new AdminGJ();
            GJ oGJ;
            List<GJ_CURR> lstGJ_CURR = new List<GJ_CURR>();
            List<GJ_DIST> lstGJ_DIST = new List<GJ_DIST>();
            List<GJ_LINE> lstGJ_LINE = new List<GJ_LINE>();
            List<VMXCONTROLPOLIZA> lstVMXPoliza = new List<VMXCONTROLPOLIZA>();
            List<VMX_CONTOLPOLIZA_LINE> lstVMXPoliza_line = new List<VMX_CONTOLPOLIZA_LINE>();

            try
            {
                oAdminGJ.crearConexiones();

                _lstCurrencyTracking = oCurrencyTracking.getCurrencyTracking(_GJ_Date);

                // a) GJ - General Journal
                oGJ = getGJ();
                oAdminGJ.createGJ(oGJ);

                // b) GJ_CURR - General Journal Currecncy
                lstGJ_CURR = getGJ_CURR();
                foreach (GJ_CURR oGJ_CURR in lstGJ_CURR)
                {
                    oAdminGJ.createGJ_CURR(oGJ_CURR);
                }

                // c) GJ_DIST - General Journal Distribution
                lstGJ_DIST = getListGJ_DIST();
                foreach (GJ_DIST oGJ_DIST in lstGJ_DIST)
                {
                    oAdminGJ.createGJ_DIST(oGJ_DIST);
                }

                // d) GJ_LINE - General Journal Line
                lstGJ_LINE = getListGJ_LINE();
                foreach (GJ_LINE oGJ_LINE in lstGJ_LINE)
                {
                    oAdminGJ.createGJ_LINE(oGJ_LINE);
                }

                // e) VMXCONTROLPOLIZA
                lstVMXPoliza = getListVMXCONTROLPOLIZA();
                foreach (VMXCONTROLPOLIZA oVMXPoliza in lstVMXPoliza)
                {
                    oAdminGJ.createVMXCONTROLPOLIZA(oVMXPoliza);
                }

                // f) VMX_CONTOLPOLIZA_LINE
                lstVMXPoliza_line = getListVMX_CONTOLPOLIZA_LINE_CXC_TCPago();

                foreach (VMX_CONTOLPOLIZA_LINE oVMXPoliza in lstVMXPoliza_line)
                {
                    oAdminGJ.createVMX_CONTOLPOLIZA_LINE_CXC(oVMXPoliza);
                }


                // Actualizar el nuevo número de póliza
                oAdmGJ.setNext_GJ_ID();

                oAdminGJ.commitTransaction();
            }
            catch (Exception ex)
            {
                oAdminGJ.rollBackTransaction();
                throw new Exception("Error al generar Poliza Contable. Detalle: " + ex.Message);
            }
            finally
            {
                oAdminGJ.cerrarConexiones();
            }
        }

        public void generateGJ_CXC_TC_Pago()
        {

        }


        public void generateGJ_CXP_TC_Factura()
        {

        }

        public void generateGJ_CXP_TC_Pago()
        {

        }

        public void agregarPoliza(DataTable pdtTbl_Traspasos, string psTipoTraspaso, DataTable pdtTbl_VMX_IVATRASTEMP)
        {
            Microsoft_SQL_Server objSql = new Microsoft_SQL_Server(Global.Servidor, Global.BaseDatos, Global.Usuario, Global.Password);
            string sInsertGJ = string.Empty;
            string sInsertGJ_CURR = string.Empty;
            StringBuilder sGJ_DIST = new StringBuilder();
            StringBuilder sGJ_LINE = new StringBuilder();
            StringBuilder sVMX_CONTOLPOLIZA_LINE = new StringBuilder();
            AdminCurrencyTracking oCurrencyTracking = new AdminCurrencyTracking();

            _dtVMX_IVATRASTEMP = pdtTbl_Traspasos;
            _dtDetalla_IVA = pdtTbl_VMX_IVATRASTEMP;
            string query = string.Empty;

            try
            {
                _lstCurrencyTracking = oCurrencyTracking.getCurrencyTracking(_GJ_Date);

                objSql.CrearConexion();
                objSql.AbrirConexion();
                objSql.CrearTransaccion();

                // General Journal
                sInsertGJ = MapeoQuerySql.ObtenerPorId("PolizaContable.agregarPoliza.GeneralJournal");
                sInsertGJ = string.Format(sInsertGJ,
                                          _ID, _GJ_Date, _Description, _PostingDate, _Total_DR_Amount, _Total_CR_Amount, _Sell_Rate, _Buy_Rate, _EntityID,
                                          _Create_Date, _UserID, _CurrencyID, _Post_All_Tracking, _Post_As_Native, _User_Exch_Rate, _Posting_Candidate);

                // Insertar encabezado de Poliza
                objSql.EjecutarDML(sInsertGJ);

                // Almacenar el tipo de moneda y el monto de la poliza.                
                sInsertGJ_CURR = MapeoQuerySql.ObtenerPorId("PolizaContable.agregarPoliza.Currency");
                sInsertGJ_CURR = string.Format(sInsertGJ_CURR, _ID, _CurrencyID, _Total_CR_Amount, _Sell_Rate, _Buy_Rate);

                if (_lstCurrencyTracking.Count > 0)
                {
                    string currencyTracking = MapeoQuerySql.ObtenerPorId("PolizaContable.agregarPoliza.Currency");
                    foreach (MonedaRastreo item in _lstCurrencyTracking)
                    {
                        sInsertGJ_CURR += string.Format(currencyTracking, _ID, item.ID, Math.Round((_Total_CR_Amount / item.SELL_RATE), 2), item.SELL_RATE, item.BUY_RATE);
                    }
                }

                objSql.EjecutarDML(sInsertGJ_CURR);

                int iDistNo = 0;
                int iLineNo = 1;
                double dperdida = 0;
                double dganancia = 0;
                string sCtaGanancia = string.Empty;
                string sCtaPerdida = string.Empty;

                foreach (DataRow drRow_Traspasos in pdtTbl_Traspasos.Rows)
                {
                    string DIST_NO = "1";
                    string NATIVE_AMOUNT = "0";
                    string POSTING_STATUS = "U";
                    string NATIVE = "Y";
                    string sCta_Origen = drRow_Traspasos["CUENTA_ORIGEN"].ToString();
                    string sCta_Destino = drRow_Traspasos["CUENTA_DESTINO"].ToString();
                    string sMontoTraspaso = drRow_Traspasos["MONTO"].ToString();
                    double dIVA_Factura = double.Parse(drRow_Traspasos["MONTO"].ToString());
                    double dIVA_Deposito = double.Parse(drRow_Traspasos["MONTO_IVA_DEPOSITO"].ToString());
                    double dGananciaPerdida = 0;
                    string str_sql = string.Empty;


                    // ganancia perdida
                    DataRow[] result = pdtTbl_VMX_IVATRASTEMP.Select("VAT_GL_ACCT_ID = '" + sCta_Origen + "' AND TRASLADO = '" + sCta_Destino + "'");
                    foreach (DataRow item in pdtTbl_VMX_IVATRASTEMP.Rows)
                    {
                        dGananciaPerdida = double.Parse(item["PERDIDA_GANANCIA"].ToString());

                        if (item["VAT_GL_ACCT_ID"].ToString().Equals(sCta_Origen) && item["TRASLADO"].ToString().Equals(sCta_Destino) && dGananciaPerdida < 0)
                        {
                            dperdida += Math.Abs(dGananciaPerdida);
                            sCtaPerdida = item["CUENTA_PER_GANANCIA"].ToString();
                        }
                        else
                        {
                            dganancia += dGananciaPerdida;
                            sCtaGanancia = item["CUENTA_PER_GANANCIA"].ToString();
                        }
                    }


                    // Encacbezado de poliza para control de CTECH
                    query = MapeoQuerySql.ObtenerPorId("PolizaContable.agregarPoliza.ControlCTech");
                    query = string.Format(query, _ID, sCta_Destino, sMontoTraspaso, _PostingDate, _Create_Date, _Create_Date, _UserID, psTipoTraspaso);
                    objSql.EjecutarDML(query);

                    // En caso de que el monto sea cero no se genera
                    // ningun movimiento contable.
                    if (dIVA_Factura > 0)
                    {
                        // Detalle de los movimientos de la poliza. 
                        //      Nota:En caso de tener moneda de rastreo generar un registro automaticamente para esta moneda
                        // CXC.- Cuentas por Cobrar
                        // Verificar el caso en que la cuenta origen sea diferente a la cuenta destino
                        if (psTipoTraspaso == "CXC")
                        {
                            // DR.- Debito
                            query = MapeoQuerySql.ObtenerPorId("PolizaContable.agregarPoliza.CxC");
                            query = string.Format(query, _ID, DIST_NO, iDistNo, dIVA_Factura, "DR", sCta_Origen, NATIVE_AMOUNT, _PostingDate, POSTING_STATUS, _Create_Date, _EntityID, _CurrencyID, NATIVE);
                            sGJ_DIST.AppendLine(query);

                            iDistNo++;

                            // CR.- Credito
                            query = MapeoQuerySql.ObtenerPorId("PolizaContable.agregarPoliza.CxC");
                            query = string.Format(query, _ID, DIST_NO, iDistNo.ToString(), dIVA_Deposito, "CR", sCta_Destino, NATIVE_AMOUNT, _PostingDate, POSTING_STATUS, _Create_Date, _EntityID, _CurrencyID, NATIVE);
                            sGJ_DIST.AppendLine(query);

                            // Moneda rastreable
                            foreach (MonedaRastreo moneda in _lstCurrencyTracking)
                            {
                                iDistNo++;

                                // DR.- Debito
                                query = MapeoQuerySql.ObtenerPorId("PolizaContable.agregarPoliza.CxC");
                                query = string.Format(query, _ID, DIST_NO, iDistNo, Math.Round((double.Parse(sMontoTraspaso) / moneda.SELL_RATE), 2), "DR", sCta_Destino, NATIVE_AMOUNT, _PostingDate, POSTING_STATUS, _Create_Date, _EntityID, moneda.ID, "N");
                                sGJ_DIST.AppendLine(query);

                                iDistNo++;

                                // CR.- Credito
                                query = MapeoQuerySql.ObtenerPorId("PolizaContable.agregarPoliza.CxC");
                                query = string.Format(query, _ID, DIST_NO, iDistNo, Math.Round((double.Parse(sMontoTraspaso) / moneda.SELL_RATE), 2), "CR", sCta_Destino, NATIVE_AMOUNT, _PostingDate, POSTING_STATUS, _Create_Date, _EntityID, moneda.ID, "N");
                                sGJ_DIST.AppendLine(query);
                            }
                        }
                        else
                        {
                            // CR.- Credito
                            query = MapeoQuerySql.ObtenerPorId("PolizaContable.agregarPoliza.CxC");
                            query = string.Format(query, _ID, DIST_NO, iDistNo, sMontoTraspaso, "CR", sCta_Origen, NATIVE_AMOUNT, _PostingDate, POSTING_STATUS, _Create_Date, _EntityID, _CurrencyID, NATIVE);
                            sGJ_DIST.AppendLine(query);

                            iDistNo++;

                            // DR.- Debito
                            query = MapeoQuerySql.ObtenerPorId("PolizaContable.agregarPoliza.CxC");
                            query = string.Format(query, _ID, DIST_NO, iDistNo, sMontoTraspaso, "DR", sCta_Destino, NATIVE_AMOUNT, _PostingDate, POSTING_STATUS, _Create_Date, _EntityID, _CurrencyID, NATIVE);
                            sGJ_DIST.AppendLine(query);

                            // Moneda rastreable
                            foreach (MonedaRastreo moneda in _lstCurrencyTracking)
                            {
                                iDistNo++;

                                // CR.- Credito
                                query = MapeoQuerySql.ObtenerPorId("PolizaContable.agregarPoliza.CxC");
                                query = string.Format(query, _ID, DIST_NO, iDistNo, Math.Round((double.Parse(sMontoTraspaso) / moneda.SELL_RATE), 2), "CR", sCta_Origen, NATIVE_AMOUNT, _PostingDate, POSTING_STATUS, _Create_Date, _EntityID, moneda.ID, "N");
                                sGJ_DIST.AppendLine(query);

                                iDistNo++;

                                // DR.- Debito
                                query = MapeoQuerySql.ObtenerPorId("PolizaContable.agregarPoliza.CxC");
                                query = string.Format(query, _ID, DIST_NO, iDistNo, Math.Round((double.Parse(sMontoTraspaso) / moneda.SELL_RATE), 2), "DR", sCta_Destino, NATIVE_AMOUNT, _PostingDate, POSTING_STATUS, _Create_Date, _EntityID, moneda.ID, "N");
                                sGJ_DIST.AppendLine(query);
                            }
                        }

                        iDistNo++;

                        // Detalle de movimientos de la poliza
                        // CXP.- Cuentas por Pagar
                        if (psTipoTraspaso == "CXP")
                        {
                            query = MapeoQuerySql.ObtenerPorId("PolizaContable.agregarPoliza.CxP");
                            query = string.Format(query, _ID, iLineNo, sCta_Destino, sMontoTraspaso, 0);
                            sGJ_LINE.AppendLine(query);

                            iLineNo++;

                            query = MapeoQuerySql.ObtenerPorId("PolizaContable.agregarPoliza.CxP");
                            query = string.Format(query, _ID, iLineNo, sCta_Origen, 0, sMontoTraspaso);
                            sGJ_LINE.AppendLine(query);
                        }
                        else
                        {
                            query = MapeoQuerySql.ObtenerPorId("PolizaContable.agregarPoliza.CxP");
                            query = string.Format(query, _ID, iLineNo, sCta_Origen, sMontoTraspaso, 0);
                            sGJ_LINE.AppendLine(query);

                            iLineNo++;

                            query = MapeoQuerySql.ObtenerPorId("PolizaContable.agregarPoliza.CxP");
                            query = string.Format(query, _ID, iLineNo, sCta_Destino, 0, sMontoTraspaso);
                            sGJ_LINE.AppendLine(query);
                        }

                        iLineNo++;
                    }

                    foreach (DataRow dr_i in pdtTbl_VMX_IVATRASTEMP.Rows)
                    {
                        string BANK_ACCOUNT_ID = dr_i["BANK_ACCOUNT_ID"].ToString();
                        string CONTROL_NO = dr_i["CONTROL_NO"].ToString();
                        string VAT_AMOUNT = "0";
                        string SELL_RATE = dr_i["TC_FACTURA"].ToString();
                        string MONTO = dr_i["IVA_MXN_TRASLADAR_FACT"].ToString();
                        string CUENTA_ORIGEN = dr_i["VAT_GL_ACCT_ID"].ToString();
                        string DESCRIPCION = dr_i["DESCRIPCION"].ToString();
                        string CUENTA_DESTINO = dr_i["TRASLADO"].ToString();
                        string COD_CLIENTE = string.Empty;

                        if (sCta_Destino == CUENTA_DESTINO && sCta_Origen == CUENTA_ORIGEN)
                        {
                            // CXC - CASH_RECEIPT
                            if (psTipoTraspaso == "CXC")
                            {
                                COD_CLIENTE = dr_i["Cliente"].ToString();
                                query = MapeoQuerySql.ObtenerPorId("PolizaContable.agregarPoliza.CashReceiptCxC");
                                query = string.Format(query, _ID, BANK_ACCOUNT_ID, CONTROL_NO, VAT_AMOUNT, SELL_RATE, MONTO, CUENTA_ORIGEN, DESCRIPCION, CUENTA_DESTINO, CUENTA_DESTINO, psTipoTraspaso, COD_CLIENTE);
                                sVMX_CONTOLPOLIZA_LINE.AppendLine(query);
                            }
                            else
                            {
                                // CXP - CASH_DISBURSEMENT
                                query = MapeoQuerySql.ObtenerPorId("PolizaContable.agregarPoliza.CashDisbursementCxP");
                                query = string.Format(query, _ID, BANK_ACCOUNT_ID, CONTROL_NO, VAT_AMOUNT, SELL_RATE, MONTO, CUENTA_ORIGEN, DESCRIPCION, CUENTA_DESTINO, CUENTA_DESTINO, psTipoTraspaso);
                                sVMX_CONTOLPOLIZA_LINE.AppendLine(query);
                            }
                        }
                    }
                }

                // Guardar todos los traspasos en GJ_DIST
                objSql.EjecutarDML(sGJ_DIST.ToString());

                // Guarddar todo los traspasos en GJ_LINE
                objSql.EjecutarDML(sGJ_LINE.ToString());

                // Guardar los traspasos en tabla de CTECH
                objSql.EjecutarDML(sVMX_CONTOLPOLIZA_LINE.ToString());

                // Actualizar el nuevo numero de poliza
                query = MapeoQuerySql.ObtenerPorId("PolizaContable.agregarPoliza.ActualizaNumeroPoliza");
                query = string.Format(query, ((int.Parse(_ID) + 1)).ToString());
                objSql.EjecutarDML(query);

                objSql.TransCommit();
            }
            catch (Exception ex)
            {
                objSql.TransRollback();

                throw new Exception(string.Format("Ocurrió un error al generar la póliza.\nDetalle: {0}.", ex.Message));
            }
            finally
            {
                objSql.DestruirTransaccion();

                objSql.CerrarConexion();
                objSql.DestruirConexion();
            }
        }
    }
}

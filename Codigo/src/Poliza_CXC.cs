using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace VMXTRASPIVA
{
    public class Poliza_CXC : Journal
    {
        public DataTable _dtVMX_IVATRASTEMP { get; set; }
        public DataTable _dtDetalla_IVA { get; set; }
        private List<MonedaRastreo> _lstCurrencyTracking;

        public string TipoCambio { get; set; }

        public void generate()
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

                _lstCurrencyTracking = oCurrencyTracking.getCurrencyTracking(GJ_DATE);

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

        public GJ getGJ()
        {
            GJ oGJ = new GJ();

            oGJ.ID = ID;
            oGJ.GJ_DATE = GJ_DATE;
            oGJ.DESCRIPTION = DESCRIPTION;
            oGJ.POSTING_DATE = POSTING_DATE;
            oGJ.TOTAL_DR_AMOUNT = TOTAL_DR_AMOUNT;
            oGJ.TOTAL_CR_AMOUNT = TOTAL_CR_AMOUNT;
            oGJ.SELL_RATE = SELL_RATE;
            oGJ.BUY_RATE = BUY_RATE;
            oGJ.ENTITY_ID = ENTITY_ID;
            oGJ.CREATE_DATE = CREATE_DATE;
            oGJ.USER_ID = USER_ID;
            oGJ.CURRENCY_ID = CURRENCY_ID;
            oGJ.POST_ALL_TRACKING = POST_ALL_TRACKING;
            oGJ.POST_AS_NATIVE = POST_AS_NATIVE;
            oGJ.USER_EXCH_RATE = USER_EXCH_RATE;
            oGJ.POSTING_CANDIDATE = POSTING_CANDIDATE;

            return oGJ;
        }

        public List<GJ_CURR> getGJ_CURR()
        {
            List<GJ_CURR> lstGJ_CURR = new List<GJ_CURR>();
            GJ_CURR oGJ_CURR = new GJ_CURR();

            oGJ_CURR.GJ_ID = ID;
            oGJ_CURR.CURRENCY_ID = CURRENCY_ID;
            oGJ_CURR.AMOUNT = TOTAL_DR_AMOUNT;
            oGJ_CURR.SELL_RATE = SELL_RATE;
            oGJ_CURR.BUY_RATE = BUY_RATE;

            lstGJ_CURR.Add(oGJ_CURR);

            if (_lstCurrencyTracking.Count > 0)
            {
                foreach (MonedaRastreo item in _lstCurrencyTracking)
                {
                    GJ_CURR oGJ_CURR_Trace = new GJ_CURR();

                    oGJ_CURR_Trace.GJ_ID = ID;
                    oGJ_CURR_Trace.CURRENCY_ID = item.ID;
                    oGJ_CURR_Trace.AMOUNT = Math.Round((TOTAL_DR_AMOUNT / item.SELL_RATE), 2);
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

            if (TipoCambio.Equals("FACTURA"))
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

                if (TipoCambio.Equals("PAGO"))
                {
                    oGJ_LINE = getGJ_LINE(iEntry_No++, sCta_Origen, 0, dMontoPago);
                }
                else
                {
                    oGJ_LINE = getGJ_LINE(iEntry_No++, sCta_Origen, 0, dMontoFactura);
                }

                lstGJ_LINE.Add(oGJ_LINE);

                // Perdida Ganancia   
                if (TipoCambio.Equals("PAGO"))
                {
                    DataRow[] result = _dtDetalla_IVA.Select("VAT_GL_ACCTID = '" + sCta_Origen + "' AND TRASLADO = '" + sCta_Destino + "'");

                    foreach (DataRow rowDetalle in _dtDetalla_IVA.Rows)
                    {
                        dPerdidaGanancia = double.Parse(rowDetalle["PERDIDA_GANANCIA"].ToString());

                        if (rowDetalle["VAT_GL_ACCTID"].ToString().Equals(sCta_Origen) && rowDetalle["TRASLADO"].ToString().Equals(sCta_Destino) && dPerdidaGanancia < 0)
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

        private GJ_DIST getGJ_DIST_NATIVE(int pEntry_No, double pAmount, string pAmountType, string sAccount)
        {
            GJ_DIST oGJ_DIST = new GJ_DIST();

            oGJ_DIST.GJ_ID = ID;
            oGJ_DIST.DIST_NO = "1";
            oGJ_DIST.ENTRY_NO = pEntry_No.ToString();
            oGJ_DIST.AMOUNT = pAmount;
            oGJ_DIST.AMOUNT_TYPE = pAmountType;
            oGJ_DIST.GL_ACCOUNT_ID = sAccount;
            oGJ_DIST.NATIVE_AMOUNT = "0";
            oGJ_DIST.POSTING_DATE = POSTING_DATE;
            oGJ_DIST.POSTING_STATUS = "U";
            oGJ_DIST.CREATE_DATE = CREATE_DATE;
            oGJ_DIST.ENTITY_ID = ENTITY_ID;
            oGJ_DIST.CURRENCY_ID = CURRENCY_ID;
            oGJ_DIST.NATIVE = "Y";

            return oGJ_DIST;
        }

        private GJ_DIST getGJ_DIST_TRACK(int pEntry_No, double pAmount, string pAmountType, string sAccount, string pCurrency)
        {
            GJ_DIST oGJ_DIST_TRACK = new GJ_DIST();

            oGJ_DIST_TRACK.GJ_ID = ID;
            oGJ_DIST_TRACK.DIST_NO = "1";
            oGJ_DIST_TRACK.ENTRY_NO = pEntry_No.ToString();
            oGJ_DIST_TRACK.AMOUNT = pAmount;
            oGJ_DIST_TRACK.AMOUNT_TYPE = pAmountType;
            oGJ_DIST_TRACK.GL_ACCOUNT_ID = sAccount;
            oGJ_DIST_TRACK.NATIVE_AMOUNT = "0";
            oGJ_DIST_TRACK.POSTING_DATE = POSTING_DATE;
            oGJ_DIST_TRACK.POSTING_STATUS = "U";
            oGJ_DIST_TRACK.CREATE_DATE = CREATE_DATE;
            oGJ_DIST_TRACK.ENTITY_ID = ENTITY_ID;
            oGJ_DIST_TRACK.CURRENCY_ID = pCurrency;
            oGJ_DIST_TRACK.NATIVE = "N";

            return oGJ_DIST_TRACK;
        }

        private GJ_LINE getGJ_LINE(int pEntry_No, string pCuenta, double pDebit, double pCredit)
        {
            GJ_LINE oGJ_Line = new GJ_LINE();

            oGJ_Line.GJ_ID = ID;
            oGJ_Line.LINE_NO = pEntry_No;
            oGJ_Line.GL_ACCOUNT_ID = pCuenta;
            oGJ_Line.DEBIT_AMOUNT = pDebit;
            oGJ_Line.CREDIT_AMOUNT = pCredit;

            return oGJ_Line;
        }

        private List<VMXCONTROLPOLIZA> getListVMXCONTROLPOLIZA()
        {
            // Encacbezado de poliza para control de CTECH
            List<VMXCONTROLPOLIZA> lstVMXPoliza = new List<VMXCONTROLPOLIZA>();

            foreach (DataRow drRow_Traspasos in _dtVMX_IVATRASTEMP.Rows)
            {
                VMXCONTROLPOLIZA ovmPoliza = new VMXCONTROLPOLIZA();

                ovmPoliza.NO_TRANSACCION = ID;
                ovmPoliza.CUENTA = drRow_Traspasos["CUENTA_DESTINO"].ToString();
                ovmPoliza.MONTO = double.Parse(drRow_Traspasos["MONTO"].ToString());
                ovmPoliza.FECHA_PERIODO = POSTING_DATE;
                ovmPoliza.FECHA_TRANSACCION = CREATE_DATE;
                ovmPoliza.FECHA_CREACION = CREATE_DATE;
                ovmPoliza.USUARIO = USER_ID;
                ovmPoliza.TIPO_OPERACION = "CXC";

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

                oPL.NO_TRANSACCION = ID;
                oPL.BANK_ACCOUNT_ID = dr_i["BANK_ACCOUNTID"].ToString();
                oPL.VAT_AMOUNT = "0";
                oPL.SELL_RATE = double.Parse(dr_i["TC_FACTURA"].ToString());
                oPL.MONTO = double.Parse(dr_i["IVA_MXN_TRASLADAR_FACT"].ToString());
                oPL.CUENTA = dr_i["VAT_GL_ACCTID"].ToString();
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

                oPL.NO_TRANSACCION = ID;
                oPL.BANK_ACCOUNT_ID = dr_i["BANK_ACCOUNTID"].ToString();
                oPL.VAT_AMOUNT = "0";
                oPL.SELL_RATE = double.Parse(dr_i["TC_FACTURA"].ToString());
                oPL.MONTO = double.Parse(dr_i["IVA_MXN_TRASLADAR_FACT"].ToString());
                oPL.CUENTA = dr_i["VAT_GL_ACCTID"].ToString();
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
    }
}

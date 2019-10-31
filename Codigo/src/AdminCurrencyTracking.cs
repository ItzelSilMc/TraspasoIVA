using System;
using System.Collections.Generic;
using System.Data;

namespace VMXTRASPIVA
{
    public class AdminCurrencyTracking
    {
        public List<MonedaRastreo> getCurrencyTracking(string pFechaPeriodo, string siteid)
        {
            Moneda oMoneda = new Moneda();
            DataTable odtMonedasRasteables = null;
            DataTable odtDatosMonetarios = null;
            string sMonedaRastreo = string.Empty;
            MonedaRastreo oMonedaRastreo;
            List<MonedaRastreo> lstMonedasRastreo = new List<MonedaRastreo>();
            odtMonedasRasteables = oMoneda.obtenerMonedasRasteables(siteid);

            if (odtMonedasRasteables != null && odtMonedasRasteables.Rows.Count > 0)
            {
                foreach (DataRow rowMoneda in odtMonedasRasteables.Rows)
                {
                    sMonedaRastreo = rowMoneda["CURRENCY_ID"].ToString();
                    odtDatosMonetarios = oMoneda.obtenerDatosMonetarios(sMonedaRastreo, pFechaPeriodo,siteid);

                    oMonedaRastreo = new MonedaRastreo();
                    oMonedaRastreo.ID = sMonedaRastreo;
                    oMonedaRastreo.SELL_RATE = double.Parse(odtDatosMonetarios.Rows[0]["SELL_RATE"].ToString());
                    oMonedaRastreo.BUY_RATE = double.Parse(odtDatosMonetarios.Rows[0]["BUY_RATE"].ToString());
                    oMonedaRastreo.EFFECTIVE_DATE = DateTime.Parse(odtDatosMonetarios.Rows[0]["EFFECTIVE_DATE"].ToString());

                    lstMonedasRastreo.Add(oMonedaRastreo);
                }
            }

            return lstMonedasRastreo;
        }
    }
}

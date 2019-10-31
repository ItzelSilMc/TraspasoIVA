using System.Data;
using System;

namespace VMXTRASPIVA
{
    public static class AdminFuncionalCurrency
    {
        public static string getFunctionalCurrency(string siteID)
        {
            Moneda mo = new Moneda();
            string nativa = mo.obtenerMonedaNativa(siteID);
            return nativa;
        }

        public static DataTable getFunctionalCurrencyExchange(string currency, string fecha, string siteid)
        {
            Moneda mo = new Moneda();
            DataTable tc = mo.obtenerMonedaNativaTipoCambio(currency, fecha,siteid);
            return tc;
        }
    }
}

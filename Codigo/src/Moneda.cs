using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CTECH.Acceso_a_Datos;
using CTech.Helper.Configuracion;

namespace VMXTRASPIVA
{
    public class Moneda
    {
        public Moneda()
        {

        }

        /// <summary>
        /// Obtener las monedas de rastreo que no sean la moneda con la que trabaja la empresa.
        /// </summary>
        /// <returns>DataTable con las monedas de rastreo.</returns>
        public DataTable obtenerMonedasRasteables(string siteId)
        {
            Microsoft_SQL_Server oSql = new Microsoft_SQL_Server(Global.Servidor, Global.BaseDatos, Global.Usuario, Global.Password);
            DataTable odtMonedas = null;

            string query = MapeoQuerySql.ObtenerPorId("Moneda.obtenerMonedasRasteables");
            query = string.Format(query, siteId);
            odtMonedas = oSql.EjecutarConsulta(query, "CURRENCY");

            if (odtMonedas != null && odtMonedas.Rows.Count > 0)
            {
                return odtMonedas;
            }

            return null;
        }

        /// <summary>
        /// Obtener el Sell_Rate y Buy_Rate de la moneda de rastreo.
        /// Ordenar por fecha descendente y tomar el primero.
        /// </summary>
        /// <param name="pMoneda">Moneda de rastreo de la que se ontendra el Sell_Rate y BuyRate.</param>
        /// <param name="pFecha">Fecha del periodo contable en el que se genera la poliza.</param>
        /// <returns>DataTable con los datos monetarios.</returns>
        public DataTable obtenerDatosMonetarios(string pMoneda, string pFecha, string siteid)
        {
            Microsoft_SQL_Server oSql = new Microsoft_SQL_Server(Global.Servidor, Global.BaseDatos, Global.Usuario, Global.Password);

            string query = MapeoQuerySql.ObtenerPorId("Moneda.obtenerDatosMonetarios");
            query = string.Format(query, pMoneda, pFecha, siteid);

            return oSql.EjecutarConsulta(query, "CURRENCY");
        }

        /// <summary>
        /// Obtener el listado de monedas de la empresa
        /// </summary>
        /// <param name="pEntity"></param>
        /// <returns></returns>
        public DataTable obtenerMonedasEntidad(string pEntidad)
        {
            Microsoft_SQL_Server oSql = new Microsoft_SQL_Server(Global.Servidor, Global.BaseDatos, Global.Usuario, Global.Password);

            string query = MapeoQuerySql.ObtenerPorId("Moneda.obtenerMonedasEntidad");
            query = string.Format(query, pEntidad);

            return oSql.EjecutarConsulta(query, "CURRENCY");
        }

        public string obtenerMonedaNativa(String pSite)
        {
            Microsoft_SQL_Server oSql = new Microsoft_SQL_Server(Global.Servidor, Global.BaseDatos, Global.Usuario, Global.Password);

            string query = MapeoQuerySql.ObtenerPorId("Moneda.obtenerMonedaNativa");
            query = string.Format(query, pSite);
            DataTable dt = oSql.EjecutarConsulta(query, "CURRENCY");

            return dt.Rows.Count > 0 ? dt.Rows[0][0].ToString(): string.Empty;
        }

        public DataTable obtenerMonedaNativaTipoCambio(String currency, string fecha, string site_id)
        {
            Microsoft_SQL_Server oSql = new Microsoft_SQL_Server(Global.Servidor, Global.BaseDatos, Global.Usuario, Global.Password);

            string query = MapeoQuerySql.ObtenerPorId("Moneda.obtenerDatosMonetarios");
            query = string.Format(query, currency,fecha, site_id);
            DataTable dt = oSql.EjecutarConsulta(query, "CURRENCY");

            return dt;
        }

    }

    public class monedaEntidad
    {
        public string moneda { get; set; }
        public string rastreable { get; set; }
    }
}

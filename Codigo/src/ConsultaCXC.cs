using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CTech.Helper.Configuracion;

namespace VMXTRASPIVA
{
    public class ConsultaCXC
    {
        public static string conciliados(string pFechaAl, int pAnioConciliacion, int pMesConciliacion, string site_id)
        {
            string query = MapeoQuerySql.ObtenerPorId("ConsultaCXC.conciliados");
            return string.Format(query, pFechaAl, pAnioConciliacion, pMesConciliacion, site_id);
        }

        public static string conciliadosNegativos(string pFechaAl, int pAnioConciliacion, int pMesConciliacion, string id_site)
        {
            string query = MapeoQuerySql.ObtenerPorId("ConsultaCXC.conciliadosNegativos");
            return string.Format(query, pFechaAl, pAnioConciliacion, pMesConciliacion, id_site);
        }

        public static string noConciliados(string pFechaAl, int pAnioConciliacion, int pMesConciliacion)
        {
            string query = MapeoQuerySql.ObtenerPorId("ConsultaCXC.noConciliados");
            return string.Format(query, pFechaAl, pAnioConciliacion, pMesConciliacion);
        }

        public static string noConciliadosNegativos(string pFechaAl, int pAnioConciliacion, int pMesConciliacion, string id_site)
        {
            string query = MapeoQuerySql.ObtenerPorId("ConsultaCXC.noConciliadosNegativos");
            return string.Format(query, pFechaAl, pAnioConciliacion, pMesConciliacion, id_site);
        }

        public static string procesados(string sTransacciones, string site_id, out string sDetalle)
        {
            sDetalle = MapeoQuerySql.ObtenerPorId("ConsultaCXC.procesadosDetalle");
            string query = MapeoQuerySql.ObtenerPorId("ConsultaCXC.procesados");
            return string.Format(query, sTransacciones, site_id);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CTech.Helper.Configuracion;

namespace VMXTRASPIVA
{
    public class ConsultaCXP
    {
        public static string conciliados(string pFechaAl, int pAnioConciliacion, int pMesConciliacion, string site_id)
        {
            string query = MapeoQuerySql.ObtenerPorId("ConsultaCXP.conciliados");
            return string.Format(query, pFechaAl, pAnioConciliacion, pMesConciliacion, site_id);
        }

        public static string conciliadosNegativos(int pAnioConciliacion, int pMesConciliacion, string id_site)
        {
            string query = MapeoQuerySql.ObtenerPorId("ConsultaCXP.conciliadosNegativos");
            query += MapeoQuerySql.ObtenerPorId("ConsultaCXP.conciliadosNegativosRet");
            return string.Format(query, pAnioConciliacion, pMesConciliacion, id_site);
        }

        public static string noConciliados(string pFechaAl, int pAnioPosteo, int pMesPosteo)
        {
            string query = MapeoQuerySql.ObtenerPorId("ConsultaCXP.noConciliados");
            return string.Format(query, pFechaAl, pAnioPosteo, pMesPosteo);
        }

        public static string noConciliadosNegativos(string pFechaAl, int pAnioPosteo, int pMesPosteo, string id_site)
        {
            string query = MapeoQuerySql.ObtenerPorId("ConsultaCXP.noConciliadosNegativos");
            query += MapeoQuerySql.ObtenerPorId("ConsultaCXP.noConciliadosNegativosRet");
            return string.Format(query, pFechaAl, pAnioPosteo, pMesPosteo, id_site);
        }

        public static string procesados(string sTransacciones, string site_id, out string sDetalle)
        {
            sDetalle = MapeoQuerySql.ObtenerPorId("ConsultaCXP.procesadosDetalle");
            string query = MapeoQuerySql.ObtenerPorId("ConsultaCXP.procesados");
            return string.Format(query, sTransacciones, site_id);
        }

    }
}

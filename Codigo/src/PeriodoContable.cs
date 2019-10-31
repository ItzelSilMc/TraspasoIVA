using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CTECH.Acceso_a_Datos;
using System.Data;
using CTech.Helper.Configuracion;

namespace VMXTRASPIVA
{
    public class PeriodoContable
    {
        private string _servidor = string.Empty;
        private string _baseDatos = string.Empty;
        private string _usuario = string.Empty;
        private string _password = string.Empty;

        public PeriodoContable(string pServidor, string pBaseDatos, string pUsuario, string pPassword)
        {
            _servidor = pServidor;
            _baseDatos = pBaseDatos;
            _usuario = pUsuario;
            _password = pPassword;
        }

        /// <summary>
        /// Obtiene los periodos contables para el año seleccionado
        /// </summary>
        /// <param name="pAnio">Año contable del que se desean obtener los periodos</param>
        public DataTable obtenerPeriodosContables(int pAnio, string id_site)
        {
            DataTable oDtTbl = new DataTable();
            string sConsulta = MapeoQuerySql.ObtenerPorId("PeriodoContable.obtenerPeriodosContables");
            sConsulta = string.Format(sConsulta, pAnio, id_site);

            using (Microsoft_SQL_Server oSQL = new Microsoft_SQL_Server(_servidor, _baseDatos, _usuario, _password))
            {
                oSQL.CrearConexion();
                oSQL.AbrirConexion();

                oDtTbl = oSQL.EjecutarConsulta(sConsulta, "ACCOUNT_PERIOD");
            }

            return oDtTbl;
        }
    }
}

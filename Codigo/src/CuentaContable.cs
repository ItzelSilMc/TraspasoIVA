using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CTECH.Acceso_a_Datos;
using System.Data;
using CTech.Helper.Configuracion;

namespace VMXTRASPIVA
{
    public class CuentaContable
    {
        private DatosConexion _oDatosConexion = null;

        public CuentaContable(DatosConexion DatosConexion)
        {
            _oDatosConexion = DatosConexion;
        }

        /// <summary>
        /// Permite obtener información de la cuenta de interface [GL_INTERFACE_ACCT]
        /// </summary>
        /// <param name="pInterface">Nombre de la interface [INTERFACE_ID]</param>
        /// <param name="pEntidad">Nombre de la entidad [ENTITY_ID]</param>
        /// <returns>Un DataTabe con los datos de la cuenta.</returns>
        public DataTable obtenerDatosCuenta(string pInterface, string pEntidad)
        {
            Microsoft_SQL_Server oSql = new Microsoft_SQL_Server(_oDatosConexion.Server, _oDatosConexion.Database, _oDatosConexion.Usuario_cliente, _oDatosConexion.Password);
            string query = MapeoQuerySql.ObtenerPorId("CuentaContable.obtenerDatosCuenta");
            return oSql.EjecutarConsulta(query, "CURRENCY");
        }
    }
}

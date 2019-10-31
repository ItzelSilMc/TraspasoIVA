using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CTECH.Acceso_a_Datos;
using CTech.Helper.Configuracion;

namespace VMXTRASPIVA
{
    public class APPLICATION_GLOBAL
    {
        public DataTable obtenerDatos()
        {
            DataTable odtApp;
            Microsoft_SQL_Server oSQL = null;
            string sTipoCambio = string.Empty;

            try
            {
                oSQL = new Microsoft_SQL_Server(Global.Servidor, Global.BaseDatos, Global.Usuario, Global.Password);
                oSQL.CrearConexion();
                oSQL.AbrirConexion();

                string query = MapeoQuerySql.ObtenerPorId("APPLICATION_GLOBAL.obtenerDatos");
                odtApp = oSQL.EjecutarConsulta(query, "APPLICATION_GLOBAL");

                return odtApp;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrio un error al obtener información de APPLICATION_GLOBAL. Detalle: " + ex.Message);
            }
            finally
            {
                oSQL.CerrarConexion();
                oSQL.DestruirConexion();
            }
        }
    }
}

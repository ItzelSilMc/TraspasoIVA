using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using CTech.Helper.Configuracion;
using CTECH.Acceso_a_Datos;
using CTECH.Directorios;

namespace VMXTRASPIVA
{
    class Retenciones
    {
        private DatosConexion dc;
        private LogErrores archivoErrores = null; 
        
        public Retenciones(DatosConexion conexion, LogErrores a)
        {
            dc = conexion;
            archivoErrores = a;
        }

        /// <summary>
        /// Retorna un DataTable de cuentas de retencion
        /// </summary>
        public DataTable consultaRetencionesDT()
        {
            DataTable dt = new DataTable();
            string id = "Retenciones.consultaRetencionesDT";
            string sql = MapeoQuerySql.ObtenerPorId(id);
            dt = consultaSQL_DT(sql, id);          
            return dt;
        }
        
        /// <summary>
        /// Retorna un listado de cuentas de retencion
        /// </summary>
        public List<Retencion> consultaRetencionesLS()
        {
            DataTable cuentas = new DataTable();
            List<Retencion> rs = new List<Retencion>();
            cuentas = consultaRetencionesDT();
            try
            {
                rs = cuentas.AsEnumerable().Select(m => new Retencion()
                {
                    cuenta = m.Field<string>("ID"),
                    descripcion = m.Field<string>("DESCRIPCION"),
                    retencion = m.Field<decimal>("RETENCION"),
                    traslado = m.Field<string>("TRASLADO"),
                }).ToList();
            }
            catch (Exception ex)
            {
                archivoErrores = new LogErrores();
                archivoErrores.escribir("Retenciones", "public List<Retencion> consultaRetencionesLS()", ex.Message);
            }          
            return rs;
        }

        public int guardarCuenta(Retencion r)
        {
            int i = 0;
            string id = "Retenciones.guardarCuenta";
            string sql = MapeoQuerySql.ObtenerPorId(id);
            sql = string.Format(sql, r.estado, r.cuenta, r.descripcion, r.retencion, r.traslado);
            i = ejecutaSQL(sql,id);
            return i;
        }

        public int eliminarCuenta(string cuenta)
        {
            int i = 0;
            string id = "Retenciones.eliminarCuenta";
            string sql = MapeoQuerySql.ObtenerPorId(id);
            sql = string.Format(sql, cuenta);
            i = ejecutaSQL(sql,id);
            return i;
        }

        public int modificarCuenta(Retencion r)
        {
            int i = 0;
            string id = "Retenciones.modificarCuenta";
            string sql = MapeoQuerySql.ObtenerPorId(id);
            sql = string.Format(sql, r.cuenta,r.estado,r.descripcion,r.retencion,r.traslado);
            i = ejecutaSQL(sql,id);
            return i;
        }

        private int ejecutaSQL(string sql, string id)
        {
           int i = 0;
           Microsoft_SQL_Server mc = new Microsoft_SQL_Server(dc.Server,dc.Database,dc.Usuario_cliente,dc.Password);
           SqlConnection cnn = new SqlConnection(mc.stringConexion);
           try
           {
               SqlCommand c = new SqlCommand(sql, cnn);
               c.CommandType = CommandType.Text;
               cnn.Open();
               i = c.ExecuteNonQuery();
               cnn.Close();
           }
           catch (Exception ex)
           {
               archivoErrores = new LogErrores();
               archivoErrores.escribir("Retenciones", id, ex.Message);
               if (cnn.State != ConnectionState.Closed) cnn.Close();
           }
           return i;
        }

        private DataTable consultaSQL_DT(string sql,string id)
        {
            DataTable dt = new DataTable();
            using (Microsoft_SQL_Server oSQL = new Microsoft_SQL_Server(dc.Server, dc.Database, dc.Usuario_cliente, dc.Password))
            {
                try
                {
                    oSQL.CrearConexion();
                    oSQL.AbrirConexion();
                    dt = oSQL.EjecutarConsulta(sql, "TB");
                    oSQL.CerrarConexion();
                }
                catch (Exception ex)
                {
                    archivoErrores = new LogErrores();
                    archivoErrores.escribir("Retenciones", id, ex.Message);
                }
                finally
                {
                    oSQL.CerrarConexion();
                }
            }
            return dt;
        }

    }

    /// <summary>
    /// Clase de cuenta de retención
    /// </summary>
    public class Retencion
    {
        public string estado { get; set; }
        public string cuenta { get; set; }
        public string descripcion { get; set; }
        public decimal retencion { get; set; }
        public string traslado { get; set; }

        public Retencion()
        {
            estado = "";
            cuenta = "";
            descripcion = "";
            retencion = 0;
            traslado = "";
        }

    }
}

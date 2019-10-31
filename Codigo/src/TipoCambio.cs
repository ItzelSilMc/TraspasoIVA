using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CTECH.Acceso_a_Datos;
using System.Data;
using CTech.Helper.Configuracion;
using System.Windows.Forms;

namespace VMXTRASPIVA
{
    public class TipoCambio
    {
        /// <summary>
        /// Permite obtener el tipo de cambio que maneja la empresa,
        /// las opciones son Tipo de Cambio de la Factura y del Pago.
        /// </summary>
        /// <returns>Una cadena con el tipo de cambio "FACTURA" o "PAGO".</returns>
        public static string obtenerTipoCambio(string id_entidad, Microsoft_SQL_Server conn)
        {
            DataTable odtTipoCambio;
            string sTipoCambio = string.Empty;

            try
            {
                string query = MapeoQuerySql.ObtenerPorId("TipoCambio.obtenerTipoCambio");
                odtTipoCambio = conn.EjecutarConsulta(string.Format("SELECT TIPO FROM VMX_DIOT_TIPOCAMBIO WHERE ENTITY_ID = '{0}'", id_entidad), "TIPOCAMBIO");

                if (odtTipoCambio.Rows.Count >= 1)
                {
                    sTipoCambio = odtTipoCambio.Rows[0]["TIPO"].ToString();
                }
                else 
                {
                    MessageBox.Show("No existe un tipo de cambio 'FACTURA' o 'PAGO' en la Entidad", "Error del sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


                return sTipoCambio;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrio un error al obtener el Tipo de Cambio. Detalle: " + ex.Message);
            }
        }

        public static double obtenerTipoCambio(string pBanco, string pNoControl, string pMoneda)
        {
            DataTable odtTipoCambio;
            Microsoft_SQL_Server oSQL = null;
            double dTC = 0.0;

            try
            {
                oSQL = new Microsoft_SQL_Server(Global.Servidor, Global.BaseDatos, Global.Usuario, Global.Password);
                oSQL.CrearConexion();
                oSQL.AbrirConexion();

                string query = MapeoQuerySql.ObtenerPorId("TipoCambio.obtenerTipoCambio2");
                query = string.Format(query, pBanco, pNoControl, pMoneda);
                odtTipoCambio = oSQL.EjecutarConsulta(query, "TIPOCAMBIO");

                dTC = Convert.ToDouble(odtTipoCambio.Rows[0]["SELL_RATE"].ToString());

                return dTC;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrio un error al obtener el Tipo de Cambio. Detalle: " + ex.Message);
            }
            finally
            {
                oSQL.CerrarConexion();
                oSQL.DestruirConexion();
            }
        }
    }
}

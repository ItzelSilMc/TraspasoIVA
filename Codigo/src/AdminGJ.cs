using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CTECH.Acceso_a_Datos;
using System.Data;
using CTech.Helper.Configuracion;
using System.Data.SqlClient;

namespace VMXTRASPIVA
{
    public class AdminGJ
    {

        public AdminGJ() { 
        
        }

        public string getNext_GJ_ID(string id_site, int add_Id = 0)
        {
            Microsoft_SQL_Server objSQL = new Microsoft_SQL_Server(Global.Servidor, Global.BaseDatos, Global.Usuario, Global.Password);
            DataTable dtTblID_Poliza = new DataTable("NEXT_NUMBER_GEN");
            string sNextNumber = string.Empty;
            string ALPHA_PREFIX = string.Empty;
            string ALPHA_SUFFIX = string.Empty;
            string journalID = string.Empty;
            int sDecimalPlaces = 0;
            int iNextNumber = 0;

            objSQL.CrearConexion();
            objSQL.AbrirConexion();

            // Este numero consecutivo NEXT_NUMBER  es generado por Visual ERP
            string query = string.Format(MapeoQuerySql.ObtenerPorId("AdminGJ.getNext_GJ_ID"),id_site);

            dtTblID_Poliza = objSQL.EjecutarConsulta(string.Format(query), "NEXT_NUMBER_GEN");

            objSQL.CerrarConexion();
            objSQL.DestruirConexion();

            if (dtTblID_Poliza.Rows.Count > 0)
            {
                //sNextNumber = dtTblID_Poliza.Rows[0]["NEXT_NUMBER"].ToString();
                iNextNumber = Convert.ToInt32(dtTblID_Poliza.Rows[0]["NEXT_NUMBER"]) + add_Id;
                sNextNumber = iNextNumber.ToString();
                sDecimalPlaces = int.Parse(dtTblID_Poliza.Rows[0]["DECIMAL_PLACES"].ToString());

                // Concatenar ceros a la izquierda
                if (sNextNumber.Length < sDecimalPlaces)
                    while (sNextNumber.Length < sDecimalPlaces)
                        sNextNumber = "0" + sNextNumber;
            }

            ALPHA_PREFIX = dtTblID_Poliza.Rows[0]["ALPHA_PREFIX"].ToString();
            ALPHA_SUFFIX = dtTblID_Poliza.Rows[0]["ALPHA_SUFFIX"].ToString();

            journalID = ALPHA_PREFIX + sNextNumber + ALPHA_SUFFIX;

            // Siguiente ID_Poliza
            return journalID;
        }

        public void setNext_GJ_ID(string site_id)
        {
            int nextNumber = 0;
            DataTable dtTblID_Poliza = new DataTable("NEXT_NUMBER_GEN");
            Microsoft_SQL_Server objSQL = new Microsoft_SQL_Server(Global.Servidor, Global.BaseDatos, Global.Usuario, Global.Password);
            objSQL.CrearConexion();
            objSQL.AbrirConexion();

            // Este numero consecutivo NEXT_NUMBER  es generado por Visual ERP
            string query = string.Format(MapeoQuerySql.ObtenerPorId("AdminGJ.getNext_GJ_ID"),site_id);

            dtTblID_Poliza = objSQL.EjecutarConsulta(query, "NEXT_NUMBER_GEN");

            nextNumber = int.Parse(dtTblID_Poliza.Rows[0]["NEXT_NUMBER"].ToString());

            query = MapeoQuerySql.ObtenerPorId("AdminGJ.setNext_GJ_ID");

            // Actualizar el nuevo numero de poliza
            objSQL.EjecutarDML(string.Format(query, nextNumber + 1, site_id));

            objSQL.CerrarConexion();
            objSQL.DestruirConexion();
        }

        public string getMax_GJ_ID(string PrefijoID_GJ, string site_id)
        {
            string GJ_id = "";
            int decimal_places = 0;

            Microsoft_SQL_Server objSQL = new Microsoft_SQL_Server(Global.Servidor, Global.BaseDatos, Global.Usuario, Global.Password);

            // Este numero consecutivo se obtiene de GJ sin importarle el site_id
            string sql = MapeoQuerySql.ObtenerPorId("AdminGJ.getMax_GJ_ID");
            sql = string.Format(sql,PrefijoID_GJ,PrefijoID_GJ.Length);

            objSQL.CrearConexion();
            string s= obtenerConsulta(objSQL._oConn,sql);
          
            sql = MapeoQuerySql.ObtenerPorId("AdminGJ.decimal_places");
            sql = string.Format(sql,site_id);

            string paso = obtenerConsulta(objSQL._oConn, sql);
            objSQL.CerrarConexion();
            objSQL.DestruirConexion();


            if (paso != "") decimal_places = Convert.ToInt16(paso);

            if (s!=null)
            {
                GJ_id = (Convert.ToInt32(s)+1).ToString();

                // Concatenar ceros a la izquierda
                if (GJ_id.Length < decimal_places)
                    while (GJ_id.Length < decimal_places)
                        GJ_id = "0" + GJ_id;
                // Concatenar prefijo
                GJ_id = PrefijoID_GJ + GJ_id;
            }
            return GJ_id;
        }

        private string obtenerConsulta(SqlConnection conexion, string sql)
        {
            string s = "0";
            DataTable dt = new DataTable();

            SqlConnection cnn = new SqlConnection(conexion.ConnectionString);
            SqlCommand c = new SqlCommand();
            c.CommandText = sql;
            c.Connection = cnn;
            c.CommandType = CommandType.Text;

            SqlDataAdapter a = new SqlDataAdapter(c);

            try
            {
                cnn.Open();
                //s = c.ExecuteScalar().ToString();
                a.Fill(dt);
                cnn.Close();
                
                if (dt.Rows.Count > 0) s = dt.Rows[0][0].ToString();

            }
            catch (Exception e)
            {
            }
            return s;
        }


    }
}

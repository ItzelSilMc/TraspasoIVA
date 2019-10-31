using System.Data;
using CTECH.Acceso_a_Datos;
using CTECH.Directorios;
using CTech.Helper.Configuracion;

namespace VMXTRASPIVA
{
    /// <summary>
    /// Permite obtener las polizas de cuentas por pagar.
    /// </summary>
    public class CuentasPorCobrar
    {
        private DatosConexion _SqlCnn;        

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="pSQLCnn">Parametros de conexion sql.</param>
        public CuentasPorCobrar(DatosConexion pSQLCnn)
        {
            _SqlCnn = pSQLCnn;
        }

        /// <summary>
        /// Permite obtener las polizas por traspsar.
        /// </summary>
        /// <param name="pAnio">Año del periodo contable</param>
        /// <param name="pMes">Mes del periodo contable</param>
        /// <param name="pFecha">Fecha del perido contable</param>
        /// <param name="pConsiliado">Indica si los movimientos estan o no consiliados.</param>
        /// <returns>DataTable con los movimientos oendientes por traspasar.</returns>
        public DataTable obtenerMovimientos(int pAnio, int pMes, string pFecha, int pConsiliado, string id_site, string sCurrencyId, int pSoloPosteado)
        {
            DataSet DS = new DataSet();            

            using (Microsoft_SQL_Server oSql = new Microsoft_SQL_Server(_SqlCnn.Server, _SqlCnn.Database, _SqlCnn.Usuario_cliente, _SqlCnn.Password))
            {
                oSql.CrearConexion();
                oSql.AbrirConexion();

                string query = MapeoQuerySql.ObtenerPorId("CuentasPorCobrar.obtenerMovimientos");

                oSql.sNombreProcedimiento = query;                

                oSql.NumParametros(7);
                oSql.AgregarParametro("@ANIO", pAnio.ToString(), 18, eTipoDato.Entero, eDireccion.Entrada);
                oSql.AgregarParametro("@MES", pMes.ToString(), 18, eTipoDato.Entero, eDireccion.Entrada);
                oSql.AgregarParametro("@FECHA", pFecha.ToString(), 15, eTipoDato.Caracter, eDireccion.Entrada);
                oSql.AgregarParametro("@CONSILIADO", pConsiliado.ToString(), 18, eTipoDato.Entero, eDireccion.Entrada);
                oSql.AgregarParametro("@SITE_ID", id_site, 18, eTipoDato.Caracter, eDireccion.Entrada);
                oSql.AgregarParametro("@CURRENCY_ID", sCurrencyId, 18, eTipoDato.Caracter, eDireccion.Entrada);

                oSql.AgregarParametro("@SoloPosteados", pSoloPosteado.ToString(), 18, eTipoDato.Entero, eDireccion.Entrada);

                oSql.EjecutarSP(ref DS, "Polizas");
            }

            if (DS.Tables.Contains("Polizas") && DS.Tables["Polizas"] != null)
            {
                return DS.Tables["Polizas"];
            }
            else
            {
                return null;
            }
        }


        public DataTable obtenerMovimientosDetalle(int pAnio, int pMes, string pFecha, int pConsiliado, string id_site, string currency,int pSoloPosteado)
        {
            DataSet DS = new DataSet();

            using (Microsoft_SQL_Server oSql = new Microsoft_SQL_Server(_SqlCnn.Server, _SqlCnn.Database, _SqlCnn.Usuario_cliente, _SqlCnn.Password))
            {
                oSql.CrearConexion();
                oSql.AbrirConexion();

                string query = MapeoQuerySql.ObtenerPorId("CuentasPorCobrar.obtenerMovimientosDetalle");

                oSql.sNombreProcedimiento = query;

                oSql.NumParametros(7);
                oSql.AgregarParametro("@ANIO", pAnio.ToString(), 18, eTipoDato.Entero, eDireccion.Entrada);
                oSql.AgregarParametro("@MES", pMes.ToString(), 18, eTipoDato.Entero, eDireccion.Entrada);
                oSql.AgregarParametro("@FECHA", pFecha.ToString(), 15, eTipoDato.Caracter, eDireccion.Entrada);
                oSql.AgregarParametro("@CONSILIADO", pConsiliado.ToString(), 18, eTipoDato.Entero, eDireccion.Entrada);
                oSql.AgregarParametro("@SITE_ID", id_site, 18, eTipoDato.Caracter, eDireccion.Entrada);
                oSql.AgregarParametro("@CURRENCY_ID", currency, 18, eTipoDato.Caracter, eDireccion.Entrada);

                oSql.AgregarParametro("@SoloPosteados", pSoloPosteado.ToString(), 18, eTipoDato.Entero, eDireccion.Entrada);

                oSql.EjecutarSP(ref DS, "Polizas");
            }

            if (DS.Tables.Contains("Polizas") && DS.Tables["Polizas"] != null)
            {
                return DS.Tables["Polizas"];
            }
            else
            {
                return null;
            }
        }
        
        
    }
}

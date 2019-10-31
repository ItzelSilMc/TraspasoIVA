using System.Data;
using CTECH.Acceso_a_Datos;
using CTECH.Directorios;
using CTech.Helper.Configuracion;

namespace VMXTRASPIVA
{
    /// <summary>
    /// Permite obtener las polizas de cuentas por pagar.
    /// </summary>
    public class CuentasPorPagar
    {
        private DatosConexion _SqlCnn;
        private LogErrores _ArchivoErrores = null;

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="pSQLCnn">Parametros de conexion sql.</param>
        public CuentasPorPagar(DatosConexion pSQLCnn)
        {
            _SqlCnn = pSQLCnn;
        }

        /// <summary>
        /// Obtiene las polizas por traspazar por tipo de consulta
        /// </summary>
        /// <param name="pAnio"></param>
        /// <param name="pMes"></param>
        /// <param name="pFecha"></param>
        /// <param name="pConsiliado"></param>
        /// <param name="id_site"></param>
        /// <param name="TipoMovimiento">P:Polizas,PD:PolizasDetalle,R:Retenciones,RD:RetencionesDetalle </param>
        /// <returns>DataTable movimientos pendientes por traspazar</returns>
        public DataTable obtenerMovimientosTipo(int pAnio, int pMes, string pFecha, int pConsiliado, string id_site, string TipoMovimiento,string currency, int PG_Intercambiada,int pSoloPosteado)
        {
            string sp = "";
            switch(TipoMovimiento){
                case "P": sp = "CuentasPorPagar.obtenerMovimientos";
                    break;
                case "PD": sp = "CuentasPorPagar.obtenerMovimientosDetalle";
                    break;
                case "R": sp = "CuentasPorPagar.obtenerMovimientosRetencion";
                    break;
                case "RD": sp = "CuentasPorPagar.obtenerMovimientosRetencionDetalle";
                    break;
                default: sp = "";
                    break;
                }

            DataSet DS = new DataSet();

            using (Microsoft_SQL_Server oSql = new Microsoft_SQL_Server(_SqlCnn.Server, _SqlCnn.Database, _SqlCnn.Usuario_cliente, _SqlCnn.Password))
            {
                oSql.CrearConexion();
                oSql.AbrirConexion();

                string query = MapeoQuerySql.ObtenerPorId(sp);

                oSql.sNombreProcedimiento = query;
                oSql.NumParametros(8);
                oSql.AgregarParametro("@ANIO", pAnio.ToString(), 18, eTipoDato.Entero, eDireccion.Entrada);
                oSql.AgregarParametro("@MES", pMes.ToString(), 18, eTipoDato.Entero, eDireccion.Entrada);
                oSql.AgregarParametro("@FECHA", pFecha.ToString(), 15, eTipoDato.Caracter, eDireccion.Entrada);
                oSql.AgregarParametro("@CONSILIADO", pConsiliado.ToString(), 18, eTipoDato.Entero, eDireccion.Entrada);
                oSql.AgregarParametro("@SITE_ID", id_site, 18, eTipoDato.Caracter, eDireccion.Entrada);
                oSql.AgregarParametro("@CURRENCY_ID", currency, 18, eTipoDato.Caracter, eDireccion.Entrada);
                oSql.AgregarParametro("@PG_Intercarmbiada", PG_Intercambiada.ToString(), 18, eTipoDato.Entero, eDireccion.Entrada);

                oSql.AgregarParametro("@SoloPosteados", pSoloPosteado.ToString(), 18, eTipoDato.Entero, eDireccion.Entrada);

                _ArchivoErrores = new LogErrores();
                string sParametros = string.Format("AÑO: {0} MES: {1} FECHA: {2} CONSILIADO: {3}", pAnio, pMes, pFecha, pConsiliado);
                _ArchivoErrores.escribir("CuentasPorPagar", "obtenerMovimientos", sParametros);
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
        
        /*
        /// <summary>
        /// Permite obtener las polizas por traspsar.
        /// </summary>
        /// <param name="pAnio">Año del periodo contable</param>
        /// <param name="pMes">Mes del periodo contable</param>
        /// <param name="pFecha">Fecha del perido contable</param>
        /// <param name="pConsiliado">Indica si los movimientos estan o no consiliados.</param>
        /// <returns>DataTable con los movimientos pendientes por traspasar.</returns>
        public DataTable obtenerMovimientos(int pAnio, int pMes, string pFecha, int pConsiliado, string id_site)
        {
            DataSet DS = new DataSet();

            using (Microsoft_SQL_Server oSql = new Microsoft_SQL_Server(_SqlCnn.Server, _SqlCnn.Database, _SqlCnn.Usuario_cliente, _SqlCnn.Password))
            {
                oSql.CrearConexion();
                oSql.AbrirConexion();

                string query = MapeoQuerySql.ObtenerPorId("CuentasPorPagar.obtenerMovimientos");

                oSql.sNombreProcedimiento = query;
                oSql.NumParametros(5);
                oSql.AgregarParametro("@ANIO", pAnio.ToString(), 18, eTipoDato.Entero, eDireccion.Entrada);
                oSql.AgregarParametro("@MES", pMes.ToString(), 18, eTipoDato.Entero, eDireccion.Entrada);
                oSql.AgregarParametro("@FECHA", pFecha.ToString(), 15, eTipoDato.Caracter, eDireccion.Entrada);
                oSql.AgregarParametro("@CONSILIADO", pConsiliado.ToString(), 18, eTipoDato.Entero, eDireccion.Entrada);
                oSql.AgregarParametro("@SITE_ID", id_site, 18, eTipoDato.Caracter, eDireccion.Entrada);

                _ArchivoErrores = new LogErrores();
                string sParametros = string.Format("AÑO: {0} MES: {1} FECHA: {2} CONSILIADO: {3}", pAnio, pMes, pFecha, pConsiliado);
                _ArchivoErrores.escribir("CuentasPorPagar", "obtenerMovimientos", sParametros);
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

        /// <summary>
        /// Permite obtener las polizas por traspsar de Retencion
        /// </summary>
        /// <param name="pAnio"></param>
        /// <param name="pMes"></param>
        /// <param name="pFecha"></param>
        /// <param name="pConsiliado"></param>
        /// <param name="id_site"></param>
        /// <returns></returns>
        public DataTable obtenerMovimientosRetencion(int pAnio, int pMes, string pFecha, int pConsiliado, string id_site)
        {
            DataSet DS = new DataSet();

            using (Microsoft_SQL_Server oSql = new Microsoft_SQL_Server(_SqlCnn.Server, _SqlCnn.Database, _SqlCnn.Usuario_cliente, _SqlCnn.Password))
            {
                oSql.CrearConexion();
                oSql.AbrirConexion();

                string query = MapeoQuerySql.ObtenerPorId("CuentasPorPagar.obtenerMovimientosRetencion");

                oSql.sNombreProcedimiento = query;
                oSql.NumParametros(5);
                oSql.AgregarParametro("@ANIO", pAnio.ToString(), 18, eTipoDato.Entero, eDireccion.Entrada);
                oSql.AgregarParametro("@MES", pMes.ToString(), 18, eTipoDato.Entero, eDireccion.Entrada);
                oSql.AgregarParametro("@FECHA", pFecha.ToString(), 15, eTipoDato.Caracter, eDireccion.Entrada);
                oSql.AgregarParametro("@CONSILIADO", pConsiliado.ToString(), 18, eTipoDato.Entero, eDireccion.Entrada);
                oSql.AgregarParametro("@SITE_ID", id_site, 18, eTipoDato.Caracter, eDireccion.Entrada);

                _ArchivoErrores = new LogErrores();
                string sParametros = string.Format("AÑO: {0} MES: {1} FECHA: {2} CONSILIADO: {3}", pAnio, pMes, pFecha, pConsiliado);
                _ArchivoErrores.escribir("CuentasPorPagar", "obtenerMovimientos", sParametros);
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
        }*/
    }
}

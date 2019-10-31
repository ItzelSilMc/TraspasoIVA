using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CTECH.Acceso_a_Datos;
using System.Data;
using CTech.Helper.Configuracion;

namespace VMXTRASPIVA
{
    public class AdminGeneralJournal
    {
        /// <summary>
        /// Objeto para establecer conexión con la base de datos de Visual ERP
        /// </summary>
        private Microsoft_SQL_Server _oSQL;

        /// <summary>
        /// Movimientos realizados por la clase
        /// </summary>
        public StringBuilder _trace;

        /// <summary>
        /// Constructor que estable las cadenas de conexion
        /// a las bases de datos de Visual ERP.        
        /// </summary>
        public AdminGeneralJournal()
        {
            _oSQL = new Microsoft_SQL_Server(Global.Servidor, Global.BaseDatos, Global.Usuario, Global.Password);
            _trace = new StringBuilder();
        }

        /// <summary>
        /// Crear y abrir las conexiones a las dos base de datos.
        /// Ademas de agregar la transacción correspondiente a cada
        /// base de datos.
        /// </summary>
        public void crearConexiones()
        {
            // Conexion CTI
            _oSQL.CrearConexion();
            _oSQL.AbrirConexion();
            _oSQL.CrearTransaccion();
        }

        /// <summary>
        /// Cerrar conexiones a las bases de datos.
        /// Ademas de cerrar las transacciones activas.
        /// </summary>
        public void cerrarConexiones()
        {
            try
            {
                _oSQL.CerrarConexion();
                _oSQL.DestruirConexion();
                _oSQL.DestruirTransaccion();
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Contador para el manejo de sqlException cuando ocurre violación de primary key en método de createGJ
        /// </summary>
        private int createGJ_cont_exSQL = 0;

        public void createGJ(GJ oGJ)
        {
            string sCons = string.Empty;

            try
            {
                string query = MapeoQuerySql.ObtenerPorId("AdminGeneralJournal.createGJ");

                sCons = string.Format(query
                , oGJ.ID, oGJ.GJ_DATE, oGJ.DESCRIPTION, oGJ.POSTING_DATE, oGJ.TOTAL_DR_AMOUNT
                , oGJ.TOTAL_CR_AMOUNT, oGJ.SELL_RATE, oGJ.BUY_RATE, oGJ.SITE_ID, oGJ.CREATE_DATE
                , oGJ.USER_ID, oGJ.CURRENCY_ID, oGJ.POST_ALL_TRACKING, oGJ.POST_AS_NATIVE, oGJ.USER_EXCH_RATE, oGJ.POSTING_CANDIDATE);

                _trace.AppendLine(sCons);
                _oSQL.EjecutarDML(sCons);
            }

            catch (Exception ex)
            {
                if (ex.Message.Contains("Violation of PRIMARY KEY constraint 'PK_GJ'"))
                {
                    createGJ_cont_exSQL += 1;
                    oGJ.getNext_ID(oGJ.SITE_ID, createGJ_cont_exSQL);
                    if (createGJ_cont_exSQL < 10) createGJ(oGJ);
                }
                else throw new Exception("Error al insertar en GJ. Detalle: " + ex.Message);
            }

            if (createGJ_cont_exSQL!=0) createGJ_cont_exSQL = 0;
        }

        public void createGJ_CURR(GJ_CURR oGJ_CURR)
        {
            string sCons = string.Empty;

            try
            {
                string query = MapeoQuerySql.ObtenerPorId("AdminGeneralJournal.createGJ_CURR");

                sCons = string.Format(query, oGJ_CURR.GJ_ID, oGJ_CURR.CURRENCY_ID, oGJ_CURR.AMOUNT, oGJ_CURR.SELL_RATE, oGJ_CURR.BUY_RATE);

                _trace.AppendLine(sCons);
                _oSQL.EjecutarDML(sCons);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar en GJ_CURR. Detalle: " + ex.Message);
            }
        }

        public void createGJ_DIST(GJ_DIST oGJ_DIST)
        {
            string sCons = string.Empty;

            try
            {
                if (oGJ_DIST.AMOUNT > 0)
                {
                    string query = MapeoQuerySql.ObtenerPorId("AdminGeneralJournal.createGJ_DIST");

                    sCons = string.Format(query,
                        oGJ_DIST.GJ_ID
                        , oGJ_DIST.DIST_NO
                        , oGJ_DIST.ENTRY_NO
                        , oGJ_DIST.AMOUNT
                        , oGJ_DIST.AMOUNT_TYPE
                        , oGJ_DIST.GL_ACCOUNT_ID
                        , oGJ_DIST.NATIVE_AMOUNT
                        , oGJ_DIST.POSTING_DATE
                        , oGJ_DIST.POSTING_STATUS
                        , oGJ_DIST.CREATE_DATE
                        , oGJ_DIST.SITE_ID
                        , oGJ_DIST.CURRENCY_ID
                        , oGJ_DIST.NATIVE);

                    _trace.AppendLine(sCons);
                    _oSQL.EjecutarDML(sCons);
                }                
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar en GJ_DIST. Detalle: " + ex.Message);
            }
        }

        public void createGJ_LINE(GJ_LINE oGJ_LINE)
        {
            string sCons = string.Empty;
            double dTotalLinea = 0;

            try
            {
                string query = MapeoQuerySql.ObtenerPorId("AdminGeneralJournal.createGJ_LINE");
                dTotalLinea = oGJ_LINE.CREDIT_AMOUNT + oGJ_LINE.DEBIT_AMOUNT;

                if (dTotalLinea > 0)
                {
                    sCons = string.Format(query,
                                           oGJ_LINE.GJ_ID
                                           , oGJ_LINE.LINE_NO
                                           , oGJ_LINE.GL_ACCOUNT_ID
                                           , oGJ_LINE.DEBIT_AMOUNT
                                           , oGJ_LINE.CREDIT_AMOUNT);

                    _trace.AppendLine(sCons);
                    _oSQL.EjecutarDML(sCons);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar en GJ_LINE. Detalle: " + ex.Message);
            }
        }

        public void createVMXCONTROLPOLIZA(VMXCONTROLPOLIZA oVMXPoliza)
        {
            string sCons = string.Empty;

            try
            {
                string query = MapeoQuerySql.ObtenerPorId("AdminGeneralJournal.createVMXCONTROLPOLIZA");

                sCons = string.Format(query
                    , oVMXPoliza.NO_TRANSACCION, oVMXPoliza.CUENTA, oVMXPoliza.MONTO
                    , oVMXPoliza.FECHA_PERIODO, oVMXPoliza.FECHA_TRANSACCION, oVMXPoliza.FECHA_CREACION
                    , oVMXPoliza.USUARIO, oVMXPoliza.TIPO_OPERACION, oVMXPoliza.SITE_ID);

                _trace.AppendLine(sCons);
                _oSQL.EjecutarDML(sCons);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar en VMXCONTROLPOLIZA. Detalle: " + ex.Message);
            }
        }

        public void createVMX_CONTOLPOLIZA_LINE_CXC(VMX_CONTOLPOLIZA_LINE oVMXLine)
        {
            string sCons = string.Empty;
            try
            {
                string query = MapeoQuerySql.ObtenerPorId("AdminGeneralJournal.createVMX_CONTOLPOLIZA_LINE_CXC");

                sCons = string.Format(query
                    , oVMXLine.NO_TRANSACCION, oVMXLine.BANK_ACCOUNT_ID, oVMXLine.CHECK_ID
                        , oVMXLine.VAT_AMOUNT, oVMXLine.SELL_RATE, oVMXLine.MONTO
                        , oVMXLine.CUENTA, oVMXLine.DESCRIPCION, oVMXLine.CUENTA_TRASLADO, oVMXLine.DESCRIPTION, oVMXLine.TIPO_OPERACION, oVMXLine.CUSTOMER_ID, oVMXLine.PERDIDA_GANANCIA, oVMXLine.IVA_DEPOSITO, oVMXLine.SITE_ID);

                _trace.AppendLine(sCons);
                _oSQL.EjecutarDML(sCons);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar en VMX_CONTOLPOLIZA_LINE - CXC. Detalle: " + ex.Message);
            }
        }

        public void createVMX_CONTOLPOLIZA_LINE_CXP(VMX_CONTOLPOLIZA_LINE oVMXLine)
        {
            string sCons = string.Empty;

            try
            {
                string query = MapeoQuerySql.ObtenerPorId("AdminGeneralJournal.createVMX_CONTOLPOLIZA_LINE_CXP");

                sCons = string.Format(query
                    , oVMXLine.NO_TRANSACCION, oVMXLine.BANK_ACCOUNT_ID, oVMXLine.CONTROL_NO
                    , oVMXLine.VAT_AMOUNT, oVMXLine.SELL_RATE, oVMXLine.MONTO
                    , oVMXLine.CUENTA, oVMXLine.DESCRIPCION, oVMXLine.CUENTA_TRASLADO, oVMXLine.DESCRIPTION, oVMXLine.TIPO_OPERACION, oVMXLine.CUSTOMER_ID,oVMXLine.PERDIDA_GANANCIA, oVMXLine.IVA_DEPOSITO, oVMXLine.SITE_ID, oVMXLine.MONTO_RET,oVMXLine.RET_DEPOSITO);

                _trace.AppendLine(sCons);
                _oSQL.EjecutarDML(sCons);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar en VMX_CONTOLPOLIZA_LINE - CXP. Detalle: " + ex.Message);
            }
        }

        public string getRoundedAccount(string id_site)
        {
            DataTable oDt = null;

            try
            {
                string query = string.Format(MapeoQuerySql.ObtenerPorId("AdminGeneralJournal.getRoundedAccount"), id_site);
                _trace.AppendLine(query);
                oDt = _oSQL.EjecutarConsulta(query, "GL_INTERFACE");

                return oDt.Rows[0]["GL_ACCOUNT_ID"].ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener cuanta para s. Detalle: " + ex.Message);
            }
        }

        public void commitTransaction()
        {
            _oSQL.TransCommit();
        }

        public void rollBackTransaction()
        {
            _oSQL.TransRollback();
        }
    }
}

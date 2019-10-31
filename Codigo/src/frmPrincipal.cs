using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using CTECH.Acceso_a_Datos;
using CTECH.Directorios;
using UtilidadesX;
using System.Globalization;
using CTech.Helper.Configuracion;
using System.Linq;
using CTECH.Configuracion.Entities;
using CTECH.Configuracion.Business;
using System.Configuration;
using System.Threading;

namespace VMXTRASPIVA
{
    public partial class frmPrincipal : Form
    {

        private bool button = false;
        private UtilidadesFBX _utilidades = new UtilidadesFBX();
        Microsoft_SQL_Server _SQL;
        private DatosConexion _oDatosConexion = null;
        private LogErrores _ArchivoErrores = null;
        /// <summary>
        /// Contiene los periodos contables que se obtienen de la tabla ACCOUNT_PERIOD
        /// </summary>
        DataTable _dtTblPeriodosContables = null;
        /// <summary>
        /// Contiene las polizas que generan movimientos negativos.
        /// </summary>
        DataTable _odtPolizasNegativas = null;
        private string _FecAl;
        private DateConverter _DateConverter;
        private string _ConnSql;

        ///////CARGAR
        string sTipoTraspaso;

        DataTable dtTbl_PendientesPorTraspasar = new DataTable();
        DataTable dtTblProcesados = new DataTable();
        DataTable poliza = new DataTable();

        DataTable dtTbl_PendientesPorTraspasar2 = new DataTable();

        private List<Entidad> entidades = new List<Entidad>();

        DataTable _dtTraspasos_Agrupados_Resumen;
        DataTable _dtTraspasos_Seleccionados_Detalle;

        PeriodoContable oPeriodo;

        /// <summary>
        /// Contiene el detalle de las polizas por traspazar.
        /// </summary>
        DataTable _odtDetallePolizasTraspasar = null;

        /// <summary>
        /// Contiene el detalle de las polizas procesadas.
        /// </summary>
        DataTable _odtDetalleProcesados = null;
        

        private string superUsuario = MapeoQuerySql.ObtenerPorId("superUsr");
        private bool PG_Intercarmbiada = false;




        public frmPrincipal()
        {
            comboCampos.SelectedIndex = 0;
            picCarga.Visible = false;
            InitializeComponent();
        }


        public frmPrincipal(string pConn)
        {
            try
            {
                this.InitializeComponent();
                _ConnSql = pConn;

                _oDatosConexion = new DatosConexion(pConn);
                _SQL = new Microsoft_SQL_Server(_oDatosConexion.Server, _oDatosConexion.Database, _oDatosConexion.Usuario_cliente, _oDatosConexion.Password);
                _SQL.CrearConexion();
                _SQL.AbrirConexion();

                ConexionSQL._Servidor = _oDatosConexion.Server;
                ConexionSQL._BaseDatos = _oDatosConexion.Database;
                ConexionSQL._Usuario = _oDatosConexion.Usuario_cliente;
                ConexionSQL._Password = _oDatosConexion.Password;

                Global.Usuario = ConexionSQL._Usuario;
                Global.Password = ConexionSQL._Password;
                Global.Servidor = ConexionSQL._Servidor;
                Global.BaseDatos = ConexionSQL._BaseDatos;

                oPeriodo = new PeriodoContable(_oDatosConexion.Server, _oDatosConexion.Database, _oDatosConexion.Usuario_cliente, _oDatosConexion.Password);
                cargar_combos();

                //establecerParametrosGlobales();
                _DateConverter = new DateConverter(CultureInfo.CurrentCulture.Name);
                this.Text = string.Format("{0} - {1}/{2}  V.{3}", Application.ProductName, _oDatosConexion.Database, Usuario.userName, Application.ProductVersion);
                txtAnio.Text = DateTime.Now.Year.ToString();

                //_dtTblPeriodosContables = oPeriodo.obtenerPeriodosContables(int.Parse(txtAnio.Text));
                //cargar_combos();

                //Mostrar botón de configuración de diferencia cambiaria
                if (superUsuario == Usuario.userName)
                {
                    configurarPerdidaGananciaCambiariaToolStripMenuItem.Enabled = true;
                }

                //cargar configuración de perdida ganancia
                string cbpg = ConfigurationManager.AppSettings["PG_Intercarmbiada"].ToString();
                PG_Intercarmbiada = cbpg.ToUpper() == "TRUE" ? true : false;

            }
            catch (Exception ex)
            {
                _ArchivoErrores = new LogErrores();
                _ArchivoErrores.escribir("frmPrincipal", "public frmPrincipal(string pConn)", ex.Message);
            }
        }

        private void button_Cargar_Click(object sender, EventArgs e)
        {
           // botones();
            //MessageBox.Show("Los datos se estan cargando ", "...", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            this.picCarga.Visible = true;
            
            toolStripButton1.Enabled = false;
           
            button = false;
            btnBuscar.Enabled = true;
            toolStripButton1.Enabled = false;

            this.picCarga.Visible = true;
            MessageBox.Show("Espere un momento, los datos se estan cargando");
            Thread hilo = new Thread(
                new ThreadStart(() =>
                {

                    
                    //Aqui invoca al grid cual sea
                    DataTable dtTbl_PendientesPorTraspasar3 = new DataTable();
                    string sTransacciones;
                    DataTable dtTblResultado = new DataTable();
                    //DataTable dtTbl_PendientesPorTraspasar2 = new DataTable();

                    sTransacciones = string.Empty;
                    string sPolizasNegativas = string.Empty;
                    string sPolizasContables = "";

                    TreeView forr = new TreeView();

                    
                    string sCurrencyId = "";
                    string sFecInicial = "";
                    string sFecFinal = "";
                    int mes = 0;
                    int anio = 0;

                    this.BeginInvoke(
                        new Action(() =>
                        {

                            this.progressBar1.Value = 0;
                            sTipoTraspaso = rbtnCXC.Checked ? "CXC" : "CXP";
                            sCurrencyId = comboBox_Currency.Text;

                            //Se validan los los campos del periodo
                            //string sFecInicial = _DateConverter.getDate(lblDel.Text);
                            sFecInicial = !(string.IsNullOrEmpty(lblDel.Text)) ? _DateConverter.getDate(lblDel.Text) : "";

                            //string sFecFinal = _DateConverter.getDate(lblAl.Text);
                            sFecFinal = !(string.IsNullOrEmpty(lblAl.Text)) ? _DateConverter.getDate(lblAl.Text) : "";
                            mes = !(string.IsNullOrEmpty(lblAl.Text)) ? int.Parse(_DateConverter.getMonth(lblAl.Text)) : 0;

                            //int anio = int.Parse(txtAnio.Text);
                            //!(string.IsNullOrEmpty(txtAnio.Text))?int.Parse(txtAnio.Text):0;

                            anio = !(string.IsNullOrEmpty(lblAl.Text)) ? int.Parse(DateTime.Parse(lblAl.Text).Year.ToString()) : 0;


                            if (sFecFinal == "" || sFecFinal == "" || mes == 0 || anio == 0)
                            {
                                MessageBox.Show("Revise el periodo contable seleccionado", "Traspaso de IVA", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return;
                            }

                            _FecAl = _DateConverter.getDate(lblAl.Text); //string Global
                            _odtPolizasNegativas = new DataTable(); //DataTable Global

                            //DEBUG EN PRODUCTIVO
                            LogErrores _DEBUG = new LogErrores();
                            _DEBUG.escribir("frmPrincipal", "Private void cargar()", "--START DEBUG");


                            //Se limpia la tabla de detalle o se crea por cada carga para que acumule los sites.
                            if (_odtDetallePolizasTraspasar != null)
                            {
                                _odtDetallePolizasTraspasar.Clear();
                                _odtDetallePolizasTraspasar.Columns.Clear();
                            }
                            else _odtDetallePolizasTraspasar = new DataTable();

                            //Se limpia la tabla de detalle de procesados.
                            if (_odtDetalleProcesados != null)
                            {
                                _odtDetalleProcesados.Clear();
                                _odtDetalleProcesados.Columns.Clear();
                            }
                            else _odtDetalleProcesados = new DataTable();

                            toolStripButton1.Enabled = false;


                        }
                    ));
                    //MensajeTm.Show("Texto", "Titulo", 10, false);
                    
                    frmProcesando frp = new frmProcesando(10, "Cargando");

                    this.BeginInvoke(
                        new Action(() =>
                        {
                            // consulta
                            //frmProcesando frp = new frmProcesando(100, " h");
                            
                            foreach (TreeNode site in treeSite.Nodes)
                            {
                                //    //this.progressBar1.Value += this.progressBar1.Maximum / (6+treeSite.Nodes.Count);

                                //if (!site.Checked) continue;
                                string id_site = site.Name;

                                //toolStripButton1.Enabled = false;
                                // Pestaña Por Traspasar
                                //dtTbl_PendientesPorTraspasar = new DataTable();
                                //ThreadStart inicio = new ThreadStart();
                                dtTbl_PendientesPorTraspasar.Merge(porTraspasar(sTipoTraspaso, sFecFinal, anio, mes, id_site, out sPolizasNegativas, sCurrencyId));
                                //this.progressBar1.Value += this.progressBar1.Maximum / (6 + treeSite.Nodes.Count);

                                //dtTbl_PendientesPorTraspasar.Merge(porTraspasar(sTipoTraspaso, sFecFinal, anio, mes, id_site, out sPolizasNegativas, sCurrencyId));
                                //dtTbl_PendientesPorTraspasar = dtTbl_PendientesPorTraspasar2;

                                // Pestaña Negativos
                                _odtPolizasNegativas.Merge(_SQL.EjecutarConsulta(sPolizasNegativas, "Documentos"));
                                //this.progressBar1.Value += this.progressBar1.Maximum / (6 + treeSite.Nodes.Count);

                                // Resultado
                                sPolizasContables = MapeoQuerySql.ObtenerPorId("frmPrincipal.button_Cargar_Click"); //busca las polizas ya traspazadas en VMXCONTROLPOLIZA (?)
                                sPolizasContables = string.Format(sPolizasContables, sFecInicial, sFecFinal, sTipoTraspaso, id_site);
                                dtTblResultado.Merge(_SQL.EjecutarConsulta(sPolizasContables, "Transacciones"));
                                //this.progressBar1.Value += this.progressBar1.Maximum / (6 + treeSite.Nodes.Count);

                                foreach (DataRow dr in dtTblResultado.Rows)
                                    if (dr["NO_TRANSACCION"].ToString().Length > 0)
                                        sTransacciones = sTransacciones + "'" + dr["NO_TRANSACCION"].ToString() + "',";
                                //this.progressBar1.Value += this.progressBar1.Maximum / (6 + treeSite.Nodes.Count);

                                //Pestaña Procesados
                                dtTblProcesados.Merge(Procesados(sTipoTraspaso, id_site, sTransacciones, out sTransacciones)); //busca los vaucher ya traspazados de sTransacciones
                                                                                                                               //this.progressBar1.Value += this.progressBar1.Maximum / (6 + treeSite.Nodes.Count);

                                //poliza
                                poliza.Merge(mostrarPoliza(sFecInicial, sFecFinal, sTipoTraspaso, id_site));
                                //this.progressBar1.Value += this.progressBar1.Maximum/(6 + treeSite.Nodes.Count);

                            }

                            dtTbl_PendientesPorTraspasar2.Merge(dtTbl_PendientesPorTraspasar3);
                            // LLenado grid
                            gvPoliza.DataSource = poliza;

                            gvNegativos.DataSource = _odtPolizasNegativas;
                            gvNegativos.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.ColumnHeader);
                            txtNegativos.Text = gvNegativos.RowCount.ToString();

                            gvProcesados.DataSource = dtTblProcesados;

                            txtProcesados.Text = gvProcesados.RowCount.ToString();
                            formatoGridProcesados(sTipoTraspaso);

                            //Ordena movimientos por traspasar
                            DataView dv = dtTbl_PendientesPorTraspasar.DefaultView;
                            dv.Sort = "BANK_ACCOUNT_ID ASC, CONTROL_NO ASC";
                            dtTbl_PendientesPorTraspasar = dv.ToTable();
                            gvMovPorTraspasar.DataSource = dtTbl_PendientesPorTraspasar;
                            txtPorTraspasar.Text = gvMovPorTraspasar.RowCount.ToString();
                            formatoGridPorTraspasar(sTipoTraspaso);
                            toolStripButton1.Enabled = true;
                            
                        }
                    ));
                    
                }
                ));
            //
            
            //this.picCarga.Visible = true;
            hilo.Start();

            // hilo.Start();
            //cargar();
            //this.picCarga.Visible = true;
            picCarga.Visible = false;
            toolStripButton1.Enabled = true;
            txtFiltro.Text = "";
            comboCampos.SelectedIndex = 0;
            if (comboCampos.SelectedIndex == 0)
            {
                btnBuscar.Enabled = false;

            }
            else
            {
                btnBuscar.Enabled = true;
            }

        }

        /// <summary>
        /// Permite mostrar todos los movimientos realizados en el periodo.
        /// </summary>
        private void cargar()
        {
            string sTransacciones;
            DataTable dtTblResultado = new DataTable();
            //DataTable poliza = new DataTable();
            //DataTable dtTblProcesados = new DataTable();
            DataTable dtTbl_PendientesPorTraspasar2 = new DataTable();


            sTransacciones = string.Empty;
            string sPolizasNegativas = string.Empty;
            string sPolizasContables = "";

            this.progressBar1.Value = 0;
            sTipoTraspaso = rbtnCXC.Checked ? "CXC" : "CXP";
            string sCurrencyId = this.comboBox_Currency.Text;

            //Se validan los los campos del periodo
            //string sFecInicial = _DateConverter.getDate(lblDel.Text);
            string sFecInicial = !(string.IsNullOrEmpty(lblDel.Text)) ? _DateConverter.getDate(lblDel.Text) : "";

            //string sFecFinal = _DateConverter.getDate(lblAl.Text);
            string sFecFinal = !(string.IsNullOrEmpty(lblAl.Text)) ? _DateConverter.getDate(lblAl.Text) : "";
            int mes = !(string.IsNullOrEmpty(lblAl.Text)) ? int.Parse(_DateConverter.getMonth(lblAl.Text)) : 0;

            //int anio = int.Parse(txtAnio.Text);
            //!(string.IsNullOrEmpty(txtAnio.Text))?int.Parse(txtAnio.Text):0;

            int anio = !(string.IsNullOrEmpty(lblAl.Text)) ? int.Parse(DateTime.Parse(lblAl.Text).Year.ToString()) : 0;


            if (sFecFinal == "" || sFecFinal == "" || mes == 0 || anio == 0)
            {
                MessageBox.Show("Revise el periodo contable seleccionado", "Traspaso de IVA", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }



            _FecAl = _DateConverter.getDate(lblAl.Text); //string Global
            _odtPolizasNegativas = new DataTable(); //DataTable Global

            //DEBUG EN PRODUCTIVO
            LogErrores _DEBUG = new LogErrores();
            _DEBUG.escribir("frmPrincipal", "Private void cargar()", "--START DEBUG");

            try
            {

                //Se limpia la tabla de detalle o se crea por cada carga para que acumule los sites.
                if (_odtDetallePolizasTraspasar != null)
                {
                    _odtDetallePolizasTraspasar.Clear();
                    _odtDetallePolizasTraspasar.Columns.Clear();
                }
                else _odtDetallePolizasTraspasar = new DataTable();

                //Se limpia la tabla de detalle de procesados.
                if (_odtDetalleProcesados != null)
                {
                    _odtDetalleProcesados.Clear();
                    _odtDetalleProcesados.Columns.Clear();
                }
                else _odtDetalleProcesados = new DataTable();


                foreach (TreeNode site in treeSite.Nodes)
                {
                    //this.progressBar1.Value += this.progressBar1.Maximum / (6+treeSite.Nodes.Count);

                    if (!site.Checked) continue;
                    string id_site = site.Name;

                    //toolStripButton1.Enabled = false;
                    // Pestaña Por Traspasar
                    dtTbl_PendientesPorTraspasar = new DataTable();
                    //ThreadStart inicio = new ThreadStart();
                    dtTbl_PendientesPorTraspasar2.Merge(porTraspasar(sTipoTraspaso, sFecFinal, anio, mes, id_site, out sPolizasNegativas, sCurrencyId));
                    //this.progressBar1.Value += this.progressBar1.Maximum / (6 + treeSite.Nodes.Count);
                    dtTbl_PendientesPorTraspasar = dtTbl_PendientesPorTraspasar2;
                    // Pestaña Negativos
                    _odtPolizasNegativas.Merge(_SQL.EjecutarConsulta(sPolizasNegativas, "Documentos"));
                    //this.progressBar1.Value += this.progressBar1.Maximum / (6 + treeSite.Nodes.Count);

                    // Resultado
                    sPolizasContables = MapeoQuerySql.ObtenerPorId("frmPrincipal.button_Cargar_Click"); //busca las polizas ya traspazadas en VMXCONTROLPOLIZA (?)
                    sPolizasContables = string.Format(sPolizasContables, sFecInicial, sFecFinal, sTipoTraspaso, id_site);
                    dtTblResultado.Merge(_SQL.EjecutarConsulta(sPolizasContables, "Transacciones"));
                    //this.progressBar1.Value += this.progressBar1.Maximum / (6 + treeSite.Nodes.Count);

                    foreach (DataRow dr in dtTblResultado.Rows)
                        if (dr["NO_TRANSACCION"].ToString().Length > 0)
                            sTransacciones = sTransacciones + "'" + dr["NO_TRANSACCION"].ToString() + "',";
                    //this.progressBar1.Value += this.progressBar1.Maximum / (6 + treeSite.Nodes.Count);

                    //Pestaña Procesados
                    dtTblProcesados.Merge(Procesados(sTipoTraspaso, id_site, sTransacciones, out sTransacciones)); //busca los vaucher ya traspazados de sTransacciones
                                                                                                                   //this.progressBar1.Value += this.progressBar1.Maximum / (6 + treeSite.Nodes.Count);

                    //poliza
                    poliza.Merge(mostrarPoliza(sFecInicial, sFecFinal, sTipoTraspaso, id_site));
                    //this.progressBar1.Value += this.progressBar1.Maximum/(6 + treeSite.Nodes.Count);

                }

                gvPoliza.DataSource = poliza;

                gvNegativos.DataSource = _odtPolizasNegativas;
                gvNegativos.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.ColumnHeader);
                txtNegativos.Text = gvNegativos.RowCount.ToString();

                gvProcesados.DataSource = dtTblProcesados;

                txtProcesados.Text = gvProcesados.RowCount.ToString();
                formatoGridProcesados(sTipoTraspaso);




                //Ordena movimientos por traspasar
                DataView dv = dtTbl_PendientesPorTraspasar2.DefaultView;
                dv.Sort = "BANK_ACCOUNT_ID ASC, CONTROL_NO ASC";
                dtTbl_PendientesPorTraspasar2 = dv.ToTable();
                gvMovPorTraspasar.DataSource = dtTbl_PendientesPorTraspasar2;
                txtPorTraspasar.Text = gvMovPorTraspasar.RowCount.ToString();
                formatoGridPorTraspasar(sTipoTraspaso);
                toolStripButton1.Enabled = true;
                //}

            }
            catch (Exception ex)
            {
                toolStripButton1.Enabled = true;
                MessageBox.Show("Ocurrio un error al obtener los movimientos.", "Error del sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _ArchivoErrores = new LogErrores();
                _ArchivoErrores.escribir("frmPrincipal", "Private void cargar()", ex.Message);
            }
            //this.progressBar1.Value = this.progressBar1.Maximum;
        }


        private DataTable porTraspasar(string sTipoTraspaso, string sFecFinal, int anio, int mes, string id_site, out string sPolizasNegativas, string sCurrencyId)
        {
            DataTable dt = new DataTable();
            sPolizasNegativas = "";

            try
            {
                /*
                if (_odtDetallePolizasTraspasar != null)
                {
                    _odtDetallePolizasTraspasar.Clear();
                    _odtDetallePolizasTraspasar.Columns.Clear();
                }
                else
                {
                    _odtDetallePolizasTraspasar = new DataTable();
                }
                */

                if (sTipoTraspaso == "CXC")
                {
                    CuentasPorCobrar oCXC = new CuentasPorCobrar(_oDatosConexion);
                    //Almacena el query que va a ejecutar
                    sPolizasNegativas = chkConsiliados.Checked ? ConsultaCXC.conciliadosNegativos(sFecFinal, anio, mes, id_site) : ConsultaCXC.noConciliadosNegativos(sFecFinal, anio, mes, id_site);
                    dt.Merge(oCXC.obtenerMovimientos(anio, mes, sFecFinal, chkConsiliados.Checked ? 1 : 0, id_site, sCurrencyId, chkSoloPosteados.Checked ? 1 : 0));

                    //Detalle
                    _odtDetallePolizasTraspasar.Merge(oCXC.obtenerMovimientosDetalle(anio, mes, sFecFinal, chkConsiliados.Checked ? 1 : 0, id_site, sCurrencyId, chkSoloPosteados.Checked ? 1 : 0));
                }
                else
                {

                    CuentasPorPagar oCXP = new CuentasPorPagar(_oDatosConexion);
                    sPolizasNegativas = chkConsiliados.Checked ? ConsultaCXP.conciliadosNegativos(anio, mes, id_site) : ConsultaCXP.noConciliadosNegativos(sFecFinal, anio, mes, id_site);

                    //agrupadas
                    dt.Merge(oCXP.obtenerMovimientosTipo(anio, mes, sFecFinal, chkConsiliados.Checked ? 1 : 0, id_site, "P", sCurrencyId, PG_Intercarmbiada == true ? 1 : 0, chkSoloPosteados.Checked ? 1 : 0)); //polizas
                    dt.Merge(oCXP.obtenerMovimientosTipo(anio, mes, sFecFinal, chkConsiliados.Checked ? 1 : 0, id_site, "R", sCurrencyId, PG_Intercarmbiada == true ? 1 : 0, chkSoloPosteados.Checked ? 1 : 0)); //retenciones

                    //detalle
                    _odtDetallePolizasTraspasar.Merge(oCXP.obtenerMovimientosTipo(anio, mes, sFecFinal, chkConsiliados.Checked ? 1 : 0, id_site, "PD", sCurrencyId, PG_Intercarmbiada == true ? 1 : 0, chkSoloPosteados.Checked ? 1 : 0));//Polizas
                    _odtDetallePolizasTraspasar.Merge(oCXP.obtenerMovimientosTipo(anio, mes, sFecFinal, chkConsiliados.Checked ? 1 : 0, id_site, "RD", sCurrencyId, PG_Intercarmbiada == true ? 1 : 0, chkSoloPosteados.Checked ? 1 : 0));//Retenciones

                }
                if (dt.Rows.Count > 0) this.progressBar1.Maximum = dt.Rows.Count;
            }
            catch (Exception e)
            {
                MessageBox.Show("Ocurrio un error al obtener los movimientos por traspasar.", "Error del sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _ArchivoErrores = new LogErrores();
                _ArchivoErrores.escribir("frmPrincipal", "DataTable porTraspasar()", e.Message);
            }
            return dt;
        }


        private void formatoGridPorTraspasar(string sTipoTraspaso)
        {
            if (gvMovPorTraspasar.Rows.Count > 0 || gvMovPorTraspasar.Columns.Count > 0)
            {
                // Establecer Titulos del Grid
                if (sTipoTraspaso == "CXC")
                {
                    gvMovPorTraspasar.Columns["CONTROL_NO"].HeaderText = "No. Cheque";
                    gvMovPorTraspasar.Columns["CLIENTE"].HeaderText = "Cliente";
                    gvMovPorTraspasar.Columns["RET_MXN_TRASLADAR_FACT"].Visible = false;
                    gvMovPorTraspasar.Columns["RET_MXN_TRASLADAR_DEP"].Visible = false;
                }
                else
                {
                    gvMovPorTraspasar.Columns["CONTROL_NO"].HeaderText = "No. Control";
                    gvMovPorTraspasar.Columns["CLIENTE"].HeaderText = "Proveedor";
                    gvMovPorTraspasar.Columns["RET_MXN_TRASLADAR_FACT"].Visible = true;
                    gvMovPorTraspasar.Columns["RET_MXN_TRASLADAR_DEP"].Visible = true;
                }
                gvMovPorTraspasar.Columns["BANK_ACCOUNT_ID"].HeaderText = "Banco";
                gvMovPorTraspasar.Columns["VAT_GL_ACCT_ID"].HeaderText = "Cuenta";
                gvMovPorTraspasar.Columns["DESCRIPCION"].HeaderText = "Descripción";
                gvMovPorTraspasar.Columns["TRASLADO"].HeaderText = "Cuenta Traslado";
                gvMovPorTraspasar.Columns["IVA_MXN_TRASLADAR_FACT"].HeaderText = "IVA Factura";
                gvMovPorTraspasar.Columns["RET_MXN_TRASLADAR_FACT"].HeaderText = "Retención Factura";

                if (Global.TipoCambioPagos.Equals("FACTURA"))
                {
                    gvMovPorTraspasar.Columns["TC_FACTURA"].Visible = false;
                    gvMovPorTraspasar.Columns["TC_DEPOSITO"].Visible = false;
                    gvMovPorTraspasar.Columns["IVA_MXN_TRASLADAR_DEP"].Visible = false;
                    gvMovPorTraspasar.Columns["RET_MXN_TRASLADAR_DEP"].Visible = false;
                    gvMovPorTraspasar.Columns["PERDIDA_GANANCIA"].Visible = false;
                    gvMovPorTraspasar.Columns["CUENTA_PER_GANANCIA"].Visible = false;
                }
                else
                {
                    gvMovPorTraspasar.Columns["TC_FACTURA"].HeaderText = "Tipo Cambio Factura";
                    gvMovPorTraspasar.Columns["TC_DEPOSITO"].HeaderText = "Tipo Cambio Deposito";
                    gvMovPorTraspasar.Columns["IVA_MXN_TRASLADAR_DEP"].HeaderText = "IVA Deposito";
                    gvMovPorTraspasar.Columns["RET_MXN_TRASLADAR_DEP"].HeaderText = "Retención Deposito";
                    gvMovPorTraspasar.Columns["PERDIDA_GANANCIA"].HeaderText = "Perdida Ganancia";
                    gvMovPorTraspasar.Columns["CUENTA_PER_GANANCIA"].HeaderText = "Cuenta Perdida Ganancia";
                }

                gvMovPorTraspasar.Columns["FECHA"].HeaderText = "Fecha";
            }
        }

        private DataTable Procesados(string sTipoTraspaso, string id_site, string sTransacciones, out string sTrans)
        {
            DataTable dt = new DataTable();
            sTrans = "";

            if (sTransacciones.Length > 0)
            {
                try
                {
                    sTrans = sTransacciones.Substring(0, sTransacciones.Length - 1);
                    string sConsultaProcesados = string.Empty;
                    string sCons = string.Empty;
                    string sDetalle = string.Empty;
                    //gvProcesados.DataSource = null;

                    if (sTipoTraspaso == "CXC")
                    {
                        sCons = ConsultaCXC.procesados(sTrans, id_site, out sDetalle);
                    }
                    else
                        sCons = ConsultaCXP.procesados(sTrans, id_site, out sDetalle);

                    dt.Merge(_SQL.EjecutarConsulta(sCons, "Procesados"));

                    //Ejecuta el detalle de los procesados
                    sDetalle = string.Format(sDetalle, id_site, sTipoTraspaso, sTrans);
                    _odtDetalleProcesados.Merge(_SQL.EjecutarConsulta(sDetalle, "Detalle"));
                }
                catch (Exception e)
                {
                    MessageBox.Show("Ocurrio un error al obtener los movimientos procesados.", "Error del sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _ArchivoErrores = new LogErrores();
                    _ArchivoErrores.escribir("frmPrincipal", "DataTable Procesados()", e.Message);
                }

            }
            else
                gvProcesados.DataSource = null;
            return dt;
        }

        private void formatoGridProcesados(string sTipoTraspaso)
        {
            // Se concentra la información de las entidades en datatable dtTblProcesados
            if (gvProcesados.Rows.Count > 0)
            {
                if (sTipoTraspaso == "CXC")
                {
                    gvProcesados.Columns["CHECK_ID"].HeaderText = "No. Cheque";
                    gvProcesados.Columns["CUSTOMER_ID"].HeaderText = "Cliente";
                    gvProcesados.Columns["Nombre"].Visible = true;
                }
                else
                {
                    gvProcesados.Columns["CONTROL_NO"].HeaderText = "No. Control";
                    gvProcesados.Columns["MONTO_RET"].HeaderText = "Retención Factura";
                    gvProcesados.Columns["RET_DEPOSITO"].HeaderText = "Retención Depósito";
                }


                // Establecer Titulos del Grid  
                gvProcesados.Columns["NO_TRANSACCION"].HeaderText = "No. Transacción";
                gvProcesados.Columns["BANK_ACCOUNT_ID"].HeaderText = "Banco";
                gvProcesados.Columns["CUENTA"].HeaderText = "Cuenta Origen";
                gvProcesados.Columns["DESCRIPCION"].HeaderText = "Descripción";
                gvProcesados.Columns["CUENTA_TRASLADO"].HeaderText = "Cuenta Destino";
                gvProcesados.Columns["MONTO"].HeaderText = "IVA Factura";
                gvProcesados.Columns["FECHA_PERIODO"].HeaderText = "Fecha";

                if (Global.TipoCambioPagos.Equals("FACTURA"))
                {
                    gvProcesados.Columns["IVA_DEPOSITO"].Visible = false; ;
                    gvProcesados.Columns["PERDIDA_GANACIA"].Visible = false;
                }
                else
                {
                    gvProcesados.Columns["IVA_DEPOSITO"].Visible = true;
                    gvProcesados.Columns["PERDIDA_GANACIA"].Visible = true;
                    gvProcesados.Columns["IVA_DEPOSITO"].HeaderText = "IVA Deposito";
                    gvProcesados.Columns["PERDIDA_GANACIA"].HeaderText = "Perdida Ganancia";
                }

            }
        }



        private DataTable mostrarPoliza(string sFecInicial, string sFecFinal, string sTipoTraspaso, string site)
        {
            DataTable DT;
            string sCons = string.Empty;
            sCons = MapeoQuerySql.ObtenerPorId("frmPrincipal.button_Cargar_Click.mostrarPoliza");
            sCons = string.Format(sCons, sFecInicial, sFecFinal, sTipoTraspaso, site);
            DT = _SQL.EjecutarConsulta(sCons, "Procesados");
            return DT;
        }


        private bool validaciones()
        {
            bool esValido = true;

            // Tab Page movimientos por traspasr
            if (tbTraspasos.SelectedIndex != 0)
                return false;

            if (!validarNumeroRegistros())
                return false;

            // Validar si van a hacer traspasos negativos
            if (gvNegativos.Rows.Count > 0)
            {
                DialogResult res = MessageBox.Show("Existen traspasos con monto negativo. ¿Desea continuar?", "Traspaso de IVA", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                esValido = (res == DialogResult.OK) ? true : false;
            }

            return esValido;
        }

        private DataTable procesarTraspasos(string site)
        {
            string sAgruparTraspasos = MapeoQuerySql.ObtenerPorId("frmPrincipal.procesarTraspasos");
            return _SQL.EjecutarConsulta(string.Format(sAgruparTraspasos, site), "VMX_IVATRASTEMP_Agrupado");
        }


        /// <summary>
        /// Realiza el traspaso de iva de todos los movimientos en el mes.
        /// 
        /// Cuentas afectadas:
        ///     CxC.- Cuentas por Cobrar. 
        ///     CxP.- Cuentas por Pagar
        /// </summary>       
        private void button_Traspaso_Click(object sender, EventArgs e)
        {


            //Revisa el periodo de las polizas seleccionadas
            foreach (DataGridViewRow row in gvMovPorTraspasar.SelectedRows)
            {
                try
                {
                    //if ((DateTime.Parse(row.Cells["FECHA"].Value.ToString())).Month != int.Parse(cmbPeriodos.Text)) genera error en clientes con periodods contables desfazados
                    if ((DateTime.Parse(row.Cells["FECHA"].Value.ToString())) < DateTime.Parse(lblDel.Text.ToString()) || (DateTime.Parse(row.Cells["FECHA"].Value.ToString())) > DateTime.Parse(lblAl.Text))
                    {
                        MessageBox.Show("El periodo seleccionado difiere del periodo de las pólizas por traspasar", "Traspaso de IVA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show("Ocurrio un error al revisar periodos del traspaso.", "Error del sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _ArchivoErrores = new LogErrores();
                    _ArchivoErrores.escribir("frmPrincipal", "button_Traspaso_Click", exc.Message);
                }
            }

            try
            {
                //Inserta en una tabla temporal las cuentas a procesar
                insertarTraspasosAProcesar();

                DataTable polizas = (gvMovPorTraspasar.DataSource as DataTable).Clone();
                foreach (DataGridViewRow row in gvMovPorTraspasar.SelectedRows)
                    polizas.ImportRow(((DataRowView)row.DataBoundItem).Row);

                DataTable sites = polizas.AsEnumerable().GroupBy(r => r["SITE_ID"]).Select(g => g.First()).CopyToDataTable();

                foreach (DataRow row in sites.Rows)
                    generarPoliza(row["SITE_ID"].ToString(), procesarTraspasos(row["SITE_ID"].ToString()), rbtnCXC.Checked ? "CXC" : "CXP");

            }
            catch (Exception ex)
            {
                string s = (ex.Message.Contains("Linea de p&oacute;liza.")) ? s = "\nEl total del pago difiere al total de la factura." : "";

                MessageBox.Show("Ocurrio un error al realizar el traspaso." + s, "Error del sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _ArchivoErrores = new LogErrores();
                _ArchivoErrores.escribir("frmPrincipal", "button_Traspaso_Click", ex.Message);
            }


        }

        private void insertarTraspasosAProcesar()
        {

            // Eliminar todos los traspasos temporales
            _ArchivoErrores = new LogErrores();
            _ArchivoErrores.escribir("frmPrincipal", "button_Traspaso_Click", "1");
            string query = MapeoQuerySql.ObtenerPorId("frmPrincipal.procesarTraspasos.delIVATRASTEMP");
            _SQL.EjecutarDML(string.Format(query));
            _ArchivoErrores.escribir("frmPrincipal", "button_Traspaso_Click", "2");
            List<string> lstBanco_NoControl = new List<string>();
            _dtTraspasos_Agrupados_Resumen = new DataTable();
            _dtTraspasos_Seleccionados_Detalle = new DataTable("VMX_IVATRASTEMP_Detalle");
            _ArchivoErrores.escribir("frmPrincipal", "button_Traspaso_Click", "3");
            // Copiar columnas al DataTable
            /*foreach (DataGridViewColumn dgr in gvMovPorTraspasar.Columns)
                _dtTraspasos_Seleccionados_Detalle.Columns.Add(dgr.Name);*/
            _ArchivoErrores.escribir("frmPrincipal", "button_Traspaso_Click", "4");
            foreach (DataGridViewRow item in gvMovPorTraspasar.SelectedRows)
            {
                string sBanco = item.Cells["BANK_ACCOUNT_ID"].Value.ToString();
                string sNoControl = item.Cells["CONTROL_NO"].Value.ToString();

                // No procesar nuevamente el mismo Banco y NoControl
                if (!lstBanco_NoControl.Contains(sBanco + sNoControl))
                {
                    // Obtener todo lo del banco y No Control
                    foreach (DataGridViewRow traspaso in gvMovPorTraspasar.Rows)
                    {
                        string monto_fact = "0.00";
                        string monto_dep = "0.00";

                        if (sBanco.Equals(traspaso.Cells["BANK_ACCOUNT_ID"].Value.ToString()) && sNoControl.Equals(traspaso.Cells["CONTROL_NO"].Value.ToString()))
                        {
                            /*object[] arrColumnas = new object[traspaso.Cells.Count];
                            int iColumn = 0;

                            foreach (DataGridViewCell cell in traspaso.Cells)
                            {
                                arrColumnas[iColumn] = cell.Value;
                                iColumn++;
                            }

                            // Guardar el traspaso actual en DataTable
                            _dtTraspasos_Seleccionados_Detalle.Rows.Add(arrColumnas);*/

                            string sConsulta = MapeoQuerySql.ObtenerPorId("frmPrincipal.procesarTraspasos.insIVATRASTEMP");

                            //Revisión si el movimiento es de retención o de iva
                            if (!Decimal.Parse(traspaso.Cells["RET_MXN_TRASLADAR_FACT"].Value.ToString()).Equals(0) && Decimal.Parse(traspaso.Cells["IVA_MXN_TRASLADAR_FACT"].Value.ToString()).Equals(0))
                            {
                                //Es una retención cuando el monto de la retención no sea 0 y el de iva sí lo sea.
                                monto_fact = traspaso.Cells["RET_MXN_TRASLADAR_FACT"].Value.ToString();
                                monto_dep = traspaso.Cells["RET_MXN_TRASLADAR_DEP"].Value.ToString();
                            }
                            else
                            {
                                monto_fact = traspaso.Cells["IVA_MXN_TRASLADAR_FACT"].Value.ToString();
                                monto_dep = traspaso.Cells["IVA_MXN_TRASLADAR_DEP"].Value.ToString();
                            }

                            sConsulta = string.Format(sConsulta, traspaso.Cells["VAT_GL_ACCT_ID"].Value.ToString(), traspaso.Cells["TRASLADO"].Value.ToString(), monto_fact, monto_dep, traspaso.Cells["SITE_ID"].Value.ToString());
                            _SQL.EjecutarDML(sConsulta);
                        }
                    }
                    lstBanco_NoControl.Add(sBanco + sNoControl);
                }
            }
        }

        private void configurarIVA()
        {
            try
            {
                // _SQL = new Microsoft_SQL_Server(_oDatosConexion.Server, _oDatosConexion.Database, _oDatosConexion.Usuario_cliente, _oDatosConexion.Password);
                // _SQL.CrearConexion();
                // _SQL.AbrirConexion();

                //UtilImpuestos utilidades_impuestos = new UtilImpuestos(_SQL._oConn);
                string tipo = rbtnCXC.Checked ? "CXC" : "CXP";

                if (!(tipo == ""))
                    new frmCatCuentas(tipo, _SQL._oConn).ShowDialog();

                // _SQL.CerrarConexion();
                // _SQL.DestruirConexion();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrio un error en la aplicación.", "Error del sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _ArchivoErrores = new LogErrores();
                _ArchivoErrores.escribir("frmPrincipal", "button_Configuracion_Click", ex.Message);
            }
        }

        private void configurarRetenciones()
        {
            frmCRetenciones fRet = new frmCRetenciones(_oDatosConexion, _ArchivoErrores);
            fRet.Show();
        }

        private void button_Configuracion_Click(object sender, EventArgs e)
        {

        }

        private DataTable obtenerPolizas()
        {
            List<string> lstBanco_NoControl = new List<string>();
            //_dtTraspasos_Agrupados_Resumen = new DataTable();
            //_dtTraspasos_Seleccionados_Detalle = new DataTable("VMX_IVATRASTEMP_Detalle");

            // Copiar columnas al DataTable
            /*foreach (DataGridViewColumn dgr in gvMovPorTraspasar.Columns)
                _dtTraspasos_Seleccionados_Detalle.Columns.Add(dgr.Name);*/
            DataTable dt = ((DataTable)gvMovPorTraspasar.DataSource).Clone();
            foreach (DataGridViewRow item in gvMovPorTraspasar.SelectedRows)
            {
                string sBanco = item.Cells["BANK_ACCOUNT_ID"].Value.ToString();
                string sNoControl = item.Cells["CONTROL_NO"].Value.ToString();

                // No procesar nuevamente el mismo Banco y NoControl
                if (!lstBanco_NoControl.Contains(sBanco + sNoControl))
                {
                    // Obtener todo lo del banco y No Control
                    foreach (DataGridViewRow traspaso in gvMovPorTraspasar.Rows)
                        if (sBanco.Equals(traspaso.Cells["BANK_ACCOUNT_ID"].Value.ToString()) && sNoControl.Equals(traspaso.Cells["CONTROL_NO"].Value.ToString()))
                            dt.ImportRow(((DataRowView)traspaso.DataBoundItem).Row);
                    lstBanco_NoControl.Add(sBanco + sNoControl);
                }
            }
            return dt;
        }

        private void getTotalIVA(DataTable polizas_site, out double dTotal_IVA_Factura, out double dTotal_IVA_Pago)
        {
            dTotal_IVA_Factura = 0;
            dTotal_IVA_Pago = 0;

            // Sumar los montos de los traspasos para obtener el total de la póliza
            foreach (DataRow rowTraspaso in polizas_site.Rows)
            {
                dTotal_IVA_Factura += Math.Round(double.Parse(rowTraspaso["MONTO"].ToString()), 2);
                dTotal_IVA_Pago += Math.Round(double.Parse(rowTraspaso["MONTO_IVA_DEPOSITO"].ToString()), 2);
            }
        }

        private bool esMontoPolizaMayorCero(double pTotalPoliza)
        {
            if (pTotalPoliza <= 0)
            {
                MessageBox.Show("No se generó ninguna póliza debido a que el monto a traspasar es cero.", "Estatus de Póliza", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            return true;
        }

        public void generarPoliza(string id_site, DataTable polizas_site, string sTipoTraspaso)
        {
            if (!validaciones()) return;

            //Obtiene los totales para la poliza sumando las cuentas.
            double dTotal_IVA_Factura = 0;
            double dTotal_IVA_Pago = 0;
            getTotalIVA(polizas_site, out dTotal_IVA_Factura, out dTotal_IVA_Pago);

            //Obtiene del grid las polizas agrupadas a procesar por banco y número de control
            DataTable dt = obtenerPolizas();

            if (!esMontoPolizaMayorCero(dTotal_IVA_Factura)) return;

            GJ oGJ = new GJ();

            //Obtener el siguiente id de poliza por site.
            //string journal = oGJ.getNext_ID(id_site);
            string journal = oGJ.getMax_GJ_ID(ConfigurationManager.AppSettings["PrefijoID_GJ"].ToString(), id_site);


            //LLENAR VALORES PARA INSERCIÓN?
            oGJ.GJ_DATE = _DateConverter.getDate(lblAl.Text);
            oGJ.DESCRIPTION = string.Format("Traspaso de IVA {0} Mes: {1} A\x00f1o: {2}.", sTipoTraspaso, _DateConverter.getMonth(lblAl.Text), txtAnio.Text);
            oGJ.POSTING_DATE = _DateConverter.getDate(lblAl.Text);
            oGJ.CREATE_DATE = _DateConverter.getDate(DateTime.Now.ToShortDateString());
            oGJ.USER_ID = Usuario.userName;
            oGJ.ENTITY_ID = this.comboBox_Entity.Text;
            oGJ.CURRENCY_ID = comboBox_Currency.Text;
            oGJ.CURRENCY_NATIVE = "";
            oGJ.SITE_ID = id_site;

            if (sTipoTraspaso.Equals("CXC"))
            {
                if (Global.TipoCambioPagos.Equals("FACTURA"))
                {
                    oGJ.TOTAL_DR_AMOUNT = dTotal_IVA_Factura;
                    oGJ.TOTAL_CR_AMOUNT = dTotal_IVA_Factura;
                }
                else
                {
                    PerdidaGanancia oPG = new PerdidaGanancia();
                    oPG = oPG.getPerdidaGanancia(dt.AsEnumerable().Where(r => r["SITE_ID"].ToString() == id_site).CopyToDataTable());
                    // IVA POR PAGAR = Total IVA Factura + Ganancia
                    //oGJ.TOTAL_DR_AMOUNT = Math.Round(dTotal_IVA_Factura + oPG.Ganancia, 2);
                    oGJ.TOTAL_DR_AMOUNT = Math.Round(dTotal_IVA_Factura + oPG.Perdida, 2);

                    // IVA PAGADO = Total IVA Pago + Perdida
                    //oGJ.TOTAL_CR_AMOUNT = Math.Round(dTotal_IVA_Pago + oPG.Perdida, 2);
                    oGJ.TOTAL_CR_AMOUNT = Math.Round(dTotal_IVA_Pago + oPG.Ganancia, 2);
                }
            }
            else
            {
                if (Global.TipoCambioPagos.Equals("FACTURA"))
                {
                    oGJ.TOTAL_DR_AMOUNT = dTotal_IVA_Factura;
                    oGJ.TOTAL_CR_AMOUNT = dTotal_IVA_Factura;
                }
                else
                {
                    PerdidaGanancia oPG = new PerdidaGanancia();
                    oPG = oPG.getPerdidaGanancia(dt.AsEnumerable().Where(r => r["SITE_ID"].ToString() == id_site).CopyToDataTable());
                    // IVA POR PAGAR = Total IVA Factura + Perdida

                    if (PG_Intercarmbiada)
                    {
                        oGJ.TOTAL_DR_AMOUNT = Math.Round(dTotal_IVA_Pago + oPG.Perdida, 2);
                        oGJ.TOTAL_CR_AMOUNT = Math.Round(dTotal_IVA_Factura + oPG.Ganancia, 2);
                    }
                    else
                    {
                        oGJ.TOTAL_DR_AMOUNT = Math.Round(dTotal_IVA_Factura + oPG.Perdida, 2);
                        oGJ.TOTAL_CR_AMOUNT = Math.Round(dTotal_IVA_Pago + oPG.Ganancia, 2);
                    }
                }
            }

            // Cuadrar poliza por menos de 1 peso de diferencia
            if (oGJ.TOTAL_CR_AMOUNT != oGJ.TOTAL_DR_AMOUNT)
            {
                // Se hace la asignación vacia de la variable que va a cuadrar las diferencias
                double Diferencia_Amount = 0.00;
                //diferencia = oGJ.TOTAL_CR_AMOUNT > oGJ.TOTAL_DR_AMOUNT ? diferencia = oGJ.TOTAL_CR_AMOUNT - oGJ.TOTAL_DR_AMOUNT : diferencia = oGJ.TOTAL_DR_AMOUNT - oGJ.TOTAL_CR_AMOUNT;
                //Se obtiene la diferencia entre las variables TOTAL_CR_AMOUNT & TOTAL_DR_AMOUNT
                if (oGJ.TOTAL_CR_AMOUNT > oGJ.TOTAL_DR_AMOUNT)
                    Diferencia_Amount = Math.Round(oGJ.TOTAL_CR_AMOUNT - oGJ.TOTAL_DR_AMOUNT, 2);
                else
                    Diferencia_Amount = Math.Round(oGJ.TOTAL_DR_AMOUNT - oGJ.TOTAL_CR_AMOUNT, 2);

                // Se cuadra la diferencia cuando es menor a 1 peso
                if (Diferencia_Amount <= 1)
                {
                    if (oGJ.TOTAL_CR_AMOUNT > oGJ.TOTAL_DR_AMOUNT)
                        oGJ.TOTAL_DR_AMOUNT = oGJ.TOTAL_DR_AMOUNT + Diferencia_Amount;
                    else
                        oGJ.TOTAL_CR_AMOUNT = oGJ.TOTAL_CR_AMOUNT + Diferencia_Amount;
                }
                else
                {
                    // MessageBox.Show(string.Format("No es posible realizar la póliza debido a que los montos no son iguales.\nDR: {0}\nCR: {1}", oGJ.TOTAL_DR_AMOUNT, oGJ.TOTAL_CR_AMOUNT), "Póliza invalida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //return;

                    // Si el pago no esta conciliado existe una diferencia mayor a 1 peso. Sin embargo, es necesario que para hacer el traspaso de IVA los pagos esten conciliados
                    if (oGJ.TOTAL_CR_AMOUNT > oGJ.TOTAL_DR_AMOUNT)
                        oGJ.TOTAL_DR_AMOUNT = oGJ.TOTAL_DR_AMOUNT + Diferencia_Amount;
                    else
                        oGJ.TOTAL_CR_AMOUNT = oGJ.TOTAL_CR_AMOUNT + Diferencia_Amount;
                }
            }

            Journal_IVA oJnlIVA;
            oJnlIVA = new Journal_IVA(oGJ, id_site, PG_Intercarmbiada);
            oJnlIVA.esCXC = sTipoTraspaso.Equals("CXC") ? true : false;
            oJnlIVA._dtVMX_IVATRASTEMP = polizas_site;
            oJnlIVA._dtDetalla_IVA = dt.AsEnumerable().Where(r => r["SITE_ID"].ToString() == id_site).CopyToDataTable();
            oJnlIVA.transferTaxes();
            this.progressBar1.Value = this.progressBar1.Maximum;
            cargar();
            MessageBox.Show("El traspaso de iva se completo correctamente.", "Estatus del proceso", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

            /*
             * 
             * 
            oJnlIVA = new Journal_IVA(oGJ);
            oJnlIVA.esCXC = sTipoTraspaso.Equals("CXC") ? true : false;
            oJnlIVA._dtVMX_IVATRASTEMP = _dtTraspasos_Agrupados_Resumen;
            oJnlIVA._dtDetalla_IVA = _dtTraspasos_Seleccionados_Detalle;
            oJnlIVA.transferTaxes();
            this.progressBar1.Value = this.progressBar1.Maximum;
            button_Cargar_Click(null, null);
            MessageBox.Show("El traspaso de iva se completo correctamente.", "Estatus del proceso", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
             * 
             * 
             * 
            */
        }


        #region Validaciones

        /// <summary>
        /// Validar que el usuario seleccione por lo menos un registro para realizar el traspaso.
        /// </summary>
        /// <returns>True cuando hay uno o mas registrso seleccionados.</returns>
        private bool validarNumeroRegistros()
        {
            int numVisibles = 0;
            int numOcultos = 0;

            foreach (DataGridViewRow row in gvMovPorTraspasar.Rows)
            {
                if (row.Visible)
                    numVisibles++;
                else
                    numOcultos++;
            }

            if (numVisibles == 0)
            {
                string sMensaje = "No es posible relizar el traspaso de iva.\nSeleccione por lo menos un traspaso.";
                string sTitulo = "No hay Traspasos Seleccionados";

                MessageBox.Show(sMensaje, sTitulo, MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.progressBar1.Value = 0;

                return false;
            }

            if (gvMovPorTraspasar.SelectedRows.Count <= 0)
            {
                string sMensaje = "No es posible relizar el traspaso de iva.\nSeleccione por lo menos un traspaso.";
                string sTitulo = "No hay Traspasos Seleccionados";

                MessageBox.Show(sMensaje, sTitulo, MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.progressBar1.Value = 0;

                return false;
            }
            else
                return true;
        }

        #endregion

        /// <summary> 
        /// 1.- Eliminar todos lo registros anteriores.       
        /// 2.- Insertar los nuevos registros en tabla temporal.
        /// </summary>
        /// <param name="dgvTraspasosSeleccionados">Grid View con los traspasos seleccionados</param>
        /// <param name="dtTbl_Traspasos">Data Table donde se almacenaran los traspasos</param>
        /// <returns>True cuando los registro son validos y se guardaron en tabla temporal</returns>
        private bool insertar_en_temporal(DataGridViewSelectedRowCollection dgvTraspasosSeleccionados, out DataTable dtTbl_Traspasos)
        {
            string sConsulta = string.Empty;

            dtTbl_Traspasos = new DataTable();

            foreach (DataGridViewColumn dgr in gvMovPorTraspasar.Columns)
                dtTbl_Traspasos.Columns.Add(dgr.Name);

            // Eliminar todos los traspasos temporales            
            string query = MapeoQuerySql.ObtenerPorId("frmPrincipal.procesarTraspasos.delIVATRASTEMP");
            _SQL.EjecutarDML(query);

            for (int i = 0; i < dgvTraspasosSeleccionados.Count; i++)
            {
                string cuenta_origen = dgvTraspasosSeleccionados[i].Cells["VAT_GL_ACCT_ID"].Value.ToString();
                string cuenta_destino = dgvTraspasosSeleccionados[i].Cells["TRASLADO"].Value.ToString();
                string sIVA_Factura = dgvTraspasosSeleccionados[i].Cells["IVA_MXN_TRASLADAR_FACT"].Value.ToString();
                string sIVA_Deposito = dgvTraspasosSeleccionados[i].Cells["IVA_MXN_TRASLADAR_DEP"].Value.ToString();

                object[] arrColumnas = new object[dgvTraspasosSeleccionados[i].Cells.Count];
                int iColumn = 0;

                foreach (DataGridViewCell cell in dgvTraspasosSeleccionados[i].Cells)
                {
                    arrColumnas[iColumn] = cell.Value;
                    iColumn++;
                }

                // Guardar el traspaso actual en DataTable
                dtTbl_Traspasos.Rows.Add(arrColumnas);

                sConsulta = MapeoQuerySql.ObtenerPorId("frmPrincipal.procesarTraspasos.insIVATRASTEMP");
                sConsulta = string.Format(sConsulta, cuenta_origen, cuenta_destino, sIVA_Factura, sIVA_Deposito);

                try
                {
                    _SQL.EjecutarDML(sConsulta);
                }
                catch (Exception ex)
                {
                    _ArchivoErrores = new LogErrores();
                    _ArchivoErrores.escribir("frmPrincipal", "insertar_en_temporal", ex.Message);
                    return false;
                }
            }

            return true;
        }

        private void cargar_combos()
        {
            cargarComboEntidad();

            /*string str_sql = MapeoQuerySql.ObtenerPorId("frmPrincipal.cargar_combos.Entity");

            SqlDataReader rdr = this._utilidades.consulta_sql(str_sql, _SQL._oConn);

            while (rdr.Read())
                this.comboBox_Entity.Items.Add(rdr[0]);

            this.comboBox_Entity.Text = this.comboBox_Entity.Items[0].ToString();
            rdr.Close();

            str_sql = MapeoQuerySql.ObtenerPorId("frmPrincipal.cargar_combos.APPLICATION_GLOBAL");
            str_sql = string.Format(str_sql, this.comboBox_Entity.Text);

            rdr = this._utilidades.consulta_sql(str_sql, this._SQL._oConn);

            while (rdr.Read())
                this.comboBox_Currency.Text = rdr[1].ToString();

            rdr.Close();
            this.comboBox_Currency.Text = this._utilidades.consulta_sql_undato(str_sql, this._SQL._oConn, "SYSTEM_CURRENCY_ID");*/
        }


        private void cargarComboEntidad()
        {
            Company company = new Company(this._SQL);
            entidades = company.getEntidades();
            foreach (Entidad item in entidades)
                comboBox_Entity.Items.Add(item.id);
            comboBox_Entity.Refresh();
            comboBox_Entity.SelectedItem = 0;
            comboBox_Entity.SelectedIndex = 0;
            loadEntity();
        }

        /// <summary>
        /// Cargar los sites de la entidad
        /// </summary>
        private void cargarSites()
        {
            treeSite.Nodes.Clear();
            treeSite.CheckBoxes = true;
            if (!string.IsNullOrEmpty(comboBox_Entity.Text))
            {
                var ent = entidades.Where(e => e.id == comboBox_Entity.Text).ToList<Entidad>();
                if (ent.Count > 0)
                {
                    foreach (Site s in ent[0].sites)
                    {
                        TreeNode node = new TreeNode();
                        node.Text = s.nombre;
                        node.Name = s.id;
                        node.Checked = true;
                        treeSite.Nodes.Add(node);
                    }
                    treeSite.Refresh();
                }
            }
        }


        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string sCtaOrigen;
                string sCtaDestino;
                string sBanco = string.Empty;
                string sNoControl = string.Empty;
                string sTipo = string.Empty;
                string sSiteID = string.Empty;
                int iMes = 0;

                if ((e.ColumnIndex == 1) && (e.RowIndex >= 0))
                {
                    DataTable dt = new DataTable();
                    Detalles_Poliza detalles = new Detalles_Poliza(this._SQL._oConn);

                    sBanco = gvMovPorTraspasar.Rows[e.RowIndex].Cells["BANK_ACCOUNT_ID"].Value.ToString();
                    sNoControl = this.gvMovPorTraspasar.CurrentRow.Cells["CONTROL_NO"].Value.ToString();
                    sCtaOrigen = this.gvMovPorTraspasar.CurrentRow.Cells["VAT_GL_ACCT_ID"].Value.ToString();
                    sCtaDestino = this.gvMovPorTraspasar.CurrentRow.Cells["TRASLADO"].Value.ToString();
                    sTipo = rbtnCXC.Checked ? "CXC" : "CXP";
                    iMes = int.Parse(_DateConverter.getMonth(lblAl.Text));
                    sSiteID = this.gvMovPorTraspasar.CurrentRow.Cells["SITE_ID"].Value.ToString();
                    //string site_Id = Agregar siteId para mostrar el detalle

                    dt = detalles.detalles(sTipo, sCtaOrigen, sCtaDestino, iMes, int.Parse(txtAnio.Text), chkConsiliados.Checked, sBanco, sNoControl, sSiteID);
                    if (dt.Rows.Count > 0)
                        new frmDetalles(dt).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrio un error al seleccionar el movimiento.", "Error del sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _ArchivoErrores = new LogErrores();
                _ArchivoErrores.escribir("frmPrincipal", "dataGridView1_CellClick", ex.Message);
            }
        }

        private void frmPrincipal_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void button_agrupar_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (tbTraspasos.SelectedIndex == 0)
                {
                    if (gvMovPorTraspasar.Rows.Count <= 0)
                        return;
                    gvMovPorTraspasar.SelectAll();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrio un error al agrupar los movimientos.", "Error del sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _ArchivoErrores = new LogErrores();
                _ArchivoErrores.escribir("frmPrincipal", "button_agrupar_Click", ex.Message);
            }
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            try
            {
                if ((e.ColumnIndex == 0) && (e.RowIndex >= 0))
                {
                    DataTable dt = new DataTable();
                    Detalles_Poliza detalles = new Detalles_Poliza(this._SQL._oConn);
                    String Transaccion = this.gvPoliza.CurrentRow.Cells["ID"].Value.ToString();
                    string tipo = rbtnCXC.Checked ? "CXC" : "CXP";
                    if (!(tipo == ""))
                    {
                        dt = detalles.detalles_Mov_cabecera(Transaccion, tipo);
                        if (dt.Rows.Count > 0)
                            new frmDetalles_Mov_Cabecera(this._SQL._oConn, dt).ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrio un error al obtener el movimiento.", "Error del sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _ArchivoErrores = new LogErrores();
                _ArchivoErrores.escribir("frmPrincipal", "dataGridView2_CellClick", ex.Message);
            }
        }

        private void btnExportarExcel_Click(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                ExcelFile oExcel = new ExcelFile();
                DialogResult oResult = new DialogResult();
                string sTipoTraspaso = rbtnCXC.Checked ? "CXC" : "CXP";
                string sRuta = string.Empty;

                saveFileDialog1.DefaultExt = "*.xlsx";
                saveFileDialog1.AddExtension = false;
                saveFileDialog1.Filter = "Archivo de Excel .xlsx |*.xlsx";
                saveFileDialog1.FileName = string.Empty;
                oResult = saveFileDialog1.ShowDialog();

                DataTable tb = new DataTable();
                if (oResult != DialogResult.OK) return;

                sRuta = saveFileDialog1.FileName;

                if (tbTraspasos.SelectedTab == tbTraspasos.TabPages["tbPagePorTraspasar"])
                {
                    tb = dtTbl_PendientesPorTraspasar;
                    oExcel.exportarDetalle(tb, sRuta);
                }

                else if (tbTraspasos.SelectedTab == tbTraspasos.TabPages["tbPageTraspasados"])
                {
                    tb = dtTblProcesados;
                    oExcel.exportarDetalle(dtTblProcesados, sRuta);
                }

                else if (tbTraspasos.SelectedTab == tbTraspasos.TabPages["tbTraspasosNegativos"])
                {
                    oExcel.exportarDetalle(_odtPolizasNegativas, sRuta);
                }

                else
                    oExcel.exportarDetalle(poliza, sRuta);

                MessageBox.Show("El Excel se exportó con éxito ", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrio un error al exportar a Excel.", "Error del sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _ArchivoErrores = new LogErrores();
                _ArchivoErrores.escribir("frmPrincipal", "btnExportarExcel_Click", ex.Message);
                if (this.Cursor == Cursors.WaitCursor) this.Cursor = Cursors.Arrow;
            }
            this.Cursor = Cursors.Arrow;
        }

        private void Salir_Click(object sender, System.EventArgs e)
        {
            Application.Exit();
        }

        private void acercaDeToolStripMenuItem1_Click(object sender, System.EventArgs e)
        {
            About.About obj = new About.About("Traspaso de IVA", Application.ProductVersion, "Enero 2016");
            obj.ShowDialog();
        }

        /*private void txtAnio_Leave(object sender, EventArgs e)
        {
            try
            {
                PeriodoContable oPerido = new PeriodoContable(_oDatosConexion.Server, _oDatosConexion.Database, _oDatosConexion.Usuario_cliente, _oDatosConexion.Password);
                _dtTblPeriodosContables = oPerido.obtenerPeriodosContables(int.Parse(txtAnio.Text));

                if (_dtTblPeriodosContables.Rows.Count > 0)
                {
                    mostrarPeriodos();
                }
                else
                {
                    MessageBox.Show(string.Format("No hay periodos contables para el año [{0}]", txtAnio.Text), "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    cmbPeriodos.Items.Clear();
                    lblAl.Text = "";
                    lblDel.Text = "";
                }
            }
            catch (Exception ex)
            {
                _ArchivoErrores = new LogErrores();
                _ArchivoErrores.escribir("txtAnio_Leave", "private void txtAnio_Leave(object sender, EventArgs e)", ex.Message);
            }
        }*/

        private void mostrarPeriodos()
        {
            cmbPeriodos.Items.Clear();

            foreach (DataRow periodo in _dtTblPeriodosContables.Rows)
                cmbPeriodos.Items.Add(periodo["ACCT_PERIOD"]);

            cmbPeriodos.SelectedItem = 0;
            cmbPeriodos.Text = cmbPeriodos.Items[0].ToString();
            //cmbPeriodos.SelectedIndex = 0;
        }

        private void mostrarFechas_Periodo()
        {
            string sPeriodoSeleccionado = cmbPeriodos.SelectedItem.ToString();
            string sPeriodo = string.Empty;

            foreach (DataRow periodo in _dtTblPeriodosContables.Rows)
            {
                sPeriodo = periodo["ACCT_PERIOD"].ToString();
                DateTime fec_Inicial, fec_final;

                if (sPeriodo == sPeriodoSeleccionado)
                {
                    fec_Inicial = DateTime.Parse(periodo["BEGIN_DATE"].ToString());
                    fec_final = DateTime.Parse(periodo["END_DATE"].ToString());

                    lblDel.Text = fec_Inicial.ToShortDateString(); //String.Format("{0:dd/MM/yyyy}", fec_Inicial.ToShortDateString());
                    lblAl.Text = fec_final.ToShortDateString(); //String.Format("{0:dd/MM/yyyy}", fec_final.ToShortDateString());
                }
            }
        }

        private void cmbPeriodos_SelectedIndexChanged(object sender, EventArgs e)
        {
            mostrarFechas_Periodo();
        }

        private void tbTraspasos_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPageIndex == 0)
            {
                tsbTraspasar.Enabled = true;
                tsbSeleccionar.Enabled = true;
            }
            else
            {
                tsbTraspasar.Enabled = false;
                tsbSeleccionar.Enabled = false;
            }
            if ((e.TabPage.Name == "tbTraspasosNegativos") || (e.TabPage.Name == "tbPageTraspasos") || (e.TabPage.Name == "tabPage1"))
            {
                btnBuscar.Enabled = false;
                txtFiltro.Enabled = true;
                comboCampos.Enabled = false;
                Campo.Visible = true;

            }
        }

        private void cargarMoneda()
        {
            this.comboBox_Currency.Items.Clear();
            List<Entidad> eW = entidades.Where(e => e.id == comboBox_Entity.Text).ToList<Entidad>();

            if (eW.Count > 0)
            {
                this.comboBox_Currency.Text = eW[0].moneda;
                foreach (monedaEntidad me in eW[0].monedas)
                    this.comboBox_Currency.Items.Add(me.moneda);
            }
        }

        private void cargarAnios()
        {
            txtAnio.Items.Clear();
            if (treeSite.Nodes.Count > 0)
            {
                List<string> anios = new Company(_SQL).loadAnios(treeSite.Nodes[0].Name);
                if (anios.Count > 0)
                {
                    foreach (string anio in anios)
                    {
                        txtAnio.Items.Add(anio);
                    }
                }
                txtAnio.SelectedItem = 0;
                txtAnio.SelectedIndex = 0;
            }
        }

        private void loadEntity()
        {
            try
            {
                cargarSites();
                cargarMoneda();
                cargarAnios();
                cargarPeriodos();
                Global.TipoCambioPagos = TipoCambio.obtenerTipoCambio(comboBox_Entity.Text, _SQL);
                //cbTipoTraspaso.Text = cbTipoTraspaso.Items[0].ToString();
            }
            catch (Exception ex)
            {
                _ArchivoErrores = new LogErrores();
                _ArchivoErrores.escribir("frmPrincipal", "public frmPrincipal(string pConn)", ex.Message);
            }
        }

        private void cargarPeriodos()
        {
            _dtTblPeriodosContables = new DataTable();
            if (treeSite.Nodes.Count > 0)
            {
                _dtTblPeriodosContables = oPeriodo.obtenerPeriodosContables(int.Parse(txtAnio.Text), treeSite.Nodes[0].Name);
            }
            mostrarPeriodos();
            mostrarFechas_Periodo();
        }

        private void limpiarDatos()
        {
            txtProcesados.Text = "0";
            txtNegativos.Text = "0";
            txtPorTraspasar.Text = "0";

            gvMovPorTraspasar.DataSource = null;
            gvMovPorTraspasar.Rows.Clear();
        }

        private void txtAnio_SelectedIndexChanged(object sender, EventArgs e)
        {
            cargarPeriodos();
        }

        private void comboBox_Entity_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadEntity();
        }

        //private void cbTipoTraspaso_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    limpiarDatos();
        //    sTipoCuentas = cbTipoTraspaso.Text;
        //    toolStripButton5.ToolTipText += cbTipoTraspaso.Text;
        //}

        private void bRetencionesConfig_Click(object sender, EventArgs e)
        {
            configurarRetenciones();
        }
        /// <summary>
        /// DETALLE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bExportarDetalle_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (tbTraspasos.SelectedTab.Name == "tbPagePorTraspasar")
                {

                    //Ordena movimientos por traspasar
                    bool retencion = false;

                    if (_odtDetallePolizasTraspasar.Columns.IndexOf("No. Control") > -1)
                        _odtDetallePolizasTraspasar.Columns["No. Control"].ColumnName = "NO_CONTROL";

                    if (_odtDetallePolizasTraspasar.Columns.IndexOf("Linea Voucher") > -1)
                    {
                        _odtDetallePolizasTraspasar.Columns["Linea Voucher"].ColumnName = "LINEA";
                        retencion = true;
                    }
                    else _odtDetallePolizasTraspasar.Columns["Linea Factura"].ColumnName = "LINEA";

                    DataView dv = _odtDetallePolizasTraspasar.DefaultView;
                    if (retencion)
                        dv.Sort = "Banco ASC, NO_CONTROL ASC, Voucher ASC, LINEA ASC";
                    else
                        dv.Sort = "Banco ASC, NO_CONTROL ASC, Factura ASC, LINEA ASC";
                    _odtDetallePolizasTraspasar = dv.ToTable();

                    if (_odtDetallePolizasTraspasar.Columns.IndexOf("NO_CONTROL") > -1)
                        _odtDetallePolizasTraspasar.Columns["NO_CONTROL"].ColumnName = "No. Control";

                    if (retencion)
                        _odtDetallePolizasTraspasar.Columns["LINEA"].ColumnName = "Linea Voucher";
                    else _odtDetallePolizasTraspasar.Columns["LINEA"].ColumnName = "Linea Factura";



                    ExcelFile oExcel = new ExcelFile();
                    DialogResult oResult = new DialogResult();
                    string sTipoTraspaso = rbtnCXC.Checked ? "CXC" : "CXP";
                    string sRuta = string.Empty;

                    saveFileDialog1.DefaultExt = "*.xlsx";
                    saveFileDialog1.AddExtension = false;
                    saveFileDialog1.Filter = "Archivo de Excel .xlsx |*.xlsx";
                    saveFileDialog1.FileName = string.Empty;
                    oResult = saveFileDialog1.ShowDialog();

                    if (oResult != DialogResult.OK) return;

                    sRuta = saveFileDialog1.FileName;

                    if (_odtDetallePolizasTraspasar != null)
                        oExcel.ExportarDetalleDT(_odtDetallePolizasTraspasar, sRuta, sTipoTraspaso);
                }
                else if (tbTraspasos.SelectedTab.Name == "tbPageTraspasados")
                {
                    string sTipoTraspaso = rbtnCXC.Checked ? "CXC" : "CXP";

                    DataView dv = _odtDetalleProcesados.DefaultView;
                    if (sTipoTraspaso == "CXC")
                        dv.Sort = "Banco ASC, No_Control ASC, Factura ASC, Linea_Factura ASC";
                    else dv.Sort = "Banco ASC, No_Control ASC, Factura ASC, Voucher,Linea_Voucher ASC";
                    _odtDetalleProcesados = dv.ToTable();

                    ExcelFile oExcel = new ExcelFile();
                    DialogResult oResult = new DialogResult();
                    string sRuta = string.Empty;

                    saveFileDialog1.DefaultExt = "*.xlsx";
                    saveFileDialog1.AddExtension = false;
                    saveFileDialog1.Filter = "Archivo de Excel .xlsx |*.xlsx";
                    saveFileDialog1.FileName = string.Empty;
                    oResult = saveFileDialog1.ShowDialog();

                    if (oResult != DialogResult.OK) return;

                    sRuta = saveFileDialog1.FileName;

                    if (_odtDetalleProcesados != null)
                        oExcel.ExportarDetalleDT(_odtDetalleProcesados, sRuta, sTipoTraspaso);
                }
                MessageBox.Show("El Excel se exportó con éxito ", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrio un error al exportar a Excel.\n" + ex.Message, "Error del sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _ArchivoErrores = new LogErrores();
                _ArchivoErrores.escribir("frmPrincipal", "btnExportarExcel_Click", ex.Message);
                if (this.Cursor == Cursors.WaitCursor) this.Cursor = Cursors.Arrow;
            }
            this.Cursor = Cursors.Arrow;
        }

        private void tbTraspasos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tbTraspasos.SelectedTab.Name == "tbPagePorTraspasar" || tbTraspasos.SelectedTab.Name == "tbPageTraspasados")
                bExportarDetalle.Enabled = true;

            else bExportarDetalle.Enabled = false;
            if ((tbTraspasos.SelectedTab.Name == "tbTraspasosNegativos") || (tbTraspasos.SelectedTab.Name == "tbPageTraspasos")
                || (tbTraspasos.SelectedTab.Name == "tabPage1") || tbTraspasos.SelectedTab.Name == "tbPageTraspasados")
            {
                btnBuscar.Enabled = false;
                txtFiltro.Enabled = false;
                comboCampos.Enabled = false;
                Campo.Enabled = false;


            }
            else
            {
                if (comboCampos.SelectedIndex == 0)
                {
                    btnBuscar.Enabled = false;
                }
                btnBuscar.Enabled = true;
                txtFiltro.Enabled = true;
                comboCampos.Enabled = true;
                Campo.Enabled = true;
            }
        }

        private void configurarCuentasToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            configurarIVA();
        }


        private void configurarCuentasDeRetencionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            configurarRetenciones();
        }

        private void configurarPerdidaGananciaCambiariaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Está seguro que desa cambiar la configuración\nde la naturaleza de las cuentas para la perdida ganancia por tipo cambiario?", "Configurar Perdida Ganancia Cambiaria", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using (var form = new frmPerdidaGanancia(_SQL))
                {
                    var result = form.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        PG_Intercarmbiada = form.PG_intercambiada;
                    }
                }

                //frmPerdidaGanancia pg = new frmPerdidaGanancia(_SQL);
                //pg.Show();

                //cargar configuración de perdida ganancia
                //string cbpg = ConfigurationManager.AppSettings["PG_Intercarmbiada"].ToString();
                //PG_Intercarmbiada = cbpg.ToUpper() == "TRUE" ? true : false;
            }
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            configurarIVA();
        }

        private void comboBox_Currency_SelectedIndexChanged(object sender, EventArgs e)
        {
            cargar();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            button = true;
            try
            {
                if (button == true && txtFiltro.Text != "")
                {
                    //Crea una nueva tabla "temporal", para mostrar los datos del filtro 
                    DataTable TEST = dtTbl_PendientesPorTraspasar;
                    DataView Ordenar = TEST.DefaultView;
                    
                    //string column = Ordenar.Table.Columns
                    TEST = Ordenar.ToTable();
                    //gvMovPorTraspasar.DataSource;
                    gvMovPorTraspasar.DataSource = TEST;

                    txtPorTraspasar.Text = gvMovPorTraspasar.RowCount.ToString();
                    formatoGridPorTraspasar(sTipoTraspaso);
                    int a = gvMovPorTraspasar.CurrentCell.RowIndex;
                    //string name = gvMovPorTraspasar.Columns[a].Name;
                    string name = comboCampos.SelectedItem.ToString();
                    string datos = txtFiltro.Text;

                    if (name == "No.Control o Cheque")
                    {
                        if (sTipoTraspaso == "CXC")
                        {
                            name = "CONTROL_NO";
                            Ordenar.RowFilter = name + " LIKE '%" + datos + "%' ";
                        }
                        else
                        {
                            name = "CONTROL_NO";
                            //int result = Int32.Parse(datos);   
                        }
                    }
                    else if (name == "Nombre")
                    {
                        name = "Nombre";
                    }
                    else if (name == "Banco")
                    {
                        name = "BANK_ACCOUNT_ID";
                    }
                    else if (name == "Cuenta")
                    {
                        name = "VAT_GL_ACCT_ID";
                    }
                    else if (name == "Descripción")
                    {
                        name = "DESCRIPCION";
                    }
                    else if (name == "Cliente o Proveedor")
                    {
                        name = "CLIENTE";
                    }


                    Ordenar.RowFilter = name + " LIKE '%" + datos + "%' ";
                    Ordenar.Sort = "BANK_ACCOUNT_ID ASC, CONTROL_NO ASC";
                    TEST = Ordenar.ToTable();
                    gvMovPorTraspasar.DataSource = TEST;
                    txtPorTraspasar.Text = gvMovPorTraspasar.RowCount.ToString();
                    formatoGridPorTraspasar(sTipoTraspaso);
                }

            }
            catch (Exception er)
            {
                MessageBox.Show("Ocurrio un error al obtener los datos.", "Error del sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _ArchivoErrores = new LogErrores();
                _ArchivoErrores.escribir("frmPrincipal", "Private void cargar()", er.Message);

            }
        }

        private void ComboCampos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboCampos.SelectedIndex == 0)
            {
                btnBuscar.Enabled = false;
            }
            else
            {
                btnBuscar.Enabled = true;
            }

        }
        private void botones()
        {
            picCarga.Visible = true;
            //MessageBox.Show("Espere un momento, los datos estan siendo cargados","",MessageBoxButtons.OK,MessageBoxIcon.Information);
            toolStripButton1.Enabled = false;
        }
    }
}
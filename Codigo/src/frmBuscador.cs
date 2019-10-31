using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CTECH.Acceso_a_Datos;
using CTECH.Directorios;

namespace VMXTRASPIVA
{
    public partial class frmBuscador : Form
    {
        public DataTable _dtConsulta;
        public string _consultaSQL;
        private int _numColumnas;
        private bool _isPostBack;
        private List<string> _lstDatos;
        public List<string> LstDatos
        {
            get { return _lstDatos; }
        }
        LogErrores _ArchivoErrores;

        /// <summary>
        /// Constructor que recibe la consulta sql para realizar la búsqueda.
        /// </summary>
        /// <param name="pConsultaSQL">Consulta sql para realizar la busqueda.</param>
        public frmBuscador(string pConsultaSQL)
        {
            InitializeComponent();
            _lstDatos = new List<string>();
            _consultaSQL = pConsultaSQL;
            obtenerDatos();
            gvDatos.DataSource = _dtConsulta;
            lblNumRegistros.Text = _dtConsulta.Rows.Count.ToString();
            _numColumnas = gvDatos.Columns.Count;
            gvDatos.AllowUserToOrderColumns = false;
        }

        /// <summary>
        /// Permite guardar los datos del renglon seleccionado en una lista LstDatos, posteriormente cerrar la ventana.
        /// </summary>
        /// <param name="sender">Representa una referencia al objeto que lanza ese evento.</param>
        /// <param name="e">Contiene los eventos lanzados por el control.</param>
        private void tsbSelCerrar_Click(object sender, EventArgs e)
        {
            //Obtener la colección de celdas seleccionadas
            DataGridViewSelectedCellCollection gvCollection = gvDatos.SelectedCells;

            for (int i = 0; i <= gvCollection.Count - 1; i++)
                _lstDatos.Add(gvCollection[i].Value.ToString());

            this.Close();
        }

        /// <summary>
        /// Cierra la ventana de búsqueda.
        /// </summary>
        /// <param name="sender">Representa una referencia al objeto que lanza ese evento.</param>
        /// <param name="e">Contiene los eventos lanzados por el control.</param>
        private void tsbCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tsbFiltro_Click(object sender, EventArgs e)
        {
            if (!_isPostBack)
            {
                _dtConsulta.Rows.InsertAt(_dtConsulta.NewRow(), 0);
                gvDatos.DataSource = _dtConsulta;

                // Poner el foco en la primer celda
                gvDatos.CurrentCell = gvDatos.Rows[0].Cells[0];
                SendKeys.Send("{F2}");
                _isPostBack = true;
            }
            else
            {
                // Filtrar
                aplicarFiltro();
            }
        }

        private void gvDatos_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Solo aplica para el renglon Cero
            if (e.RowIndex > 0)
                return;

            // Poner el foco en la siguiente celda
            if (e.RowIndex < _numColumnas)
            {
                gvDatos.CurrentCell = gvDatos.Rows[0].Cells[e.ColumnIndex];
                SendKeys.Send("{F2}");
            }
        }

        private void gvDatos_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            for (int i = 0; i < _numColumnas; i++)
                _lstDatos.Add(gvDatos.Rows[e.RowIndex].Cells[i].Value.ToString());


            this.Close();
        }

        private void tsbReset_Click(object sender, EventArgs e)
        {
            obtenerDatos();
            gvDatos.DataSource = _dtConsulta;
            lblNumRegistros.Text = _dtConsulta.Rows.Count.ToString();
            _isPostBack = false;
        }

        /// <summary>
        /// Permite obtener los datos de la consulta sql.
        /// </summary>
        /// <returns>Un data table con </returns>
        private DataTable obtenerDatos()
        {
            using (Microsoft_SQL_Server oSQL = new Microsoft_SQL_Server(ConexionSQL._Servidor, ConexionSQL._BaseDatos, ConexionSQL._Usuario, ConexionSQL._Password))
            {
                oSQL.CrearConexion();
                oSQL.AbrirConexion();

                _dtConsulta = oSQL.EjecutarConsulta(_consultaSQL, "DATOS");
            }

            return _dtConsulta;
        }

        private void aplicarFiltro()
        {
            string sFiltro = string.Empty;
            string sValorCelda = string.Empty;
            string sNombreColumna = string.Empty;
            DataTable odtTemporal;

            for (int i = 0; i < _numColumnas; i++)
            {
                sValorCelda = gvDatos.Rows[0].Cells[i].Value.ToString();

                if (!string.IsNullOrEmpty(sValorCelda))
                {
                    sNombreColumna = gvDatos.Columns[i].Name;

                    if (typeof(String) == gvDatos.Rows[0].Cells[i].ValueType)
                        sFiltro += sFiltro.Length > 0 ? string.Format(" AND [{0}] LIKE '{1}' ", sNombreColumna, sValorCelda) : string.Format(" [{0}] LIKE '{1}' ", sNombreColumna, sValorCelda);

                    if (typeof(Decimal) == gvDatos.Rows[0].Cells[i].ValueType)
                        sFiltro += sFiltro.Length > 0 ? string.Format(" AND [{0}] = {1} ", sNombreColumna, sValorCelda) : string.Format(" [{0}] = {1} ", sNombreColumna, sValorCelda);
                }
            }

            //Remover el renglón con los filtros
            _dtConsulta.Rows.RemoveAt(0);
            odtTemporal = _dtConsulta.Copy();

            try
            {
                if (!string.IsNullOrEmpty(sFiltro))
                {
                    _dtConsulta = _dtConsulta.Select(sFiltro).CopyToDataTable();
                    lblNumRegistros.Text = _dtConsulta.Rows.Count.ToString();
                }
                else
                {
                    _isPostBack = false;
                    return;
                }
            }
            catch (Exception ex)
            {
                //El filtro no regresa datos
                _dtConsulta = odtTemporal;
                lblNumRegistros.Text = _dtConsulta.Rows.Count.ToString();

                _ArchivoErrores = new LogErrores();
                _ArchivoErrores.escribir("frmBuscador", "tsbCtaIVA_Click", ex.Message);
            }

            gvDatos.DataSource = _dtConsulta;
            _isPostBack = false;
        }

        /// <summary>
        /// Permite capturar el error e informale al usuario.
        /// </summary>
        /// <param name="sender">Representa una referencia al objeto que lanza ese evento.</param>
        /// <param name="e">Contiene los eventos lanzados por el control.</param>
        private void gvDatos_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            _ArchivoErrores = new LogErrores();
            _ArchivoErrores.escribir("frmBuscador", "gvDatos_DataError", e.Exception.ToString());
            MessageBox.Show(string.Format("La cadena de entrada no tiene el formato correcto.", e.ColumnIndex, e.RowIndex), "Buscador", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

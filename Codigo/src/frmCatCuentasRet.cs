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
using CTech.Helper.Configuracion;

namespace VMXTRASPIVA
{
    /// <summary>
    /// Muestra el catálogo de cuentas para agregar una cuenta de retención.
    /// </summary>
    public partial class frmCatCuentasRet : Form
    {
        private DatosConexion dc;
        private LogErrores archivoErrores = null;
        private Retencion r = null;

        public delegate void Enviar(Retencion retención);
        public event Enviar Evento;

        public frmCatCuentasRet(DatosConexion conexion)
        {
            InitializeComponent();
            dc = conexion;
            cargarCuentas();
        }

        public frmCatCuentasRet(DatosConexion conexion, Retencion ret)
        {
            InitializeComponent();
            dc = conexion;
            r = new Retencion();
            r = ret;
            cargarCuentas();
        }

        public frmCatCuentasRet()
        {
            InitializeComponent();
            cargarCuentas();
        }

        private void cargarCuentas()
        {
            string sql = MapeoQuerySql.ObtenerPorId("Retenciones.listarCuentas");

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
                    archivoErrores.escribir("Retenciones", "frmCatCuentasRet private void cargarCuentas()", ex.Message);
                }
                finally
                {
                    oSQL.CerrarConexion();
                }
            }

            if (dt != null && dt.Rows.Count > 0)
                c1gCuentas.SetDataBinding(dt, "",true);
            else MessageBox.Show("No se pudo obtener el listado de cuentas a agregar","Catálogo de cuentas de retención",MessageBoxButtons.OK,MessageBoxIcon.Stop);
        }

        private void enviarDatos()
        {
            if (c1gCuentas.Row > -1)
            {
                if (r == null)
                {
                    r = new Retencion();
                    r.cuenta = c1gCuentas.Columns["CUENTA"].CellValue(c1gCuentas.Row).ToString();
                    r.descripcion = c1gCuentas.Columns["DESCRIPCION"].CellValue(c1gCuentas.Row).ToString();
                    r.estado = "True";
                    r.retencion = Convert.ToDecimal(c1gCuentas.Columns["TAX_RCV_PERCENT"].CellValue(c1gCuentas.Row).ToString());
                }
                else if (c1gCuentas.Columns["CUENTA"].CellValue(c1gCuentas.Row).ToString() != r.cuenta)
                {
                    r.traslado = c1gCuentas.Columns["CUENTA"].CellValue(c1gCuentas.Row).ToString();
                }

                Evento(r);
                this.Close();
            }
        }

        private void c1gCuentas_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            enviarDatos();
        }

        private void c1gCuentas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                enviarDatos();
            }
        }

        private void btSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            enviarDatos();
        }
    }
}

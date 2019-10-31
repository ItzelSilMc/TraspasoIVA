using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using UtilidadesX;
using System.Collections.Generic;
using CTECH.Directorios;
using CTech.Helper.Configuracion;

namespace VMXTRASPIVA
{
    public class frmCDescuentos : Form
    {
        private SqlConnection cnn;
        private IContainer components = null;
        private DataGridViewTextBoxColumn Cuenta;
        private DataGridView dataGridView1;
        private string tipo;
        private ToolStrip toolStrip1;
        private ToolStripButton tsbAgregar;
        private ToolStripButton tsbGuardar;
        private UtilidadesFBX utilidades = new UtilidadesFBX();
        private ToolStripButton tsbEliminar;
        LogErrores _ArchivoErrores;

        public frmCDescuentos(string tipo, SqlConnection cnn)
        {
            this.cnn = cnn;
            this.tipo = tipo;
            this.InitializeComponent();
            this.Text = "Cuentas de descuento " + tipo;
            this.cargar_datos();
        }

        private void button_Cuenta_Click(object sender, EventArgs e)
        {
            try
            {
                string str = MapeoQuerySql.ObtenerPorId("frmCDescuentos.button_Cuenta_Click");
                frmBuscador oBuscar = new frmBuscador(str);
                List<string> lstDatos = new List<string>();

                oBuscar.ShowDialog();
                lstDatos = oBuscar.LstDatos;

                if (lstDatos.Count > 0)
                {
                    this.dataGridView1.Rows.Add(new object[] { lstDatos[0] });
                }
            }
            catch (Exception ex)
            {
                _ArchivoErrores = new LogErrores();
                _ArchivoErrores.escribir("frmCDescuentos", "button_Cuenta_Click", ex.Message);
            }
        }

        private void button_Guardar_Click(object sender, EventArgs e)
        {
            try
            {
                this.dataGridView1.EndEdit();
                bool flag = false;
                string str = MapeoQuerySql.ObtenerPorId("frmCDescuentos.button_Guardar_Click.delDIOTCUENTASDESCUENTOS");
                if (this.tipo == "CXC")
                {
                    str = MapeoQuerySql.ObtenerPorId("frmCDescuentos.button_Guardar_Click.delCUENTASDESCUENTOSCXC");
                }
                this.utilidades.executar_operaciones_sql(str, this.cnn, false);
                foreach (DataGridViewRow row in (IEnumerable)this.dataGridView1.Rows)
                {
                    str = MapeoQuerySql.ObtenerPorId("frmCDescuentos.button_Guardar_Click.insDIOTCUENTASDESCUENTOS");
                    str = string.Format(str, row.Cells["Cuenta"].Value.ToString());
                    if (this.tipo == "CXC")
                    {
                        str = MapeoQuerySql.ObtenerPorId("frmCDescuentos.button_Guardar_Click.insCUENTASDESCUENTOSCXC");
                        str = string.Format(str, row.Cells["Cuenta"].Value.ToString());
                    }
                    flag = this.utilidades.executar_operaciones_sql(str, this.cnn, false, true);
                }
                this.utilidades.mensajes_resultado(flag, "Aviso");
            }
            catch (Exception ex)
            {
                _ArchivoErrores = new LogErrores();
                _ArchivoErrores.escribir("frmCDescuentos", "button_Guardar_Click", ex.Message);
            }
        }

        private void dataGridView_Eliminar()
        {
            string str = MapeoQuerySql.ObtenerPorId("frmCDescuentos.dataGridView_Eliminar.delDIOTCUENTASDESCUENTOS");
            if (this.tipo == "CXC") str = MapeoQuerySql.ObtenerPorId("frmCDescuentos.dataGridView_Eliminar.delCUENTASDESCUENTOSCXC");
            str = string.Format(str, dataGridView1.CurrentRow.Cells[0].Value.ToString());
            this.utilidades.executar_operaciones_sql(str, this.cnn, false, true);
            this.dataGridView1.Rows.Remove(dataGridView1.Rows[this.dataGridView1.CurrentCell.RowIndex]);
        }

        private void button_Eliminar_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.Rows.Count > 0)
            {
                dataGridView_Eliminar();
            }
        }

        private void dataGridView_TeclaSupr_Eliminar(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 46 && this.dataGridView1.Rows.Count > 0)
            {
                e.Handled = true;
                dataGridView_Eliminar();
            }
        }

        private void cargar_datos()
        {
            string str = MapeoQuerySql.ObtenerPorId("frmCDescuentos.cargar_datos.DIOTCUENTASDESCUENTOS");
            if (this.tipo == "CXC")
            {
                str = MapeoQuerySql.ObtenerPorId("frmCDescuentos.cargar_datos.CUENTASDESCUENTOSCXC");
            }
            DataTable table = new DataTable();
            table = this.utilidades.llenar_tabla(table, str, this.cnn);
            foreach (DataRow row in table.Rows)
            {
                this.dataGridView1.Rows.Add(row.ItemArray);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCDescuentos));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Cuenta = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbAgregar = new System.Windows.Forms.ToolStripButton();
            this.tsbGuardar = new System.Windows.Forms.ToolStripButton();
            this.tsbEliminar = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Cuenta});
            this.dataGridView1.Location = new System.Drawing.Point(12, 32);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(303, 280);
            this.dataGridView1.TabIndex = 2;
            this.dataGridView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView_TeclaSupr_Eliminar);
            // 
            // Cuenta
            // 
            this.Cuenta.HeaderText = "Cuenta";
            this.Cuenta.Name = "Cuenta";
            this.Cuenta.ReadOnly = true;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.BackgroundImage = global::VMXTRASPIVA.Properties.Resources.background_image;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbAgregar,
            this.tsbGuardar,
            this.tsbEliminar});
            this.toolStrip1.Location = new System.Drawing.Point(-4, -1);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(8, 0, 1, 0);
            this.toolStrip1.Size = new System.Drawing.Size(336, 30);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbAgregar
            // 
            this.tsbAgregar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAgregar.Image = global::VMXTRASPIVA.Properties.Resources.search;
            this.tsbAgregar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAgregar.Name = "tsbAgregar";
            this.tsbAgregar.Size = new System.Drawing.Size(23, 27);
            this.tsbAgregar.Text = "Buscar";
            this.tsbAgregar.Click += new System.EventHandler(this.button_Cuenta_Click);
            // 
            // tsbGuardar
            // 
            this.tsbGuardar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbGuardar.Image = global::VMXTRASPIVA.Properties.Resources.save;
            this.tsbGuardar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbGuardar.Name = "tsbGuardar";
            this.tsbGuardar.Size = new System.Drawing.Size(23, 27);
            this.tsbGuardar.Text = "toolStripButton2";
            this.tsbGuardar.ToolTipText = "Guardar";
            this.tsbGuardar.Click += new System.EventHandler(this.button_Guardar_Click);
            // 
            // tsbEliminar
            // 
            this.tsbEliminar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbEliminar.Image = global::VMXTRASPIVA.Properties.Resources.delete;
            this.tsbEliminar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbEliminar.Name = "tsbEliminar";
            this.tsbEliminar.Size = new System.Drawing.Size(23, 27);
            this.tsbEliminar.Text = "toolStripButton1";
            this.tsbEliminar.ToolTipText = "Eliminar";
            this.tsbEliminar.Click += new System.EventHandler(this.button_Eliminar_Click);
            // 
            // frmCDescuentos
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(327, 324);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.dataGridView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmCDescuentos";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cuenta Descuentos";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}


using System;
using System.Collections;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using UtilidadesX;
using System.Collections.Generic;
using CTECH.Directorios;
using CTech.Helper.Configuracion;

namespace VMXTRASPIVA
{
    public class frmCatCuentas : Form
    {
        private SqlConnection cnn;
        private IContainer components = null;
        private DataGridViewTextBoxColumn Cuenta;
        private DataGridView dataGridView1;
        private DataGridViewTextBoxColumn Descripcion;
        private DataGridViewCheckBoxColumn Estado;
        private DataGridViewTextBoxColumn Percent_IVA;
        private DataGridViewTextBoxColumn Retencion;
        private string tipo = "";
        private DataGridViewTextBoxColumn Traslado_Iva;
        private ToolStrip tsHerramientas;
        private ToolStripButton tsbBuscar;
        private ToolStripButton tsbCtaDescuento;
        private ToolStripButton tsbGuardar;
        private UtilidadesFBX utilidades = new UtilidadesFBX();
        LogErrores _ArchivoErrores;

        public frmCatCuentas(string tipo, SqlConnection cnn)
        {
            this.cnn = cnn;
            this.tipo = tipo;
            this.InitializeComponent();
            this.Text = "Catal\x00f3go de cuentas " + tipo + " BD: " + cnn.Database + " Versi\x00f3n : " + Application.ProductVersion;
            this.cargar_datos();
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCatCuentas));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Estado = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Cuenta = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Descripcion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Percent_IVA = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Retencion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Traslado_Iva = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tsHerramientas = new System.Windows.Forms.ToolStrip();
            this.tsbBuscar = new System.Windows.Forms.ToolStripButton();
            this.tsbCtaDescuento = new System.Windows.Forms.ToolStripButton();
            this.tsbGuardar = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tsHerramientas.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Estado,
            this.Cuenta,
            this.Descripcion,
            this.Percent_IVA,
            this.Retencion,
            this.Traslado_Iva});
            this.dataGridView1.Location = new System.Drawing.Point(12, 33);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(798, 353);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // Estado
            // 
            this.Estado.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
            this.Estado.HeaderText = "Estado";
            this.Estado.Name = "Estado";
            this.Estado.Width = 5;
            // 
            // Cuenta
            // 
            this.Cuenta.HeaderText = "Cuenta";
            this.Cuenta.Name = "Cuenta";
            this.Cuenta.ReadOnly = true;
            // 
            // Descripcion
            // 
            this.Descripcion.HeaderText = "Descripcion";
            this.Descripcion.Name = "Descripcion";
            this.Descripcion.ReadOnly = true;
            // 
            // Percent_IVA
            // 
            this.Percent_IVA.HeaderText = "Percent_IVA";
            this.Percent_IVA.Name = "Percent_IVA";
            this.Percent_IVA.ReadOnly = true;
            // 
            // Retencion
            // 
            this.Retencion.HeaderText = "Retencion";
            this.Retencion.Name = "Retencion";
            this.Retencion.ReadOnly = true;
            // 
            // Traslado_Iva
            // 
            this.Traslado_Iva.HeaderText = "Traslado_Iva";
            this.Traslado_Iva.Name = "Traslado_Iva";
            this.Traslado_Iva.ReadOnly = true;
            // 
            // tsHerramientas
            // 
            this.tsHerramientas.AutoSize = false;
            this.tsHerramientas.BackgroundImage = global::VMXTRASPIVA.Properties.Resources.background_image;
            this.tsHerramientas.Dock = System.Windows.Forms.DockStyle.None;
            this.tsHerramientas.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsHerramientas.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbBuscar,
            this.tsbCtaDescuento,
            this.tsbGuardar});
            this.tsHerramientas.Location = new System.Drawing.Point(-4, 0);
            this.tsHerramientas.Name = "tsHerramientas";
            this.tsHerramientas.Padding = new System.Windows.Forms.Padding(8, 0, 1, 0);
            this.tsHerramientas.Size = new System.Drawing.Size(835, 30);
            this.tsHerramientas.TabIndex = 5;
            this.tsHerramientas.Text = "toolStrip1";
            // 
            // tsbBuscar
            // 
            this.tsbBuscar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbBuscar.Image = global::VMXTRASPIVA.Properties.Resources.search;
            this.tsbBuscar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbBuscar.Name = "tsbBuscar";
            this.tsbBuscar.Size = new System.Drawing.Size(23, 27);
            this.tsbBuscar.Text = "Buscar";
            this.tsbBuscar.Click += new System.EventHandler(this.tsbBuscar_Click);
            // 
            // tsbCtaDescuento
            // 
            this.tsbCtaDescuento.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbCtaDescuento.Image = global::VMXTRASPIVA.Properties.Resources.TBLINRFF;
            this.tsbCtaDescuento.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCtaDescuento.Name = "tsbCtaDescuento";
            this.tsbCtaDescuento.Size = new System.Drawing.Size(23, 27);
            this.tsbCtaDescuento.Text = "Cuentas Descuento";
            this.tsbCtaDescuento.Click += new System.EventHandler(this.button_ahorro_Click);
            // 
            // tsbGuardar
            // 
            this.tsbGuardar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbGuardar.Image = global::VMXTRASPIVA.Properties.Resources.save;
            this.tsbGuardar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbGuardar.Name = "tsbGuardar";
            this.tsbGuardar.Size = new System.Drawing.Size(23, 27);
            this.tsbGuardar.Text = "Guardar";
            this.tsbGuardar.Click += new System.EventHandler(this.button_guardar_Click);
            // 
            // frmCatCuentas
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(822, 398);
            this.Controls.Add(this.tsHerramientas);
            this.Controls.Add(this.dataGridView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmCatCuentas";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmCatCuentas";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tsHerramientas.ResumeLayout(false);
            this.tsHerramientas.PerformLayout();
            this.ResumeLayout(false);

        }

        private void button_ahorro_Click(object sender, EventArgs e)
        {
            new frmCDescuentos(this.tipo, this.cnn).ShowDialog();
        }

        private void button_guardar_Click(object sender, EventArgs e)
        {
            this.dataGridView1.EndEdit();
            new UtilImpuestos(this.cnn).guardar_cuentas(this.dataGridView1, this.tipo);
        }

        private void cargar_datos()
        {
            this.dataGridView1 = new UtilImpuestos(this.cnn).cargar_cuentas(this.dataGridView1, this.tipo);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == (this.dataGridView1.Columns.Count - 1))
            {
                string str = MapeoQuerySql.ObtenerPorId("frmCatCuentas.dataGridView1_CellClick");
                frmBuscador oBuscar = new frmBuscador(str);
                List<string> lstDatos = new List<string>();

                oBuscar.ShowDialog();
                lstDatos = oBuscar.LstDatos;

                try
                {
                    if (lstDatos.Count > 0)
                    {
                        this.dataGridView1[e.ColumnIndex, e.RowIndex].Value = lstDatos[0];
                    }
                }
                catch (Exception ex)
                {
                    _ArchivoErrores = new LogErrores();
                    _ArchivoErrores.escribir("frmDatProveedores", "dataGridView1_CellClick", ex.Message);
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.ColumnIndex == 0) && (this.dataGridView1.SelectedCells[0].Value.ToString() == "True"))
            {
                this.utilidades.mensaje_compuesto("Si usted inactiva esta cuenta el m\x00f3dulo de DIOT \nno tomar\x00e1 en cuenta los registros relacionados a esta cuenta", "Aviso", 1, 8);
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

      
        private void tsbBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                string str = MapeoQuerySql.ObtenerPorId("frmCatCuentas.tsbBuscar_Click");
                bool flag = true;
                frmBuscador oBuscar = new frmBuscador(str);
                List<string> lstDatos = new List<string>();

                oBuscar.ShowDialog();
                lstDatos = oBuscar.LstDatos;

                if (lstDatos.Count > 0)
                {
                    foreach (DataGridViewRow row in (IEnumerable)this.dataGridView1.Rows)
                    {
                        if (lstDatos[0].ToString() == row.Cells["Cuenta"].Value.ToString())
                        {
                            flag = false;
                        }
                    }

                    if (flag)
                    {
                        this.dataGridView1.Rows.Add(new object[] { true, lstDatos[0], lstDatos[1], lstDatos[2], 0 });
                    }
                    else
                    {
                        this.utilidades.mensaje_compuesto("Esta cuenta ya se encuentra en uso", "Aviso", 1, 1);
                    }
                }
            }
            catch (Exception ex)
            {
                _ArchivoErrores = new LogErrores();
                _ArchivoErrores.escribir("frmDatProveedores", "tsbBuscar_Click", ex.Message);
            }
        }
    }
}


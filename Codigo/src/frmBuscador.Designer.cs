namespace VMXTRASPIVA
{
    partial class frmBuscador
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmBuscador));
            this.panel1 = new System.Windows.Forms.Panel();
            this.gvDatos = new System.Windows.Forms.DataGridView();
            this.lblNumRegistros = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tstripControles = new System.Windows.Forms.ToolStrip();
            this.tsbSelCerrar = new System.Windows.Forms.ToolStripButton();
            this.tsbCerrar = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbFiltro = new System.Windows.Forms.ToolStripButton();
            this.tsbReset = new System.Windows.Forms.ToolStripButton();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvDatos)).BeginInit();
            this.tstripControles.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.gvDatos);
            this.panel1.Location = new System.Drawing.Point(12, 33);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(600, 383);
            this.panel1.TabIndex = 1;
            // 
            // gvDatos
            // 
            this.gvDatos.AllowUserToAddRows = false;
            this.gvDatos.AllowUserToDeleteRows = false;
            this.gvDatos.AllowUserToResizeRows = false;
            this.gvDatos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gvDatos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.gvDatos.BackgroundColor = System.Drawing.SystemColors.Control;
            this.gvDatos.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.gvDatos.ColumnHeadersHeight = 30;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.NullValue = null;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gvDatos.DefaultCellStyle = dataGridViewCellStyle1;
            this.gvDatos.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.gvDatos.Location = new System.Drawing.Point(3, 3);
            this.gvDatos.MultiSelect = false;
            this.gvDatos.Name = "gvDatos";
            this.gvDatos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gvDatos.Size = new System.Drawing.Size(594, 377);
            this.gvDatos.TabIndex = 0;
            this.gvDatos.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gvDatos_CellContentDoubleClick);
            this.gvDatos.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.gvDatos_CellEndEdit);
            this.gvDatos.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.gvDatos_DataError);
            // 
            // lblNumRegistros
            // 
            this.lblNumRegistros.AutoSize = true;
            this.lblNumRegistros.BackColor = System.Drawing.Color.Transparent;
            this.lblNumRegistros.Location = new System.Drawing.Point(145, 419);
            this.lblNumRegistros.Name = "lblNumRegistros";
            this.lblNumRegistros.Size = new System.Drawing.Size(41, 15);
            this.lblNumRegistros.TabIndex = 2;
            this.lblNumRegistros.Text = "label2";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(12, 419);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(127, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Número de Registros:";
            // 
            // tstripControles
            // 
            this.tstripControles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tstripControles.AutoSize = false;
            this.tstripControles.BackColor = System.Drawing.Color.CornflowerBlue;
            this.tstripControles.BackgroundImage = global::VMXTRASPIVA.Properties.Resources.background_image;
            this.tstripControles.Dock = System.Windows.Forms.DockStyle.None;
            this.tstripControles.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tstripControles.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbSelCerrar,
            this.tsbCerrar,
            this.toolStripSeparator1,
            this.tsbFiltro,
            this.tsbReset});
            this.tstripControles.Location = new System.Drawing.Point(-3, 0);
            this.tstripControles.Name = "tstripControles";
            this.tstripControles.Padding = new System.Windows.Forms.Padding(4, 0, 1, 0);
            this.tstripControles.Size = new System.Drawing.Size(631, 30);
            this.tstripControles.Stretch = true;
            this.tstripControles.TabIndex = 0;
            this.tstripControles.Text = "toolStrip1";
            // 
            // tsbSelCerrar
            // 
            this.tsbSelCerrar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSelCerrar.Image = global::VMXTRASPIVA.Properties.Resources.White_check1;
            this.tsbSelCerrar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSelCerrar.Name = "tsbSelCerrar";
            this.tsbSelCerrar.Size = new System.Drawing.Size(23, 27);
            this.tsbSelCerrar.Text = "Seleccionar y Cerrar";
            this.tsbSelCerrar.Click += new System.EventHandler(this.tsbSelCerrar_Click);
            // 
            // tsbCerrar
            // 
            this.tsbCerrar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbCerrar.Image = global::VMXTRASPIVA.Properties.Resources.close_icon_white;
            this.tsbCerrar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCerrar.Name = "tsbCerrar";
            this.tsbCerrar.Size = new System.Drawing.Size(23, 27);
            this.tsbCerrar.Text = "Cerrar";
            this.tsbCerrar.Click += new System.EventHandler(this.tsbCerrar_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 30);
            // 
            // tsbFiltro
            // 
            this.tsbFiltro.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbFiltro.Image = global::VMXTRASPIVA.Properties.Resources.filter;
            this.tsbFiltro.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbFiltro.Name = "tsbFiltro";
            this.tsbFiltro.Size = new System.Drawing.Size(23, 27);
            this.tsbFiltro.Text = "Filtro Rápido";
            this.tsbFiltro.Click += new System.EventHandler(this.tsbFiltro_Click);
            // 
            // tsbReset
            // 
            this.tsbReset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbReset.Image = global::VMXTRASPIVA.Properties.Resources.filter_cancel;
            this.tsbReset.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbReset.Name = "tsbReset";
            this.tsbReset.Size = new System.Drawing.Size(23, 27);
            this.tsbReset.Text = "Resetear";
            this.tsbReset.Click += new System.EventHandler(this.tsbReset_Click);
            // 
            // frmBuscador
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(624, 446);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblNumRegistros);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tstripControles);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmBuscador";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Búsqueda";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvDatos)).EndInit();
            this.tstripControles.ResumeLayout(false);
            this.tstripControles.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip tstripControles;
        private System.Windows.Forms.ToolStripButton tsbSelCerrar;
        private System.Windows.Forms.ToolStripButton tsbCerrar;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsbFiltro;
        private System.Windows.Forms.ToolStripButton tsbReset;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView gvDatos;
        private System.Windows.Forms.Label lblNumRegistros;
        private System.Windows.Forms.Label label2;
    }
}
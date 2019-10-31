namespace VMXTRASPIVA
{
    partial class frmCRetenciones
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCRetenciones));
            this.dgvRetenciones = new System.Windows.Forms.DataGridView();
            this.Estado = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Cuenta = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Descripcion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Retencion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Traslado = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tsHerramientas = new System.Windows.Forms.ToolStrip();
            this.bActualizar = new System.Windows.Forms.ToolStripButton();
            this.btAgregar = new System.Windows.Forms.ToolStripButton();
            this.btGuardar = new System.Windows.Forms.ToolStripButton();
            this.btEliminar = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btSalir = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRetenciones)).BeginInit();
            this.tsHerramientas.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvRetenciones
            // 
            this.dgvRetenciones.AllowUserToAddRows = false;
            this.dgvRetenciones.AllowUserToDeleteRows = false;
            this.dgvRetenciones.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvRetenciones.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvRetenciones.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRetenciones.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Estado,
            this.Cuenta,
            this.Descripcion,
            this.Retencion,
            this.Traslado});
            this.dgvRetenciones.Location = new System.Drawing.Point(12, 33);
            this.dgvRetenciones.Name = "dgvRetenciones";
            this.dgvRetenciones.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvRetenciones.Size = new System.Drawing.Size(825, 441);
            this.dgvRetenciones.TabIndex = 8;
            this.dgvRetenciones.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvRetenciones_CellContentClick);
            this.dgvRetenciones.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvRetenciones_CellDoubleClick);
            this.dgvRetenciones.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvRetenciones_KeyDown);
            // 
            // Estado
            // 
            this.Estado.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
            this.Estado.DataPropertyName = "ESTADO";
            this.Estado.FalseValue = "False";
            this.Estado.HeaderText = "Estado";
            this.Estado.Name = "Estado";
            this.Estado.TrueValue = "True";
            this.Estado.Width = 5;
            // 
            // Cuenta
            // 
            this.Cuenta.DataPropertyName = "CUENTA";
            this.Cuenta.HeaderText = "Cuenta";
            this.Cuenta.Name = "Cuenta";
            this.Cuenta.ReadOnly = true;
            // 
            // Descripcion
            // 
            this.Descripcion.DataPropertyName = "DESCRIPCION";
            this.Descripcion.HeaderText = "Descripcion";
            this.Descripcion.Name = "Descripcion";
            this.Descripcion.ReadOnly = true;
            // 
            // Retencion
            // 
            this.Retencion.DataPropertyName = "RETENCION";
            this.Retencion.HeaderText = "Retencion";
            this.Retencion.Name = "Retencion";
            this.Retencion.ReadOnly = true;
            // 
            // Traslado
            // 
            this.Traslado.DataPropertyName = "TRASLADO";
            this.Traslado.HeaderText = "Traslado";
            this.Traslado.Name = "Traslado";
            this.Traslado.ReadOnly = true;
            // 
            // tsHerramientas
            // 
            this.tsHerramientas.AutoSize = false;
            this.tsHerramientas.BackgroundImage = global::VMXTRASPIVA.Properties.Resources.background_image;
            this.tsHerramientas.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsHerramientas.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bActualizar,
            this.btAgregar,
            this.btGuardar,
            this.btEliminar,
            this.toolStripSeparator1,
            this.btSalir});
            this.tsHerramientas.Location = new System.Drawing.Point(0, 0);
            this.tsHerramientas.Name = "tsHerramientas";
            this.tsHerramientas.Padding = new System.Windows.Forms.Padding(8, 0, 1, 0);
            this.tsHerramientas.Size = new System.Drawing.Size(849, 30);
            this.tsHerramientas.TabIndex = 6;
            this.tsHerramientas.Text = "toolStrip1";
            // 
            // bActualizar
            // 
            this.bActualizar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bActualizar.Image = global::VMXTRASPIVA.Properties.Resources.reload;
            this.bActualizar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bActualizar.Name = "bActualizar";
            this.bActualizar.Size = new System.Drawing.Size(23, 27);
            this.bActualizar.Text = "Cargar cuentas";
            this.bActualizar.Click += new System.EventHandler(this.bActualizar_Click);
            // 
            // btAgregar
            // 
            this.btAgregar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btAgregar.Image = global::VMXTRASPIVA.Properties.Resources.TBLINRFF;
            this.btAgregar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btAgregar.Name = "btAgregar";
            this.btAgregar.Size = new System.Drawing.Size(23, 27);
            this.btAgregar.Text = "Agregar cuenta";
            this.btAgregar.Click += new System.EventHandler(this.btAgregar_Click);
            // 
            // btGuardar
            // 
            this.btGuardar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btGuardar.Image = global::VMXTRASPIVA.Properties.Resources.save;
            this.btGuardar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btGuardar.Name = "btGuardar";
            this.btGuardar.Size = new System.Drawing.Size(23, 27);
            this.btGuardar.Text = "Guardar cuentas";
            this.btGuardar.Click += new System.EventHandler(this.btGuardar_Click);
            // 
            // btEliminar
            // 
            this.btEliminar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btEliminar.Image = global::VMXTRASPIVA.Properties.Resources.delete;
            this.btEliminar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btEliminar.Name = "btEliminar";
            this.btEliminar.Size = new System.Drawing.Size(23, 27);
            this.btEliminar.Text = "Eliminar cuenta";
            this.btEliminar.Click += new System.EventHandler(this.btEliminar_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 30);
            // 
            // btSalir
            // 
            this.btSalir.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btSalir.Image = global::VMXTRASPIVA.Properties.Resources.off;
            this.btSalir.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btSalir.Name = "btSalir";
            this.btSalir.Size = new System.Drawing.Size(23, 27);
            this.btSalir.Text = "Salir";
            this.btSalir.Click += new System.EventHandler(this.btSalir_Click);
            // 
            // frmCRetenciones
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(849, 486);
            this.Controls.Add(this.dgvRetenciones);
            this.Controls.Add(this.tsHerramientas);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmCRetenciones";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmCuentasRetencion";
            ((System.ComponentModel.ISupportInitialize)(this.dgvRetenciones)).EndInit();
            this.tsHerramientas.ResumeLayout(false);
            this.tsHerramientas.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStrip tsHerramientas;
        private System.Windows.Forms.DataGridView dgvRetenciones;
        private System.Windows.Forms.ToolStripButton btAgregar;
        private System.Windows.Forms.ToolStripButton btGuardar;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Estado;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cuenta;
        private System.Windows.Forms.DataGridViewTextBoxColumn Descripcion;
        private System.Windows.Forms.DataGridViewTextBoxColumn Retencion;
        private System.Windows.Forms.DataGridViewTextBoxColumn Traslado;
        private System.Windows.Forms.ToolStripButton btEliminar;
        private System.Windows.Forms.ToolStripButton bActualizar;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btSalir;
    }
}
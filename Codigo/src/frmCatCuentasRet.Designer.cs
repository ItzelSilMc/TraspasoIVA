namespace VMXTRASPIVA
{
    partial class frmCatCuentasRet
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCatCuentasRet));
            this.c1gCuentas = new C1.Win.C1TrueDBGrid.C1TrueDBGrid();
            this.tsHerramientas = new System.Windows.Forms.ToolStrip();
            this.btSeleccionar = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btSalir = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.c1gCuentas)).BeginInit();
            this.tsHerramientas.SuspendLayout();
            this.SuspendLayout();
            // 
            // c1gCuentas
            // 
            this.c1gCuentas.AllowUpdate = false;
            this.c1gCuentas.AllowUpdateOnBlur = false;
            this.c1gCuentas.AlternatingRows = true;
            this.c1gCuentas.BackColor = System.Drawing.SystemColors.Control;
            this.c1gCuentas.FilterBar = true;
            this.c1gCuentas.Images.Add(((System.Drawing.Image)(resources.GetObject("c1gCuentas.Images"))));
            this.c1gCuentas.Location = new System.Drawing.Point(12, 35);
            this.c1gCuentas.MarqueeStyle = C1.Win.C1TrueDBGrid.MarqueeEnum.HighlightRowRaiseCell;
            this.c1gCuentas.Name = "c1gCuentas";
            this.c1gCuentas.PreviewInfo.Location = new System.Drawing.Point(0, 0);
            this.c1gCuentas.PreviewInfo.Size = new System.Drawing.Size(0, 0);
            this.c1gCuentas.PreviewInfo.ZoomFactor = 75D;
            this.c1gCuentas.PrintInfo.PageSettings = ((System.Drawing.Printing.PageSettings)(resources.GetObject("c1gCuentas.PrintInfo.PageSettings")));
            this.c1gCuentas.PropBag = resources.GetString("c1gCuentas.PropBag");
            this.c1gCuentas.Size = new System.Drawing.Size(674, 313);
            this.c1gCuentas.TabIndex = 8;
            this.c1gCuentas.Text = "c1TrueDBGrid1";
            this.c1gCuentas.KeyDown += new System.Windows.Forms.KeyEventHandler(this.c1gCuentas_KeyDown);
            this.c1gCuentas.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.c1gCuentas_MouseDoubleClick);
            // 
            // tsHerramientas
            // 
            this.tsHerramientas.AutoSize = false;
            this.tsHerramientas.BackgroundImage = global::VMXTRASPIVA.Properties.Resources.background_image;
            this.tsHerramientas.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsHerramientas.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btSeleccionar,
            this.toolStripSeparator1,
            this.btSalir});
            this.tsHerramientas.Location = new System.Drawing.Point(0, 0);
            this.tsHerramientas.Name = "tsHerramientas";
            this.tsHerramientas.Padding = new System.Windows.Forms.Padding(8, 0, 1, 0);
            this.tsHerramientas.Size = new System.Drawing.Size(698, 30);
            this.tsHerramientas.TabIndex = 7;
            this.tsHerramientas.Text = "toolStrip1";
            // 
            // btSeleccionar
            // 
            this.btSeleccionar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btSeleccionar.Image = global::VMXTRASPIVA.Properties.Resources.White_check1;
            this.btSeleccionar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btSeleccionar.Name = "btSeleccionar";
            this.btSeleccionar.Size = new System.Drawing.Size(23, 27);
            this.btSeleccionar.Text = "Seleccionar";
            this.btSeleccionar.Click += new System.EventHandler(this.toolStripButton1_Click);
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
            // frmCatCuentasRet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(698, 360);
            this.Controls.Add(this.c1gCuentas);
            this.Controls.Add(this.tsHerramientas);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmCatCuentasRet";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Catálogo de Cuentas (Retención)";
            ((System.ComponentModel.ISupportInitialize)(this.c1gCuentas)).EndInit();
            this.tsHerramientas.ResumeLayout(false);
            this.tsHerramientas.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStrip tsHerramientas;
        private C1.Win.C1TrueDBGrid.C1TrueDBGrid c1gCuentas;
        private System.Windows.Forms.ToolStripButton btSeleccionar;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btSalir;

    }
}
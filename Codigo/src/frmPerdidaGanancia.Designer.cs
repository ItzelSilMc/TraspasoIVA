namespace VMXTRASPIVA
{
    partial class frmPerdidaGanancia
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPerdidaGanancia));
            this.cbPerdidaGanancia = new System.Windows.Forms.CheckBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.bGuardar = new System.Windows.Forms.ToolStripButton();
            this.bCerrar = new System.Windows.Forms.ToolStripButton();
            this.lbl12 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbGdPh = new System.Windows.Forms.CheckBox();
            this.lbl11 = new System.Windows.Forms.Label();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbPerdidaGanancia
            // 
            this.cbPerdidaGanancia.AutoSize = true;
            this.cbPerdidaGanancia.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbPerdidaGanancia.Location = new System.Drawing.Point(33, 109);
            this.cbPerdidaGanancia.Name = "cbPerdidaGanancia";
            this.cbPerdidaGanancia.Size = new System.Drawing.Size(350, 20);
            this.cbPerdidaGanancia.TabIndex = 0;
            this.cbPerdidaGanancia.Text = "Criterio de Ganancia en el haber y Perdida en el debe.";
            this.cbPerdidaGanancia.UseVisualStyleBackColor = true;
            this.cbPerdidaGanancia.CheckedChanged += new System.EventHandler(this.cbPerdidaGanancia_CheckedChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.BackgroundImage = global::VMXTRASPIVA.Properties.Resources.background_image;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bGuardar,
            this.bCerrar});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(9, 0, 1, 0);
            this.toolStrip1.Size = new System.Drawing.Size(600, 35);
            this.toolStrip1.TabIndex = 14;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // bGuardar
            // 
            this.bGuardar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bGuardar.Image = global::VMXTRASPIVA.Properties.Resources.save;
            this.bGuardar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bGuardar.Name = "bGuardar";
            this.bGuardar.Size = new System.Drawing.Size(23, 32);
            this.bGuardar.Text = "Guardar";
            this.bGuardar.Click += new System.EventHandler(this.bGuardar_Click);
            // 
            // bCerrar
            // 
            this.bCerrar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bCerrar.Image = global::VMXTRASPIVA.Properties.Resources.close_icon_white;
            this.bCerrar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bCerrar.Name = "bCerrar";
            this.bCerrar.Size = new System.Drawing.Size(23, 32);
            this.bCerrar.Text = "Cerrar";
            this.bCerrar.Click += new System.EventHandler(this.bCerrar_Click);
            // 
            // lbl12
            // 
            this.lbl12.AutoSize = true;
            this.lbl12.ForeColor = System.Drawing.Color.Red;
            this.lbl12.Location = new System.Drawing.Point(389, 112);
            this.lbl12.Name = "lbl12";
            this.lbl12.Size = new System.Drawing.Size(158, 13);
            this.lbl12.TabIndex = 15;
            this.lbl12.Text = "[v.12 - correcta #intercambiada]";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(30, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(495, 17);
            this.label1.TabIndex = 16;
            this.label1.Text = "Intercambiar la naturaleza de los movimientos de perdida ganancia cambiaria";
            // 
            // cbGdPh
            // 
            this.cbGdPh.AutoSize = true;
            this.cbGdPh.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbGdPh.Location = new System.Drawing.Point(33, 83);
            this.cbGdPh.Name = "cbGdPh";
            this.cbGdPh.Size = new System.Drawing.Size(353, 20);
            this.cbGdPh.TabIndex = 17;
            this.cbGdPh.Text = "Criterio de Ganancia en el debe y Perdida en el Haber.";
            this.cbGdPh.UseVisualStyleBackColor = true;
            this.cbGdPh.CheckedChanged += new System.EventHandler(this.cbGdPh_CheckedChanged);
            // 
            // lbl11
            // 
            this.lbl11.AutoSize = true;
            this.lbl11.ForeColor = System.Drawing.Color.Red;
            this.lbl11.Location = new System.Drawing.Point(389, 86);
            this.lbl11.Name = "lbl11";
            this.lbl11.Size = new System.Drawing.Size(72, 13);
            this.lbl11.TabIndex = 18;
            this.lbl11.Text = "[v.11 - actual]";
            // 
            // frmPerdidaGanancia
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(600, 192);
            this.Controls.Add(this.lbl11);
            this.Controls.Add(this.cbGdPh);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbl12);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.cbPerdidaGanancia);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmPerdidaGanancia";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuración Perdida Ganancia Cambiaria";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmPerdidaGanancia_FormClosed);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbPerdidaGanancia;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton bGuardar;
        private System.Windows.Forms.ToolStripButton bCerrar;
        private System.Windows.Forms.Label lbl12;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbGdPh;
        private System.Windows.Forms.Label lbl11;
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VMXTRASPIVA
{
    public partial class frmProcesando : Form
    {
        public int maximo = 0;
        public int valor = 0;
        
        public frmProcesando(int max,string texto)
        {
            InitializeComponent();
            this.Text = texto;
            this.maximo = max;
            progressBar1.Minimum = 0;
            progressBar1.Maximum = maximo;
            progressBar1.Value = 0;
            this.label2.Text = valor.ToString() + "/" + maximo.ToString();
        }

        public void incremento(int v){
            this.valor += v;
            this.progressBar1.Value += v;
            this.label2.Text = valor.ToString() + "/" + maximo.ToString();
        }
    }
}

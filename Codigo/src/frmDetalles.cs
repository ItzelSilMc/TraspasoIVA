using System;
using System.Data;
using System.Windows.Forms;
using UtilidadesX;

namespace VMXTRASPIVA
{    
    public partial class frmDetalles : Form
    {       

        private UtilidadesFBX utilidades = new UtilidadesFBX();

        public frmDetalles(DataTable dt)
        {
            this.InitializeComponent();
            foreach (DataColumn dc in dt.Columns)
            {
                this.dataGridView1.Columns.Add(dc.ColumnName, dc.ColumnName);
                try
                {
                    this.dataGridView1.Columns["VAT_AMOUNT"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    this.dataGridView1.Columns["VAT_AMOUNT"].DefaultCellStyle.Format = "$ ##.###";
                }
                catch
                {
                }
                try
                {
                    this.dataGridView1.Columns["MONTO"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    this.dataGridView1.Columns["MONTO"].DefaultCellStyle.Format = "$ ##.###";
                }
                catch
                {
                }
            }
            foreach (DataRow dr in dt.Rows)
            {
                this.dataGridView1.Rows.Add(dr.ItemArray);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ExcelFile oExcel = new ExcelFile();
            DialogResult oResult = new DialogResult();            
            string sRuta = string.Empty;

            saveFileDialog1.DefaultExt = "*.xlsx";
            saveFileDialog1.AddExtension = false;
            saveFileDialog1.Filter = "Archivo de Excel .xlsx |*.xlsx";
            saveFileDialog1.FileName = string.Empty;
            oResult = saveFileDialog1.ShowDialog();

            if (oResult != DialogResult.OK)
            {
                return;
            }

            sRuta = saveFileDialog1.FileName;
            oExcel.exportarDetalle(dataGridView1, sRuta);            
        }
    }
}

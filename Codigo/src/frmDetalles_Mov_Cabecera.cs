using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using UtilidadesX;

namespace VMXTRASPIVA
{    
    public partial class frmDetalles_Mov_Cabecera : Form
    {
        private SqlConnection cnn;
        private UtilidadesFBX utilidades = new UtilidadesFBX();

        public frmDetalles_Mov_Cabecera(SqlConnection cnn, DataTable dt)
        {
            this.cnn = cnn;
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

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.ColumnIndex == 0) && (e.RowIndex >= 0))
            {
                DataTable dt = new DataTable();
                Detalles_Poliza detalles = new Detalles_Poliza(this.cnn);
                String Transaccion = this.dataGridView1.CurrentRow.Cells["NO_TRANSACCION"].Value.ToString();
                string tipo = this.dataGridView1.CurrentRow.Cells["TIPO_OPERACION"].Value.ToString();
                DateTime FECHA_PERIODO = DateTime.Parse(this.dataGridView1.CurrentRow.Cells["FECHA_PERIODO"].Value.ToString());
                if (!(tipo == ""))
                {
                    dt = detalles.detalles_Mov_linea(Transaccion, tipo);
                    if (dt.Rows.Count > 0)
                    {
                        new frmDetalles_Mov_linea(this.cnn, dt,FECHA_PERIODO).ShowDialog();
                    }
                }
            }
        }

    }
}

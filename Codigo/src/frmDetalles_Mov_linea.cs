using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using UtilidadesX;



namespace VMXTRASPIVA
{

    public partial class frmDetalles_Mov_linea : Form
    {

        private SqlConnection cnn;
        private DateTime FECHA_PERIODO;

        private UtilidadesFBX utilidades = new UtilidadesFBX();

        public frmDetalles_Mov_linea(SqlConnection cnn, DataTable dt, DateTime FECHA_PERIODO)
        {
            this.cnn = cnn;
            this.FECHA_PERIODO = FECHA_PERIODO;
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
            if ((e.ColumnIndex == 1) && (e.RowIndex >= 0))
            {
                DataTable dt = new DataTable();
                Detalles_Poliza detalles = new Detalles_Poliza(this.cnn);
                String Banco = this.dataGridView1.CurrentRow.Cells["BANK_ACCOUNT_ID"].Value.ToString();
                string Control = this.dataGridView1.CurrentRow.Cells["CONTROL_NO"].Value.ToString();
                string CUENTA_ORIGEN = this.dataGridView1.CurrentRow.Cells["CUENTA"].Value.ToString();
                string CUENTA_DESTINO = this.dataGridView1.CurrentRow.Cells["CUENTA_TRASLADO"].Value.ToString();
                string tipo = this.dataGridView1.CurrentRow.Cells["TIPO_OPERACION"].Value.ToString();
                //string site_Id = "";
                if (!(tipo == ""))
                {
                    dt = detalles.detalles(tipo, CUENTA_ORIGEN, CUENTA_DESTINO, FECHA_PERIODO.Month, FECHA_PERIODO.Year, false, Banco, Control,"");
                    if (dt.Rows.Count > 0)
                    {
                        new frmDetalles(dt).ShowDialog();
                    }
                }
            }
        }

    }
}

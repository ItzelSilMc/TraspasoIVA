using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using CTECH.Directorios;
using CTECH.Acceso_a_Datos;

namespace VMXTRASPIVA
{
    public partial class frmCRetenciones : Form
    {
        private DatosConexion dc;
        private LogErrores archivoErrores = null; 

        private Retenciones cuentasR;
        private List<Retencion> cuentasNuevas = new List<Retencion>();
        private List<Retencion> cuentasModificadas = new List<Retencion>();

        public frmCRetenciones()
        {
            InitializeComponent();
        }

        public frmCRetenciones(DatosConexion conexion, LogErrores logE)
        {
            InitializeComponent();
            dc = conexion;
            archivoErrores = logE;
            cuentasR = new Retenciones(dc, archivoErrores);
            this.Text = "Cuentas de Retención BD: " + conexion.Database + " Versión : " + Application.ProductVersion;
            
            cargarCuentas();
        }

        private void cargarCuentas()
        {
            //muestra las cuentas de retencion

            cuentasNuevas.Clear();
            cuentasModificadas.Clear();
            dgvRetenciones.Rows.Clear();

            DataTable dt = new DataTable();
            dt = cuentasR.consultaRetencionesDT();
            
            if (dt.Rows.Count > 0)
            {
                try
                {
                    object[] values = new object[dt.Columns.Count - 1];
                    foreach (DataRow row in dt.Rows)
                    {
                        values = row.ItemArray;
                        values[0] = bool.Parse(values[0].ToString());
                        dgvRetenciones.Rows.Add(values);
                    }
                }
                catch(Exception e)
                {
                    archivoErrores = new LogErrores();
                    archivoErrores.escribir("Retenciones", "frmCRetenciones private void cargarCuentas()", e.Message);
                }
            }
        }

        private void recibirDatos(Retencion ret)
        {
            int rowIndex = -1;
            try
            {
                foreach(DataGridViewRow row in dgvRetenciones.Rows){
                    if (row.Cells["CUENTA"].Value.ToString() == ret.cuenta)
                    {
                        rowIndex = row.Index;
                        break;
                    }   
                }

                if (rowIndex == -1)
                {
                    cuentasNuevas.Add(ret);
                    dgvRetenciones.Rows.Add(new object[] { true, ret.cuenta.ToString(), ret.descripcion.ToString(), ret.retencion.ToString(), ret.traslado.ToString() });
                }
                else MessageBox.Show("Esta cuenta se agregó previamente", "Cuentas de retención", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            catch (Exception e)
            {
                archivoErrores = new LogErrores();
                archivoErrores.escribir("Retenciones", "frmCRetenciones private void cargarCuentas()", e.Message);
            }
        }

        private void recibirDatosTraslado(Retencion ret)
        {
            int i = -1;
            i = cuentasNuevas.FindIndex(item => item.cuenta == ret.cuenta);
            if (i > -1 && ret.cuenta != cuentasNuevas[i].cuenta)
                cuentasNuevas[i].traslado = ret.cuenta;
            else
            {
                cuentasModificadas.Add(ret);
                dgvRetenciones.Rows[dgvRetenciones.CurrentRow.Index].Cells["TRASLADO"].Value = ret.traslado;
            }
        }

        private void guardarCuentas()
        {
            Retencion rError = new Retencion();
            try
            {
                foreach (Retencion r in cuentasNuevas)
                {
                    cuentasR.guardarCuenta(r);
                    rError.cuenta = r.cuenta;
                }


                foreach (Retencion r in cuentasModificadas)
                {
                    cuentasR.modificarCuenta(r);
                    rError.cuenta = r.cuenta;
                }

                cuentasNuevas.Clear();
                cuentasModificadas.Clear();
                MessageBox.Show("Los cambios han sido guardados", "Cuentas de Retención", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch(Exception e)
            {
                MessageBox.Show("No se guardaron los cambios, error en cuenta: " + rError.cuenta, "Cuentas de Retención", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }                      
        }

        private void eliminarCuenta(string cuenta, int i)
        {
            int resultado = cuentasR.eliminarCuenta(cuenta);
            dgvRetenciones.Rows.RemoveAt(i);

            int c = -1;
            c = cuentasNuevas.FindIndex(item => item.cuenta == cuenta);
            if (c >= 0)
                cuentasNuevas.RemoveAt(c);

            c = -1;
            c = cuentasModificadas.FindIndex(item => item.cuenta == cuenta);
            if (c >= 0)
                cuentasModificadas.RemoveAt(c);

            if(resultado>0) MessageBox.Show("La cuenta ha sido eliminada","Traspaso de Retenciones",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        private void cambiarEstado(int i){
            Retencion r = generaRetencionGrid(i);
            if (r.estado == "True")
                r.estado = "False";
            else r.estado = "True";
            int c = -1;
            c = cuentasModificadas.FindIndex(item => item.cuenta == r.cuenta);
            if (c >= 0)
                cuentasModificadas[c].estado = r.estado;
            else cuentasModificadas.Add(r);
        }

        private Retencion generaRetencionGrid(int i)
        {
            Retencion r = new Retencion();
            r.cuenta = dgvRetenciones.Rows[i].Cells["CUENTA"].Value.ToString();
            r.estado = dgvRetenciones.Rows[i].Cells["ESTADO"].Value.ToString();
            r.retencion = Convert.ToDecimal(dgvRetenciones.Rows[i].Cells["RETENCION"].Value.ToString());
            r.traslado = dgvRetenciones.Rows[i].Cells["TRASLADO"].Value.ToString();
            r.descripcion = dgvRetenciones.Rows[i].Cells["DESCRIPCION"].Value.ToString();
            return r;
        }

        private void btAgregar_Click(object sender, EventArgs e)
        {
            frmCatCuentasRet cRet = new frmCatCuentasRet(dc);
            cRet.Evento += new frmCatCuentasRet.Enviar(recibirDatos);
            cRet.Show();
        }

        private void btGuardar_Click(object sender, EventArgs e)
        {
            guardarCuentas();
        }

        private void dgvRetenciones_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                Retencion r = generaRetencionGrid(e.RowIndex);
                frmCatCuentasRet cRet = new frmCatCuentasRet(dc, r);
                cRet.Evento += new frmCatCuentasRet.Enviar(recibirDatosTraslado);
                cRet.Show();
            }
            catch (Exception ex)
            {
                archivoErrores = new LogErrores();
                archivoErrores.escribir("Retenciones", "frmCRetenciones private void dgvRetenciones_CellDoubleClick()", ex.Message);
            }
        }

        private void btEliminar_Click(object sender, EventArgs e)
        {
            if (dgvRetenciones.CurrentRow.Index > -1)
                eliminarCuenta(dgvRetenciones.Rows[dgvRetenciones.CurrentRow.Index].Cells["CUENTA"].Value.ToString(), dgvRetenciones.CurrentRow.Index);
        }

        private void dgvRetenciones_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                if (dgvRetenciones.CurrentRow.Index > -1)
                    eliminarCuenta(dgvRetenciones.Rows[dgvRetenciones.CurrentRow.Index].Cells["CUENTA"].Value.ToString(), dgvRetenciones.CurrentRow.Index);
        }

        private void bActualizar_Click(object sender, EventArgs e)
        {
            cargarCuentas();
        }

        private void dgvRetenciones_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex == 0)cambiarEstado(e.RowIndex);
        }

        private void btSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}

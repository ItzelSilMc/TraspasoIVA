using System;
using System.Configuration;
using System.Windows.Forms;
using ConfiguracionSR;
using CTECH.Acceso_a_Datos;
using CTECH.Directorios;

namespace VMXTRASPIVA
{
    public partial class frmPerdidaGanancia : Form
    {

        Microsoft_SQL_Server _SQL;
        LogErrores _ArchivoErrores;

        public bool PG_intercambiada { get; set; }

        public frmPerdidaGanancia(Microsoft_SQL_Server sql)
        {
            InitializeComponent();

            //estos labels son meramente informativos para el desarrollador
            lbl11.Visible = false; //en el documento de especificación, se indica como la fórmula del Criterio Actual
            lbl12.Visible = false; //en el documento de especificación, se indica como la fórmula del Criterio Correcto

            string cbpg = ConfigurationManager.AppSettings["PG_Intercarmbiada"].ToString();
            cbPerdidaGanancia.Checked = cbpg.ToUpper() == "TRUE"? true : false;
            cbGdPh.Checked = cbPerdidaGanancia.Checked ? false : true;
            _SQL = sql;
            _ArchivoErrores = new LogErrores();
        }

        private void bCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bGuardar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Está seguro de aplicar los cambios?", "Configurar Perdida Ganancia Cambiaria", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {

                Cursor.Current = Cursors.WaitCursor;
                try
                {

                    this.PG_intercambiada = cbPerdidaGanancia.Checked; //variable de retorno a frmPrincipal
                    string pg = this.PG_intercambiada ? "True" : "False";
                    
                    //revisa configuración anterior para realizar los cambios
                    //if (ConfigurationManager.AppSettings["PG_Intercarmbiada"].ToString() != pg)
                    //{
                        //cambiar archivo config
                        Configuration config = ConfigurationManager.OpenExeConfiguration(Application.StartupPath + @"\VKIVA.exe");
                        AppSettingsSection aps = config.AppSettings;
                        aps.Settings["PG_Intercarmbiada"].Value = pg;
                        config.Save(ConfigurationSaveMode.Full);
                        ConfigurationManager.RefreshSection("appSettings");

                        //cambiar las vistas
                        if (PG_intercambiada)
                        {
                            //si se activa, las vistas se modifican al intercambio
                            vistasIVA_PGactivado act = new vistasIVA_PGactivado();

                            _SQL.CrearConexion();
                            _SQL.AbrirConexion();
                            _SQL.EjecutarDML(act.vmx_iva_cxc_02.ToString());
                            _SQL.EjecutarDML(act.vmx_iva_trasl_cxp_01.ToString());
                            _SQL.EjecutarDML(act.vmx_iva_trasl_cxp_02.ToString());
                            _SQL.EjecutarDML(act.vmx_iva_trasl_cxp_03.ToString());
                            _SQL.CerrarConexion();
                            _SQL.DestruirConexion();

                        }
                        else
                        {
                            //si no se activa, las vistas se modifican al no intercambio
                            vistasIVA_PGdesactivado desact = new vistasIVA_PGdesactivado();

                            _SQL.CrearConexion();
                            _SQL.AbrirConexion();
                            _SQL.EjecutarDML(desact.vmx_iva_cxc_02.ToString());
                            _SQL.EjecutarDML(desact.vmx_iva_trasl_cxp_01.ToString());
                            _SQL.EjecutarDML(desact.vmx_iva_trasl_cxp_02.ToString());
                            _SQL.EjecutarDML(desact.vmx_iva_trasl_cxp_03.ToString());
                            _SQL.CerrarConexion();
                            _SQL.DestruirConexion();

                        }

                        //almacenar registro de la actualización
                        historicoCambios hc = new historicoCambios(_SQL);
                        hc.guardar("TraspasoDeIva", "Intercambio Perdida Ganancia = " + pg, Usuario.userName);

                        MessageBox.Show("Se ha actualizado la configuración de perdida/Ganancia cambiaria.\nDebe cerrar la ventana de traspaso de Iva para que los cambios surtan efecto.", "Configurar Perdida Ganancia Cambiaria", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    //}

                    this.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ocurrió un error al actualizar la configuración de perdida/Ganancia cambiaria.\n" + ex.InnerException, "Configurar Perdida Ganancia Cambiaria", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    _ArchivoErrores.escribir("frmPerdidaGanancia", "bGuardar_Click(object sender, EventArgs e)", ex.InnerException + "--" + ex.Message);
                    this.Close();
                }             
               
            }//if

        }

        private void frmPerdidaGanancia_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Cursor.Current = Cursors.WaitCursor;
        }

        private void cbPerdidaGanancia_CheckedChanged(object sender, EventArgs e)
        {
            if (cbPerdidaGanancia.Checked)
                cbGdPh.Checked = false;
            else
                cbGdPh.Checked = true;
        }

        private void cbGdPh_CheckedChanged(object sender, EventArgs e)
        {
            if (cbGdPh.Checked)
                cbPerdidaGanancia.Checked = false;
            else
                cbPerdidaGanancia.Checked = true;
        }
    }
}

using System;
using System.Security;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Threading;
using SeguridadRegionalizacion;
using System.Globalization;
using System.Configuration;
using CTECH.Log;

namespace VMXTRASPIVA
{
    public class Program
    {
        /// <summary>
        /// Punto de entrada principal de la aplicación.
        /// </summary>
        /// <param name="args">
        /// Argumentos que se envian de macro visual erp 
        /// -DMGM_CRM -USYSADM 
        /// </param>
        [STAThread, PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string sUsuario = "";
            string sBaseDatos = "";
            String sModoPrueba = "";

            string severity = ConfigurationManager.AppSettings.Get("LogSeverity");
            SingletonLogger.Instance.Severity = (LogSeverity)Enum.Parse(typeof(LogSeverity), severity, true);

            // Send log messages to a file            
            ILog log = new ObserverLogToFile(Application.StartupPath + @"\Log_VKIVA.log");
            SingletonLogger.Instance.Attach(log);

            Autenticacion oAut = new Autenticacion();
           
            //args = new string[] { "-DMGMLOC", "-SSYSADM", "-SSYSADM" };

            if (args.Length != 0)
            {
                sUsuario = args[1].Substring(2, args[1].Length - 2).Trim();
                sBaseDatos = args[0].Substring(2, args[0].Length - 2).Trim();
                Usuario.userName = sUsuario;

                if (args.Length > 3)
                    sModoPrueba = args[3];

                if ((sModoPrueba == "") && (!oAut.licenciaActiva(sBaseDatos, "VKIVA")))//"VKIVA"
                {
                    MessageBox.Show("No cuenta una licencia para abrir esta aplicación.", "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                if (oAut.probarConexion(sBaseDatos, true))
                {
                    if (oAut.validarAcceso(sUsuario, "VKIVA"))
                    {
                        Application.Run(new frmPrincipal(oAut.StringConexion));
                    }
                    else
                    {
                        MessageBox.Show("No cuenta con permisos para abrir esta aplicación.", "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
                else
                {
                    MessageBox.Show(oAut.Mensaje);
                }
            }
            else
            {
                //Mostrar ventana de loguin
                Login oLogin = new Login("VKIVA");
                oLogin.ValidarLicencia = false;
                oLogin.ValidarAcceso = false;
 
                oLogin.ShowDialog();

                Usuario.userName = oLogin.Usuario;

                if (oLogin.Logeado)
                {
                    Application.Run(new frmPrincipal(oLogin.StringConexion));
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VMXTRASPIVA
{
    /// <summary>
    /// Contiene parámetros generales de la aplicación que no cambian durante la ejecución del programa.
    /// Se pretende no realizar conexiones a bases de datos innecesarias ya que estos datos no cambiaran durante la ejecución del programa.
    /// </summary>    
    public class Global
    {
        /// <summary>
        /// Especifica el tipo de cambio para realizar los cálculos de DIOT.
        /// Este parámetro puede ser FACTURA o PAGO
        /// </summary>        
        public static string TipoCambioPagos;

        /// <summary>
        /// Nombre de la Base de datos de Visual ERP.
        /// El nombre de la base de datos se obtiene del archivo de configuración que contiene los parámetros de conexión al servidor del cliente.
        /// </summary>        
        public static string BaseDatos;

        /// <summary>
        /// Nombre de usuario valido para entrar al servidor de base de datos.
        /// </summary>
        public static string Usuario;

        /// <summary>
        /// Contraseña del usuario que tiene acceso al servidor de base de datos.
        /// Este campo se obtiene de la pestaña de login o desde parámetro en las macros de Visual ERP.
        /// </summary>        
        public static string Password;

        /// <summary>
        /// Nombre del servidor de base de datos.
        /// </summary>
        public static string Servidor;

        /// <summary>
        /// Moneda configurada para el sistema Visual ERP.
        /// </summary>
        public static string SYSTEM_CURRENCY_ID;

        public static string connectionString;
    }
}

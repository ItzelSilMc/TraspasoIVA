using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace VMXTRASPIVA
{
    /// <summary>
    /// Permite convertir fechas a cultura es-MX
    /// para trabajar con base de datos de Visual ERP
    /// </summary>
    public class DateConverter
    {
        private string _Cultura;

        public string Cultura
        {
            get { return _Cultura; }
        }

        /// <summary>
        /// Constructor que establece la cultura origen de las 
        /// fechas que se convertiran.
        /// </summary>
        /// <param name="pCultura">Cultura origen ejemplo en-US.</param>
        public DateConverter(string pCultura)
        {
            _Cultura = pCultura;
        }

        /// <summary>
        /// Convierte la fecha a Cultura es-MX        
        /// </summary>
        /// <param name="pFecha">Fecha que se va convertir</param>
        /// <returns>Fecha en formato yyyy/MM/dd</returns>
        public string getDate(string pFecha)
        {
            string sFecha = pFecha;

            //MM / dd / yyyy --truena 
            //dd / MM / yyyy --truena 

            string sysFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
            string sysUIFormat = CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern;

            if (sysFormat != sysUIFormat)
                pFecha = DateTime.Parse(pFecha).ToString(sysUIFormat);

            string[] arrFecha = pFecha.Split('/');

            //if (_Cultura.Equals("es-MX") || _Cultura.Equals("es-ES"))
            if(sysUIFormat.Equals("dd/MM/yyyy"))
            {
                sFecha = string.Format("{0}/{1}/{2}", arrFecha[2], arrFecha[1].Length ==1? '0'+ arrFecha[1] : arrFecha[1], arrFecha[0].Length == 1 ? '0' + arrFecha[0] : arrFecha[0]);
            }
            else //(_Cultura.Equals("en-US"))
            {
                // Formato MM/dd/yyyy                
                sFecha = string.Format("{0}/{1}/{2}", arrFecha[2], arrFecha[0].Length == 1 ? '0' + arrFecha[0] : arrFecha[0], arrFecha[1].Length == 1 ? '0' + arrFecha[1] : arrFecha[1]);
            }

            return sFecha;
        }

        public string getMonth(string pFecha)
        {
            string sMonth = string.Empty;

            string sysFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
            string sysUIFormat = CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern;

            if (sysFormat != sysUIFormat)
                pFecha = DateTime.Parse(pFecha).ToString(sysUIFormat);


            string[] arrFecha = pFecha.Split('/');

            if (sysUIFormat.Equals("dd/MM/yyyy"))//if (_Cultura.Equals("es-MX"))
            {
                // Formato dd/MM/yyyy                
                sMonth = arrFecha[1];
            }
            //else if (_Cultura.Equals("es-ES"))
            //{
            //    // Formato dd/MM/yyyy                
            //    sMonth = arrFecha[1];
            //}
            else //if (_Cultura.Equals("en-US"))
            {
                // Formato MM/dd/yyyy                
                sMonth = arrFecha[0];
            }

            return sMonth;
        }
    }
}

using CTECH.Acceso_a_Datos;
using CTECH.Configuracion.Business;
using CTECH.Configuracion.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using UtilidadesX;

namespace VMXTRASPIVA
{
    public class Company
    {
        Microsoft_SQL_Server _sql;
        private UtilidadesFBX _utilidades = new UtilidadesFBX();

        public Company(Microsoft_SQL_Server _sql) {
            this._sql = _sql;
        }

        public List<Entidad> getEntidades()
        {
            List<Entidad> ents = new List<Entidad>();
            string sql = CustomConfigurationManager.Instance.Get(SectionType.sql, "getEntidad");
            DataTable dt = _sql.EjecutarConsulta(string.Format(sql), "Entidad");
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    Entidad e = new Entidad();
                    e.id = row["ID"].ToString();
                    e.nombre = row["Entidad"].ToString();
                    e.sites = getSites(e.id);
                    e.moneda = getMonedaEmpresa(e.id);
                    e.monedas = getMonedas(e.id);
                    ents.Add(e);
                }
            }
            return ents;
        }
        public List<Site> getSites(string id_entidad)
        {
            List<Site> sites = new List<Site>();
            string sql = string.Format(CustomConfigurationManager.Instance.Get(SectionType.sql, "obtenerSitePorEntidadId"), id_entidad);
            DataTable dt = _sql.EjecutarConsulta(string.Format(sql), "Site");
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    Site s = new Site();
                    s.id = row["ID"].ToString();
                    s.nombre = row["SITE_NAME"].ToString();
                    sites.Add(s);
                }
            }
            return sites;
        }
        private string getMonedaEmpresa(string id_entidad)
        {
            string sql = string.Format(CustomConfigurationManager.Instance.Get(SectionType.sql, "obtenerMonedaEmpresa"), id_entidad);
            DataTable dt = _sql.EjecutarConsulta(string.Format(sql), "Site");
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            return null;
        }
        public List<string> loadAnios(string id_site) {
            List<string> anios = new List<string>();
            string sql = string.Format(CustomConfigurationManager.Instance.Get(SectionType.sql, "obtenerAniosByEntidad"), id_site);
            DataTable dt = _sql.EjecutarConsulta(string.Format(sql), "Site");
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    anios.Add(row[0].ToString());
                }
            }
            return anios;
        }

        public List<monedaEntidad> getMonedas(string id_entidad)
        {
            Moneda m = new Moneda();
            List<monedaEntidad> monedas = new List<monedaEntidad>();

            DataTable dt = m.obtenerMonedasEntidad(id_entidad);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    monedaEntidad e = new monedaEntidad();
                    e.moneda = row["CURRENCY_ID"].ToString();
                    e.rastreable = row["TRACKING_CURRENCY"].ToString();
                    monedas.Add(e);
                }
            }
            return monedas;
        }
    }

    public class Entidad
    {
        public string id { get; set; }
        public string nombre { get; set; }
        public List<Site> sites = new List<Site>();
        public string moneda { get; set; }
        public List<string> anios_periodo { get; set; }
        public List<monedaEntidad> monedas {get;set;}
        
    }
    public class Site
    {
        public string id;
        public string nombre;
    }   
}

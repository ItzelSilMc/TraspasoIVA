using System;
using System.Configuration;
using CTECH.Configuracion.Business;
using CTECH.Configuracion.Entities;

namespace CTech.Helper.Configuracion
{
    /// <summary>
    /// 
    /// </summary>
    public class MapeoQuerySql
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string ObtenerPorId(string id)
        {
            string query = string.Empty;

            query = CustomConfigurationManager.Instance.Get(SectionType.sql, id);

            if (string.IsNullOrEmpty(query))
                throw new NullReferenceException("Identificador de consulta SQL no encontrado: " + id);

            return query;
        }
    }
}
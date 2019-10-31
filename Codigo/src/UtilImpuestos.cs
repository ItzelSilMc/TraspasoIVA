using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using UtilidadesX;
using CTech.Helper.Configuracion;

namespace VMXTRASPIVA
{
    public class UtilImpuestos
    {
        private SqlConnection cnn;
        private UtilidadesFBX utilidades = new UtilidadesFBX();

        public UtilImpuestos(SqlConnection cnn)
        {
            this.cnn = cnn;
        }

        public DataGridView cargar_cuentas(DataGridView grid, string tipo)
        {
            try
            {
                grid.Rows.Clear();
                string str = MapeoQuerySql.ObtenerPorId("UtilImpuestos.cargar_cuentas.DIOTCUENTAS");
                if (tipo == "CXC")
                {
                    str = MapeoQuerySql.ObtenerPorId("UtilImpuestos.cargar_cuentas.IVACUENTASCXC");
                }
                DataTable table = new DataTable();
                table = this.utilidades.llenar_tabla(table, str, this.cnn);
                object[] values = new object[table.Columns.Count - 1];
                foreach (DataRow row in table.Rows)
                {
                    values = row.ItemArray;
                    values[0] = bool.Parse(values[0].ToString());
                    grid.Rows.Add(values);
                }
            }
            catch
            {
            }
            return grid;
        }

        public void guardar_cuentas(DataGridView grid, string tipo)
        {
            string str = MapeoQuerySql.ObtenerPorId("UtilImpuestos.guardar_cuentas.DIOTCUENTAS");
            if (tipo == "CXC")
            {
                str = MapeoQuerySql.ObtenerPorId("UtilImpuestos.guardar_cuentas.IVACUENTASCXC");
            }
            this.utilidades.executar_operaciones_sql(str, this.cnn, false, false);
            bool flag = false;
            foreach (DataGridViewRow row in (IEnumerable) grid.Rows)
            {
                try
                {
                    string str2 = "";
                    string str3 = "";
                    string str4 = "";
                    string str5 = "";
                    string str6 = "";
                    string str7 = "";
                    try
                    {
                        str2 = row.Cells["Estado"].Value.ToString();
                    }
                    catch
                    {
                    }
                    try
                    {
                        str3 = row.Cells["Cuenta"].Value.ToString();
                    }
                    catch
                    {
                    }
                    try
                    {
                        str4 = row.Cells["Descripcion"].Value.ToString();
                    }
                    catch
                    {
                    }
                    try
                    {
                        str5 = row.Cells["Percent_IVA"].Value.ToString();
                    }
                    catch
                    {
                    }
                    try
                    {
                        str6 = row.Cells["Retencion"].Value.ToString();
                    }
                    catch
                    {
                    }
                    try
                    {
                        str7 = row.Cells["Traslado_Iva"].Value.ToString();
                    }
                    catch
                    {
                    }
                    if (bool.Parse(str2))
                    {
                        str2 = "True";
                    }
                    str = MapeoQuerySql.ObtenerPorId("UtilImpuestos.guardar_cuentas.VMX_DIOTCUENTAS");
                    str = string.Format(str, str2, str3, str4, str5, str6, str7);
                    if (tipo == "CXC")
                    {
                        str = MapeoQuerySql.ObtenerPorId("UtilImpuestos.guardar_cuentas.VMX_IVACUENTASCXC");
                        str = string.Format(str, str2, str3, str4, str5, str6, str7);
                    }
                    flag = this.utilidades.executar_operaciones_sql(str, this.cnn, false, true);
                }
                catch
                {
                }
            }
            this.utilidades.mensajes_resultado(flag, "Aviso");
        }
    }
}


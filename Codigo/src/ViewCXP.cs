using System.Data;

namespace VMXTRASPIVA
{
    public class ViewCXP
    {
        public static DataTable crearDataTableCXP()
        {
            DataTable table = new DataTable();

            table.Columns.Add("Banco");
            table.Columns.Add("No. Control");
            table.Columns.Add("Cuenta");
            table.Columns.Add("Descripción");
            table.Columns.Add("Cuenta Traslado");
            table.Columns.Add("Tipo Cambio");
            table.Columns.Add("Monto");
            table.Columns.Add("Fecha");

            //table.Columns.Add("Banco", typeof(string));
            //table.Columns.Add("No. Control", typeof(string));
            //table.Columns.Add("Cuenta", typeof(string));
            //table.Columns.Add("Descripción", typeof(string));
            //table.Columns.Add("Cuenta Traslado", typeof(string));
            //table.Columns.Add("Tipo Cambio", typeof(decimal));
            //table.Columns.Add("Monto", typeof(decimal));
            //table.Columns.Add("Fecha", typeof(string));

            return table;
        }
    }
}

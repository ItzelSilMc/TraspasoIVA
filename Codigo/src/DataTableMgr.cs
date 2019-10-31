using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VMXTRASPIVA
{
    public class DataTableMgr
    {
        /// <summary>
        /// Permite copiar las columnas entre dos grid view.
        /// Name y HeaderText
        /// </summary>
        /// <param name="pgvOrigen">Data Grid View Origen</param>
        /// <param name="pgvDestino">Data Grid View Destino</param>
        public static void copiarColumnas(DataGridView pgvOrigen, DataGridView pgvDestino)
        {
            pgvDestino.Columns.Clear();
            pgvDestino.Rows.Clear();
            
            foreach (DataGridViewColumn columna in pgvOrigen.Columns)
            {
                pgvDestino.Columns.Add(columna.Name, columna.HeaderText);
            }
        }
    }
}

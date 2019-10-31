using System.Collections.Generic;
using System.Data;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows.Forms;
using System;
using System.Diagnostics;
using ClosedXML.Excel;

namespace VMXTRASPIVA
{
    public class ExcelFile
    {
        Excel.Application _AppExcel;
        Excel.Workbook _WorkBook;

        public void exportarDetalle(DataGridView gvTraspaso, string pNombreArchivo)
        {
            DataGridView gvDatos = new DataGridView();
            gvDatos = gvTraspaso;
            _AppExcel = new Excel.Application();
            _WorkBook = _AppExcel.Application.Workbooks.Add(true);

            //  Establecer titulos de columnas
            int irenglonInicial = 1;
            int iContColumns = 1;
            int iColumnasVisibles = 0;
            string sValor = string.Empty;

            foreach (DataGridViewColumn column in gvDatos.Columns)
            {
                if (column.Visible)
                {
                    SetValue(irenglonInicial, iContColumns, column.HeaderText);
                    iContColumns++;
                    iColumnasVisibles++;
                }
            }

            SetFormatTitle(1, 1, 1, iColumnasVisibles);

            // Contenido
            // número de renglon donde se comienza a escribir el GridView
            irenglonInicial = 2;

            foreach (DataGridViewRow oRow in gvDatos.Rows)
            {
                if (!oRow.Visible)
                {
                    irenglonInicial++;
                    continue;
                }

                iContColumns = 1;

                for (int i = 0; i < gvDatos.Columns.Count; i++)
                {
                    if (gvDatos.Columns[i].Visible)
                    {
                        try
                        {
                            sValor = oRow.Cells[i].Value.ToString();

                            if (gvDatos.Columns[i].HeaderText.ToString().Length < sValor.Length)
                            {
                                //Establecer el tamaño de las columnas
                                SetColumnWidth(iContColumns, sValor.Length + 3);
                            }
                            else
                            {
                                SetColumnWidth(iContColumns, gvDatos.Columns[i].HeaderText.ToString().Length + 3);
                            }
                        }
                        catch (Exception)
                        {
                            sValor = "";
                        }
                        
                        if (gvDatos.Columns[i].HeaderText == "Banco")
                        {
                            ((Excel.Range)_AppExcel.Cells[irenglonInicial, iContColumns]).NumberFormat = "@";
                        }
                        if (gvDatos.Columns[i].HeaderText == "No. Cheque")
                        {
                            ((Excel.Range)_AppExcel.Cells[irenglonInicial, iContColumns]).NumberFormat = "@";
                        }
                        else if (gvDatos.Columns[i].HeaderText == "No. Control")
                        {
                            ((Excel.Range)_AppExcel.Cells[irenglonInicial, iContColumns]).NumberFormat = "@";
                        }
                        else if (gvDatos.Columns[i].HeaderText == "No. Transacción")
                        {
                            ((Excel.Range)_AppExcel.Cells[irenglonInicial, iContColumns]).NumberFormat = "@";
                        }

                        SetValue(irenglonInicial, iContColumns, sValor);

                        iContColumns++;
                    }
                }

                irenglonInicial++;
            }

            object missing = System.Reflection.Missing.Value;

            _WorkBook.SaveAs(pNombreArchivo
                , Excel.XlFileFormat.xlOpenXMLWorkbook
                , missing
                , missing
                , false
                , false
                , Excel.XlSaveAsAccessMode.xlExclusive
                , missing
                , missing
                , missing
                , missing
                , missing);

            _AppExcel.Visible = true;
            Excel.Worksheet worksheet = (Excel.Worksheet)_AppExcel.ActiveSheet;
            ((Excel._Worksheet)worksheet).Activate();
        }

        /// <summary>
        /// DETALLE
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="pNombreArchivo"></param>
        /// 
        public void exportarDetalle(DataTable gvTraspaso, string pNombreArchivo)
        {
            _AppExcel = new Excel.Application();
            _WorkBook = _AppExcel.Application.Workbooks.Add(true);


            XLWorkbook wb = new XLWorkbook();
            //DataTable dt = GetDataTableOrWhatever();
            wb.Worksheets.Add(gvTraspaso, "Hoja1");
            wb.SaveAs(pNombreArchivo);


        }
        public void ExportarDetalleDT(DataTable dt, string pNombreArchivo, string ptipoMovimiento)
        {

            //frmProcesando frmP = new frmProcesando(dt.Rows.Count,ptipoMovimiento);
            //frmP.Show();

            _AppExcel = new Excel.Application();
            _WorkBook = _AppExcel.Application.Workbooks.Add(true);


            XLWorkbook wb = new XLWorkbook();
            //DataTable dt = GetDataTableOrWhatever();
            wb.Worksheets.Add(dt, "Hoja1");
            wb.SaveAs(pNombreArchivo);

        }

        public void SetValue(int pRow, int pColumn, string pValue)
        {
            _AppExcel.Cells[pRow, pColumn] = pValue;
        }

        public void SetColumnWidth(int col, int width)
        {
            ((Excel.Range)_AppExcel.Cells[1, col]).EntireColumn.ColumnWidth = width;
        }

        public void AutoFitColumn(int col)
        {
            ((Excel.Range)_AppExcel.Cells[1, col]).EntireColumn.AutoFit();
        }

        public void SetRowBold(int pRow)
        {
            ((Excel.Range)_AppExcel.Cells[pRow, 1]).EntireRow.Font.Bold = true;
        }

        public void setCellBold(int pRow, int pColumn)
        {
            ((Excel.Range)_AppExcel.Cells[pRow, pColumn]).Font.Bold = true;
        }

        public void SetFormatTitle(int startRow, int startCol, int endRow, int endCol)
        {
            var startCell = ((Excel.Range)_AppExcel.Cells[startRow, startCol]);
            var endCell = ((Excel.Range)_AppExcel.Cells[endRow, endCol]);

            ((Excel.Range)_AppExcel.Range[startCell, endCell]).Interior.Color = System.Drawing.Color.LightSteelBlue;
            ((Excel.Range)_AppExcel.Range[startCell, endCell]).Font.Bold = true;
        }
    }
}

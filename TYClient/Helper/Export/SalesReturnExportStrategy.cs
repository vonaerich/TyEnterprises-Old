using System;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;

namespace TY.SPIMS.Client.Helper.Export
{
    public class SalesReturnExportStrategy : IExportStrategy
    {
        private string _templatePath;
        private SalesReturnExportObject _itemsToExport;
        private const int rowsPerPage = 35;
        private const int headerRows = 9;
        private const int footerRows = 5;

        public SalesReturnExportStrategy(SalesReturnExportObject items)
        {
            this._templatePath = Directory.GetCurrentDirectory() + @"\Templates\SalesReturnTemplate.xls";
            this._itemsToExport = items;
        }

        public void CreateNewPage(Excel.Worksheet sheet, int pages)
        {
            Excel.Range source = sheet.get_Range("A1:K35");
            source.Copy(Type.Missing);

            for (int i = 1; i <= pages; i++)
            {
                int newPageFirstRow = (i * rowsPerPage) + 1;
                string newPageFirstCell = string.Format("A{0}", newPageFirstRow);
                Excel.Range destination = sheet.get_Range(newPageFirstCell);
                destination.PasteSpecial(Excel.XlPasteType.xlPasteAll, Excel.XlPasteSpecialOperation.xlPasteSpecialOperationNone, false, false);
            }
        }

        public void PerformExport()
        {
            try
            {
                Excel.Application app = new Excel.Application();
                Excel.Workbook book = app.Workbooks.Open(this._templatePath,
                    0, true, 5, "", "", false, Excel.XlPlatform.xlWindows, "",
                    true, false, 0, true, false, false);

                if(book.Worksheets.Count > 0)
                {
                    Excel.Worksheet sheet = book.Worksheets[1];
            
                    //Code
                    Excel.Range codeRange = sheet.get_Range("A5,G5");
                    codeRange.Value2 = this._itemsToExport.Code;

                    //Customer
                    Excel.Range customerRange = sheet.get_Range("A6,G6");
                    customerRange.Value2 = string.Format("Customer: {0}", this._itemsToExport.Customer);

                    int itemStartRow = 10;
                    int itemEndRow = 30;
                    int itemShowCount = itemEndRow - itemStartRow;

                    int totalPages = 1;
                    int itemCount = this._itemsToExport.Items.Count;
                    if (itemCount > itemShowCount)
                    {
                        int newPages = (itemCount - itemShowCount) / itemShowCount;
                        newPages += itemCount % itemShowCount == 0 ? 0 : 1;

                        totalPages += newPages;
                        CreateNewPage(sheet, newPages);
                    }

                    WritePageNumbers(sheet, totalPages, itemShowCount);

                    //Items
                    int i = itemStartRow;
                    int j = 1;
                    foreach (var item in this._itemsToExport.Items)
                    {
                        string invoiceCellFormat = string.Format("A{0},G{0}", i);
                        Excel.Range invoiceCell = sheet.get_Range(invoiceCellFormat);
                        invoiceCell.Value2 = item.InvoiceNumber;

                        string qtyCellFormat = string.Format("B{0},H{0}", i);
                        Excel.Range qtyCell = sheet.get_Range(qtyCellFormat);
                        qtyCell.Value2 = item.Quantity;

                        string partCellFormat = string.Format("C{0},I{0}", i);
                        Excel.Range partCell = sheet.get_Range(partCellFormat);
                        partCell.Value2 = item.PartNumber;

                        string unitCellFormat = string.Format("D{0},J{0}", i);
                        Excel.Range unitCell = sheet.get_Range(unitCellFormat);
                        unitCell.Value2 = item.UnitPrice;

                        string totalCellFormat = string.Format("E{0},K{0}", i);
                        Excel.Range totalCell = sheet.get_Range(totalCellFormat);
                        totalCell.Value2 = item.TotalAmount;

                        if (j == itemShowCount)
                        {
                            i += headerRows + footerRows + 2;
                            j = 1;
                        }
                        else
                        {
                            i++;
                            j++;
                        }
                    }

                    app.Visible = true;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private void WritePageNumbers(Excel.Worksheet sheet, int totalPages, int itemShowCount)
        {
            int pageNumberRow = 1;
            for (int currentPage = 1; currentPage <= totalPages; currentPage++)
            {
                pageNumberRow += currentPage == 1 ? headerRows + itemShowCount :
                    footerRows + headerRows + itemShowCount + 1;

                string pageCountFormat = string.Format("A{0},G{0}", pageNumberRow);
                Excel.Range pageCountCell = sheet.get_Range(pageCountFormat);
                pageCountCell.Value2 = string.Format("Page {0} of {1}", currentPage, totalPages);
            }
        }
    }
}

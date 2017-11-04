using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;

namespace TY.SPIMS.Client.Helper.Export
{
    public class PurchaseCounterExportStrategy : IExportStrategy
    {
        private string _templatePath;
        private PurchaseCounterExportObject _itemsToExport;
        private const int rowsPerPage = 35;
        private const int headerRows = 5;
        private const int footerRows = 5;

        public PurchaseCounterExportStrategy(PurchaseCounterExportObject items)
        {
            this._templatePath = Directory.GetCurrentDirectory() + @"\Templates\PurchaseCounterTemplate.xls";
            this._itemsToExport = items;
        }

        public void CreateNewPage(Excel.Worksheet sheet, int pages)
        {
            Excel.Range source = sheet.get_Range("A1:I35");
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
                    Excel.Range codeRange = sheet.get_Range("A1,F1");
                    codeRange.Value2 = this._itemsToExport.Code;

                    //Customer
                    Excel.Range supplierRange = sheet.get_Range("A2,F2");
                    supplierRange.Value2 = string.Format("Received from: {0}", this._itemsToExport.Supplier);

                    //Date
                    Excel.Range dateRange = sheet.get_Range("D1,I1");
                    dateRange.Value2 = this._itemsToExport.Date;

                    //Remarks
                    Excel.Range remarksRange = sheet.get_Range("B35,G35");
                    remarksRange.Value2 = this._itemsToExport.Remarks;

                    int itemStartRow = 6;
                    int itemEndRow = 28;
                    int itemShowCount = itemEndRow - itemStartRow + 1;

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
                        string dateCellFormat = string.Format("A{0},F{0}", i);
                        Excel.Range dateCell = sheet.get_Range(dateCellFormat);
                        dateCell.Value2 = item.Date.GetValue().ToString("d");

                        string invoiceNumberFormat = string.Format("B{0},G{0}", i);
                        Excel.Range invoiceCell = sheet.get_Range(invoiceNumberFormat);
                        invoiceCell.Value2 = item.PurchaseId != null ? item.InvoiceNumber : string.Format("({0})", item.MemoNumber);

                        string poNumberFormat = string.Format("C{0},H{0}", i);
                        Excel.Range poCell = sheet.get_Range(poNumberFormat);
                        poCell.Value2 = item.PONumber;

                        string amountFormat = string.Format("D{0},I{0}", i);
                        Excel.Range amountCell = sheet.get_Range(amountFormat);
                        amountCell.Value2 = item.PurchaseId != null ? item.Amount.GetValue() :
                            item.Amount.GetValue() * -1;

                        if (j == itemShowCount)
                        {
                            i += headerRows + footerRows + 3;
                            j = 1;
                        }
                        else
                        {
                            i++;
                            j++;
                        }
                    }

                    if (this._itemsToExport.Discount > 0)
                    {
                        Excel.Range discountRange = sheet.get_Range("C29, H29");
                        discountRange.Value2 = "less: discount";

                        Excel.Range discountValRange = sheet.get_Range("D29, I29");
                        discountValRange.Value2 = this._itemsToExport.Discount;
                    }

                    if (this._itemsToExport.WitholdingTax > 0)
                    {
                        Excel.Range taxRange = sheet.get_Range("C30, H30");
                        taxRange.Value2 = "less: w/tax";

                        Excel.Range taxValRange = sheet.get_Range("D30, I30");
                        taxValRange.Value2 = this._itemsToExport.WitholdingTax;
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
                pageNumberRow += currentPage == 1 ? headerRows + itemShowCount + 1:
                    footerRows + headerRows + itemShowCount + 2;

                string pageCountFormat = string.Format("A{0},F{0}", pageNumberRow);
                Excel.Range pageCountCell = sheet.get_Range(pageCountFormat);
                pageCountCell.Value2 = string.Format("Page {0} of {1}", currentPage, totalPages);
            }
        }
    }
}

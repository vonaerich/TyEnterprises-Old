using System;
using System.IO;
using System.Linq;
using Excel = Microsoft.Office.Interop.Excel;

namespace TY.SPIMS.Client.Helper.Export
{
    public class VoucherExportStrategy : IExportStrategy
    {
        private string _templatePath;
        private VoucherExportObject _itemsToExport;
        private const int rowsPerPage = 35;
        private const int headerRows = 4;
        private const int footerRows = 7;

        public VoucherExportStrategy(VoucherExportObject items)
        {
            this._templatePath = Directory.GetCurrentDirectory() + @"\Templates\VoucherTemplate.xls";
            this._itemsToExport = items;
        }

        public void CreateNewPage(Excel.Worksheet sheet, int pages)
        {
            Excel.Range source = sheet.get_Range("A1:M35");
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
                    Excel.Range codeRange = sheet.get_Range("A1,H1");
                    codeRange.Value2 = this._itemsToExport.Code;

                    //Customer
                    Excel.Range customerRange = sheet.get_Range("A2,H2");
                    customerRange.Value2 = string.Format("Supplier: {0}", this._itemsToExport.Supplier);

                    //Remarks
                    Excel.Range remarksRange = sheet.get_Range("B32,I32");
                    remarksRange.Value2 = this._itemsToExport.Remarks;

                    //Month
                    Excel.Range monthRange = sheet.get_Range("E3,L3");
                    monthRange.Value2 = this._itemsToExport.Month;

                    //Cash
                    Excel.Range cashRange = sheet.get_Range("F32,M32");
                    cashRange.Value2 = this._itemsToExport.Cash;

                    //Checks
                    if (this._itemsToExport != null && this._itemsToExport.Checks.Count > 0)
                    {
                        int cell = 30;
                        foreach(var check in this._itemsToExport.Checks.Take(2))
                        {
                            string rangeFormat = string.Format("A{0},H{0}", cell);
                            Excel.Range checksRange = sheet.get_Range(rangeFormat);
                            checksRange.Value2 = string.Format("DETAILS: ({0} {1} {2} {3})",
                                check.Bank, check.CheckNumber, check.ClearingDate.Date.ToShortDateString(),
                                check.Amount.ToString("Php #,##0.00"));

                            cell++;
                        }
                    }
                    
                    int itemStartRow = 5;
                    int itemEndRow = 26;
                    int itemShowCount = itemEndRow - itemStartRow + 1;
                    int itemShowCountAll = itemShowCount * 2;

                    int totalPages = 1;
                    int itemCount = this._itemsToExport.Items.Count;
                    if (itemCount > itemShowCountAll)
                    {
                        int newPages = (itemCount - itemShowCountAll) / itemShowCountAll;
                        newPages += itemCount % itemShowCountAll == 0 ? 0 : 1;

                        totalPages += newPages;
                        CreateNewPage(sheet, newPages);
                    }

                    WritePageNumbers(sheet, totalPages, itemShowCount);

                    //Items
                    int i = itemStartRow;
                    int topRowOnPage = itemStartRow;
                    int j = 1;
                    foreach (var item in this._itemsToExport.Items)
                    {
                        string invoiceNumberFormat = j < 23 ?
                            string.Format("A{0},H{0}", i) :
                            string.Format("D{0},K{0}", i);
                        Excel.Range invoiceCell = sheet.get_Range(invoiceNumberFormat);
                        invoiceCell.Value2 = item.PurchaseId != null ?
                            item.InvoiceNumber : string.Format("({0})", item.MemoNumber);

                        string dateFormat = j < 23 ?
                            string.Format("B{0},I{0}", i) :
                            string.Format("E{0},L{0}", i);
                        Excel.Range dateCell = sheet.get_Range(dateFormat);
                        dateCell.Value2 = item.Date.HasValue ?
                            item.Date.Value.ToShortDateString() : string.Empty;

                        string amountFormat = j < 23 ?
                            string.Format("C{0},J{0}", i) :
                            string.Format("F{0},M{0}", i);
                        Excel.Range amountCell = sheet.get_Range(amountFormat);
                        amountCell.Value2 = item.PurchaseId != null ? 
                            item.Amount.GetValue() : item.Amount.GetValue() * -1;

                        if (j == itemShowCountAll)
                        {
                            i += headerRows + footerRows + 3;
                            j = 1;
                            topRowOnPage = i;
                        }
                        else if (j == itemShowCount)
                        {
                            i = topRowOnPage;
                            j++;
                        }
                        else
                        {
                            i++;
                            j++;
                        }
                    }

                    if (this._itemsToExport.Discount > 0)
                    {
                        Excel.Range discountRange = sheet.get_Range("E27, L27");
                        discountRange.Value2 = "less: discount";

                        Excel.Range discountValRange = sheet.get_Range("F27, M27");
                        discountValRange.Value2 = this._itemsToExport.Discount;
                    }

                    if (this._itemsToExport.WitholdingTax > 0)
                    {
                        Excel.Range taxRange = sheet.get_Range("E28, L28");
                        taxRange.Value2 = "less: w/tax";

                        Excel.Range taxValRange = sheet.get_Range("F28, M28");
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
                pageNumberRow += currentPage == 1 ? headerRows + itemShowCount + 6 :
                    headerRows + itemShowCount + 9;

                string pageCountFormat = string.Format("A{0},H{0}", pageNumberRow);
                Excel.Range pageCountCell = sheet.get_Range(pageCountFormat);
                pageCountCell.Value2 = string.Format("Page {0} of {1}", currentPage, totalPages);
            }
        }
    }
}

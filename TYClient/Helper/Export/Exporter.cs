using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;

namespace TY.SPIMS.Client.Helper.Export
{
    public class Exporter
    {
        public static void Export()
        {
            Excel.Application app = new Excel.Application();
            Excel.Workbook workbook = app.Workbooks.Open(@"C:\Users\Von\Dropbox\counter sample.xls",
                0, false, 5, "", "", false, Excel.XlPlatform.xlWindows, "",
                true, false, 0, true, false, false);

            try
            {
                app.Visible = true;

                Excel.Worksheet sheet = workbook.Worksheets[1];
                //sheet.Copy(Type.Missing, Type.Missing);
                //sheet = app.Workbooks[2].Sheets[1];

                Excel.Range source = sheet.get_Range("A1:I35");
                Excel.Range destination = sheet.get_Range("A36:I70");

                source.Copy(Type.Missing);
                destination.PasteSpecial(Excel.XlPasteType.xlPasteAll, Excel.XlPasteSpecialOperation.xlPasteSpecialOperationNone, false, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally 
            {
                workbook.Close();
                app.Quit();
            }
        }
    }
}

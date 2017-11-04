using System.Collections.Generic;
using TY.SPIMS.POCOs;

namespace TY.SPIMS.Client.Helper.Export
{
    public class SalesCounterExportObject
    {
        public string Code { get; set; }
        public string Customer { get; set; }
        public string Remarks { get; set; }
        public decimal Discount { get; set; }
        public decimal WitholdingTax { get; set; }
        public string Date { get; set; }
        public List<SalesCounterItemModel> Items { get; set; }
    }
}

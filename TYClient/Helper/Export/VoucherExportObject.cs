using System.Collections.Generic;
using TY.SPIMS.POCOs;

namespace TY.SPIMS.Client.Helper.Export
{
    public class VoucherExportObject
    {
        public string Code { get; set; }
        public string Supplier { get; set; }
        public string Remarks { get; set; }
        public decimal Discount { get; set; }
        public decimal WitholdingTax { get; set; }
        public List<PurchaseCounterItemModel> Items { get; set; }
        public List<CheckColumnModel> Checks { get; set; }
        public decimal Cash { get; set; }
        public string Month { get; set; }
    }
}

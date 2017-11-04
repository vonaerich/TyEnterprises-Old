using System;

namespace TY.SPIMS.POCOs
{
    public class PurchaseCounterItemModel
    {
        public int? PurchaseId { get; set; }
        public int? ReturnId { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime? Date { get; set; }
        public string PONumber { get; set; }
        public string MemoNumber { get; set; }
        public decimal? Amount { get; set; }
    }
}

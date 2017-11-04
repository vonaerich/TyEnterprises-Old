using System;

namespace TY.SPIMS.POCOs
{
    public class PurchaseReturnDetailModel
    {
        public int Id { get; set; }
        public string MemoNumber { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int PartDetailId { get; set; }
        public string PartNumber { get; set; }
        public string AutoPart { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string PONumber { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Balance { get; set; }
        public string MemoDisplay { get; set; }
    }
}

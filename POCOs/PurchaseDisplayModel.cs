using System;

namespace TY.SPIMS.POCOs
{
    public class PurchaseDisplayModel
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string Customer { get; set; }
        public string PONumber { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string Type { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsPaid { get; set; }
        public bool IsDelivered { get; set; }
        public string RecordedBy { get; set; }
        public string ApprovedBy { get; set; }
        public bool IsDeleted { get; set; }
        public string InvoiceDiscount { get; set; }
    }
}

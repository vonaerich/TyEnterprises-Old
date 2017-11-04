using System;
using System.Collections.Generic;

namespace TY.SPIMS.POCOs
{
    public class PurchaseColumnModel
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string PONumber { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int Type { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsPaid { get; set; }
        public string Option { get; set; }
        public string Comment { get; set; }
        public int RecordedBy { get; set; }
        public int? ApprovedBy { get; set; }
        public bool IsDeleted { get; set; }
        public List<PurchaseDetailViewModel> Details { get; set; }
        public decimal VatableSale { get; set; }
        public decimal Vat { get; set; }
        public decimal InvoiceDiscount { get; set; }
        public decimal InvoiceDiscountPercent { get; set; }
        public string PR { get; set; }
        public string PO { get; set; }
    }
}

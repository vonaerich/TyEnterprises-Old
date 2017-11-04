using System;
using System.Collections.Generic;

namespace TY.SPIMS.POCOs
{
    public class SaleColumnModel
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime Date { get; set; }
        public int Type { get; set; }
        public decimal VatableSale { get; set; }
        public decimal Vat { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsPaid { get; set; }
        public int RecordedBy { get; set; }
        public int? ApprovedBy { get; set; }
        public bool IsDeleted { get; set; }
        public string Comment { get; set; }
        public decimal InvoiceDiscount { get; set; }
        public decimal InvoiceDiscountPercent { get; set; }
        public string PR { get; set; }
        public string PO { get; set; }
        public List<SalesDetailViewModel> Details { get; set; }
    }
}

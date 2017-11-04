using System;

namespace TY.SPIMS.POCOs
{
    public class PurchaseDetailViewModel
    {
        public int AutoPartDetailId { get; set; }
        public string AutoPartNumber { get; set; }
        public string AutoPartName { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountPercent2 { get; set; }
        public decimal DiscountPercent3 { get; set; }
        public decimal DiscountedPrice { get; set; }
        public decimal DiscountedPrice2 { get; set; }
        public decimal DiscountedPrice3 { get; set; }
        public string DiscountPercents { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalAmount { get; set; }
        public string InvoiceNumber { get; set; }
        public string Supplier { get; set; }
        public DateTime? PurchaseDate { get; set; }
    }
}

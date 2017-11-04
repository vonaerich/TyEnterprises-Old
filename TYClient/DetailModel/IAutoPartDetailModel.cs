using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TY.SPIMS.Client.DetailModel
{
    public interface IAutoPartDetailModel
    {
        int AutoPartDetailId { get; set; }
        string PartNumber { get; set; }
        string Name { get; set; }
        int Quantity { get; set; }
        string Unit { get; set; }
        decimal UnitPrice { get; set; }
        decimal DiscountPercent { get; set; }
        decimal DiscountPercent2 { get; set; }
        decimal DiscountPercent3 { get; set; }
        decimal DiscountedPrice { get; set; }
        decimal DiscountedPrice2 { get; set; }
        decimal DiscountedPrice3 { get; set; }
        string DiscountPercents { get; set; }
        decimal TotalDiscount { get; set; }
        decimal TotalAmount { get; set; }
    }
}

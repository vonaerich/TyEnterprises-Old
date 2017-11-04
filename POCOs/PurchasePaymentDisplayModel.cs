using System;

namespace TY.SPIMS.POCOs
{
    public class PurchasePaymentDisplayModel
    {
        public string VoucherNumber { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
}

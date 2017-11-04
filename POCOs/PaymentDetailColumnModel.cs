using System;
using System.Collections.Generic;

namespace TY.SPIMS.POCOs
{
    public class PaymentDetailColumnModel
    {
        public int Id { get; set; }
        public string VoucherNumber { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal TotalCashPayment { get; set; }
        public decimal TotalCheckPayment { get; set; }
        public int RecordedBy { get; set; }
        public int? ApprovedBy { get; set; }
        public bool IsDeleted { get; set; }
        public List<SalesCounterItemModel> SalesIds { get; set; }
        public List<PurchaseCounterItemModel> PurchaseIds { get; set; }
        public List<CheckColumnModel> PaymentChecks { get; set; }
        public List<CreditDetailModel> CreditIds { get; set; }
        public List<DebitDetailModel> DebitIds { get; set; }
        public decimal WitholdingTax { get; set; }
        public bool GovtForm { get; set; }
        public decimal TotalAmountDue { get; set; }
        public decimal Discount { get; set; }
    }
}

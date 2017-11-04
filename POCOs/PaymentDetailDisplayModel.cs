using System;

namespace TY.SPIMS.POCOs
{
    public class PaymentDetailDisplayModel
    {
        public int Id { get; set; }
        public string Customer { get; set; }
        public string VoucherNumber { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal TotalCashPayment { get; set; }
        public decimal TotalCheckPayment { get; set; }
        public decimal TotalAmount { get; set; }
        public string RecordedBy { get; set; }
        public string ApprovedBy { get; set; }
        public bool IsDeleted { get; set; }
        public string Status { get; set; }
    }
}

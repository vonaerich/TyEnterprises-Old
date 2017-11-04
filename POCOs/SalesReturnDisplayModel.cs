using System;

namespace TY.SPIMS.POCOs
{
    public class SalesReturnDisplayModel
    {
        public int Id { get; set; }
        public string MemoNumber { get; set; }
        public string Customer { get; set; }
        public DateTime ReturnDate { get; set; }
        public decimal AmountReturn { get; set; }
        public decimal Adjustment { get; set; }
        public decimal TotalCreditAmount { get; set; }
        public decimal AmountUsed { get; set; }
        public string Remarks { get; set; }
        public string RecordedBy { get; set; }
        public string ApprovedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}

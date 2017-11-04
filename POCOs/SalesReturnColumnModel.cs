using System;
using System.Collections.Generic;

namespace TY.SPIMS.POCOs
{
    public class SalesReturnColumnModel
    {
        public int Id { get; set; }
        public string MemoNumber { get; set; }
        public int CustomerId { get; set; }
        public DateTime ReturnDate { get; set; }
        public decimal AmountReturn { get; set; }
        public decimal Adjustment { get; set; }
        public decimal TotalCreditAmount { get; set; }
        public string Remarks { get; set; }
        public bool IsDeleted { get; set; }
        public int RecordedByUser { get; set; }
        public int? ApprovedByUser { get; set; }
        public List<SalesReturnDetailModel> Details { get; set; }
    }
}

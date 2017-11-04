using System;

namespace TY.SPIMS.POCOs
{
    public class CheckColumnModel
    {
        public int Id { get; set; }
        public string CheckNumber { get; set; }
        public string Bank { get; set; }
        public string Branch { get; set; }
        public decimal Amount { get; set; }
        public DateTime CheckDate { get; set; }
        public DateTime ClearingDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}

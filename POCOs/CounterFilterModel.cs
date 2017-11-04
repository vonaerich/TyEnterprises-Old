using System;
using TY.SPIMS.Utilities;

namespace TY.SPIMS.POCOs
{
    public class CounterFilterModel
    {
        public int CustomerId { get; set; }
        public string CounterNumber { get; set; }
        public DateSearchType DateType { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}

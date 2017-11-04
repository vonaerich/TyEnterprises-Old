using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TY.SPIMS.Utilities;

namespace TY.SPIMS.POCOs.BaseClasses
{
    public abstract class FilterModelBase
    {
        public int CustomerId { get; set; }
        public string InvoiceNumber { get; set; }
        public int Type { get; set; }
        public NumericSearchType AmountType { get; set; }
        public decimal AmountValue { get; set; }
        public DateSearchType DateType { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public PaidType Paid { get; set; }
        public string PR { get; set; }
        public string PO { get; set; }
    }
}

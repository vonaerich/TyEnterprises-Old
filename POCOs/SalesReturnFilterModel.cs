﻿using System;
using TY.SPIMS.Utilities;

namespace TY.SPIMS.POCOs
{
    public class SalesReturnFilterModel
    {
        public int CustomerId { get; set; }
        public string MemoNumber { get; set; }
        public NumericSearchType AmountType { get; set; }
        public decimal AmountValue { get; set; }
        public DateSearchType DateType { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public ReturnStatusType Status { get; set; }
    }
}

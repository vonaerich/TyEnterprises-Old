﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TY.SPIMS.POCOs
{
    public class SalesCounterDisplayModel
    {
        public int Id { get; set; }
        public string CounterNumber { get; set; }
        public string Customer { get; set; }
        public DateTime? Date { get; set; }
        public decimal TotalAmount { get; set; }
    }
}

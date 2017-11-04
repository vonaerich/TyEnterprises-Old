using System.Collections.Generic;
using TY.SPIMS.POCOs;

namespace TY.SPIMS.Client.Helper.Export
{
    public class PurchaseReturnExportObject
    {
        public string Code { get; set; }
        public string Supplier { get; set; }
        public List<PurchaseReturnDetailModel> Items { get; set; }
    }
}

using System.Collections.Generic;
using TY.SPIMS.POCOs;

namespace TY.SPIMS.Client.Helper.Export
{
    public class SalesReturnExportObject
    {
        public string Code { get; set; }
        public string Customer { get; set; }
        public List<SalesReturnDetailModel> Items { get; set; }
    }
}

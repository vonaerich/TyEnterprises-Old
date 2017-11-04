using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TY.SPIMS.Utilities;

namespace TY.SPIMS.Client.Helper.CodeGenerator
{
    public class ReceivableCodeGenerator : IGenerator
    {
        #region IGenerator Members

        public string GenerateCode()
        {
            try
            {
                string number = "1";

                var db = ConnectionManager.Instance.Connection;
                var p = db.PaymentDetail
                    .Where(a => a.SalesPayments.Any())
                    .OrderByDescending(a => a.Id)
                    .FirstOrDefault();

                if (p != null) //If there are previous items
                {
                    string[] vCode = p.VoucherNumber.Split('-');
                    if (vCode.Length == 3)
                    {
                        int thisYear = DateTime.Now.Year;
                        int voucherYear = 0;
                        if (int.TryParse(vCode[1], out voucherYear))
                        {
                        }

                        int oldNumber = 0;
                        if (voucherYear == thisYear)
                        {
                            if (int.TryParse(vCode[2], out oldNumber))
                            {
                                number = (oldNumber + 1).ToString();
                            }
                        }
                    }
                }

                //For building the code: RCV-YYYY-00001
                StringBuilder codeBuilder = new StringBuilder();
                codeBuilder.AppendFormat("RCV-{0}-{1}",
                    DateTime.Now.Year.ToString(),
                    number.PadLeft(5, '0'));

                return codeBuilder.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}

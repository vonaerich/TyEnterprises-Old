using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TY.SPIMS.Utilities;

namespace TY.SPIMS.Client.Helper.CodeGenerator
{
    public class VoucherPurchaseCodeGenerator : IGenerator
    {
        //Format: CK[YY]-0001 or SCK[YY]-0001
        public string GenerateCode()
        {
            try
            {
                string number = "1";

                var db = ConnectionManager.Instance.Connection;
                var p = db.PaymentDetail
                    .Where(a => a.PurchasePayments.Any())
                    .OrderByDescending(a => a.Id)
                    .FirstOrDefault();

                if (p != null) //If there are previous items
                {
                    string[] vCode = p.VoucherNumber.Split('-');
                    if (vCode.Length == 2)
                    {
                        string orCode = vCode[0];
                        if (orCode.StartsWith("CK") || orCode.StartsWith("SCK"))
                        {
                            string thisYear = DateTime.Now.Year.ToString().Substring(2, 2);
                            string voucherYear = vCode[0].Length == 4 ? vCode[0].Substring(2, 2) : vCode[0].Substring(3, 2);

                            int oldNumber = vCode[1].ToInt();
                            if (string.Compare(voucherYear, thisYear) == 0)
                                number = (oldNumber + 1).ToString();
                        }
                    }
                }

                StringBuilder codeBuilder = new StringBuilder();
                codeBuilder.AppendFormat("CK{0}-{1}",
                    DateTime.Now.Year.ToString().Substring(2, 2),
                    number.PadLeft(4, '0'));

                return codeBuilder.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

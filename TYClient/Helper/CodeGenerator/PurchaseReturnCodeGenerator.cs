﻿using System;
using System.Linq;
using System.Text;
using TY.SPIMS.Utilities;

namespace TY.SPIMS.Client.Helper.CodeGenerator
{
    public class PurchaseReturnCodeGenerator : IGenerator
    {
        //Format: DM[YY]-0001
        public string GenerateCode()
        {
            try
            {
                string number = "1";

                var db = ConnectionManager.Instance.Connection;
                var p = db.PurchaseReturn
                    .OrderByDescending(a => a.Id)
                    .FirstOrDefault();

                if (p != null) //If there are previous items
                {
                    string[] vCode = p.MemoNumber.Split('-');
                    if (vCode.Length == 2)
                    {
                        string thisYear = DateTime.Now.Year.ToString().Substring(2, 2);
                        string voucherYear = vCode[0].Substring(2, 2);

                        int oldNumber = vCode[1].ToInt();
                        if (string.Compare(voucherYear, thisYear) == 0)
                            number = (oldNumber + 1).ToString();
                    }
                }

                StringBuilder codeBuilder = new StringBuilder();
                codeBuilder.AppendFormat("DM{0}-{1}",
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TY.SISystem.SIClient.Controllers;

namespace TY.SISystem.Utilities
{
    public class DuplicateChecker
    {
        public static bool CodeHasDuplicate(CodeType type, string codeToCheck, int id)
        {
            switch (type)
            {
                case CodeType.Brand: 
                    return BrandNameHasDuplicate(codeToCheck, id);
                case CodeType.Customer:
                    return CustomerHasDuplicate(codeToCheck, id);
                case CodeType.Item:
                    return ItemHasDuplicate(codeToCheck, id);
                case CodeType.Purchase:
                    return PurchaseHasDuplicate(codeToCheck, id);
                case CodeType.PurchasePayment:
                    return PurchasePaymentHasDuplicate(codeToCheck, id);
                case CodeType.PurchaseReturn:
                    return PurchaseReturnHasDuplicate(codeToCheck, id);
                case CodeType.SalesReturn:
                    return SalesReturnHasDuplicate(codeToCheck, id);
                case CodeType.Sale:
                    return SaleHasDuplicate(codeToCheck, id);
                case CodeType.SalesPayment:
                    return SalesPaymentHasDuplicate(codeToCheck, id);
                case CodeType.Supplier:
                    return SupplierHasDuplicate(codeToCheck, id);
                case CodeType.User:
                    return UserHasDuplicate(codeToCheck, id);
                default: 
                    return false;
            }
        }

        private static bool BrandNameHasDuplicate(string codeToCheck, int id)
        {
            bool result = BrandController.Instance.FetchAllBrands()
                            .Any(a => a.BrandName == codeToCheck && a.Id != id);

            return result;
        }

        private static bool CustomerHasDuplicate(string codeToCheck, int id)
        {
            bool result = CustomerController.Instance.FetchAllCustomers()
                            .Any(a => a.CustomerCode == codeToCheck && a.Id != id);

            return result;
        }

        private static bool SupplierHasDuplicate(string codeToCheck, int id)
        {
            bool result = SupplierController.Instance.FetchAllSuppliers()
                            .Any(a => a.SupplierCode == codeToCheck && a.Id != id);

            return result;
        }

        private static bool UserHasDuplicate(string codeToCheck, int id)
        {
            bool result = InventoryUserController.Instance.FetchAllInventoryUsers()
                            .Any(a => a.Username == codeToCheck && a.Id != id);

            return result;
        }

        private static bool ItemHasDuplicate(string codeToCheck, int id)
        {
            bool result = AutoPartController.Instance.FetchAllAutoPartDetails()
                            .Any(a => a.PartNumber == codeToCheck && a.Id != id);

            return result;
        }

        private static bool SaleHasDuplicate(string codeToCheck, int id)
        {
            bool result = SaleController.Instance.FetchAllSales()
                            .Any(a => a.InvoiceNumber == codeToCheck && a.Id != id);

            return result;
        }

        private static bool SalesPaymentHasDuplicate(string codeToCheck, int id)
        {
            bool result = PaymentDetailController.Instance.FetchAllPaymentDetails(PaymentType.Sales)
                            .Any(a => a.VoucherNumber == codeToCheck && a.Id != id);

            return result;
        }

        private static bool PurchaseHasDuplicate(string codeToCheck, int id)
        {
            bool result = PurchaseController.Instance.FetchAllPurchases()
                            .Any(a => a.PONumber == codeToCheck && a.Id != id);

            return result;
        }

        private static bool PurchasePaymentHasDuplicate(string codeToCheck, int id)
        {
            bool result = PaymentDetailController.Instance.FetchAllPaymentDetails(PaymentType.Purchase)
                            .Any(a => a.VoucherNumber == codeToCheck && a.Id != id);

            return result;
        }

        private static bool SalesReturnHasDuplicate(string codeToCheck, int id)
        {
            bool result = SalesReturnController.Instance.FetchAllSalesReturns()
                            .Any(a => a.MemoNumber == codeToCheck && a.Id != id);

            return result;
        }

        private static bool PurchaseReturnHasDuplicate(string codeToCheck, int id)
        {
            bool result = PurchaseReturnController.Instance.FetchAllPurchaseReturns()
                            .Any(a => a.MemoNumber == codeToCheck && a.Id != id);

            return result;
        }
    }
}

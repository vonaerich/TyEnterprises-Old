using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TY.SPIMS.Controllers;
using TY.SPIMS.Utilities;

namespace TY.SPIMS.Client
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
                case CodeType.User:
                    return UserHasDuplicate(codeToCheck, id);
                default: 
                    return false;
            }
        }

        //For Purchase
        public static bool CodeHasDuplicate(string codeToCheck, int id, int supplierId)
        {
            var purchaseController = IOC.Container.GetInstance<PurchaseController>();
            bool result = purchaseController.CheckIfInvoiceHasDuplicate(codeToCheck, id, supplierId);

            return result;
        }

        private static bool BrandNameHasDuplicate(string codeToCheck, int id)
        {
            var brandController = IOC.Container.GetInstance<BrandController>();
            bool result = brandController.FetchAllBrands().Any(a => a.BrandName == codeToCheck && a.Id != id);

            return result;
        }

        private static bool CustomerHasDuplicate(string codeToCheck, int id)
        {
            var customerController = IOC.Container.GetInstance<CustomerController>();
            bool result = customerController.FetchAllCustomers().Any(a => a.CustomerCode == codeToCheck && a.Id != id);

            return result;
        }

        private static bool UserHasDuplicate(string codeToCheck, int id)
        {
            var inventoryUserController = IOC.Container.GetInstance<InventoryUserController>();
            bool result = inventoryUserController.FetchAllInventoryUsers().Any(a => a.Username == codeToCheck && a.Id != id);

            return result;
        }

        private static bool ItemHasDuplicate(string codeToCheck, int id)
        {
            var autoPartController = IOC.Container.GetInstance<AutoPartController>();
            bool result = autoPartController.FetchAllAutoPartDetails().Any(a => a.PartNumber == codeToCheck && a.Id != id);

            return result;
        }

        private static bool SaleHasDuplicate(string codeToCheck, int id)
        {
            var salesController = IOC.Container.GetInstance<SaleController>();
            bool result = salesController.CheckIfInvoiceHasDuplicate(codeToCheck, id);

            return result;
        }

        private static bool SalesPaymentHasDuplicate(string codeToCheck, int id)
        {
            var paymentDetailController = IOC.Container.GetInstance<PaymentDetailController>();
            bool result = paymentDetailController.FetchAllPaymentDetails(PaymentType.Sales).Any(a => a.VoucherNumber == codeToCheck && a.Id != id);

            return result;
        }

        private static bool PurchaseHasDuplicate(string codeToCheck, int id)
        {
            var purchaseController = IOC.Container.GetInstance<PurchaseController>();
            bool result = purchaseController.FetchAllPurchases().Any(a => a.InvoiceNumber == codeToCheck && a.Id != id);

            return result;
        }

        private static bool PurchasePaymentHasDuplicate(string codeToCheck, int id)
        {
            var paymentDetailController = IOC.Container.GetInstance<PaymentDetailController>(); 
            bool result = paymentDetailController.FetchAllPaymentDetails(PaymentType.Purchase).Any(a => a.VoucherNumber == codeToCheck && a.Id != id);

            return result;
        }

        private static bool SalesReturnHasDuplicate(string codeToCheck, int id)
        {
            var salesReturnController = IOC.Container.GetInstance<SalesReturnController>();
            bool result = salesReturnController.CheckReturnHasDuplicate(codeToCheck, id);

            return result;
        }

        private static bool PurchaseReturnHasDuplicate(string codeToCheck, int id)
        {
            var purchaseReturnController = IOC.Container.GetInstance<PurchaseReturnController>();
            bool result = purchaseReturnController.CheckReturnHasDuplicate(codeToCheck, id);

            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;

namespace TY.SPIMS.Utilities
{
    public class SystemSettings
    {
        public static Dictionary<string, bool> Approval = new Dictionary<string, bool>();
        public static Dictionary<string, Dictionary<string, bool>> AccessRights = new Dictionary<string, Dictionary<string, bool>>();

        public static void RefreshApproval()
        {
            try
            {
                var db = ConnectionManager.Instance.Connection;
                var settings = db.ApprovalSetting.FirstOrDefault();

                Approval.Clear();
                if (settings != null)
                {
                    Approval.Add("InventoryAdd", settings.InventoryAdd.HasValue ? settings.InventoryAdd.Value : false);
                    Approval.Add("InventoryEdit", settings.InventoryEdit.HasValue ? settings.InventoryEdit.Value : false);
                    Approval.Add("InventoryDelete", settings.InventoryDelete.HasValue ? settings.InventoryDelete.Value : false);
                    Approval.Add("PaymentsAdd", settings.PaymentAdd.HasValue ? settings.PaymentAdd.Value : false);
                    Approval.Add("PaymentsEdit", settings.PaymentEdit.HasValue ? settings.PaymentEdit.Value : false);
                    Approval.Add("PaymentsDelete", settings.PaymentDelete.HasValue ? settings.PaymentDelete.Value : false);
                    Approval.Add("PurchaseAdd", settings.PurchaseAdd.HasValue ? settings.PurchaseAdd.Value : false);
                    Approval.Add("PurchaseEdit", settings.PurchaseEdit.HasValue ? settings.PurchaseEdit.Value : false);
                    Approval.Add("PurchaseDelete", settings.PurchaseDelete.HasValue ? settings.PurchaseDelete.Value : false);
                    Approval.Add("PurchaseReturnAdd", settings.PurchaseReturnAdd.HasValue ? settings.PurchaseReturnAdd.Value : false);
                    Approval.Add("PurchaseReturnEdit", settings.PurchaseReturnEdit.HasValue ? settings.PurchaseReturnEdit.Value : false);
                    Approval.Add("PurchaseReturnDelete", settings.PurchaseReturnDelete.HasValue ? settings.PurchaseReturnDelete.Value : false);
                    Approval.Add("SalesAdd", settings.SalesAdd.HasValue ? settings.SalesAdd.Value : false);
                    Approval.Add("SalesEdit", settings.SalesEdit.HasValue ? settings.SalesEdit.Value : false);
                    Approval.Add("SalesDelete", settings.SalesDelete.HasValue ? settings.SalesDelete.Value : false);
                    Approval.Add("SalesReturnAdd", settings.SalesReturnAdd.HasValue ? settings.SalesReturnAdd.Value : false);
                    Approval.Add("SalesReturnEdit", settings.SalesReturnEdit.HasValue ? settings.SalesReturnEdit.Value : false);
                    Approval.Add("SalesReturnDelete", settings.SalesReturnDelete.HasValue ? settings.SalesReturnDelete.Value : false);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public static void SaveApproval()
        {
            try
            {
                var db = ConnectionManager.Instance.Connection;
                var settings = db.ApprovalSetting.FirstOrDefault();

                if (settings != null)
                {
                    settings.InventoryAdd = Approval["InventoryAdd"];
                    settings.InventoryEdit = Approval["InventoryEdit"];
                    settings.InventoryDelete = Approval["InventoryDelete"];
                    settings.PaymentAdd = Approval["PaymentsAdd"];
                    settings.PaymentEdit = Approval["PaymentsEdit"];
                    settings.PaymentDelete = Approval["PaymentsDelete"];
                    settings.PurchaseAdd = Approval["PurchaseAdd"];
                    settings.PurchaseEdit = Approval["PurchaseEdit"];
                    settings.PurchaseDelete = Approval["PurchaseDelete"];
                    settings.PurchaseReturnAdd = Approval["PurchaseReturnAdd"];
                    settings.PurchaseReturnEdit = Approval["PurchaseReturnEdit"];
                    settings.PurchaseReturnDelete = Approval["PurchaseReturnDelete"];
                    settings.SalesAdd = Approval["SalesAdd"];
                    settings.SalesEdit = Approval["SalesEdit"];
                    settings.SalesDelete = Approval["SalesDelete"];
                    settings.SalesReturnAdd = Approval["SalesReturnAdd"];
                    settings.SalesReturnEdit = Approval["SalesReturnEdit"];
                    settings.SalesReturnDelete = Approval["SalesReturnDelete"];

                    db.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void RefreshAccessRights()
        {
            try
            {
                var db = ConnectionManager.Instance.Connection;
                var access = db.AccessRights;

                AccessRights.Clear();
                foreach (var a in access)
                {
                    Dictionary<string, bool> c = new Dictionary<string, bool>();
                    c.Add("AccessBrands", a.AccessBrands.HasValue ? a.AccessBrands.Value : false);
                    c.Add("AccessChecks", a.AccessChecks.HasValue ? a.AccessChecks.Value : false);
                    c.Add("AccessCustomers", a.AccessCustomers.HasValue ? a.AccessCustomers.Value : false);
                    c.Add("AccessInventory", a.AccessInventory.HasValue ? a.AccessInventory.Value : false);
                    c.Add("AccessLogs", a.AccessLogs.HasValue ? a.AccessLogs.Value : false);
                    c.Add("AccessPayment", a.AccessPayment.HasValue ? a.AccessPayment.Value : false);
                    c.Add("AccessPurchase", a.AccessPurchase.HasValue ? a.AccessPurchase.Value : false);
                    c.Add("AccessPurchaseReturn", a.AccessPurchaseReturn.HasValue ? a.AccessPurchaseReturn.Value : false);
                    c.Add("AccessReports", a.AccessReports.HasValue ? a.AccessReports.Value : false);
                    c.Add("AccessSales", a.AccessSales.HasValue ? a.AccessSales.Value : false);
                    c.Add("AccessSalesReturn", a.AccessSalesReturn.HasValue ? a.AccessSalesReturn.Value : false);
                    c.Add("AccessSettings", a.AccessSettings.HasValue ? a.AccessSettings.Value : false);
                    c.Add("AccessSuppliers", a.AccessSuppliers.HasValue ? a.AccessSuppliers.Value : false);
                    c.Add("AccessUsers", a.AccessUsers.HasValue ? a.AccessUsers.Value : false);

                    AccessRights.Add(a.UserType, c);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void SaveAccessRights()
        {
            try
            {
                var db = ConnectionManager.Instance.Connection;

                foreach (string key in AccessRights.Keys)
                {
                    var access = db.AccessRights.Where(x => x.UserType == key).FirstOrDefault();

                    if (access != null)
                    {
                        Dictionary<string, bool> c = AccessRights[key];

                        access.AccessBrands = c["AccessBrands"];
                        access.AccessChecks = c["AccessChecks"];
                        access.AccessCustomers = c["AccessCustomers"];
                        access.AccessInventory = c["AccessInventory"];
                        access.AccessLogs = c["AccessLogs"];
                        access.AccessPayment = c["AccessPayment"];
                        access.AccessPurchase = c["AccessPurchase"];
                        access.AccessPurchaseReturn = c["AccessPurchaseReturn"];
                        access.AccessReports = c["AccessReports"];
                        access.AccessSales = c["AccessSales"];
                        access.AccessSalesReturn = c["AccessSalesReturn"];
                        access.AccessSettings = c["AccessSettings"];
                        access.AccessSuppliers = c["AccessSuppliers"];
                        access.AccessUsers = c["AccessUsers"];
                    }
                }

                db.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}

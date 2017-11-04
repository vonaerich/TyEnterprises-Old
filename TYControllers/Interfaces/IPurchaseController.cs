using System;
using TY.SPIMS.Utilities;
using TY.SPIMS.Entities;
using TY.SPIMS.POCOs;
using System.Collections.Generic;

namespace TY.SPIMS.Controllers.Interfaces
{
    public interface IPurchaseController
    {
        bool CheckIfInvoiceHasDuplicate(string codeToCheck, int id, int supplierId);
        void DeletePurchase(int id);
        string[] FetchAllPONumbersByCustomer(int CustomerId);
        SortableBindingList<PurchasesView> FetchAllPurchases();
        SortableBindingList<PurchaseDisplayModel> FetchAPWithSearch(PurchaseFilterModel filter);
        Purchase FetchPurchaseById(int id);
        SortableBindingList<PurchasesView> FetchPurchaseByIds(List<int> ids);
        Purchase FetchPurchaseByPO(string poNumber);
        Purchase FetchPurchaseByPOAndSupplier(string poNumber, int supplierId);
        SortableBindingList<PurchaseDetailViewModel> FetchPurchaseDetails(int id);
        List<PurchaseDetailViewModel> FetchPurchaseDetailsPerPO(string poNumber);
        List<PurchaseDetailViewModel> FetchPurchaseDetailsPerPOAndSupplier(string poNumber, int supplierId);
        List<PurchaseDetailViewModel> FetchPurchasesPerItemPerSupplier(int autoPartDetailId, int supplierId = 0);
        SortableBindingList<PurchasesView> FetchPurchaseWithSearch(PurchaseFilterModel filter);
        string GetPONumber(int purchaseId);
        void InsertPurchase(PurchaseColumnModel model);
        bool ItemHasPaymentOrReturn(int id);
        void UpdatePurchase(PurchaseColumnModel model);
    }
}

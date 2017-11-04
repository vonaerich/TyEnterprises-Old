using System.Collections.Generic;
using TY.SPIMS.Entities;
using TY.SPIMS.POCOs;
using TY.SPIMS.Utilities;

namespace TY.SPIMS.Controllers.Interfaces
{
    public interface IPurchaseReturnController
    {
        bool CheckReturnHasDuplicate(string codeToCheck, int id);
        void DeletePurchaseReturn(int id);
        SortableBindingList<PurchaseReturnDetailModel> FetchAllPurchaseReturnDetails();
        List<PurchaseReturnDisplayModel> FetchAllPurchaseReturns();
        SortableBindingList<PurchaseReturnDetailModel> FetchAllReturnsInPurchase(string poNumber);
        SortableBindingList<PaymentDebitModel> FetchPaymentDetails(int returnId);
        PurchaseReturn FetchPurchaseReturnById(int id);
        SortableBindingList<PurchaseReturnDetailModel> FetchPurchaseReturnByIds(List<int> list);
        SortableBindingList<PurchaseReturnDetailModel> FetchPurchaseReturnDetails(int id);
        SortableBindingList<PurchaseReturnDisplayModel> FetchPurchaseReturnWithSearch(PurchaseReturnFilterModel filter);
        int FetchQuantityReturned(int CustomerId, string poNumber, int itemId, int returnId);
        List<PurchaseReturnDetailModel> GetReturnsPerInvoice(string invoiceNumber);
        List<SalesReturnDetailModel> GetReturnsWithBalance(int customerId);
        void InsertPurchaseReturn(PurchaseReturnColumnModel model);
        bool? IsItemDebited(int id);
        bool POHasReturn(string poNumber);
        void UpdatePurchaseReturn(PurchaseReturnColumnModel model);
    }
}

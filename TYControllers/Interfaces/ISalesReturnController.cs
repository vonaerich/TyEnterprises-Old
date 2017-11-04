using System.Collections.Generic;
using TY.SPIMS.POCOs;
using TY.SPIMS.Utilities;
using TY.SPIMS.Entities;

namespace TY.SPIMS.Controllers.Interfaces
{
    public interface ISalesReturnController
    {
        bool CheckReturnHasDuplicate(string codeToCheck, int id);
        void DeleteSalesReturn(int id);
        SortableBindingList<SalesReturnDetailModel> FetchAllReturnsInSale(string invoiceNumber);
        SortableBindingList<SalesReturnDetailModel> FetchAllSalesReturnDetails();
        SortableBindingList<SalesReturnDisplayModel> FetchAllSalesReturns();
        SortableBindingList<PaymentCreditModel> FetchPaymentDetails(int returnId);
        int FetchQuantityReturned(int customerId, string invoiceNumber, int itemId, int returnId);
        SalesReturn FetchSalesReturnById(int id);
        SortableBindingList<SalesReturnDetailModel> FetchSalesReturnByIds(List<int> list);
        SortableBindingList<SalesReturnDetailModel> FetchSalesReturnDetails(int id);
        SortableBindingList<SalesReturnDisplayModel> FetchSalesReturnWithSearch(SalesReturnFilterModel filter);
        List<SalesReturnDetailModel> GetReturnsPerInvoice(string invoiceNumber);
        List<SalesReturnDetailModel> GetReturnsWithBalance(int customerId);
        void InsertSalesReturn(SalesReturnColumnModel model);
        bool InvoiceHasReturn(string invoiceNumber);
        bool? IsItemCredited(int id);
        void UpdateSalesReturn(SalesReturnColumnModel model);
    }
}

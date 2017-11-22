using System;
using TY.SPIMS.Utilities;
using TY.SPIMS.Entities;
using System.Collections.Generic;
using TY.SPIMS.POCOs;
using System.Linq;

namespace TY.SPIMS.Controllers.Interfaces
{
    public interface ISaleController
    {
        bool CheckIfInvoiceHasDuplicate(string codeToCheck, int id);
        void DeleteSale(int id);
        string[] FetchAllInvoiceNumbersByCustomer(int customerId);
        SortableBindingList<SalesView> FetchAllSales();
        SortableBindingList<SaleDisplayModel> FetchARWithSearch(SaleFilterModel filter);
        Sale FetchSaleById(int id);
        SortableBindingList<SalesView> FetchSaleByIds(List<int> ids);
        Sale FetchSaleByInvoiceNumber(string invoiceNumber);
        SortableBindingList<SalesDetailViewModel> FetchSaleDetails(int id);
        List<SalesDetailViewModel> FetchSaleDetailsPerInvoice(string invoiceNumber);
        List<SalesDetailViewModel> FetchSalesPerItemPerCustomer(int autoPartDetailId, int customerId = 0);
        SortableBindingList<SalesView> FetchSaleWithSearch(SaleFilterModel filter);
        string GetPONumber(int saleId);
        void InsertSale(SaleColumnModel model);
        bool ItemHasPaymentOrReturn(int id);
        void UpdateSale(SaleColumnModel model);
        IQueryable<SalesView> FetchSalesWithSearchGeneric(SaleFilterModel filter);
    }
}

using System.Collections.Generic;
using TY.SPIMS.Entities;
using TY.SPIMS.POCOs;
using TY.SPIMS.Utilities;

namespace TY.SPIMS.Controllers.Interfaces
{
    public interface IPaymentDetailController
    {
        void DeletePaymentDetail(int id);
        List<PaymentDetailDisplayModel> FetchAllPaymentDetails(PaymentType type);
        List<CheckColumnModel> FetchPaymentChecks(int paymentId);
        PaymentDetail FetchPaymentDetailById(int id);
        SortableBindingList<PaymentDetailDisplayModel> FetchPaymentDetailWithSearch(PaymentType type, PaymentDetailFilterModel filter);
        SortableBindingList<SaleDisplayModel> FetchSaleDetails(int paymentId);
        void InsertPaymentDetail(PaymentDetail payment);
        void RevertItem(PaymentRevertType type, int paymentId, int id, decimal amount);
        SortableBindingList<PurchasePaymentDisplayModel> SearchPurchasePayment(int purchaseId);
        SortableBindingList<SalesPaymentDisplayModel> SearchSalePayment(int saleId);
        void UpdatePaymentDetail(PaymentDetail payment);
    }
}

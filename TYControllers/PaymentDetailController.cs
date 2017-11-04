using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TY.SPIMS.Controllers.Interfaces;
using TY.SPIMS.Entities;
using TY.SPIMS.POCOs;
using TY.SPIMS.Utilities;

namespace TY.SPIMS.Controllers
{
    public class PaymentDetailController : IPaymentDetailController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IActionLogController actionLogController;

        private TYEnterprisesEntities db
        {
            get
            { return unitOfWork.Context; }
        }

        public PaymentDetailController(IUnitOfWork unitOfWork, IActionLogController actionLogController)
        {
            this.unitOfWork = unitOfWork;
            this.actionLogController = actionLogController;
        }

        #region CUD Functions

        public void InsertPaymentDetail(PaymentDetail payment)
        {
            try
            {
                using (this.unitOfWork)
                {
                    if (payment != null)
                    {
                        this.ProcessChildrenInfo(ref payment);

                        payment.IsDeleted = false;
                        this.unitOfWork.Context.PaymentDetail.AddObject(payment);

                        string action = string.Format("Added New Payment - {0}", payment.VoucherNumber);
                        this.actionLogController.AddToLog(action, UserInfo.UserId);
                        this.unitOfWork.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdatePaymentDetail(PaymentDetail payment)
        {
            try
            {
                using (this.unitOfWork)
                {
                    var original = db.PaymentDetail.Single(a => a.Id == payment.Id);
                    this.SetChildrenToDefault(ref original);
                    this.ProcessChildrenInfo(ref payment);

                    db.PaymentDetail.Attach(original);
                    db.PaymentDetail.ApplyCurrentValues(payment);

                    payment.SalesPayments.ToList().ForEach(a => original.SalesPayments.Add(a));
                    payment.PurchasePayments.ToList().ForEach(a => original.PurchasePayments.Add(a));
                    payment.Check.ToList().ForEach(a => original.Check.Add(a));

                    string action = string.Format("Updated Payment - {0}", payment.VoucherNumber);
                    this.actionLogController.AddToLog(action, UserInfo.UserId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeletePaymentDetail(int id)
        {
            try
            {
                using (this.unitOfWork)
                {
                    PaymentDetail item = FetchPaymentDetailById(id);
                    this.SetChildrenToDefault(ref item);

                    item.IsDeleted = true;
                    
                    string action = string.Format("Deleted Payment - {0}", item.VoucherNumber);
                    this.actionLogController.AddToLog(action, UserInfo.UserId);
                    this.unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ProcessChildrenInfo(ref PaymentDetail payment)
        {
            if (payment.SalesPayments != null)
            {
                foreach (var item in payment.SalesPayments)
                {
                    if (item.SalesId != null)
                    {
                        var s = db.Sale.Single(a => a.Id == item.SalesId);
                        s.Balance -= item.Amount;
                        if (s.Balance == 0)
                            s.IsPaid = true;
                    }
                    else if (item.SalesReturnDetailId != null)
                    {
                        var r = db.SalesReturnDetail.Single(a => a.Id == item.SalesReturnDetailId);
                        r.Balance -= item.Amount;
                        r.SalesReturn.AmountUsed += item.Amount;
                        r.SalesReturn.IsUsed = true;
                    }
                }
            }

            if (payment.PurchasePayments != null)
            {
                foreach (var item in payment.PurchasePayments)
                {
                    if (item.PurchaseId != null)
                    {
                        var p = db.Purchase.Single(a => a.Id == item.PurchaseId);
                        p.Balance -= item.Amount;
                        if (p.Balance == 0)
                            p.IsPaid = true;
                    }
                    else if (item.PurchaseReturnDetailId != null)
                    {
                        var d = db.PurchaseReturnDetail.Single(a => a.Id == item.PurchaseReturnDetailId);
                        d.Balance -= item.Amount;
                        d.PurchaseReturn.AmountUsed += item.Amount;
                        d.PurchaseReturn.IsUsed = true;
                    }

                }
            }
        }

        private void SetChildrenToDefault(ref PaymentDetail item)
        {
            foreach(var saleItem in item.SalesPayments)
            {
                if(saleItem.SalesId != null)
                {
                    saleItem.Sale.IsPaid = false;
                    saleItem.Sale.Balance += saleItem.Amount;
                }
                else if(saleItem.SalesReturnDetailId != null)
                {
                    saleItem.SalesReturnDetail.Balance += saleItem.Amount;
                    saleItem.SalesReturnDetail.SalesReturn.AmountUsed -= saleItem.Amount;
                    saleItem.SalesReturnDetail.SalesReturn.IsUsed = false;
                }
            }

            foreach(var purchaseItem in item.PurchasePayments)
            {
                if(purchaseItem.PurchaseId != null)
                {
                    purchaseItem.Purchase.IsPaid = false;
                    purchaseItem.Purchase.Balance += purchaseItem.Amount;
                }
                else if(purchaseItem.PurchaseReturnDetailId != null)
                {
                    purchaseItem.PurchaseReturnDetail.Balance += purchaseItem.Amount;
                    purchaseItem.PurchaseReturnDetail.PurchaseReturn.AmountUsed -= purchaseItem.Amount;
                    purchaseItem.PurchaseReturnDetail.PurchaseReturn.IsUsed = false;
                }
            }

            item.SalesPayments.ToList().ForEach(a => db.DeleteObject(a));
            item.PurchasePayments.ToList().ForEach(a => db.DeleteObject(a));
            item.Check.ToList().ForEach(a => db.DeleteObject(a));
        }

        #endregion

        #region Fetch Functions

        private IQueryable<PaymentDetail> CreateQuery(PaymentType type, PaymentDetailFilterModel filter)
        {
            var items = from i in db.PaymentDetail
                        select i;

            if (type == PaymentType.Sales)
                items = items.Where(a => a.SalesPayments.Any());
            else
                items = items.Where(a => a.PurchasePayments.Any());

            if(filter != null)
            {
                if (filter.CustomerId != 0)
                    items = items.Where(a => a.CustomerId == filter.CustomerId);

                if (!string.IsNullOrWhiteSpace(filter.VoucherNumber))
                    items = items.Where(a => a.VoucherNumber.Contains(filter.VoucherNumber));

                if (filter.AmountType != NumericSearchType.All)
                {
                    if (filter.AmountType == NumericSearchType.Equal)
                        items = items.Where(a => a.TotalCashPayment + a.TotalCheckPayment == filter.AmountValue);
                    else if (filter.AmountType == NumericSearchType.GreaterThan)
                        items = items.Where(a => a.TotalCashPayment + a.TotalCheckPayment > filter.AmountValue);
                    else if (filter.AmountType == NumericSearchType.LessThan)
                        items = items.Where(a => a.TotalCashPayment + a.TotalCheckPayment < filter.AmountValue);
                }

                if (filter.DateType != DateSearchType.All)
                {
                    DateTime dateFrom = filter.DateFrom.Date;
                    DateTime dateTo = filter.DateTo.AddDays(1).Date;

                    items = items.Where(a => a.PaymentDate >= dateFrom && a.PaymentDate < dateTo);
                }
            }

            return items;
        }

        public SortableBindingList<PaymentDetailDisplayModel> FetchPaymentDetailWithSearch(PaymentType type, PaymentDetailFilterModel filter)
        {
            try
            {
                var query = CreateQuery(type, filter);

                var modResult = query.Select(a => new PaymentDetailDisplayModel() { 
                    Id = a.Id,
                    VoucherNumber = a.VoucherNumber,
                    PaymentDate = a.PaymentDate.Value,
                    TotalCashPayment = a.TotalCashPayment.HasValue ? a.TotalCashPayment.Value : 0,
                    TotalCheckPayment = a.TotalCheckPayment.HasValue ? a.TotalCheckPayment.Value : 0,
                    TotalAmount = (a.TotalCashPayment.HasValue ? a.TotalCashPayment.Value : 0) +
                                  (a.TotalCheckPayment.HasValue ? a.TotalCheckPayment.Value : 0),
                    RecordedBy = a.RecordedByUser.Firstname + " " + a.RecordedByUser.Lastname,
                    IsDeleted = a.IsDeleted.Value,
                    Customer = a.CustomerId != null ? a.Customer.CompanyName : "-",
                    Status = a.TotalAmountDue < a.TotalCashPayment + a.TotalCheckPayment ? "Fully Paid" : 
                        a.TotalCashPayment == 0 && a.TotalCheckPayment == 0 ? "Not Paid" : "Partially Paid"
                }); 

                if (filter == null)
                    modResult = modResult.OrderByDescending(a => a.PaymentDate);

                SortableBindingList<PaymentDetailDisplayModel> b = new SortableBindingList<PaymentDetailDisplayModel>(modResult);

                return b;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string ComposeSalesPaymentDetail(int id)
        {
            StringBuilder builder = new StringBuilder();

            var payment = (from p in db.PaymentDetail
                           where p.Id == id
                           select p).FirstOrDefault();

            //bool firstLine = true;
            //if (payment.Sale.Count > 0) //Display Sales Details
            //{
            //    decimal total = 0m;
            //    foreach (var s in payment.Sale)
            //    {
            //        builder.AppendFormat("[{0}] {1}", 
            //            s.InvoiceNumber,
            //            s.TotalAmount.Value.ToString("Php #,##0.00"));
            //        builder.AppendLine();

            //        total += s.TotalAmount.HasValue ? s.TotalAmount.Value : 0;
            //    }

            //    builder.AppendLine("-----------------------");
            //    builder.AppendFormat("Php {0}", total.ToString("N2"));
            //}
            //else if (payment.Purchase.Count > 0) //Display Purchase Details
            //{
            //    foreach (var s in payment.Purchase)
            //    {
            //        if (firstLine)
            //        {
            //            builder.AppendLine(s.Customer.CompanyName);
            //            firstLine = false;
            //        }

            //        builder.AppendFormat("{0} - {1} - {2}",
            //            s.PONumber, s.PurchaseDate.Value.ToShortDateString(),
            //            s.TotalAmount.Value.ToString("Php #,##0.00"));
            //        builder.AppendLine();
            //    }
            //}

            return builder.ToString();
        }

        public List<PaymentDetailDisplayModel> FetchAllPaymentDetails(PaymentType type)
        {
            try
            {
                var query = CreateQuery(type, null);

                var result = from a in query
                             select new PaymentDetailDisplayModel
                             {
                                 Id = a.Id,
                                 VoucherNumber = a.VoucherNumber,
                                 PaymentDate = a.PaymentDate.Value,
                                 TotalCashPayment = a.TotalCashPayment.Value,
                                 TotalCheckPayment = a.TotalCheckPayment.Value,
                                 RecordedBy = a.RecordedByUser.Firstname + " " + a.RecordedByUser.Lastname,
                                 IsDeleted = a.IsDeleted.Value,
                             };

                return result.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PaymentDetail FetchPaymentDetailById(int id)
        {
            try
            {
                var item = (from i in db.PaymentDetail
                            where i.Id == id
                            select i).FirstOrDefault();

                return item;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SortableBindingList<SalesPaymentDisplayModel> SearchSalePayment(int saleId)
        {
            try
            { 
                var detail = db.SalesPayments
                    .Where(a => a.SalesId != null && a.SalesId == saleId && a.PaymentDetail.IsDeleted != true)
                    .Select(a => new SalesPaymentDisplayModel() { 
                        ORNumber = a.PaymentDetail.VoucherNumber,
                        Date = a.PaymentDetail.PaymentDate.HasValue ? a.PaymentDetail.PaymentDate.Value : DateTime.Now,
                        Amount = a.Amount.HasValue ? a.Amount.Value : 0
                    });

                var result = new SortableBindingList<SalesPaymentDisplayModel>(detail);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SortableBindingList<PurchasePaymentDisplayModel> SearchPurchasePayment(int purchaseId)
        {
            try
            {
                var detail = db.PurchasePayments
                    .Where(a => a.PurchaseId != null && a.PurchaseId == purchaseId && a.PaymentDetail.IsDeleted != true)
                    .Select(a => new PurchasePaymentDisplayModel()
                    {
                        VoucherNumber = a.PaymentDetail.VoucherNumber,
                        Date = a.PaymentDetail.PaymentDate.HasValue ? a.PaymentDetail.PaymentDate.Value : DateTime.Now,
                        Amount = a.Amount.HasValue ? a.Amount.Value : 0
                    });

                var result = new SortableBindingList<PurchasePaymentDisplayModel>(detail);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SortableBindingList<SaleDisplayModel> FetchSaleDetails(int paymentId)
        {
            try
            {
                var payments = (from p in db.PaymentDetail
                            where p.Id == paymentId
                            select p).FirstOrDefault();

                SortableBindingList<SaleDisplayModel> result = new SortableBindingList<SaleDisplayModel>();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CheckColumnModel> FetchPaymentChecks(int paymentId)
        {
            try
            {
                var payments = (from p in db.PaymentDetail
                                where p.Id == paymentId
                                select p).FirstOrDefault();

                List<CheckColumnModel> result = new List<CheckColumnModel>();
                if (payments.Check.Any())
                {
                    foreach (Check c in payments.Check)
                    {
                        CheckColumnModel model = new CheckColumnModel() { 
                            Id = c.Id,
                            Amount = c.Amount.HasValue ? c.Amount.Value : 0,
                            Bank = c.Bank,
                            Branch = c.Branch,
                            CheckDate = c.CheckDate.HasValue ? c.CheckDate.Value : DateTime.Today,
                            ClearingDate = c.ClearingDate.HasValue ? c.ClearingDate.Value : DateTime.Today,
                            CheckNumber = c.CheckNumber,
                            IsDeleted = c.IsDeleted.HasValue ? c.IsDeleted.Value : false
                        };

                        result.Add(model);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void RevertItem(PaymentRevertType type, int paymentId, int id, decimal amount)
        {
            try
            {
                switch (type)
                {
                    case PaymentRevertType.Sale:
                        var saleItem = db.SalesPayments.FirstOrDefault(a => a.PaymentId == paymentId && a.SalesId == id);
                        saleItem.Sale.Balance += amount;
                        saleItem.Sale.IsPaid = false;
                        db.DeleteObject(saleItem);
                        break;
                    case PaymentRevertType.SalesReturn: 
                        var returnItem = db.SalesPayments.FirstOrDefault(a => a.PaymentId == paymentId && a.SalesReturnDetailId == id);
                        returnItem.SalesReturnDetail.Balance += amount;
                        db.DeleteObject(returnItem);
                        break;
                    case PaymentRevertType.Purchase: 
                        var purchaseItem = db.PurchasePayments.FirstOrDefault(a => a.PaymentId == paymentId && a.PurchaseId == id);
                        purchaseItem.Purchase.Balance += amount;
                        purchaseItem.Purchase.IsPaid = false;
                        db.DeleteObject(purchaseItem);
                        break;
                    case PaymentRevertType.PurchaseReturn:
                        var purchaseReturnItem = db.PurchasePayments.FirstOrDefault(a => a.PaymentId == paymentId && a.PurchaseReturnDetailId == id);
                        purchaseReturnItem.PurchaseReturnDetail.Balance += amount;
                        db.DeleteObject(purchaseReturnItem);
                        break;
                    default: break;
                }

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Helper

        private string GetSaleType(int type)
        {
            string saleType = string.Empty;

            switch (type)
            {
                case 0: saleType = "Cash Invoice";
                    break;
                case 1: saleType = "Cash/Petty/SOR";
                    break;
                case 2: saleType = "Cash/Charge Invoice";
                    break;
                case 3: saleType = "Sales Order Slip";
                    break;
                case 4: saleType = "Charge Invoice";
                    break;
                case 5: saleType = "No Invoice";
                    break;
            }

            return saleType;
        }

        #endregion
    }
}

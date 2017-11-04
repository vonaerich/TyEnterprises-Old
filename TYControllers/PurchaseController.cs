using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using TY.SPIMS.Controllers.Interfaces;
using TY.SPIMS.Entities;
using TY.SPIMS.POCOs;
using TY.SPIMS.Utilities;

namespace TY.SPIMS.Controllers
{
    public class PurchaseController : IPurchaseController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IActionLogController actionLogController;
        private readonly IPurchaseReturnController purchaseReturnController;

        private TYEnterprisesEntities db
        {
            get
            { return unitOfWork.Context; }
        }

        public PurchaseController(IUnitOfWork unitOfWork, IPurchaseReturnController purchaseReturnController, IActionLogController actionLogController)
        {
            this.unitOfWork = unitOfWork;
            this.purchaseReturnController = purchaseReturnController;
            this.actionLogController = actionLogController;
        }

        #region CUD Functions

        public void InsertPurchase(PurchaseColumnModel model)
        {
            try
            {
                using (this.unitOfWork)
                {
                    Purchase item = new Purchase()
                    {
                        CustomerId = model.CustomerId,
                        PONumber = model.PONumber,
                        Date = model.PurchaseDate,
                        Type = model.Type,
                        TotalAmount = model.TotalAmount,
                        IsPaid = model.IsPaid,
                        PurchaseOption = model.Option,
                        Comment = model.Comment,
                        RecordedBy = model.RecordedBy,
                        ApprovedBy = model.ApprovedBy,
                        IsDeleted = model.IsDeleted,
                        Vat = model.Vat,
                        VatableSale = model.VatableSale,
                        InvoiceDiscount = model.InvoiceDiscount,
                        InvoiceDiscountPercent = model.InvoiceDiscountPercent,
                        Balance = model.TotalAmount
                    };

                    foreach (PurchaseDetailViewModel detail in model.Details)
                    {
                        var autoPartDetail = db.AutoPartDetail.FirstOrDefault(a => a.Id == detail.AutoPartDetailId);
                        PurchaseDetail d = new PurchaseDetail()
                        {
                            Purchase = item,
                            AutoPartDetail = autoPartDetail,
                            Quantity = detail.Quantity,
                            Unit = detail.Unit,
                            Price = detail.UnitPrice,
                            TotalDiscount = detail.TotalDiscount,
                            DiscountPercent = detail.DiscountPercent,
                            DiscountPercent2 = detail.DiscountPercent2,
                            DiscountPercent3 = detail.DiscountPercent3,
                            TotalAmount = detail.TotalAmount
                        };

                        //If delivered, add delivered quantity
                        autoPartDetail.Quantity += detail.Quantity;

                        var discountedPrice = detail.UnitPrice - detail.TotalDiscount;
                        if (discountedPrice < autoPartDetail.BuyingPrice)
                            autoPartDetail.BuyingPrice = discountedPrice;
                    }

                    if (!string.IsNullOrWhiteSpace(model.PR))
                    {
                        string[] PRs = model.PR.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string s in PRs)
                        {
                            PurchaseRequisition r = new PurchaseRequisition();
                            r.PRNumber = s;
                            item.PurchaseRequisition.Add(r);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(model.PO))
                    {
                        string[] POs = model.PO.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string s in POs)
                        {
                            PurchaseOrder o = new PurchaseOrder();
                            o.PONumber = s;
                            item.PurchaseOrder.Add(o);
                        }
                    }

                    this.unitOfWork.Context.AddToPurchase(item);

                    string action = string.Format("Added New Purchase - {0}", item.PONumber);
                    this.actionLogController.AddToLog(action, UserInfo.UserId);

                    this.unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdatePurchase(PurchaseColumnModel model)
        {
            try
            {
                using (this.unitOfWork)
                {
                    //Get Purchase Details
                    var details = (from d in db.PurchaseDetail
                                   where d.PurchaseId == model.Id
                                   select d).ToList();

                    //Subtract the added items then delete original purchase details
                    foreach (var d in details)
                    {
                        var autoPartDetail = d.AutoPartDetail;
                        autoPartDetail.Quantity -= d.Quantity;

                        db.DeleteObject(d);
                    }

                    var item = FetchPurchaseById(model.Id);
                    if (item != null)
                    {
                        item.CustomerId = model.CustomerId;
                        item.PONumber = model.PONumber;
                        item.Date = model.PurchaseDate;
                        item.Type = model.Type;
                        item.Comment = model.Comment;
                        item.PurchaseOption = model.Option;
                        item.RecordedBy = model.RecordedBy;
                        item.ApprovedBy = model.ApprovedBy;
                        item.Vat = model.Vat;
                        item.VatableSale = model.VatableSale;
                        item.IsDeleted = model.IsDeleted;
                        item.InvoiceDiscount = model.InvoiceDiscount;
                        item.InvoiceDiscountPercent = model.InvoiceDiscountPercent;

                        decimal? newBalance = model.TotalAmount - item.TotalAmount + item.Balance;
                        if (newBalance.HasValue && newBalance.Value > 0)
                        {
                            item.Balance = newBalance.Value;
                            item.IsPaid = false;
                        }
                        else
                        {
                            item.Balance = 0;
                            item.IsPaid = true;
                        }
                        item.TotalAmount = model.TotalAmount;

                        foreach (var po in item.PurchaseOrder.ToList())
                        {
                            db.DeleteObject(po);
                        }

                        foreach (var pr in item.PurchaseRequisition.ToList())
                        {
                            db.DeleteObject(pr);
                        }
                    }

                    //Add the new purchase details
                    foreach (PurchaseDetailViewModel detail in model.Details)
                    {
                        var autoPartDetail = db.AutoPartDetail.FirstOrDefault(a => a.Id == detail.AutoPartDetailId);
                        PurchaseDetail d = new PurchaseDetail()
                        {
                            Purchase = item,
                            AutoPartDetail = autoPartDetail,
                            Quantity = detail.Quantity,
                            Unit = detail.Unit,
                            Price = detail.UnitPrice,
                            TotalDiscount = detail.TotalDiscount,
                            DiscountPercent = detail.DiscountPercent,
                            DiscountPercent2 = detail.DiscountPercent2,
                            DiscountPercent3 = detail.DiscountPercent3,
                            TotalAmount = detail.TotalAmount
                        };

                        autoPartDetail.Quantity += detail.Quantity;
                    }

                    if (!string.IsNullOrWhiteSpace(model.PR))
                    {
                        string[] PRs = model.PR.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string s in PRs)
                        {
                            PurchaseRequisition r = new PurchaseRequisition();
                            r.PRNumber = s;
                            item.PurchaseRequisition.Add(r);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(model.PO))
                    {
                        string[] POs = model.PO.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string s in POs)
                        {
                            PurchaseOrder o = new PurchaseOrder();
                            o.PONumber = s;
                            item.PurchaseOrder.Add(o);
                        }
                    }

                    string action = string.Format("Updated Purchase - {0}", item.PONumber);
                    this.actionLogController.AddToLog(action, UserInfo.UserId);

                    this.unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeletePurchase(int id)
        {
            try
            {
                using (this.unitOfWork)
                {
                    var item = FetchPurchaseById(id);
                    if (item != null)
                    {
                        item.IsDeleted = true;

                        if (item.PurchaseDetail.Any())
                        {
                            foreach (var detail in item.PurchaseDetail)
                            {
                                var autoPartDetail = detail.AutoPartDetail;
                                autoPartDetail.Quantity -= detail.Quantity;
                            }
                        }
                    }

                    string action = string.Format("Deleted Purchase - {0}", item.PONumber);
                    this.actionLogController.AddToLog(action, UserInfo.UserId);

                    this.unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Fetch Functions

        private IQueryable<PurchasesView> CreateQuery(PurchaseFilterModel filter)
        {
            db.PurchasesView.MergeOption = MergeOption.OverwriteChanges;

            var items = from i in db.PurchasesView
                        where i.IsDeleted == false
                        select i;

            if (filter != null)
            {
                if (filter.CustomerId != 0)
                    items = items.Where(a => a.CustomerId == filter.CustomerId);

                if (!string.IsNullOrWhiteSpace(filter.InvoiceNumber))
                    items = items.Where(a => a.InvoiceNumber.Contains(filter.InvoiceNumber));

                if (filter.AmountType != NumericSearchType.All)
                {
                    if (filter.AmountType == NumericSearchType.Equal)
                        items = items.Where(a => a.TotalAmount == filter.AmountValue);
                    else if (filter.AmountType == NumericSearchType.GreaterThan)
                        items = items.Where(a => a.TotalAmount > filter.AmountValue);
                    else if (filter.AmountType == NumericSearchType.LessThan)
                        items = items.Where(a => a.TotalAmount < filter.AmountValue);
                }

                if (filter.Type != -1)
                    items = items.Where(a => a.Type == filter.Type);

                if (filter.DateType != DateSearchType.All)
                {
                    DateTime dateFrom = filter.DateFrom.Date;
                    DateTime dateTo = filter.DateTo.AddDays(1).Date;

                    items = items.Where(a => a.Date >= dateFrom && a.Date < dateTo);
                }

                if (filter.Paid != PaidType.None)
                {
                    if (filter.Paid == PaidType.Paid)
                        items = items.Where(a => a.IsPaid == true);
                    else if(filter.Paid == PaidType.NotPaid)
                        items = items.Where(a => a.IsPaid == false);
                }

                if (!string.IsNullOrWhiteSpace(filter.PR))
                    items = items.Where(a => a.PurchaseRequisitions.Any(b => b.PRNumber.Contains(filter.PR)));

                if (!string.IsNullOrWhiteSpace(filter.PO))
                    items = items.Where(a => a.PurchaseOrders.Any(b => b.PONumber.Contains(filter.PO)));
            }
            else
            {
                //Default sorting
                items = items.OrderByDescending(a => a.Date);
            }

            return items;
        }

        public SortableBindingList<PurchasesView> FetchPurchaseWithSearch(PurchaseFilterModel filter)
        {
            try
            {
                var query = CreateQuery(filter);

                SortableBindingList<PurchasesView> b = new SortableBindingList<PurchasesView>(query);
                return b;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SortableBindingList<PurchasesView> FetchAllPurchases()
        {
            try
            {
                return FetchPurchaseWithSearch(null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Purchase FetchPurchaseById(int id)
        {
            try
            {
                var item = (from i in db.Purchase
                            where i.Id == id
                            select i).FirstOrDefault();

                return item;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Purchase FetchPurchaseByPO(string poNumber)
        {
            try
            {
                var item = (from i in db.Purchase
                            where i.PONumber == poNumber
                            select i).FirstOrDefault();

                return item;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Purchase FetchPurchaseByPOAndSupplier(string poNumber, int supplierId)
        {
            try
            {
                var item = (from i in db.Purchase
                            where i.PONumber == poNumber && i.CustomerId == supplierId &&
                            i.IsDeleted != true
                            select i).FirstOrDefault();

                return item;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SortableBindingList<PurchaseDetailViewModel> FetchPurchaseDetails(int id)
        {
            try
            {
                var details = (from d in db.PurchaseDetail
                               where d.PurchaseId == id
                               select new
                               {
                                   AutoPartDetailId = d.AutoPartDetail.Id,
                                   AutoPartNumber = d.AutoPartDetail.PartNumber,
                                   AutoPartName = d.AutoPartDetail.AutoPart.PartName + " - " +
                                     d.AutoPartDetail.PartNumber + "\n" +
                                     d.AutoPartDetail.Brand.BrandName + " / " +
                                     d.AutoPartDetail.Model + " / " +
                                     d.AutoPartDetail.Make,
                                   Quantity = d.Quantity.HasValue ? d.Quantity.Value : 0,
                                   Unit = d.Unit,
                                   UnitPrice = d.Price.HasValue ? d.Price.Value : 0,
                                   TotalAmount = d.TotalAmount.HasValue ? d.TotalAmount.Value : 0,
                                   d.TotalDiscount,
                                   d.DiscountPercent,
                                   d.DiscountPercent2,
                                   d.DiscountPercent3,
                                   DiscountedPrice = d.Price - d.TotalDiscount
                               }).ToList();

                var result = from d in details
                             select new PurchaseDetailViewModel()
                             {
                                 AutoPartDetailId = d.AutoPartDetailId,
                                 AutoPartNumber = d.AutoPartNumber,
                                 AutoPartName = d.AutoPartName,
                                 Quantity = d.Quantity,
                                 TotalAmount = d.TotalAmount,
                                 Unit = d.Unit,
                                 UnitPrice = d.UnitPrice,
                                 TotalDiscount = d.TotalDiscount.HasValue ? d.TotalDiscount.Value : 0,
                                 DiscountedPrice = d.DiscountedPrice.HasValue ? d.DiscountedPrice.Value : 0,
                                 DiscountedPrice3 = d.DiscountedPrice.HasValue ? d.DiscountedPrice.Value : 0
                             };

                SortableBindingList<PurchaseDetailViewModel> b = new SortableBindingList<PurchaseDetailViewModel>(result);

                return b;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PurchaseDetailViewModel> FetchPurchaseDetailsPerPO(string poNumber)
        {
            try
            {
                List<PurchaseDetailViewModel> result = new List<PurchaseDetailViewModel>();

                Purchase p = FetchPurchaseByPO(poNumber);
                if (p != null)
                    result = FetchPurchaseDetails(p.Id).ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PurchaseDetailViewModel> FetchPurchaseDetailsPerPOAndSupplier(string poNumber, int supplierId)
        {
            try
            {
                List<PurchaseDetailViewModel> result = new List<PurchaseDetailViewModel>();

                Purchase p = FetchPurchaseByPOAndSupplier(poNumber, supplierId);
                if (p != null)
                    result = FetchPurchaseDetails(p.Id).ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SortableBindingList<PurchaseDisplayModel> FetchAPWithSearch(PurchaseFilterModel filter)
        {
            try
            {
                var query = CreateQuery(filter);

                query = query.Where(a => a.IsPaid == false);

                var result = from a in query.ToList()
                             select new PurchaseDisplayModel
                             {
                                 Id = a.Id,
                                 Customer = a.Supplier,
                                 PONumber = a.InvoiceNumber,
                                 PurchaseDate = a.Date.Value,
                                 Type = a.Type.HasValue ? GetPurchaseType(a.Type.Value) : "-",
                                 TotalAmount = a.TotalAmount.Value,
                                 IsPaid = a.IsPaid.Value,
                                 //IsDelivered = a.IsDelivered.Value,
                                 RecordedBy = a.RecordedBy,
                                 //ApprovedBy = a.ApprovedBy.HasValue ? a.ApprovedByUser.Firstname + " " + a.ApprovedByUser.Lastname : "-",
                                 IsDeleted = a.IsDeleted.Value,
                             };

                SortableBindingList<PurchaseDisplayModel> b = new SortableBindingList<PurchaseDisplayModel>(result);

                return b;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] FetchAllPONumbersByCustomer(int CustomerId)
        {
            try
            {
                var po = from i in db.Purchase
                          where i.IsDeleted == false
                            && i.CustomerId == CustomerId
                          orderby i.PONumber
                          select i.PONumber;

                return po.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ItemHasPaymentOrReturn(int id)
        {
            try
            {
                bool result = false;

                var puchase = (from s in db.Purchase
                            where s.Id == id
                            select s).FirstOrDefault();

                //Check if sale has a payment detail
                result = puchase.PurchasePayments.Any(a => a.PaymentDetail.IsDeleted != true);

                //If no payment has been made yet, check if sale has returned items
                if (!result)
                    result = this.purchaseReturnController.POHasReturn(puchase.PONumber);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PurchaseDetailViewModel> FetchPurchasesPerItemPerSupplier(int autoPartDetailId, int supplierId = 0)
        {
            try
            {
                var db = ConnectionManager.Instance.Connection;
                var purchases = db.PurchaseDetail
                    .Where(a => a.AutoPartDetail.Id == autoPartDetailId && 
                        a.Purchase.IsDeleted != null && !a.Purchase.IsDeleted.Value);

                if (supplierId != 0)
                    purchases = purchases.Where(a => a.Purchase.CustomerId == supplierId);

                List<PurchaseDetailViewModel> result = purchases.OrderByDescending(a => a.Purchase.Date)
                    .ToList()
                    .Select(a => new PurchaseDetailViewModel()
                    {
                        InvoiceNumber = string.Format("{0} {1}", a.Purchase.PONumber,
                            CheckIfInvoiceHasReturn(a.Purchase.PONumber) ? "*" : string.Empty),
                        Supplier = a.Purchase.Customer.CompanyName,
                        PurchaseDate = a.Purchase.Date,
                        Quantity = a.Quantity.HasValue ? a.Quantity.Value : 0,
                        Unit = a.Unit,
                        UnitPrice = (a.Price.HasValue &&  a.TotalDiscount.HasValue) ? 
                            a.Price.Value - a.TotalDiscount.Value : (a.Price.HasValue ? a.Price.Value : 0),
                        TotalAmount = a.TotalAmount.HasValue ? a.TotalAmount.Value : 0,
                        TotalDiscount = a.TotalDiscount.HasValue ? a.TotalDiscount.Value : 0
                    }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool CheckIfInvoiceHasReturn(string invoiceNumber)
        {
            try
            {
                return db.PurchaseReturnDetail.Any(a => a.PONumber == invoiceNumber &&
                    a.PurchaseReturn.IsDeleted != true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SortableBindingList<PurchasesView> FetchPurchaseByIds(List<int> ids)
        {
            try
            {
                var p = db.PurchasesView.Where(a => ids.Contains(a.Id));
                return new SortableBindingList<PurchasesView>(p);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool CheckIfInvoiceHasDuplicate(string codeToCheck, int id, int supplierId)
        {
            try
            {
                var hasDuplicate = db.Purchase.Where(a => a.IsDeleted == false)
                    .Any(a => a.PONumber == codeToCheck && a.Id != id && a.CustomerId == supplierId);

                return hasDuplicate;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public string GetPONumber(int purchaseId)
        {
            try
            {
                StringBuilder builder = new StringBuilder();

                var po = db.PurchaseOrder.Where(a => a.PurchaseId == purchaseId);
                foreach (var p in po)
                {
                    if (builder.Length > 0)
                        builder.Append(", ");
                    builder.Append(p.PONumber);
                }

                return builder.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Helper

        private string GetPurchaseType(int type)
        {
            string purchaseType = string.Empty;

            switch (type)
            {
                case 0: purchaseType = "Cash Invoice";
                    break;
                case 1: purchaseType = "Delivery Receipt";
                    break;
                case 2: purchaseType = "Charge Invoice";
                    break;
                case 3: purchaseType = "Cash No Invoice";
                    break;
            }

            return purchaseType;
        }

        #endregion
    }
}

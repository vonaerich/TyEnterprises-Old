using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using TY.SPIMS.Entities;
using TY.SPIMS.POCOs;
using TY.SPIMS.Utilities;
using System.Text;
using TY.SPIMS.POCOs.BaseClasses;
using TY.SPIMS.Controllers.Interfaces;

namespace TY.SPIMS.Controllers
{
    public class SaleController : ISaleController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IActionLogController actionLogController;
        private readonly IAutoPartController autoPartController;
        private readonly ISalesReturnController salesReturnController;

        private TYEnterprisesEntities db 
        { 
            get
            { return unitOfWork.Context; } 
        }

        public SaleController(IUnitOfWork unitOfWork, IAutoPartController autoPartController, ISalesReturnController salesReturnController, IActionLogController actionLogController)
        {
            this.unitOfWork = unitOfWork;
            this.autoPartController = autoPartController;
            this.salesReturnController = salesReturnController;
            this.actionLogController = actionLogController;
        }

        #region CUD Functions

        public void InsertSale(SaleColumnModel model)
        {
            try
            {
                using (this.unitOfWork)
                {
                    Sale item = new Sale()
                    {
                        CustomerId = model.CustomerId,
                        InvoiceNumber = model.InvoiceNumber,
                        Date = model.Date,
                        Type = model.Type,
                        VatableSale = model.VatableSale,
                        Vat = model.Vat,
                        TotalAmount = model.TotalAmount,
                        IsPaid = model.IsPaid,
                        RecordedBy = model.RecordedBy,
                        ApprovedBy = model.ApprovedBy,
                        Comment = model.Comment,
                        IsDeleted = model.IsDeleted,
                        InvoiceDiscount = model.InvoiceDiscount,
                        InvoiceDiscountPercent = model.InvoiceDiscountPercent,
                        Balance = model.TotalAmount
                    };

                    foreach (SalesDetailViewModel detail in model.Details)
                    {
                        var autoPartDetail = this.autoPartController.FetchAutoPartDetailById(detail.AutoPartDetailId);
                        SaleDetail d = new SaleDetail()
                        {
                            Sale = item,
                            AutoPartDetail = autoPartDetail,
                            Quantity = detail.Quantity,
                            Unit = detail.Unit,
                            SRP = detail.UnitPrice,
                            TotalDiscount = detail.TotalDiscount,
                            DiscountPercent = detail.DiscountPercent,
                            DiscountPercent2 = detail.DiscountPercent2,
                            DiscountPercent3 = detail.DiscountPercent3,
                            TotalAmount = detail.TotalAmount
                        };

                        //Subtract Sold Quantity
                        autoPartDetail.Quantity -= detail.Quantity;
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

                    this.unitOfWork.Context.AddToSale(item);

                    string action = string.Format("Added New Sale - {0}", item.InvoiceNumber);
                    this.actionLogController.AddToLog(action, UserInfo.UserId);

                    this.unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateSale(SaleColumnModel model)
        {
            try
            {
                using (this.unitOfWork)
                {
                    //Get Sale Details
                    var details = (from d in db.SaleDetail
                                   where d.SaleId == model.Id
                                   select d).ToList();

                    //Add back the subtracted items then delete original sale details
                    foreach (var d in details)
                    {
                        var autoPartDetail = d.AutoPartDetail;
                        autoPartDetail.Quantity += d.Quantity;

                        db.DeleteObject(d);
                    }

                    var item = FetchSaleById(model.Id);
                    if (item != null)
                    {
                        item.CustomerId = model.CustomerId;
                        item.InvoiceNumber = model.InvoiceNumber;
                        item.Date = model.Date;
                        item.Type = model.Type;
                        item.VatableSale = model.VatableSale;
                        item.Vat = model.Vat;
                        item.RecordedBy = model.RecordedBy;
                        item.ApprovedBy = model.ApprovedBy;
                        item.Comment = model.Comment;
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

                    //Add the new sale details
                    foreach (SalesDetailViewModel detail in model.Details)
                    {
                        var autoPartDetail = db.AutoPartDetail.FirstOrDefault(a => a.Id == detail.AutoPartDetailId);
                        SaleDetail d = new SaleDetail()
                        {
                            Sale = item,
                            AutoPartDetail = autoPartDetail,
                            Quantity = detail.Quantity,
                            Unit = detail.Unit,
                            SRP = detail.UnitPrice,
                            TotalDiscount = detail.TotalDiscount,
                            DiscountPercent = detail.DiscountPercent,
                            DiscountPercent2 = detail.DiscountPercent2,
                            DiscountPercent3 = detail.DiscountPercent3,
                            TotalAmount = detail.TotalAmount
                        };

                        //Subtract Sold Quantity
                        autoPartDetail.Quantity -= detail.Quantity;
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

                    string action = string.Format("Updated Sale - {0}", item.InvoiceNumber);
                    this.actionLogController.AddToLog(action, UserInfo.UserId);

                    this.unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteSale(int id)
        {
            try
            {
                using (this.unitOfWork)
                {
                    var item = FetchSaleById(id);
                    if (item != null)
                    {
                        item.IsDeleted = true;

                        if (item.SaleDetail.Any())
                        {
                            foreach (var detail in item.SaleDetail)
                            {
                                var autoPartDetail = detail.AutoPartDetail;
                                autoPartDetail.Quantity += detail.Quantity;
                            }
                        }
                    }

                    string action = string.Format("Deleted Sale - {0}", item.InvoiceNumber);
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

        private IQueryable<SalesView> CreateQuery(SaleFilterModel filter)
        {
            db.SalesView.MergeOption = MergeOption.OverwriteChanges;

            var items = db.SalesView
                .Where(a => a.IsDeleted == false);

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
                    else if (filter.Paid == PaidType.NotPaid)
                        items = items.Where(a => a.IsPaid == false);
                }

                if (!string.IsNullOrWhiteSpace(filter.PR))
                    items = items.Where(a => a.PurchaseRequisitions.Any(b => b.PRNumber.Contains(filter.PR)));

                if (!string.IsNullOrWhiteSpace(filter.PO))
                    items = items.Where(a => a.PurchaseOrders.Any(b => b.PONumber.Contains(filter.PO)));
            }
            else
            {
                items = items.OrderByDescending(a => a.Date);
            }

            return items;
        }

        public SortableBindingList<SalesView> FetchSaleWithSearch(SaleFilterModel filter)
        {
            try
            {
                var query = CreateQuery(filter);

                SortableBindingList<SalesView> b = new SortableBindingList<SalesView>(query);
                return b;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SortableBindingList<SalesView> FetchAllSales()
        {
            try
            {
                return FetchSaleWithSearch(null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Sale FetchSaleById(int id)
        {
            try
            {
                var item = (from i in db.Sale
                            where i.Id == id
                            select i).FirstOrDefault();

                return item;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Sale FetchSaleByInvoiceNumber(string invoiceNumber)
        {
            try
            {
                var item = (from i in db.Sale
                            where i.InvoiceNumber == invoiceNumber
                            select i).FirstOrDefault();

                return item;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SalesDetailViewModel> FetchSaleDetailsPerInvoice(string invoiceNumber)
        {
            try
            {
                List<SalesDetailViewModel> result = new List<SalesDetailViewModel>();

                Sale s = FetchSaleByInvoiceNumber(invoiceNumber);
                if (s != null)
                    result = FetchSaleDetails(s.Id).ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SortableBindingList<SalesDetailViewModel> FetchSaleDetails(int id)
        {
            try
            {
                var details = (from d in db.SaleDetail
                               where d.SaleId == id
                               select new
                               {
                                   AutoPartDetailId = d.AutoPartDetail.Id,
                                   AutoPartNumber = d.AutoPartDetail.PartNumber,
                                   AutoPartName = d.AutoPartDetail.AutoPart.PartName + " - " +
                                     d.AutoPartDetail.PartNumber + "\n" +
                                     d.AutoPartDetail.Brand.BrandName + " / " +
                                     d.AutoPartDetail.Model + " / " +
                                     d.AutoPartDetail.Make,
                                   Quantity = d.Quantity.Value,
                                   Unit = d.Unit != "" ? d.Unit : "-",
                                   UnitPrice = d.SRP.Value,
                                   TotalAmount = d.TotalAmount.Value,
                                   Discount = d.TotalDiscount.HasValue ? d.TotalDiscount.Value : 0.00m,
                                   DiscountPercent = d.DiscountPercent.HasValue ? d.DiscountPercent.Value : 0,
                                   DiscountPercent2 = d.DiscountPercent2.HasValue ? d.DiscountPercent2.Value : 0,
                                   DiscountPercent3 = d.DiscountPercent3.HasValue ? d.DiscountPercent3.Value : 0,
                                   DiscountedPrice = d.SRP - d.TotalDiscount
                               }).ToList();

                var result = from d in details
                             select new SalesDetailViewModel()
                             {
                                 AutoPartDetailId = d.AutoPartDetailId,
                                 AutoPartNumber = d.AutoPartNumber,
                                 AutoPartName = d.AutoPartName,
                                 DiscountPercents = string.Format("Less: {0}% + {1}% + {2}%",
                                     d.DiscountPercent.ToString(),
                                     d.DiscountPercent2.ToString(),
                                     d.DiscountPercent3.ToString()),
                                 Quantity = d.Quantity,
                                 TotalAmount = d.TotalAmount,
                                 Unit = d.Unit,
                                 UnitPrice = d.UnitPrice,
                                 TotalDiscount = d.Discount,
                                 DiscountedPrice = d.DiscountedPrice.HasValue ? d.DiscountedPrice.Value : 0,
                                 DiscountedPrice3 = d.DiscountedPrice.HasValue ? d.DiscountedPrice.Value : 0
                             };

                SortableBindingList<SalesDetailViewModel> b = new SortableBindingList<SalesDetailViewModel>(result);

                return b;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SortableBindingList<SaleDisplayModel> FetchARWithSearch(SaleFilterModel filter)
        {
            try
            {
                var query = CreateQuery(filter);

                query = query.Where(a => a.Type == 1 && a.IsPaid == false);

                var result = from a in query.ToList()
                             select new SaleDisplayModel
                             {
                                 Id = a.Id,
                                 Customer = a.CompanyName,
                                 InvoiceNumber = a.InvoiceNumber,
                                 Date = a.Date.Value,
                                 Type = a.Type.HasValue ? GetSaleType(a.Type.Value) : "-",
                                 TotalAmount = a.TotalAmount.Value,
                                 IsPaid = a.IsPaid.Value,
                                 RecordedBy = a.RecordedBy,
                                 //ApprovedBy = a.ApprovedBy.HasValue ? a.ApprovedByUser.Firstname + " " + a.ApprovedByUser.Lastname : "-",
                                 IsDeleted = a.IsDeleted.Value
                             };

                SortableBindingList<SaleDisplayModel> b = new SortableBindingList<SaleDisplayModel>(result);

                return b;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] FetchAllInvoiceNumbersByCustomer(int customerId)
        {
            try
            {
                var inv = from i in db.Sale
                          where i.IsDeleted == false
                            && i.CustomerId == customerId
                          orderby i.InvoiceNumber
                          select i.InvoiceNumber;

                return inv.ToArray();
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

                var sale = (from s in db.Sale
                            where s.Id == id
                            select s).FirstOrDefault();

                //Check if sale has a payment detail
                result = sale.SalesPayments.Any(a => a.PaymentDetail.IsDeleted != true);

                //If no payment has been made yet, check if sale has returned items
                if (!result)
                    result = this.salesReturnController.InvoiceHasReturn(sale.InvoiceNumber);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SalesDetailViewModel> FetchSalesPerItemPerCustomer(int autoPartDetailId, int customerId = 0)
        {
            try
            {
                var sales = db.SaleDetail.Where(a => a.AutoPartDetail.Id == autoPartDetailId
                    && a.Sale.IsDeleted != null && !a.Sale.IsDeleted.Value);

                if (customerId != 0)
                    sales = sales.Where(a => a.Sale.CustomerId == customerId);

                List<SalesDetailViewModel> result = sales
                    .OrderByDescending(a => a.Sale.Date)
                    .ToList()
                    .Select(a => new SalesDetailViewModel()
                    {
                        InvoiceNumber = string.Format("{0} {1}", a.Sale.InvoiceNumber,
                            CheckIfInvoiceHasReturn(a.Sale.InvoiceNumber) ? "*" : string.Empty),
                        Customer = a.Sale.Customer.CompanyName,
                        SalesDate = a.Sale.Date,
                        Quantity = a.Quantity.HasValue ? a.Quantity.Value : 0,
                        Unit = a.Unit,
                        UnitPrice = (a.SRP.HasValue && a.TotalDiscount.HasValue) ?
                            a.SRP.Value - a.TotalDiscount.Value :
                            (a.SRP.HasValue ? a.SRP.Value : 0),
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
                return db.SalesReturnDetail.Any(a => a.InvoiceNumber == invoiceNumber &&
                    a.SalesReturn.IsDeleted != true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SortableBindingList<SalesView> FetchSaleByIds(List<int> ids)
        {
            try
            {
                var sales = db.SalesView.Where(a => ids.Contains(a.Id));
                return new SortableBindingList<SalesView>(sales);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool CheckIfInvoiceHasDuplicate(string codeToCheck, int id)
        {
            try
            {
                var hasDuplicate = db.Sale.Where(a => a.IsDeleted == false)
                    .Any(a => a.InvoiceNumber == codeToCheck && a.Id != id);

                return hasDuplicate;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string GetPONumber(int saleId)
        {
            try
            {
                StringBuilder builder = new StringBuilder();

                var po = db.PurchaseOrder.Where(a => a.SaleId == saleId);
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

        public IQueryable<SalesView> FetchSalesWithSearchGeneric(SaleFilterModel filter)
        {
            try
            {
                return this.CreateQuery(filter);
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

using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using TY.SPIMS.Controllers.Interfaces;
using TY.SPIMS.Entities;
using TY.SPIMS.POCOs;
using TY.SPIMS.Utilities;

namespace TY.SPIMS.Controllers
{
    public class PurchaseReturnController : IPurchaseReturnController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IActionLogController actionLogController;
        private readonly IAutoPartController autopartController;

        private TYEnterprisesEntities db
        {
            get
            { return unitOfWork.Context; }
        }

        public PurchaseReturnController(IUnitOfWork unitOfWork, IAutoPartController autopartController, IActionLogController actionLogController)
        {
            this.unitOfWork = unitOfWork;
            this.actionLogController = actionLogController;
            this.autopartController = autopartController;
        }

        #region CUD Functions

        public void InsertPurchaseReturn(PurchaseReturnColumnModel model)
        {
            try
            {
                using (this.unitOfWork)
                {
                    PurchaseReturn item = new PurchaseReturn()
                    {
                        MemoNumber = model.MemoNumber,
                        CustomerId = model.CustomerId,
                        ReturnDate = model.ReturnDate,
                        AmountReturn = model.AmountReturn,
                        Adjustment = model.Adjustment,
                        TotalDebitAmount = model.TotalDebitAmount,
                        Remarks = model.Remarks,
                        IsDeleted = model.IsDeleted,
                        RecordedBy = model.RecordedByUser,
                        ApprovedBy = model.ApprovedByUser,
                        AmountUsed = 0
                    };

                    if (model.Details.Count > 0)
                    {
                        foreach (PurchaseReturnDetailModel d in model.Details)
                        {
                            PurchaseReturnDetail detail = new PurchaseReturnDetail()
                            {
                                PartDetailId = d.PartDetailId,
                                PONumber = d.PONumber,
                                Quantity = d.Quantity,
                                UnitPrice = d.UnitPrice,
                                TotalAmount = d.TotalAmount,
                                Balance = d.TotalAmount
                            };

                            item.PurchaseReturnDetail.Add(detail);

                            AutoPartDetail autoDetail = db.AutoPartDetail.FirstOrDefault(a => a.Id == d.PartDetailId);
                            if (autoDetail != null)
                                autoDetail.Quantity -= d.Quantity;
                        }
                    }

                    this.unitOfWork.Context.AddToPurchaseReturn(item);

                    string action = string.Format("Added New Purchase Return - {0}", item.MemoNumber);
                    this.actionLogController.AddToLog(action, UserInfo.UserId);

                    this.unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdatePurchaseReturn(PurchaseReturnColumnModel model)
        {
            try
            {
                using (this.unitOfWork)
                {
                    var item = FetchPurchaseReturnById(model.Id);
                    if (item != null)
                    {
                        //Delete old details
                        if (item.PurchaseReturnDetail.Any())
                        {
                            foreach (var d in item.PurchaseReturnDetail.ToList())
                            {
                                AutoPartDetail detail = db.AutoPartDetail.FirstOrDefault(a => a.Id == d.PartDetailId);
                                detail.Quantity += d.Quantity;

                                foreach (var payment in d.PurchasePayments.ToList())
                                {
                                    item.AmountUsed -= payment.Amount;
                                    db.DeleteObject(payment);
                                }

                                db.DeleteObject(d);
                            }
                        }

                        item.MemoNumber = model.MemoNumber;
                        item.CustomerId = model.CustomerId;
                        item.ReturnDate = model.ReturnDate;
                        item.AmountReturn = model.AmountReturn;
                        item.Adjustment = model.Adjustment;
                        item.TotalDebitAmount = model.TotalDebitAmount;
                        item.Remarks = model.Remarks;
                        item.IsDeleted = model.IsDeleted;
                        item.RecordedBy = model.RecordedByUser;
                        item.ApprovedBy = model.ApprovedByUser;

                        //Add the new items
                        if (model.Details.Count > 0)
                        {
                            foreach (PurchaseReturnDetailModel d in model.Details)
                            {
                                var autoPart = db.AutoPartDetail.FirstOrDefault(a => a.Id == d.PartDetailId);
                                autoPart.Quantity -= d.Quantity;

                                PurchaseReturnDetail detail = new PurchaseReturnDetail()
                                {
                                    PartDetailId = d.PartDetailId,
                                    PONumber = d.PONumber,
                                    Quantity = d.Quantity,
                                    UnitPrice = d.UnitPrice,
                                    TotalAmount = d.TotalAmount,
                                    Balance = d.TotalAmount
                                };

                                item.PurchaseReturnDetail.Add(detail);
                            }
                        }
                    }

                    string action = string.Format("Updated Purchase Return - {0}", item.MemoNumber);
                    this.actionLogController.AddToLog(action, UserInfo.UserId);

                    this.unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeletePurchaseReturn(int id)
        {
            try
            {
                using (this.unitOfWork)
                {
                    var item = FetchPurchaseReturnById(id);
                    if (item != null)
                    {
                        item.IsDeleted = true;

                        foreach (var i in item.PurchaseReturnDetail)
                        {
                            AutoPartDetail a = db.AutoPartDetail.FirstOrDefault(b => b.Id == i.PartDetailId);
                            a.Quantity += i.Quantity;
                        }
                    }

                    string action = string.Format("Deleted Purchase Return - {0}", item.MemoNumber);
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

        private IQueryable<PurchaseReturn> CreateQuery(PurchaseReturnFilterModel filter)
        {
            var items = from i in db.PurchaseReturn
                        where i.IsDeleted == false
                        select i;

            if (filter != null)
            {
                if (filter.CustomerId != 0)
                    items = items.Where(a => a.CustomerId == filter.CustomerId);

                if (!string.IsNullOrWhiteSpace(filter.MemoNumber))
                    items = items.Where(a => a.MemoNumber.Contains(filter.MemoNumber));

                if (filter.AmountType != NumericSearchType.All)
                {
                    if (filter.AmountType == NumericSearchType.Equal)
                        items = items.Where(a => a.TotalDebitAmount == filter.AmountValue);
                    else if (filter.AmountType == NumericSearchType.GreaterThan)
                        items = items.Where(a => a.TotalDebitAmount > filter.AmountValue);
                    else if (filter.AmountType == NumericSearchType.LessThan)
                        items = items.Where(a => a.TotalDebitAmount < filter.AmountValue);
                }

                if (filter.DateType != DateSearchType.All)
                {
                    DateTime dateFrom = filter.DateFrom.Date;
                    DateTime dateTo = filter.DateTo.AddDays(1).Date;

                    items = items.Where(a => a.ReturnDate >= dateFrom && a.ReturnDate < dateTo);
                }

                if (filter.Status != ReturnStatusType.All)
                {
                    if (filter.Status == ReturnStatusType.Used)
                        items = items.Where(a => a.IsUsed != null && a.IsUsed == true);
                    else
                        items = items.Where(a => a.IsUsed == null || a.IsUsed == false);
                }
            }
            else
            {
                //Default sorting
                items = items.OrderByDescending(a => a.ReturnDate);
            }

            return items;
        }

        public SortableBindingList<PurchaseReturnDisplayModel> FetchPurchaseReturnWithSearch(PurchaseReturnFilterModel filter)
        {
            try
            {
                var query = CreateQuery(filter);

                var result = from a in query
                             select new PurchaseReturnDisplayModel
                             {
                                 Id = a.Id,
                                 MemoNumber = a.MemoNumber,
                                 Customer = a.Customer.CompanyName,
                                 ReturnDate = a.ReturnDate.Value,
                                 AmountReturn = a.AmountReturn.Value,
                                 Adjustment = a.Adjustment.Value,
                                 TotalDebitAmount = a.TotalDebitAmount.Value,
                                 Remarks = a.Remarks,
                                 RecordedBy = a.RecordedByUser.Firstname + " " + a.RecordedByUser.Lastname,
                                 AmountUsed = a.AmountUsed.HasValue ? a.AmountUsed.Value : 0,
                                 //ApprovedBy = a.ApprovedBy.HasValue ? a.ApprovedByUser.Firstname + " " + a.ApprovedByUser.Lastname : "-",
                                 IsDeleted = a.IsDeleted.Value,
                             };

                SortableBindingList<PurchaseReturnDisplayModel> b = new SortableBindingList<PurchaseReturnDisplayModel>(result);

                return b;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PurchaseReturnDisplayModel> FetchAllPurchaseReturns()
        {
            try
            {
                var query = CreateQuery(null);

                var result = from a in query
                             select new PurchaseReturnDisplayModel
                             {
                                 Id = a.Id,
                                 MemoNumber = a.MemoNumber,
                                 Customer = a.Customer.CompanyName,
                                 ReturnDate = a.ReturnDate.Value,
                                 AmountReturn = a.AmountReturn.Value,
                                 Adjustment = a.Adjustment.Value,
                                 TotalDebitAmount = a.TotalDebitAmount.Value,
                                 Remarks = a.Remarks,
                                 IsDeleted = a.IsDeleted.Value,
                             };

                return result.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PurchaseReturn FetchPurchaseReturnById(int id)
        {
            try
            {
                var item = (from i in db.PurchaseReturn
                            where i.Id == id
                            select i).FirstOrDefault();

                return item;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SortableBindingList<PurchaseReturnDetailModel> FetchPurchaseReturnDetails(int id)
        {
            try
            {
                var details = (from r in db.PurchaseReturnDetail
                               where r.PRId == id &&
                                r.PurchaseReturn.IsDeleted != true
                               select r).ToList();

                var results = from d in details
                              select new PurchaseReturnDetailModel()
                              {
                                  AutoPart = d.PartDetailId.HasValue ?
                                          this.autopartController.FetchPartNameById(d.PartDetailId.Value) : "-",
                                  PartDetailId = d.PartDetailId.HasValue ? d.PartDetailId.Value : 0,
                                  PartNumber = d.PartDetailId.HasValue ?
                                          this.autopartController.FetchAutoPartDetailById(d.PartDetailId.Value).PartNumber : "-",
                                  PONumber = d.PONumber,
                                  Quantity = d.Quantity.HasValue ? d.Quantity.Value : 0,
                                  UnitPrice = d.UnitPrice.HasValue ? d.UnitPrice.Value : 0,
                                  TotalAmount = d.TotalAmount.HasValue ? d.TotalAmount.Value : 0
                              };

                SortableBindingList<PurchaseReturnDetailModel> b = new SortableBindingList<PurchaseReturnDetailModel>(results);

                return b;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int FetchQuantityReturned(int CustomerId, string poNumber, int itemId, int returnId)
        {
            try
            {
                var qty = (from r in db.PurchaseReturnDetail
                           where r.PurchaseReturn.CustomerId == CustomerId &&
                             r.PONumber == poNumber && 
                             r.PartDetailId == itemId &&
                             r.PRId != returnId &&
                             r.PurchaseReturn.IsDeleted != true 
                           select r.Quantity).Sum();

                return qty.HasValue ? qty.Value : 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool POHasReturn(string poNumber)
        {
            try
            {
                var result = (from r in db.PurchaseReturnDetail
                              select r).Any(a => a.PurchaseReturn.IsDeleted == false && a.PONumber == poNumber);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SortableBindingList<PurchaseReturnDetailModel> FetchAllPurchaseReturnDetails()
        {
            try
            {
                var details = (from r in db.PurchaseReturnDetail
                               where r.PurchaseReturn.IsDeleted != true
                               select r).ToList();

                var results = from d in details
                              select new PurchaseReturnDetailModel()
                              {
                                  MemoNumber = d.PurchaseReturn.MemoNumber,
                                  ReturnDate = d.PurchaseReturn.ReturnDate,
                                  AutoPart = d.PartDetailId.HasValue ?
                                          this.autopartController.FetchPartNameById(d.PartDetailId.Value) : "-",
                                  PartNumber = d.PartDetailId.HasValue ?
                                          this.autopartController.FetchAutoPartDetailById(d.PartDetailId.Value).PartNumber : "-",
                                  PONumber = d.PONumber,
                                  Quantity = d.Quantity.HasValue ? d.Quantity.Value : 0,
                                  UnitPrice = d.UnitPrice.HasValue ? d.UnitPrice.Value : 0,
                                  TotalAmount = d.TotalAmount.HasValue ? d.TotalAmount.Value : 0
                              };

                SortableBindingList<PurchaseReturnDetailModel> b = new SortableBindingList<PurchaseReturnDetailModel>(results);

                return b;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool? IsItemDebited(int id)
        {
            try
            {
                var debited = db.PurchasePayments.Any(a => a.PurchaseReturnDetail.PRId == id);
                return debited;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool CheckReturnHasDuplicate(string codeToCheck, int id)
        {
            try
            {
                var hasReturn = db.PurchaseReturn.Where(a => a.IsDeleted == false)
                    .Any(a => a.MemoNumber == codeToCheck && a.Id != id);

                return hasReturn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PurchaseReturnDetailModel> GetReturnsPerInvoice(string invoiceNumber)
        {
            try
            {
                var details = db.PurchaseReturnDetail
                    .Where(a => a.PurchaseReturn.IsDeleted != true && 
                        a.PONumber == invoiceNumber && a.Balance != 0)
                    .Select(a => new PurchaseReturnDetailModel()
                    {
                        Id = a.Id,
                        MemoNumber = a.PurchaseReturn.MemoNumber,
                        MemoDisplay = a.PurchaseReturn.MemoNumber + " - " + a.AutoPartDetail.PartNumber,
                        ReturnDate = a.PurchaseReturn.ReturnDate,
                        TotalAmount = a.TotalAmount.HasValue ? a.TotalAmount.Value : 0,
                        Balance = a.Balance.HasValue ? a.Balance.Value : 0
                    })
                    .ToList();

                return details;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SortableBindingList<PaymentDebitModel> FetchPaymentDetails(int returnId)
        {
            try
            {
                var payment = db.PurchasePayments.Where(a => a.PurchaseReturnDetailId != null &&
                    a.PurchaseReturnDetail.PurchaseReturn.IsDeleted != true &&
                    a.PurchaseReturnDetail.PurchaseReturn.Id == returnId);

                var result = payment.Select(a => new PaymentDebitModel
                {
                    VoucherNumber = a.PaymentDetail.VoucherNumber,
                    Amount = a.Amount.HasValue ? a.Amount.Value : 0
                });

                return new SortableBindingList<PaymentDebitModel>(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SortableBindingList<PurchaseReturnDetailModel> FetchAllReturnsInPurchase(string poNumber)
        {
            try
            {
                var returns = db.PurchaseReturnDetail.Where(a => a.PurchaseReturn.IsDeleted != true && a.PONumber == poNumber)
                    .ToList()
                    .Select(a => new PurchaseReturnDetailModel()
                    {
                        MemoNumber = a.PurchaseReturn.MemoNumber,
                        ReturnDate = a.PurchaseReturn.ReturnDate,
                        Quantity = a.Quantity.HasValue ? a.Quantity.Value : 0,
                        AutoPart = a.PartDetailId.HasValue ? this.autopartController.FetchPartNameById(a.PartDetailId.Value) : "-"
                    });

                return new SortableBindingList<PurchaseReturnDetailModel>(returns);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SalesReturnDetailModel> GetReturnsWithBalance(int customerId)
        {
            try
            {
                var details = db.PurchaseReturnDetail
                    .Where(a => a.PurchaseReturn.IsDeleted != true
                        && a.Balance != 0
                        && a.PurchaseReturn.CustomerId == customerId)
                    .Select(a => new SalesReturnDetailModel()
                    {
                        Id = a.Id,
                        MemoNumber = a.PurchaseReturn.MemoNumber,
                        InvoiceNumber = a.PONumber,
                        TotalAmount = a.TotalAmount.HasValue ? a.TotalAmount.Value : 0,
                        Balance = a.Balance.HasValue ? a.Balance.Value : 0,
                        ReturnDate = a.PurchaseReturn.ReturnDate
                    })
                    .ToList();

                return details;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SortableBindingList<PurchaseReturnDetailModel> FetchPurchaseReturnByIds(List<int> list)
        {
            try
            {
                var purchaseReturns = db.PurchaseReturnDetail.Where(a => list.Contains(a.Id))
                    .Select(a => new PurchaseReturnDetailModel()
                    {
                        Id = a.Id,
                        MemoNumber = a.PurchaseReturn.MemoNumber,
                        PONumber = a.PONumber,
                        TotalAmount = a.TotalAmount.HasValue ? a.TotalAmount.Value : 0,
                        Balance = a.Balance.HasValue ? a.Balance.Value : 0,
                        ReturnDate = a.PurchaseReturn.ReturnDate
                    });
                return new SortableBindingList<PurchaseReturnDetailModel>(purchaseReturns);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}

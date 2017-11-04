using System;
using System.Data.Objects;
using System.Linq;
using TY.SPIMS.Entities;
using TY.SPIMS.POCOs;
using TY.SPIMS.Utilities;
using System.Collections.Generic;
using TY.SPIMS.Controllers.Interfaces;

namespace TY.SPIMS.Controllers
{
    public class SalesReturnController : ISalesReturnController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IAutoPartController autopartController;
        private readonly IActionLogController actionLogController;

        private TYEnterprisesEntities db
        {
            get
            { return unitOfWork.Context; }
        }

        public SalesReturnController(IUnitOfWork unitOfWork, IAutoPartController autopartController, IActionLogController actionLogController)
        {
            this.unitOfWork = unitOfWork;
            this.autopartController = autopartController;
            this.actionLogController = actionLogController;
        }

        #region CUD Functions

        public void InsertSalesReturn(SalesReturnColumnModel model)
        {
            try
            {
                using (this.unitOfWork)
                {
                    SalesReturn item = new SalesReturn()
                    {
                        MemoNumber = model.MemoNumber,
                        CustomerId = model.CustomerId,
                        ReturnDate = model.ReturnDate,
                        AmountReturn = model.AmountReturn,
                        Adjustment = model.Adjustment,
                        TotalCreditAmount = model.TotalCreditAmount,
                        Remarks = model.Remarks,
                        IsDeleted = model.IsDeleted,
                        RecordedBy = model.RecordedByUser,
                        ApprovedBy = model.ApprovedByUser,
                        AmountUsed = 0
                    };

                    if (model.Details.Count > 0)
                    {
                        foreach (SalesReturnDetailModel d in model.Details)
                        {
                            SalesReturnDetail detail = new SalesReturnDetail()
                            {
                                PartDetailId = d.PartDetailId,
                                InvoiceNumber = d.InvoiceNumber,
                                Quantity = d.Quantity,
                                UnitPrice = d.UnitPrice,
                                TotalAmount = d.TotalAmount,
                                Balance = d.TotalAmount
                            };

                            item.SalesReturnDetail.Add(detail);

                            AutoPartDetail autoDetail = db.AutoPartDetail.FirstOrDefault(a => a.Id == d.PartDetailId);
                            if (autoDetail != null)
                                autoDetail.Quantity += d.Quantity;
                        }
                    }

                    this.unitOfWork.Context.AddToSalesReturn(item);

                    string action = string.Format("Added new Sales Return - {0}", item.MemoNumber);
                    this.actionLogController.AddToLog(action, UserInfo.UserId);

                    this.unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateSalesReturn(SalesReturnColumnModel model)
        {
            try
            {
                using (this.unitOfWork)
                {
                    var item = FetchSalesReturnById(model.Id);

                    if (item != null)
                    {
                        //Delete old details
                        if (item.SalesReturnDetail.Any())
                        {
                            foreach (var d in item.SalesReturnDetail.ToList())
                            {
                                AutoPartDetail detail = db.AutoPartDetail.FirstOrDefault(a => a.Id == d.PartDetailId);
                                detail.Quantity -= d.Quantity;

                                foreach (var payment in d.SalesPayments.ToList())
                                {
                                    item.AmountUsed -= payment.Amount;
                                    db.DeleteObject(payment);
                                }

                                db.DeleteObject(d);
                            }
                        }

                        //Update details
                        item.MemoNumber = model.MemoNumber;
                        item.CustomerId = model.CustomerId;
                        item.ReturnDate = model.ReturnDate;
                        item.AmountReturn = model.AmountReturn;
                        item.Adjustment = model.Adjustment;
                        item.TotalCreditAmount = model.TotalCreditAmount;
                        item.Remarks = model.Remarks;
                        item.IsDeleted = model.IsDeleted;
                        item.RecordedBy = model.RecordedByUser;
                        item.ApprovedBy = model.ApprovedByUser;

                        //Add the new items
                        if (model.Details.Count > 0)
                        {
                            foreach (SalesReturnDetailModel d in model.Details)
                            {
                                var autoPart = db.AutoPartDetail.FirstOrDefault(a => a.Id == d.PartDetailId);
                                autoPart.Quantity += d.Quantity;

                                SalesReturnDetail detail = new SalesReturnDetail()
                                {
                                    SalesReturn = item,
                                    AutoPartDetail = autoPart,
                                    InvoiceNumber = d.InvoiceNumber,
                                    Quantity = d.Quantity,
                                    UnitPrice = d.UnitPrice,
                                    TotalAmount = d.TotalAmount,
                                    Balance = d.TotalAmount
                                };
                            }
                        }
                    }

                    string action = string.Format("Updated Sales Return - {0}", item.MemoNumber);
                    this.actionLogController.AddToLog(action, UserInfo.UserId);

                    this.unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteSalesReturn(int id)
        {
            try
            {
                using (this.unitOfWork)
                {
                    var item = FetchSalesReturnById(id);
                    if (item != null)
                    {
                        item.IsDeleted = true;

                        foreach (var i in item.SalesReturnDetail)
                        {
                            AutoPartDetail a = db.AutoPartDetail.FirstOrDefault(b => b.Id == i.PartDetailId);
                            a.Quantity -= i.Quantity;
                        }
                    }

                    string action = string.Format("Deleted Sales Return - {0}", item.MemoNumber);
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

        private IQueryable<SalesReturn> CreateQuery(SalesReturnFilterModel filter)
        {
            var items = from i in db.SalesReturn
                        where i.IsDeleted == false
                        select i;

            if (filter != null)
            {
                if (filter.CustomerId != 0)
                    items = items.Where(a => a.CustomerId == filter.CustomerId);

                if (!string.IsNullOrWhiteSpace(filter.MemoNumber))
                    items = items.Where(a => a.MemoNumber.Contains(filter.MemoNumber));

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

        public SortableBindingList<SalesReturnDisplayModel> FetchSalesReturnWithSearch(SalesReturnFilterModel filter)
        {
            try
            {
                var query = CreateQuery(filter);

                var result = from a in query
                             select new SalesReturnDisplayModel
                             {
                                 Id = a.Id,
                                 MemoNumber = a.MemoNumber,
                                 Customer = a.Customer.CompanyName,
                                 ReturnDate = a.ReturnDate.Value,
                                 AmountReturn = a.AmountReturn.Value,
                                 Adjustment = a.Adjustment.Value,
                                 TotalCreditAmount = a.TotalCreditAmount.Value,
                                 Remarks = a.Remarks,
                                 RecordedBy = a.RecordedByUser.Firstname + " " + a.RecordedByUser.Lastname,
                                 AmountUsed = a.AmountUsed.HasValue ? a.AmountUsed.Value : 0,
                                 //ApprovedBy = a.ApprovedBy.HasValue ? a.ApprovedByUser.Firstname + " " + a.ApprovedByUser.Lastname : "-",
                                 IsDeleted = a.IsDeleted.Value,
                             };

                SortableBindingList<SalesReturnDisplayModel> b = new SortableBindingList<SalesReturnDisplayModel>(result);

                return b;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SortableBindingList<SalesReturnDisplayModel> FetchAllSalesReturns()
        {
            try
            {
                return FetchSalesReturnWithSearch(null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SalesReturn FetchSalesReturnById(int id)
        {
            try
            {
                var item = (from i in db.SalesReturn
                            where i.Id == id
                            select i).FirstOrDefault();

                return item;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SortableBindingList<SalesReturnDetailModel> FetchSalesReturnDetails(int id)
        {
            try
            {
                var details = (from r in db.SalesReturnDetail
                               where r.SRId == id &&
                                 r.SalesReturn.IsDeleted != true
                               select r).ToList();

                var results = from d in details
                              select new SalesReturnDetailModel()
                              {
                                  AutoPart = d.PartDetailId.HasValue ?
                                          this.autopartController.FetchPartNameById(d.PartDetailId.Value) : "-",
                                  PartDetailId = d.PartDetailId.HasValue ? d.PartDetailId.Value : 0,
                                  PartNumber = d.PartDetailId.HasValue ?
                                          this.autopartController.FetchAutoPartDetailById(d.PartDetailId.Value).PartNumber : "-",
                                  InvoiceNumber = d.InvoiceNumber,
                                  Quantity = d.Quantity.HasValue ? d.Quantity.Value : 0,
                                  UnitPrice = d.UnitPrice.HasValue ? d.UnitPrice.Value : 0,
                                  TotalAmount = d.TotalAmount.HasValue ? d.TotalAmount.Value : 0
                              };

                SortableBindingList<SalesReturnDetailModel> b = new SortableBindingList<SalesReturnDetailModel>(results);

                return b;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int FetchQuantityReturned(int customerId, string invoiceNumber, int itemId, int returnId)
        {
            try
            {
                var qty = (from r in db.SalesReturnDetail
                           where r.SalesReturn.CustomerId == customerId &&
                             r.InvoiceNumber == invoiceNumber &&
                             r.PartDetailId == itemId &&
                             r.SRId != returnId &&
                             r.SalesReturn.IsDeleted != true
                           select r.Quantity).Sum();

                return qty.HasValue ? qty.Value : 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool InvoiceHasReturn(string invoiceNumber)
        {
            try
            {
                var result = (from r in db.SalesReturnDetail
                              select r).Any(a => a.SalesReturn.IsDeleted == false && a.InvoiceNumber == invoiceNumber);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SortableBindingList<SalesReturnDetailModel> FetchAllSalesReturnDetails()
        {
            try
            {
                var details = (from r in db.SalesReturnDetail
                               where r.SalesReturn.IsDeleted != true
                               select r).ToList();

                var results = from d in details
                              select new SalesReturnDetailModel()
                              {
                                  MemoNumber = d.SalesReturn.MemoNumber,
                                  ReturnDate = d.SalesReturn.ReturnDate,
                                  AutoPart = d.PartDetailId.HasValue ?
                                    this.autopartController.FetchPartNameById(d.PartDetailId.Value) : "-",
                                  PartNumber = d.PartDetailId.HasValue ?
                                    this.autopartController.FetchAutoPartDetailById(d.PartDetailId.Value).PartNumber : "-",
                                  InvoiceNumber = d.InvoiceNumber,
                                  Quantity = d.Quantity.HasValue ? d.Quantity.Value : 0,
                                  UnitPrice = d.UnitPrice.HasValue ? d.UnitPrice.Value : 0,
                                  TotalAmount = d.TotalAmount.HasValue ? d.TotalAmount.Value : 0
                              };

                SortableBindingList<SalesReturnDetailModel> b = new SortableBindingList<SalesReturnDetailModel>(results);

                return b;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool? IsItemCredited(int id)
        {
            try
            {
                var credited = db.SalesPayments.Any(a => a.SalesReturnDetail.SRId == id);
                return credited;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public SortableBindingList<PaymentCreditModel> FetchPaymentCreditByMemoNumber(string memoNumber)
        //{
        //    //try
        //    //{
        //    //    var credits = db.PaymentCredit.Where(a => a.CustomerCredit.Reference == memoNumber)
        //    //        .Select(a => new PaymentCreditModel 
        //    //        {
        //    //            Amount = a.Amount.HasValue ? a.Amount.Value : 0,
        //    //            VoucherNumber = a.PaymentDetail.VoucherNumber
        //    //        });

        //    //    return new SortableBindingList<PaymentCreditModel>(credits);
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    throw ex;
        //    //}
        //}

        public bool CheckReturnHasDuplicate(string codeToCheck, int id)
        {
            try
            {
                var hasReturn = db.SalesReturn.Where(a => a.IsDeleted == false)
                    .Any(a => a.MemoNumber == codeToCheck && a.Id != id);

                return hasReturn;
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
                var details = db.SalesReturnDetail
                    .Where(a => a.SalesReturn.IsDeleted != true
                        && a.Balance != 0
                        && a.SalesReturn.CustomerId == customerId)
                    .Select(a => new SalesReturnDetailModel()
                    {
                        Id = a.Id,
                        MemoNumber = a.SalesReturn.MemoNumber,
                        InvoiceNumber = a.InvoiceNumber,
                        TotalAmount = a.TotalAmount.HasValue ? a.TotalAmount.Value : 0,
                        Balance = a.Balance.HasValue ? a.Balance.Value : 0,
                        ReturnDate = a.SalesReturn.ReturnDate
                    })
                    .ToList();

                return details;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SalesReturnDetailModel> GetReturnsPerInvoice(string invoiceNumber)
        {
            try
            {
                var details = db.SalesReturnDetail
                    .Where(a => a.SalesReturn.IsDeleted != true &&
                        a.InvoiceNumber == invoiceNumber && a.Balance != 0)
                    .Select(a => new SalesReturnDetailModel()
                    {
                        Id = a.Id,
                        MemoNumber = a.SalesReturn.MemoNumber,
                        MemoDisplay = a.SalesReturn.MemoNumber + " - " + a.AutoPartDetail.PartNumber,
                        ReturnDate = a.SalesReturn.ReturnDate,
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

        public SortableBindingList<PaymentCreditModel> FetchPaymentDetails(int returnId)
        {
            try
            {
                var payment = db.SalesPayments.Where(a => a.SalesReturnDetailId != null &&
                    a.SalesReturnDetail.SalesReturn.IsDeleted != true &&
                    a.SalesReturnDetail.SalesReturn.Id == returnId);

                var result = payment.Select(a => new PaymentCreditModel
                {
                    VoucherNumber = a.PaymentDetail.VoucherNumber,
                    Amount = a.Amount.HasValue ? a.Amount.Value : 0
                });

                return new SortableBindingList<PaymentCreditModel>(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SortableBindingList<SalesReturnDetailModel> FetchAllReturnsInSale(string invoiceNumber)
        {
            try
            {
                var returns = db.SalesReturnDetail.Where(a => a.SalesReturn.IsDeleted != true && a.InvoiceNumber == invoiceNumber)
                    .ToList()
                    .Select(a => new SalesReturnDetailModel()
                    {
                        MemoNumber = a.SalesReturn.MemoNumber,
                        ReturnDate = a.SalesReturn.ReturnDate,
                        Quantity = a.Quantity.HasValue ? a.Quantity.Value : 0,
                        AutoPart = a.PartDetailId.HasValue ? this.autopartController.FetchPartNameById(a.PartDetailId.Value) : "-"
                    });

                return new SortableBindingList<SalesReturnDetailModel>(returns);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SortableBindingList<SalesReturnDetailModel> FetchSalesReturnByIds(List<int> list)
        {
            try
            {
                var saleReturns = db.SalesReturnDetail.Where(a => list.Contains(a.Id))
                    .Select(a => new SalesReturnDetailModel()
                    {
                        Id = a.Id,
                        MemoNumber = a.SalesReturn.MemoNumber,
                        InvoiceNumber = a.InvoiceNumber,
                        TotalAmount = a.TotalAmount.HasValue ? a.TotalAmount.Value : 0,
                        Balance = a.Balance.HasValue ? a.Balance.Value : 0,
                        ReturnDate = a.SalesReturn.ReturnDate
                    });
                return new SortableBindingList<SalesReturnDetailModel>(saleReturns);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}

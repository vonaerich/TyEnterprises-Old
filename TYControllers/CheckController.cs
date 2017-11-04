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
    public class CheckController : ICheckController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IActionLogController actionLogController;

        private TYEnterprisesEntities db
        {
            get
            { return unitOfWork.Context; }
        }

        public CheckController(IUnitOfWork unitOfWork, IActionLogController actionLogController)
        {
            this.unitOfWork = unitOfWork;
            this.actionLogController = actionLogController;
        }

        #region CUD Functions

        public void InsertCheck(CheckColumnModel model)
        {
            try
            {
                using (this.unitOfWork)
                {
                    Check item = new Check()
                    {
                        CheckNumber = model.CheckNumber,
                        Bank = model.Bank,
                        Branch = model.Branch,
                        Amount = model.Amount,
                        CheckDate = model.CheckDate,
                        ClearingDate = model.ClearingDate,
                        IsDeleted = model.IsDeleted,
                    };

                    this.unitOfWork.Context.AddToCheck(item);
                    this.actionLogController.AddToLog("Added new Check", UserInfo.UserId);
                    this.unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateCheck(CheckColumnModel model)
        {
            try
            {
                using (this.unitOfWork)
                {
                    var item = FetchCheckById(model.Id);
                    if (item != null)
                    {
                        item.CheckNumber = model.CheckNumber;
                        item.Bank = model.Bank;
                        item.Branch = model.Branch;
                        item.Amount = model.Amount;
                        item.CheckDate = model.CheckDate;
                        item.ClearingDate = model.ClearingDate;
                        item.IsDeleted = model.IsDeleted;
                    }

                    this.actionLogController.AddToLog("Updated Check", UserInfo.UserId);
                    this.unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteCheck(int id)
        {
            try
            {
                using (this.unitOfWork)
                {
                    var item = FetchCheckById(id);
                    if (item != null)
                        item.IsDeleted = true;

                    this.actionLogController.AddToLog("Deleted Check", UserInfo.UserId);
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

        private IQueryable<Check> CreateQuery(CheckFilterModel filter)
        {
            var items = from i in db.Check
                        where i.IsDeleted == false
                        select i;

            if (filter != null)
            {
                if (!string.IsNullOrEmpty(filter.CheckDetail))
                {
                    items = items.Where(a => a.CheckNumber.Contains(filter.CheckDetail)
                        || a.Bank.Contains(filter.CheckDetail) || a.Branch.Contains(filter.CheckDetail));
                }

                if (filter.DateType != DateSearchType.All)
                {
                    DateTime dateFrom = filter.DateFrom.Date;
                    DateTime dateTo = filter.DateTo.AddDays(1).Date;

                    items = items.Where(a => a.ClearingDate >= dateFrom && a.ClearingDate < dateTo);
                }

                if (filter.Issuer != IssuerType.All)
                {
                    if (filter.Issuer == IssuerType.Customer)
                        items = items.Where(a => a.PaymentDetail.Any(b => b.SalesPayments.Any()));
                    else if (filter.Issuer == IssuerType.Us)
                        items = items.Where(a => a.PaymentDetail.Any(b => b.PurchasePayments.Any()));
                }
            }
            else
            {
                //Default Sort
                items = items.OrderBy(a => a.PaymentDetail.FirstOrDefault().VoucherNumber)
                    .ThenBy(a => a.CheckNumber)
                    .ThenBy(a => a.Bank);
            }

            return items;
        }

        public SortableBindingList<CheckDisplayModel> FetchCheckWithSearch(CheckFilterModel filter)
        {
            try
            {
                var query = CreateQuery(filter);

                var result = from a in query
                             select new CheckDisplayModel
                             {
                                 Id = a.Id,
                                 Voucher = a.PaymentDetail.FirstOrDefault() != null ? 
                                    a.PaymentDetail.FirstOrDefault().VoucherNumber : "-",
                                 CheckNumber = a.CheckNumber,
                                 Bank = a.Bank,
                                 Branch = a.Branch,
                                 Amount = a.Amount.Value,
                                 CheckDate = a.CheckDate.Value,
                                 ClearingDate = a.ClearingDate.Value,
                                 IsDeleted = a.IsDeleted.Value,
                             };

                SortableBindingList<CheckDisplayModel> c = new SortableBindingList<CheckDisplayModel>(result);

                return c;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CheckDisplayModel> FetchAllChecks()
        {
            try
            {
                return FetchCheckWithSearch(null).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Check FetchCheckById(int id)
        {
            try
            {
                var item = (from i in db.Check
                            where i.Id == id
                            select i).FirstOrDefault();

                return item;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CheckDisplayModel> FetchAllClearingChecks()
        {
            try
            {
                var query = CreateQuery(null);

                DateTime tom = DateTime.Now.AddDays(1).Date;
                var result = from a in query
                             where a.ClearingDate >= DateTime.Today.Date &&
                                a.ClearingDate < tom
                             select new CheckDisplayModel
                             {
                                 Id = a.Id,
                                 CheckNumber = a.CheckNumber,
                                 Bank = a.Bank,
                                 Branch = a.Branch,
                                 Amount = a.Amount.Value,
                                 CheckDate = a.CheckDate.Value,
                                 ClearingDate = a.ClearingDate.Value,
                                 IsDeleted = a.IsDeleted.Value,
                             };

                return result.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}

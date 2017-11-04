using System;
using System.Linq;
using TY.SPIMS.Controllers.Interfaces;
using TY.SPIMS.Entities;
using TY.SPIMS.POCOs;
using TY.SPIMS.Utilities;

namespace TY.SPIMS.Controllers
{
    public class PurchaseCounterController : IPurchaseCounterController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IPurchaseController purchaseController;
        
        private TYEnterprisesEntities db
        {
            get
            { return unitOfWork.Context; }
        }

        public PurchaseCounterController(IUnitOfWork unitOfWork, IPurchaseController purchaseController)
        {
            this.unitOfWork = unitOfWork;
            this.purchaseController = purchaseController;
        }

        public void Insert(CounterPurchas counter)
        {
            try
            {
                using (this.unitOfWork)
                {
                    if (counter != null)
                    {
                        counter.IsDeleted = false;
                        this.unitOfWork.Context.CounterPurchases.AddObject(counter);
                        this.unitOfWork.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Update(CounterPurchas newCounter)
        {
            try
            {
                using (this.unitOfWork)
                {
                    var original = db.CounterPurchases.Single(a => a.Id == newCounter.Id);
                    original.CounterPurchasesItems.ToList().ForEach(a => db.DeleteObject(a));
                    newCounter.CounterPurchasesItems.ToList().ForEach(a => original.CounterPurchasesItems.Add(a));

                    this.unitOfWork.Context.CounterPurchases.Attach(original);
                    this.unitOfWork.Context.CounterPurchases.ApplyCurrentValues(newCounter);
                    this.unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Delete(int id)
        {
            try
            {
                using (this.unitOfWork)
                {
                    var counter = db.CounterPurchases.Single(a => a.Id == id);
                    counter.IsDeleted = true;
                    counter.CounterPurchasesItems.ToList().ForEach(a => db.DeleteObject(a));

                    this.unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private IQueryable<CounterPurchas> CreateQuery(CounterFilterModel filter)
        {
            var items = from i in db.CounterPurchases
                        where i.IsDeleted == null || i.IsDeleted != true
                        select i;

            if (filter != null)
            {
                if (filter.CustomerId != 0)
                    items = items.Where(a => a.SupplierId == filter.CustomerId);

                if (!string.IsNullOrWhiteSpace(filter.CounterNumber))
                    items = items.Where(a => a.CounterNumber.Contains(filter.CounterNumber));

                if (filter.DateType != DateSearchType.All)
                {
                    DateTime dateFrom = filter.DateFrom.Date;
                    DateTime dateTo = filter.DateTo.AddDays(1).Date;

                    items = items.Where(a => a.Date >= dateFrom && a.Date < dateTo);
                }
            }
            else
            {
                //Default sorting
                items = items.OrderByDescending(a => a.Date);
            }

            return items;
        }

        public SortableBindingList<PurchaseCounterDisplayModel> FetchPurchaseCounterWithSearch(CounterFilterModel filter)
        {
            try
            {
                var query = CreateQuery(filter);

                var result = from a in query
                             select new PurchaseCounterDisplayModel
                             {
                                 Id = a.Id,
                                 CounterNumber = a.CounterNumber,
                                 Supplier = a.Customer.CompanyName,
                                 Date = a.Date,
                                 TotalAmount = a.Total.HasValue ? a.Total.Value : 0
                             };

                SortableBindingList<PurchaseCounterDisplayModel> b = new SortableBindingList<PurchaseCounterDisplayModel>(result);

                return b;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SortableBindingList<PurchaseCounterItemModel> FetchPurchaseItems(int id)
        {
            try
            {
                var result = db.CounterPurchasesItems.Where(a => a.CounterPurchasesId == id)
                    .Select(a => new PurchaseCounterItemModel()
                    {
                        InvoiceNumber = a.Purchase.PONumber,
                        MemoNumber = a.PurchaseReturnDetail.PurchaseReturn.MemoNumber,
                        Amount = a.Amount,
                        Date = a.PurchaseId != null ? a.Purchase.Date : a.PurchaseReturnDetail.PurchaseReturn.ReturnDate,
                        PurchaseId = a.PurchaseId,
                        ReturnId = a.PurchaseReturnDetailId
                    });

                foreach (var r in result)
                {
                    if (r.PurchaseId != null)
                        r.PONumber = this.purchaseController.GetPONumber(r.PurchaseId.Value);
                }

                SortableBindingList<PurchaseCounterItemModel> b = new SortableBindingList<PurchaseCounterItemModel>(result);

                return b;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CounterPurchas FetchCounterById(int id)
        {
            try
            {
                var counter = db.CounterPurchases.Single(a => a.Id == id);
                return counter;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

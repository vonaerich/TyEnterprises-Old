using System;
using System.Linq;
using TY.SPIMS.Controllers.Interfaces;
using TY.SPIMS.Entities;
using TY.SPIMS.POCOs;
using TY.SPIMS.Utilities;

namespace TY.SPIMS.Controllers
{
    public class SalesCounterController : ISalesCounterController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ISaleController saleController;

        private TYEnterprisesEntities db
        {
            get
            { return unitOfWork.Context; }
        }

        public SalesCounterController(IUnitOfWork unitOfWork, ISaleController saleController)
        {
            this.unitOfWork = unitOfWork;
            this.saleController = saleController;
        }

        public void Insert(CounterSale counter)
        {
            try
            {
                using (this.unitOfWork)
                {
                    if (counter != null)
                    {
                        counter.IsDeleted = false;
                        this.unitOfWork.Context.CounterSales.AddObject(counter);
                        this.unitOfWork.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Update(CounterSale newCounter)
        {
            try
            {
                using (this.unitOfWork)
                {
                    var original = db.CounterSales.Single(a => a.Id == newCounter.Id);
                    original.CounterSalesItems.ToList().ForEach(a => db.DeleteObject(a));
                    newCounter.CounterSalesItems.ToList().ForEach(a => original.CounterSalesItems.Add(a));

                    this.unitOfWork.Context.CounterSales.Attach(original);
                    this.unitOfWork.Context.CounterSales.ApplyCurrentValues(newCounter);
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
                    var counter = db.CounterSales.Single(a => a.Id == id);
                    counter.IsDeleted = true;
                    counter.CounterSalesItems.ToList().ForEach(a => db.DeleteObject(a));

                    this.unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
 
        }

        private IQueryable<CounterSale> CreateQuery(CounterFilterModel filter)
        {
            var items = from i in db.CounterSales
                        where i.IsDeleted == null || i.IsDeleted != true
                        select i;

            if (filter != null)
            {
                if (filter.CustomerId != 0)
                    items = items.Where(a => a.CustomerId == filter.CustomerId);

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

        public SortableBindingList<SalesCounterDisplayModel> FetchSalesCounterWithSearch(CounterFilterModel filter)
        {
            try
            {
                var query = CreateQuery(filter);

                var result = from a in query
                             select new SalesCounterDisplayModel
                             {
                                 Id = a.Id,
                                 CounterNumber = a.CounterNumber,
                                 Customer = a.Customer.CompanyName,
                                 Date = a.Date,
                                 TotalAmount = a.Total.HasValue ? a.Total.Value : 0
                             };

                SortableBindingList<SalesCounterDisplayModel> b = new SortableBindingList<SalesCounterDisplayModel>(result);

                return b;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SortableBindingList<SalesCounterItemModel> FetchSalesItems(int id)
        {
            try
            {
                var result = db.CounterSalesItems.Where(a => a.CounterSalesId == id)
                    .Select(a => new SalesCounterItemModel() 
                    {
                        InvoiceNumber = a.Sale.InvoiceNumber,
                        MemoNumber = a.SalesReturnDetail.SalesReturn.MemoNumber,
                        Amount = a.Amount,
                        Date = a.SaleId != null ? a.Sale.Date : a.SalesReturnDetail.SalesReturn.ReturnDate,
                        SaleId = a.SaleId,
                        ReturnId = a.SalesReturnDetailId
                    });

                foreach (var r in result)
                {
                    if(r.SaleId != null)
                        r.PONumber = this.saleController.GetPONumber(r.SaleId.Value);
                }

                SortableBindingList<SalesCounterItemModel> b = new SortableBindingList<SalesCounterItemModel>(result);

                return b;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CounterSale FetchCounterById(int id)
        {
            try
            {
                var counter = db.CounterSales.Single(a => a.Id == id);
                return counter;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

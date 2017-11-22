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
    public class BrandController : IBrandController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IActionLogController actionLogController;

        private TYEnterprisesEntities db
        {
            get { return unitOfWork.Context; }
        }

        public BrandController(IUnitOfWork unitOfWork, IActionLogController actionLogController)
        {
            this.unitOfWork = unitOfWork;
            this.actionLogController = actionLogController;
        }

        #region CUD Functions

        public void InsertBrand(BrandColumnModel model)
        {
            try
            {
                using (this.unitOfWork)
                {
                    var item = new Brand()
                    {
                        BrandName = model.BrandName,
                        IsDeleted = model.IsDeleted,
                    };

                    this.unitOfWork.Context.AddToBrand(item);

                    string action = string.Format("Added new Brand - {0}", item.BrandName);
                    this.actionLogController.AddToLog(action, UserInfo.UserId);

                    this.unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateBrand(BrandColumnModel model)
        {
            try
            {
                using (this.unitOfWork)
                {
                    var item = FetchBrandById(model.Id);
                    if (item != null)
                    {
                        item.BrandName = model.BrandName;
                        item.IsDeleted = model.IsDeleted;
                    }

                    string action = string.Format("Updated Brand - {0}", item.BrandName);
                    this.actionLogController.AddToLog(action, UserInfo.UserId);

                    this.unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteBrand(int id)
        {
            try
            {
                using (this.unitOfWork)
                {
                    var item = FetchBrandById(id);
                    if (item != null)
                        item.IsDeleted = true;

                    string action = string.Format("Deleted Brand - {0}", item.BrandName);
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

        private IQueryable<Brand> CreateQuery(string filter)
        {
            var items = db.Brand
                .Where(a => a.IsDeleted == false);

            if (!string.IsNullOrWhiteSpace(filter))
                items = items.Where(a => a.BrandName.Contains(filter));

            return items;
        }

        public SortableBindingList<BrandDisplayModel> FetchBrandWithSearch(string filter)
        {
            try
            {
                var query = CreateQuery(filter);

                var result = from a in query
                             select new BrandDisplayModel { 
                                Id = a.Id,
                                BrandName = a.BrandName,
                             };


                SortableBindingList<BrandDisplayModel> b = new SortableBindingList<BrandDisplayModel>(result);
                
                return b;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Brand> FetchAllBrands()
        {
            try
            {
                var query = CreateQuery(string.Empty);

                query = query.OrderBy(a => a.BrandName);

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Brand FetchBrandById(int id)
        {
            try
            {
                var item = (from i in db.Brand
                            where i.Id == id
                            select i).FirstOrDefault();

                return item;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion
    }
}

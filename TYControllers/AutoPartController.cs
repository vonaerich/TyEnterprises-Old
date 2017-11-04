using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.IO;
using System.Linq;
using System.Text;
using TY.SPIMS.Entities;
using TY.SPIMS.Utilities;
using TY.SPIMS.Controllers.Interfaces;

namespace TY.SPIMS.Controllers
{
    public class AutoPartController : IAutoPartController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IActionLogController actionLogController;

        private TYEnterprisesEntities db
        {
            get
            { return unitOfWork.Context; }
        }

        public AutoPartController(IUnitOfWork unitOfWork, IActionLogController actionLogController)
        {
            this.unitOfWork = unitOfWork;
            this.actionLogController = actionLogController;
        }

        #region CUD Functions

        public void InsertAutoPart(AutoPartColumnModel model)
        {
            try
            {
                using (this.unitOfWork)
                {
                    AutoPart item = FetchAutoPartById(model.PartId);
                    if (item != null)
                        item.PartName = model.PartName;
                    else
                    {
                        item = new AutoPart()
                        {
                            PartName = model.PartName,
                            IsDeleted = false
                        };

                        db.AddToAutoPart(item);
                    }

                    //Add details
                    if (model.AutoPartDetail != null)
                    {
                        AutoPartDetailColumnModel detail = model.AutoPartDetail;
                        AutoPartDetail d = new AutoPartDetail()
                        {
                            Make = detail.Make,
                            Model = detail.Model,
                            BrandId = detail.BrandId,
                            IsDeleted = false,
                            PartNumber = detail.PartNumber,
                            Quantity = detail.Quantity,
                            SellingPrice1 = detail.SellingPrice,
                            SellingPrice2 = detail.SellingPrice2,
                            Unit = detail.Unit,
                            ReorderLimit = detail.ReorderLimit,
                            BuyingPrice = detail.PurchasePrice,
                            Size = detail.Size,
                            Picture = !string.IsNullOrWhiteSpace(detail.Picture) ? Path.GetFileName(detail.Picture) : string.Empty,
                            Description = detail.Description
                        };

                        if (model.AutoPartDetail.AltPartNumbers.Count > 0)
                        {
                            foreach (string n in model.AutoPartDetail.AltPartNumbers)
                            {
                                AutoPartAltPartNumber altNumber = new AutoPartAltPartNumber() { AltPartNumber = n };
                                d.AutoPartAltPartNumber.Add(altNumber);
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(model.AutoPartDetail.Picture))
                        {
                            SavePictureFile(model.AutoPartDetail.Picture);
                        }

                        item.AutoPartDetail.Add(d);
                    }

                    string action = string.Format("Added new Auto Part - {0} {1}", model.PartName, model.AutoPartDetail.PartNumber);
                    this.actionLogController.AddToLog(action, UserInfo.UserId);

                    this.unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateAutoPart(AutoPartColumnModel model)
        {
            try
            {
                using (this.unitOfWork)
                {
                    var item = FetchAutoPartDetailById(model.Id);
                    if (item != null)
                    {
                        //Auto Part
                        if (item.AutoPartId != null & item.AutoPartId.Value != model.PartId)
                        {
                            AutoPart part = FetchAutoPartById(model.PartId);
                            if (part != null)
                                item.AutoPartId = model.PartId;
                            else
                            {
                                AutoPart newPart = new AutoPart();
                                newPart.PartName = model.PartName;
                                newPart.IsDeleted = false;

                                item.AutoPart = newPart;
                            }
                        }

                        if (item.AutoPartAltPartNumber.Any())
                            item.AutoPartAltPartNumber.Clear();

                        if (model.AutoPartDetail.AltPartNumbers.Any())
                        {
                            foreach (var alt in model.AutoPartDetail.AltPartNumbers)
                            {
                                if (!item.AutoPartAltPartNumber.Any(a => a.AltPartNumber == alt))
                                {
                                    item.AutoPartAltPartNumber.Add(
                                        new AutoPartAltPartNumber() { AltPartNumber = alt });
                                }
                            }
                        }

                        //Auto Part Detail
                        if (model.AutoPartDetail != null)
                        {
                            item.Make = model.AutoPartDetail.Make;
                            item.Model = model.AutoPartDetail.Model;
                            item.BrandId = model.AutoPartDetail.BrandId;
                            item.PartNumber = model.AutoPartDetail.PartNumber;
                            //item.AltPartNumber = model.AutoPartDetail.AltPartNumber;
                            item.SellingPrice1 = model.AutoPartDetail.SellingPrice;
                            item.SellingPrice2 = model.AutoPartDetail.SellingPrice2;
                            item.Unit = model.AutoPartDetail.Unit;
                            item.ReorderLimit = model.AutoPartDetail.ReorderLimit;
                            item.BuyingPrice = model.AutoPartDetail.PurchasePrice;
                            item.Size = model.AutoPartDetail.Size;
                            item.Description = model.AutoPartDetail.Description;
                            item.Quantity = model.AutoPartDetail.Quantity;

                            if (!string.IsNullOrWhiteSpace(item.Picture))
                                RemovePictureFile(item.Picture);

                            if (!string.IsNullOrWhiteSpace(model.AutoPartDetail.Picture))
                            {
                                if (string.Compare(item.Picture, model.AutoPartDetail.Picture, true) != 0)
                                {
                                    item.Picture = Path.GetFileName(model.AutoPartDetail.Picture);
                                    SavePictureFile(model.AutoPartDetail.Picture);
                                }
                            }
                            else
                            {
                                item.Picture = string.Empty;
                            }
                        }
                    }

                    string action = string.Format("Updated Auto Part - {0}", model.PartName);
                    this.actionLogController.AddToLog(action, UserInfo.UserId);

                    this.unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteAutoPart(int id)
        {
            try
            {
                using (this.unitOfWork)
                {
                    var item = FetchAutoPartDetailById(id);
                    if (item != null)
                        item.IsDeleted = true;

                    string action = string.Format("Deleted Auto Part - {0}", item.AutoPart.PartName);
                    this.actionLogController.AddToLog(action, UserInfo.UserId);

                    this.unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteAllAutoPart(int id)
        {
            try
            {
                var item = FetchAutoPartById(id);
                if (item != null)
                    item.IsDeleted = true;

                foreach (AutoPartDetail d in item.AutoPartDetail)
                {
                    d.IsDeleted = true;
                }

                string action = string.Format("Deleted Auto Part - {0}", item.PartName);
                this.actionLogController.AddToLog(action, UserInfo.UserId);

                this.unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateReorderDates(List<int> ids)
        {
            try
            {
                var db = ConnectionManager.Instance.Connection;
                var itemsToUpdate = from a in db.AutoPartDetail
                                    where ids.Contains(a.Id)
                                    select a;

                foreach (var item in itemsToUpdate)
                    item.ReorderDate = DateTime.Now;

                db.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateAutoPartName(int id, string newAutoPart)
        {
            try
            {
                var autoPart = FetchAutoPartById(id);
                autoPart.PartName = newAutoPart;

                db.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdatePartPrices(int id, decimal newBuyPrice, decimal newSell1, decimal newSell2)
        {
            try
            {
                var autoPartDetail = FetchAutoPartDetailById(id);
                autoPartDetail.BuyingPrice = newBuyPrice;
                autoPartDetail.SellingPrice1 = newSell1;
                autoPartDetail.SellingPrice2 = newSell2;

                db.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateBuyPrice(int id, decimal newBuyPrice)
        {
            try
            {
                var autoPartDetail = FetchAutoPartDetailById(id);
                autoPartDetail.BuyingPrice = newBuyPrice;

                db.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Fetch Functions

        private IQueryable<AutoPartDetail> CreateQuery(AutoPartFilterModel filter)
        {
            var items = from i in db.AutoPartDetail
                        where i.IsDeleted == false
                        select i;

            if (filter != null)
            {
                if (filter.AutoPartId != 0)
                    items = items.Where(a => a.AutoPartId == filter.AutoPartId);

                if(!string.IsNullOrWhiteSpace(filter.PartNumber))
                    items = items.Where(a => a.PartNumber.Contains(filter.PartNumber) || 
                        a.AutoPartAltPartNumber.Any(b => b.AltPartNumber.Contains(filter.PartNumber)));

                if(filter.BrandId != 0)
                    items = items.Where(a => a.BrandId == filter.BrandId);

                if (!string.IsNullOrWhiteSpace(filter.Model))
                    items = items.Where(a => a.Model.Contains(filter.Model));

                if (!string.IsNullOrWhiteSpace(filter.Size))
                    items = items.Where(a => a.Size.Contains(filter.Size));

                if (filter.ForReorder)
                {
                    items = items.Where(a => a.Quantity <= a.ReorderLimit && a.Quantity > 0);

                    DateTime reorderWindowDate = DateTime.Now.AddDays(-4).Date;
                    if (filter.ReorderList)
                        items = items.Where(a => (a.ReorderDate < reorderWindowDate || a.ReorderDate.HasValue == false));
                    else
                        items = items.Where(a => (a.ReorderDate >= reorderWindowDate));
 
                }
                    
            }

            return items;
        }

        public SortableBindingList<AutoPartDisplayModel> FetchAutoPartWithSearch(AutoPartFilterModel filter)
        {
            try
            {
                var query = CreateQuery(filter);

                var result = from a in query
                             select new AutoPartDisplayModel
                             {
                                 Id = a.Id,
                                 PartName = a.AutoPart.PartName,
                                 Description = a.Description != string.Empty ? a.Description : "-",
                                 BrandName = a.Brand.BrandName,
                                 Make = a.Make != string.Empty ? a.Make : "-",
                                 Model = a.Model != string.Empty ? a.Model : "-",
                                 Quantity = a.Quantity.HasValue ? a.Quantity.Value : 0,
                                 PartNumber = a.PartNumber,
                                 Unit = a.Unit != string.Empty ? a.Unit : "-",
                                 RetailPrice = a.SellingPrice1.HasValue ? a.SellingPrice1.Value : 0,
                                 PurchasePrice = a.BuyingPrice.HasValue ? a.BuyingPrice.Value : 0,
                                 Size = a.Size != string.Empty ? a.Size : "-"
                             };

                SortableBindingList<AutoPartDisplayModel> b = new SortableBindingList<AutoPartDisplayModel>(result);

                return b;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<AutoPart> FetchAllAutoParts()
        {
            try
            {
                var result = from a in db.AutoPart
                             where a.IsDeleted == false
                             orderby a.PartName
                             select a;

                return result.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<AutoPart> FetchAPWithSearch(string search)
        {
            try
            {
                var result = from a in db.AutoPart
                             where a.IsDeleted == false &&
                                a.PartName.Contains(search)
                             orderby a.PartName
                             select a;

                return result.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<AutoPartDetail> FetchAllAutoPartDetails()
        {
            try
            {
                var result = from a in db.AutoPartDetail
                             where a.IsDeleted != true
                             orderby a.AutoPart.PartName
                             select a;

                return result.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public AutoPart FetchAutoPartById(int id)
        {
            try
            {
                var item = (from i in db.AutoPart
                            where i.Id == id && i.IsDeleted != true
                            select i).FirstOrDefault();

                return item;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<AutoPartDetail> FetchAutoPartDetails(int id)
        {
            try
            {
                var result = from a in db.AutoPartDetail
                             where a.Id == id && a.IsDeleted != true
                             orderby a.PartNumber
                             select a;

                return result.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<AutoPartDetail> FetchAutoPartDetails(string partNumber)
        {
            try
            {
                var result = from a in db.AutoPartDetail
                             where (a.PartNumber == partNumber 
                             || a.AutoPartAltPartNumber.Any(b => b.AltPartNumber == partNumber)) 
                             && a.IsDeleted != true
                             orderby a.PartNumber
                             select a;

                return result.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public AutoPartDetail FetchAutoPartDetailById(int id)
        {
            try
            {
                var result = (from a in db.AutoPartDetail
                             where a.Id == id && a.IsDeleted != true
                             select a).FirstOrDefault();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public AutoPartDetail FetchAutoPartDetailByPartNumber(string partNumber)
        {
            try
            {
                var result = (from a in db.AutoPartDetail
                              where a.PartNumber == partNumber &&
                                a.IsDeleted != true
                              select a).FirstOrDefault();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<string> FetchAllPartNames()
        {
            try
            {
                var partNames = from p in db.AutoPart
                                where p.IsDeleted == false
                                orderby p.PartName
                                select p.PartName;

                return partNames.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] FetchAllPartNumbers()
        {
            try
            {
                var partNumbers = from p in db.AutoPartDetail
                                where p.IsDeleted == false
                                orderby p.PartNumber
                                select p.PartNumber;

                return partNumbers.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<string> FetchAllPartNumbersWithAlternateNumbers()
        {
            try
            {
                List<string> allNumbers = new List<string>();

                var partNumbers = from p in db.AutoPartDetail
                                  where p.IsDeleted == false
                                  select p.PartNumber;

                var altNumbers = from a in db.AutoPartAltPartNumber
                                 where a.AutoPartDetail.IsDeleted == false
                                 select a.AltPartNumber;

                allNumbers.AddRange(partNumbers);

                altNumbers.ToList().ForEach((a) => 
                {
                    if (!allNumbers.Contains(a))
                        allNumbers.Add(a);
                });

                allNumbers = allNumbers.OrderBy(a => a).ToList();

                return allNumbers;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public AutoPart FetchAutoPartByName(string name)
        {
            try
            {
                var item = (from i in db.AutoPart
                            where i.PartName == name
                                && i.IsDeleted != true
                            select i).FirstOrDefault();

                return item;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string FetchPartNameByPartNumber(string partNumber)
        {
            try
            {
                var item = (from i in db.AutoPartDetail
                            where i.PartNumber == partNumber
                                && i.IsDeleted != true
                            select i).FirstOrDefault();

                StringBuilder result = new StringBuilder();

                if (item != null)
                {
                    result.AppendFormat("{0} - {1}\n{2} / {3} / {4}", item.AutoPart.PartName,
                        item.PartNumber, item.Brand.BrandName, item.Model, item.Make);
                }

                return result.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string FetchPartNameById(int id)
        {
            try
            {
                var item = (from i in db.AutoPartDetail
                            where i.Id == id
                            select i).FirstOrDefault();

                StringBuilder result = new StringBuilder();

                if (item != null)
                {
                    result.AppendFormat("{0} - {1}\n{2} / {3} / {4}", item.AutoPart.PartName,
                        item.PartNumber, item.Brand.BrandName, item.Model, item.Make);
                }

                return result.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool AutoPartHasDetails(int id)
        {
            try
            {
                var item = db.AutoPart.Single(a => a.Id == id);
                return item.AutoPartDetail.Any(a => a.IsDeleted != true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool AutoPartHasSalesOrPurchases(int id)
        {
            try
            {
                var hasSales = db.SaleDetail.Any(a => a.AutoPartDetail.Id == id
                        && a.Sale.IsDeleted != null && !a.Sale.IsDeleted.Value);
                var hasPurchases = db.PurchaseDetail.Any(a => a.AutoPartDetail.Id == id
                        && a.Purchase.IsDeleted != null && !a.Purchase.IsDeleted.Value);

                return hasSales || hasPurchases;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Helper
        public static string _pictureFolderName = "AutoPartPictures";
        private void SavePictureFile(string picture)
        {
            string serverIP = Helper.GetServerIP();
            string destinationPath = Path.Combine(@"\\", serverIP, AutoPartController._pictureFolderName, Path.GetFileName(picture));

            string destinationDir = Path.GetDirectoryName(destinationPath);
            if (!System.IO.Directory.Exists(destinationDir))
                System.IO.Directory.CreateDirectory(destinationDir);

            File.Copy(picture, destinationPath, true);
        }

        private void RemovePictureFile(string picture)
        { 
            string serverIP = Helper.GetServerIP();
            string destinationPath = Path.Combine(@"\\", serverIP, AutoPartController._pictureFolderName, Path.GetFileName(picture));

            if (File.Exists(destinationPath))
                File.Delete(destinationPath);
        }

        #endregion
    }

    #region Model

    public class AutoPartFilterModel
    {
        public int AutoPartId { get; set; }
        public string PartNumber { get; set; }
        public string PartName { get; set; }
        public int BrandId { get; set; }
        public string Model { get; set; }
        public string Size { get; set; }
        public NumericSearchType QuantitySearchType { get; set; }
        public int QuantitySearchValue { get; set; }
        public bool ForReorder { get; set; }
        public bool ReorderList { get; set; }
    }

    public class AutoPartColumnNames
    {
        public static string Id = "Id";
        public static string PartName = "PartName";
        public static string Description = "Description";
        public static string TotalQuantity = "TotalQuantity";
        public static string IsDeleted = "IsDeleted";
        public static string AutoPartDetail = "AutoPartDetail";
    }

    public class AutoPartColumnModel
    {
        public int Id { get; set; }
        public int PartId { get; set; }
        public string PartName { get; set; }
        //public string Description { get; set; }
        public int TotalQuantity { get; set; }
        public bool IsDeleted { get; set; }
        public AutoPartDetailColumnModel AutoPartDetail { get; set; }
    }

    public class AutoPartDetailColumnModel
    {
        public int Id { get; set; }
        public int AutoPartId { get; set; }
        public string Model { get; set; }
        public string Make { get; set; }
        public int BrandId { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public string Size { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal SellingPrice2 { get; set; }
        public decimal PurchasePrice { get; set; }
        public string PartNumber { get; set; }
        //public string AltPartNumber { get; set; }
        public int ReorderLimit { get; set; }
        public string Picture { get; set; }
        public bool IsDeleted { get; set; }
        public string Description { get; set; }
        public List<string> AltPartNumbers { get; set; }
    }


    public class AutoPartDisplayModel
    {
        public int Id { get; set; }
        public string PartName { get; set; }
        public string Description { get; set; }
        public int TotalQuantity { get; set; }
        public bool IsDeleted { get; set; }
        public string Model { get; set; }
        public string Make { get; set; }
        public string BrandName { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public string Size { get; set; }
        public decimal RetailPrice { get; set; }
        public decimal PurchasePrice { get; set; }
        public string PartNumber { get; set; }
        //public string AltPartNumber { get; set; }
    }

    #endregion


}

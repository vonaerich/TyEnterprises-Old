using System;
using System.Data;
using System.Data.Objects;
using System.Linq;
using TY.SPIMS.Controllers.Interfaces;
using TY.SPIMS.Entities;
using TY.SPIMS.POCOs;
using TY.SPIMS.Utilities;

namespace TY.SPIMS.Controllers
{
    public class InventoryUserController : IInventoryUserController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IActionLogController actionLogController;

        private TYEnterprisesEntities db
        {
            get
            { return unitOfWork.Context; }
        }

        public InventoryUserController(IUnitOfWork unitOfWork, IActionLogController actionLogController)
        {
            this.unitOfWork = unitOfWork;
            this.actionLogController = actionLogController;
        }

        #region CUD Functions

        public void InsertInventoryUser(InventoryUserColumnModel model)
        {
            try
            {
                using (this.unitOfWork)
                {
                    InventoryUser item = new InventoryUser()
                    {
                        Username = model.Username,
                        Password = model.Password,
                        Firstname = model.Firstname,
                        Lastname = model.Lastname,
                        IsAdmin = model.IsAdmin,
                        IsApprover = model.IsApprover,
                        IsVisitor = model.IsVisitor,
                        IsDeleted = model.IsDeleted
                    };

                    this.unitOfWork.Context.AddToInventoryUser(item);
                    string action = string.Format("Added new User - {0}", item.Username);
                    this.actionLogController.AddToLog(action, UserInfo.UserId);
                    this.unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateInventoryUser(InventoryUserColumnModel model)
        {
            try
            {
                using (this.unitOfWork)
                {
                    var item = FetchInventoryUserById(model.Id);
                    if (item != null)
                    {
                        item.Username = model.Username;
                        item.Password = model.Password;
                        item.Firstname = model.Firstname;
                        item.Lastname = model.Lastname;
                        item.IsAdmin = model.IsAdmin;
                        item.IsApprover = model.IsApprover;
                        item.IsVisitor = model.IsVisitor;
                        item.IsDeleted = model.IsDeleted;
                    }

                    string action = string.Format("Updated User - {0}", item.Username);
                    this.actionLogController.AddToLog(action, UserInfo.UserId);
                    this.unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteInventoryUser(int id)
        {
            try
            {
                using (this.unitOfWork)
                {
                    var item = FetchInventoryUserById(id);
                    if (item != null)
                        item.IsDeleted = true;

                    string action = string.Format("Deleted User - {0}", item.Username);
                    this.actionLogController.AddToLog(action, UserInfo.UserId);
                    this.unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateUserTheme(int id, bool theme)
        {
            try
            {
                using (this.unitOfWork)
                {
                    var user = (from u in db.InventoryUser
                                where u.Id == id
                                select u).FirstOrDefault();

                    if (user != null)
                        user.Theme = theme;

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

        private IQueryable<InventoryUser> CreateQuery(string filter)
        {
            var items = from i in db.InventoryUser
                        where i.IsDeleted == false
                        select i;

            if (!string.IsNullOrWhiteSpace(filter))
                items = items.Where(a => a.Username.Contains(filter) 
                    || a.Firstname.Contains(filter) 
                    || a.Lastname.Contains(filter));

            return items;
        }

        public SortableBindingList<InventoryUserDisplayModel> FetchInventoryUserWithSearch(string filter)
        {
            try
            {
                var query = CreateQuery(filter);

                var result = from a in query
                             select new InventoryUserDisplayModel
                             {
                                 Id = a.Id,
                                 Username = a.Username,
                                 Firstname = a.Firstname,
                                 Lastname = a.Lastname,
                                 IsAdmin = a.IsAdmin.HasValue ? a.IsAdmin.Value : false,
                                 IsApprover = a.IsApprover.HasValue ? a.IsApprover.Value : false,
                                 IsVisitor = a.IsVisitor.HasValue ? a.IsVisitor.Value : false
                             };

                SortableBindingList<InventoryUserDisplayModel> b = new SortableBindingList<InventoryUserDisplayModel>(result);

                return b;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SortableBindingList<InventoryUserDisplayModel> FetchAllInventoryUsers()
        {
            try
            {
                var query = CreateQuery(string.Empty);

                var result = from a in query
                             select new InventoryUserDisplayModel
                             {
                                 Id = a.Id,
                                 Username = a.Username,
                                 Firstname = a.Firstname,
                                 Lastname = a.Lastname,
                                 IsAdmin = a.IsAdmin.HasValue ? a.IsAdmin.Value : false,
                                 IsApprover = a.IsApprover.HasValue ? a.IsApprover.Value : false
                             };

                SortableBindingList<InventoryUserDisplayModel> b = new SortableBindingList<InventoryUserDisplayModel>(result);

                return b;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public InventoryUser FetchInventoryUserById(int id)
        {
            try
            {
                var item = (from i in db.InventoryUser
                            where i.Id == id
                            select i).FirstOrDefault();

                return item;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public InventoryUser LoginUser(string username, string pw)
        {
            try
            {
                string encryptedPassword = Helper.EncryptString(pw);

                var user = (from u in db.InventoryUser
                            where u.Username == username &&
                             u.Password == encryptedPassword &&
                             u.IsDeleted == false
                            select u).FirstOrDefault();

                return user;
            }
            catch (EntityException entEx)
            {
                throw entEx;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}



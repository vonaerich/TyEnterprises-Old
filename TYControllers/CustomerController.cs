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
    public class CustomerController : ICustomerController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IActionLogController actionLogController;

        private TYEnterprisesEntities db
        {
            get
            { return unitOfWork.Context; }
        }

        public CustomerController(IUnitOfWork unitOfWork, IActionLogController actionLogController)
        {
            this.unitOfWork = unitOfWork;
            this.actionLogController = actionLogController;
        }

        #region CUD Functions

        public void InsertCustomer(CustomerColumnModel model)
        {
            try
            {
                using (this.unitOfWork)
                {
                    Customer item = new Customer()
                    {
                        CustomerCode = model.CustomerCode,
                        CompanyName = model.CompanyName,
                        ContactPerson = model.ContactPerson,
                        Address = model.Address,
                        PhoneNumber = model.PhoneNumber,
                        FaxNumber = model.FaxNumber,
                        TIN = model.TIN,
                        Agent = model.Agent,
                        PaymentTerms = model.PaymentTerms,
                        IsDeleted = model.IsDeleted,
                    };

                    this.unitOfWork.Context.AddToCustomer(item);

                    string action = string.Format("Added new Customer - {0}", item.CompanyName);
                    this.actionLogController.AddToLog(action, UserInfo.UserId);

                    this.unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateCustomer(CustomerColumnModel model)
        {
            try
            {
                using (this.unitOfWork)
                {
                    var item = FetchCustomerById(model.Id);
                    if (item != null)
                    {
                        item.CustomerCode = model.CustomerCode;
                        item.CompanyName = model.CompanyName;
                        item.ContactPerson = model.ContactPerson;
                        item.Address = model.Address;
                        item.PhoneNumber = model.PhoneNumber;
                        item.FaxNumber = model.FaxNumber;
                        item.TIN = model.TIN;
                        item.Agent = model.Agent;
                        item.PaymentTerms = model.PaymentTerms;
                        item.IsDeleted = model.IsDeleted;
                    }
                    this.unitOfWork.SaveChanges();

                    string action = string.Format("Updated Customer - {0}", item.CompanyName);
                    this.actionLogController.AddToLog(action, UserInfo.UserId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteCustomer(int id)
        {
            try
            {
                using (this.unitOfWork)
                {
                    var item = FetchCustomerById(id);
                    if (item != null)
                        item.IsDeleted = true;

                    this.unitOfWork.SaveChanges();

                    string action = string.Format("Deleted Customer - {0}", item.CompanyName);
                    this.actionLogController.AddToLog(action, UserInfo.UserId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Fetch Functions

        public IQueryable<Customer> CreateQuery(string filter)
        {
            var items = from i in db.Customer
                        where i.IsDeleted == false
                        select i;

            if (!string.IsNullOrWhiteSpace(filter))
                items = items.Where(a => a.CompanyName.Contains(filter)
                    || a.CustomerCode.Contains(filter)
                    || a.ContactPerson.Contains(filter)
                    || a.Address.Contains(filter)
                    || a.CustomerCode.Contains(filter));

            return items;
        }

        public SortableBindingList<CustomerDisplayModel> FetchCustomerWithSearch(string filter)
        {
            try
            {
                var query = CreateQuery(filter);

                var result = from a in query
                             orderby a.CompanyName
                             select new CustomerDisplayModel
                             {
                                 Id = a.Id,
                                 Address = a.Address,
                                 Agent = a.Agent,
                                 CompanyName = a.CompanyName,
                                 ContactPerson = a.ContactPerson,
                                 CustomerCode = a.CustomerCode,
                                 FaxNumber = a.FaxNumber,
                                 TIN = a.TIN,
                                 PaymentTerms = a.PaymentTerms.Value,
                                 PhoneNumber = a.PhoneNumber
                             };

                SortableBindingList<CustomerDisplayModel> b = new SortableBindingList<CustomerDisplayModel>(result);

                return b;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CustomerDisplayModel> FetchAllCustomers()
        {
            try
            {
                var query = CreateQuery(string.Empty);

                var result = query
                    .OrderBy(a => a.CompanyName)
                    .Select(a => new CustomerDisplayModel
                        {
                            Id = a.Id,
                            Address = a.Address,
                            Agent = a.Agent,
                            CompanyName = a.CompanyName,
                            ContactPerson = a.ContactPerson,
                            CustomerCode = a.CustomerCode,
                            FaxNumber = a.FaxNumber,
                            TIN = a.TIN,
                            PaymentTerms = a.PaymentTerms.Value,
                            PhoneNumber = a.PhoneNumber
                        })
                    .ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Customer FetchCustomerById(int id)
        {
            try
            {
                var item = (from i in db.Customer
                            where i.Id == id
                            select i).FirstOrDefault();

                return item;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Customer FetchCustomerByName(string customerName)
        {
            try
            {
                var item = (from i in db.Customer
                            where i.CompanyName == customerName
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

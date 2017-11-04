using System.Collections.Generic;
using System.Linq;
using TY.SPIMS.Entities;
using TY.SPIMS.POCOs;

namespace TY.SPIMS.Controllers.Interfaces
{
    public interface ICustomerController
    {
        IQueryable<Customer> CreateQuery(string filter);
        void DeleteCustomer(int id);
        List<CustomerDisplayModel> FetchAllCustomers();
        Customer FetchCustomerById(int id);
        Customer FetchCustomerByName(string customerName);
        TY.SPIMS.Utilities.SortableBindingList<CustomerDisplayModel> FetchCustomerWithSearch(string filter);
        void InsertCustomer(CustomerColumnModel model);
        void UpdateCustomer(CustomerColumnModel model);
    }
}

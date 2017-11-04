using TY.SPIMS.Entities;
using TY.SPIMS.POCOs;
using TY.SPIMS.Utilities;

namespace TY.SPIMS.Controllers.Interfaces
{
    public interface ISalesCounterController
    {
        void Delete(int id);
        CounterSale FetchCounterById(int id);
        SortableBindingList<SalesCounterDisplayModel> FetchSalesCounterWithSearch(CounterFilterModel filter);
        SortableBindingList<SalesCounterItemModel> FetchSalesItems(int id);
        void Insert(CounterSale counter);
        void Update(CounterSale newCounter);
    }
}

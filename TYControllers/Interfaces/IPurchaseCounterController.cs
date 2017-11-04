using TY.SPIMS.Entities;
using TY.SPIMS.POCOs;
using TY.SPIMS.Utilities;

namespace TY.SPIMS.Controllers.Interfaces
{
    public interface IPurchaseCounterController
    {
        void Delete(int id);
        CounterPurchas FetchCounterById(int id);
        SortableBindingList<PurchaseCounterDisplayModel> FetchPurchaseCounterWithSearch(CounterFilterModel filter);
        SortableBindingList<PurchaseCounterItemModel> FetchPurchaseItems(int id);
        void Insert(CounterPurchas counter);
        void Update(CounterPurchas newCounter);
    }
}

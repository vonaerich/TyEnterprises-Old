using TY.SPIMS.Entities;
using TY.SPIMS.POCOs;
using TY.SPIMS.Utilities;

namespace TY.SPIMS.Controllers.Interfaces
{
    public interface IInventoryUserController
    {
        void DeleteInventoryUser(int id);
        SortableBindingList<InventoryUserDisplayModel> FetchAllInventoryUsers();
        InventoryUser FetchInventoryUserById(int id);
        SortableBindingList<InventoryUserDisplayModel> FetchInventoryUserWithSearch(string filter);
        void InsertInventoryUser(InventoryUserColumnModel model);
        InventoryUser LoginUser(string username, string pw);
        void UpdateInventoryUser(InventoryUserColumnModel model);
        void UpdateUserTheme(int id, bool theme);
    }
}

using System.Collections.Generic;
using TY.SPIMS.Entities;
using TY.SPIMS.POCOs;
using TY.SPIMS.Utilities;

namespace TY.SPIMS.Controllers.Interfaces
{
    public interface ICheckController
    {
        void DeleteCheck(int id);
        List<CheckDisplayModel> FetchAllChecks();
        List<CheckDisplayModel> FetchAllClearingChecks();
        Check FetchCheckById(int id);
        SortableBindingList<CheckDisplayModel> FetchCheckWithSearch(CheckFilterModel filter);
        void InsertCheck(CheckColumnModel model);
        void UpdateCheck(CheckColumnModel model);
    }
}

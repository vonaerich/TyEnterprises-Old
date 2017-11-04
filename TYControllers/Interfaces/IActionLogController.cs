using System;
using System.Collections.Generic;
using TY.SPIMS.POCOs;

namespace TY.SPIMS.Controllers.Interfaces
{
    public interface IActionLogController
    {
        void AddToLog(string action, int userId);
        List<ActionDisplayModel> FetchActionsWithSearch(bool filter, DateTime dateFrom, DateTime dateTo);
        List<ActionDisplayModel> FetchRecentActions();
    }
}

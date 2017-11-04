using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TY.SPIMS.Entities;
using TY.SPIMS.Utilities;
using TY.SPIMS.POCOs;
using TY.SPIMS.Controllers.Interfaces;
using System.Data.Objects;

namespace TY.SPIMS.Controllers
{
    public class ActionLogController : IActionLogController
    {
        //Delete
        public static ActionLogController Instance = new ActionLogController(new UnitOfWork());

        private readonly IUnitOfWork unitOfWork;
        private TYEnterprisesEntities db
        {
            get { return this.unitOfWork.Context; }
        }

        public ActionLogController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public void AddToLog(string action, int userId)
        {
            try
            {
                var computerName = Environment.MachineName;
                var actionToSave = string.Format("From {0}: {1}", computerName, action);

                ActionLog log = new ActionLog() { 
                    Action = actionToSave,
                    ActionDate = DateTime.Now,
                    ActionUserId = userId
                };

                db.AddToActionLog(log);
                db.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ActionDisplayModel> FetchRecentActions()
        {
            try
            {
                var actions = from a in db.ActionLog
                              orderby a.ActionDate descending
                              select new ActionDisplayModel { 
                                ActionDate = a.ActionDate.Value,
                                User = a.InventoryUser.Firstname + " " + a.InventoryUser.Lastname,
                                Action = a.Action
                              };

                return actions.Take(10).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ActionDisplayModel> FetchActionsWithSearch(bool filter, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                var actions = from a in db.ActionLog
                              orderby a.ActionDate descending
                              select new ActionDisplayModel
                              {
                                  ActionDate = a.ActionDate.Value,
                                  User = a.InventoryUser.Firstname + " " + a.InventoryUser.Lastname,
                                  Action = a.Action
                              };

                if (filter)
                {
                    DateTime nextDay = dateTo.AddDays(1);
                    actions = actions.Where(a => a.ActionDate > dateFrom && a.ActionDate <= nextDay);
                }

                return actions.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

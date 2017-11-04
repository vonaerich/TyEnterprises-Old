using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TY.SPIMS.Controllers.Interfaces;
using TY.SPIMS.Entities;
using System.Data.Objects;

namespace TY.SPIMS.Controllers
{
    public class UnitOfWork : IUnitOfWork
    {
        private TYEnterprisesEntities _context = new TYEnterprisesEntities();
        public TYEnterprisesEntities Context
        {
            get 
            {
                if (this._context == null)
                    this._context = new TYEnterprisesEntities();
                return _context; 
            }
        }

        public void SaveChanges()
        {
            this.Context.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
        }

        public void Dispose()
        {
            this._context = null;
        }

    }
}

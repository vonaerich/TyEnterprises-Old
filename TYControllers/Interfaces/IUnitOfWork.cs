using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TY.SPIMS.Entities;

namespace TY.SPIMS.Controllers.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        TYEnterprisesEntities Context { get; }
        void SaveChanges();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TY.SPIMS.Entities;

namespace TY.SPIMS.Controllers
{
    public class ConnectionFactory
    {
        private static TYEnterprisesEntities _longTermContext;
        public static TYEnterprisesEntities LongTermContext
        {
            get 
            {
                if (_longTermContext == null)
                    _longTermContext = new TYEnterprisesEntities();
                return _longTermContext;
            } 
        }

        public static bool HasShortTermContext
        {
            get 
            {
                return _shortTermContext != null;
            }
        }

        private static TYEnterprisesEntities _shortTermContext;
        public static TYEnterprisesEntities ShortTermContext
        {
            get
            {
                return _shortTermContext;
            }
        }

        public static TYEnterprisesEntities GetShortTermContext()
        {
            _shortTermContext = new TYEnterprisesEntities();
            return _shortTermContext;
        }
    }
}

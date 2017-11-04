using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TY.SPIMS.Entities;
using System.Data;

namespace TY.SI.Client
{
    public class ConnectionManager
    {
        private static object obj = new object();

        private static ConnectionManager _instance = new ConnectionManager();
        public static ConnectionManager Instance
        {
            get
            {
                lock (obj)
                {
                    if (_instance == null)
                        return new ConnectionManager();
                    return _instance;
                }
            }
        }

        private TYEnterprisesEntities _connection = new TYEnterprisesEntities();
        public TYEnterprisesEntities Connection
        {
            get
            {
                try
                {
                    if (_connection == null)
                        return new TYEnterprisesEntities();
                    return _connection;
                }
                catch (EntityException entEx)
                {
                    throw entEx;
                }
            }
        }

    }
}

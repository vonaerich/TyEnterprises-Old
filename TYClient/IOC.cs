using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleInjector;

namespace TY.SPIMS.Client
{
    public static class IOC
    {
        private static object _lock = new object();

        private static Container _container;
        public static Container Container
        {
            get
            {
                lock (_lock)
                {
                    if (_container != null)
                        return _container;
                    return new Container();
                }
            }
            set
            {
                _container = value;
            }
        }
    }
}

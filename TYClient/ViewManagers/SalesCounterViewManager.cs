using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TY.SPIMS.Client.ViewManagers
{
    public sealed class SalesCounterViewManager<T, T1>
    {
        #region Singleton
        private static object m_lock = new object();

        private static SalesCounterViewManager<T, T1> m_instance = new SalesCounterViewManager<T, T1>();
        public static SalesCounterViewManager<T, T1> Instance 
        {
            get
            {
                lock (m_lock)
                {
                    if (m_instance == null)
                        m_instance = new SalesCounterViewManager<T, T1>();
                    return m_instance;
                }
            }
        }
        #endregion

        public T Parent { get; set; }

        private List<T1> m_children = new List<T1>();
        public List<T1> Children 
        {
            get
            {
                if (m_children == null)
                    m_children = new List<T1>();
                return m_children;
            }
            set
            {
                m_children = value;
            }
        }

        public void AddItems(T1 child)
        {
            Children.Add(child);
        }

        public void ClearAllItems()
        {
            Children.Clear();
        }



    }
}

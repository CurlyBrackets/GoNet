using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.Utils
{
    class BinnedDictionary<K,V>
    {
        public List<V> this[K key]
        {
            get
            {
                return m_core[key];
            }
        }

        private Dictionary<K, List<V>> m_core;

        public BinnedDictionary()
        {
            m_core = new Dictionary<K, List<V>>();
        }

        public void Clear()
        {
            m_core.Clear();
        }

        public void Add(K key, V val)
        {
            if (!m_core.ContainsKey(key))
                m_core.Add(key, new List<V>());
            m_core[key].Add(val);
        }

        public bool ContainsKey(K key)
        {
            return m_core.ContainsKey(key);
        }
    }
}

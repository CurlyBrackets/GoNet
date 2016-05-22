using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.Utils
{
    class LookupList<Key, Value> where Key : IComparable
    {
        public KeyValuePair<Key, Value> this[int index]
        {
            get
            {
                return m_core[index];
            }
            set
            {
                m_core[index] = value;
            }
        }

        public Value this[Key k]
        {
            get
            {
                foreach (var kvp in m_core)
                    if (kvp.Key.CompareTo(k) == 0)
                        return kvp.Value;
                throw new KeyNotFoundException();
            }
            set
            {
                for(int i = 0; i < m_core.Count; i++)
                {
                    if (m_core[i].Key.CompareTo(k) == 0)
                    {
                        m_core[i] = new KeyValuePair<Key, Value>(k, value);
                        return;
                    }
                }

                Add(k, value);
            }
        }

        public int RankOf(Key k)
        {
            for(int i = 0; i < m_core.Count; i++)
            {
                if (m_core[i].Key.CompareTo(k) == 0)
                    return i;
            }

            return -1;
        }

        private List<KeyValuePair<Key, Value>> m_core;

        public LookupList()
        {
            m_core = new List<KeyValuePair<Key, Value>>();
        }

        public void Add(Key k, Value v)
        {
            m_core.Add(new KeyValuePair<Key, Value>(k, v));
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    abstract class Node : Base
    {
        private List<Node> m_children;
        private bool m_container, m_indexDependent;
        private int m_limit;

        public Node Parent { get; private set; }

        public bool IsContainer
        {
            get
            {
                return m_container;
            }
        }

        public IReadOnlyList<Node> Children
        {
            get
            {
                return m_children;
            }
        }

        protected Node(bool container)
            : this(container, 0)
        {
        }

        protected Node(bool container, int limit)
        {
            m_container = container;
            m_children = new List<Node>(limit);
            m_limit = limit;
        }

        private Node() { }

        public void AddChild(Node n)
        {
            if (!m_container)
                throw new InvalidOperationException();
            if (m_limit != 0 && m_children.Count == m_limit)
                throw new Exception($"Node cannot have more than {m_limit} children");

            if (n.Parent != null)
                n.Parent.RemoveChild(n);
            n.Parent = this;
            m_children.Add(n);
        }

        public int NumChildren()
        {
            return m_children.Count;
        }

        protected void SetChild(Node n, int index)
        {
            if (!m_container)
                throw new InvalidOperationException();
            if (m_limit != 0 && index >= m_limit)
                throw new Exception($"Node cannot have more than {m_limit} children");

            if (!m_indexDependent)
                m_indexDependent = true;

            if (n != null)
            {
                if (n.Parent != null)
                    n.Parent.RemoveChild(n);
                n.Parent = this;
            }

            while (index >= m_children.Count)
                m_children.Add(null);

            if(m_children[index] != null)
                m_children[index].Parent = null;
            m_children[index] = n;                
        }

        public T GetChild<T>(int index) where T : Node
        {
            return Children[index] as T;
        }

        public Node GetChild(int index)
        {
            return Children[index];
        }

        public void RemoveChild(Node n)
        {
            if (!m_container)
                throw new InvalidOperationException();

            n.Parent = null;
            if (m_indexDependent)
            {
                int index = m_children.IndexOf(n);
                if (index != -1)
                    m_children[index] = null;
            }
            else
                m_children.Remove(n);
        }

        public void Replace(Node old, Node n)
        {
            if (!m_container)
                throw new InvalidOperationException();

            int index = m_children.IndexOf(old);
            if(index != -1)
            {
                old.Parent = null;
                n.Parent = this;
                m_children[index] = n;
            }
        }

        public IEnumerable<T> FilteredChildren<T>() where T : Node
        {
            return m_children.Where(n => n is T).Select(n => n as T);
        }
    }
}

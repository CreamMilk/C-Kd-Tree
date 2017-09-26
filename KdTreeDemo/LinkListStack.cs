using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KdTreeDemo
{
    public class Stack<T> : IEnumerable<T>
    {
        private Node first;
        private int number = 0;
        private class Node
        {
            public T Item { get; set; }
            public Node Next { get; set; }
        }
        public bool IsEmpty()
        {
            return first == null;
        }

        public void Push(T item)
        {
            Node oldFirst = first;
            first = new Node();
            first.Item = item;
            first.Next = oldFirst;
            number++;
        }

        public T Pop()
        {
            T item = first.Item;
            first = first.Next;
            number--;
            return item;
        }

        public int GetSize()
        {
            return number;
        }

        /// <summary>
        /// 删除最后一个元素
        /// </summary>
        public void DeleteLastNode()
        {
            Node node = first;
            Node nodePre = null;
            while (node.Next != null)
            {
                nodePre = node;
                node = node.Next;
            }
            nodePre.Next = null;
            number--;
        }

        /// <summary>
        /// 根据索引删除节点
        /// </summary>
        /// <param name="index"></param>
        public void Delete(int index)
        {
            if (index >= number)
            {
                throw new IndexOutOfRangeException();
            }
            int n = -1;
            Node node = new Node();
            node.Next = first;
            while (node.Next != null)
            {
                n++;
                if (n == index)
                {
                    node.Next = node.Next.Next;
                    break;
                }
                node = node.Next;

            }
        }

        private Node FindNode(T item)
        {
            Node node = new Node();
            node.Next = first;
            while (node.Next != null)
            {
                node = node.Next;
                if (node.Item.Equals(item))
                {
                    return node;
                }
            }
            return null;
        }
        public void RemoveAfter(T item)
        {
            Node node = FindNode(item);
            if (node != null)
            {
                node.Next = null;
            }
        }

        public void InsertAfter(T first, T second)
        {
            Node firstNode = FindNode(first);
            if (firstNode != null && second != null)
            {
                Node secondNode = new Node();
                secondNode.Item = second;
                secondNode.Next = firstNode.Next;
                firstNode.Next = secondNode;
                number++;
            }
        }

        /// <summary>
        /// 删除所有为T的项，并更新堆栈数量
        /// </summary>
        /// <param name="item"></param>
        public void Remove(T item)
        {
            Node node = new Node();
            if (first.Item.Equals(item))
            {
                first = first.Next;
                number--;
            }
            node = first;
            while (node.Next != null)
            {
                if (node.Next.Item.Equals(item))
                {
                    node.Next = node.Next.Next;
                    number--;
                }
                node = node.Next;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new LinkListStackIEumertor(this.first);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class LinkListStackIEumertor : IEnumerator<T>
        {
            public LinkListStackIEumertor()
            {

            }
            public LinkListStackIEumertor(Node first)
            {
                this.first = first;
                currentNode = this.first;
            }
            private readonly Node first;
            private Node currentNode;
            private T currentItem;
            public T Current
            {
                get { return currentItem; }
            }

            public void Dispose()
            {
            }

            object System.Collections.IEnumerator.Current
            {
                get { return currentItem; }
            }

            public bool MoveNext()
            {
                if (currentNode != null)
                {
                    currentItem = currentNode.Item;
                    currentNode = currentNode.Next;
                    return true;
                }
                else
                {
                    return false;
                }

            }

            public void Reset()
            {
                currentNode.Next = first;
            }
        }
    }
}

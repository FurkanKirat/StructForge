using System;
using System.Collections;
using System.Collections.Generic;

namespace StructForge.Collections
{
    /// <summary>
    /// Represents a doubly linked list data structure.
    /// Supports adding/removing elements at both ends, insertion, deletion, and enumeration.
    /// </summary>
    /// <typeparam name="T">Type of elements stored in the list.</typeparam>
    public class SfLinkedList<T> : ISfLinkedList<T>
    {
        /// <summary>Number of elements in the list.</summary>
        public int Count { get; private set; }

        /// <summary>Returns true if the list has no elements.</summary>
        public bool IsEmpty => Count == 0;

        private SfLinkedListNode<T> _head;
        private SfLinkedListNode<T> _tail;

        /// <summary>Gets the first node in the list.</summary>
        public SfLinkedListNode<T> First => _head;

        /// <summary>Gets the last node in the list.</summary>
        public SfLinkedListNode<T> Last => _tail;

        /// <summary>Initializes an empty linked list.</summary>
        public SfLinkedList()
        {
            _head = null;
            _tail = null;
            Count = 0;
        }

        /// <summary>Initializes a linked list from an enumerable collection.</summary>
        /// <param name="enumerable">Collection of items to add to the list.</param>
        public SfLinkedList(IEnumerable<T> enumerable)
        {
            ArgumentNullException.ThrowIfNull(enumerable);
            SfLinkedListNode<T> current = null;
            foreach (var item in enumerable)
            {
                if (Count == 0)
                {
                    _head = new SfLinkedListNode<T>(item, this);
                    current = _head;
                }
                else
                {
                    SfLinkedListNode<T> prev = current;
                    current = new SfLinkedListNode<T>(item, this);
                    prev!.Next = current;
                    current.Prev = prev;
                }
                Count++;
            }
            _tail = current;
        }

        /// <summary>Returns an enumerator that iterates through the list from head to tail.</summary>
        public IEnumerator<T> GetEnumerator()
        {
            SfLinkedListNode<T> current = _head;
            while (current != null)
            {
                yield return current.Value;
                current = current.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>Executes the specified action for each element in forward order.</summary>
        public void ForEach(Action<T> action)
        {
            SfLinkedListNode<T> current = _head;
            while (current != null)
            {
                action(current.Value);
                current = current.Next;
            }
        }

        /// <summary>Executes the specified action for each element in backward order.</summary>
        public void ForEachBackward(Action<T> action)
        {
            SfLinkedListNode<T> current = _tail;
            while (current != null)
            {
                action(current.Value);
                current = current.Prev;
            }
        }

        /// <summary>Checks if the list contains a specific item using the default comparer.</summary>
        public bool Contains(T item) => Contains(item, EqualityComparer<T>.Default);

        /// <summary>Checks if the list contains a specific item using a custom comparer.</summary>
        public bool Contains(T item, IEqualityComparer<T> comparer)
        {
            SfLinkedListNode<T> current = _head;
            while (current != null)
            {
                if (comparer.Equals(current.Value, item))
                    return true;
                current = current.Next;
            }
            return false;
        }

        /// <summary>Removes all elements from the list.</summary>
        public void Clear()
        {
            SfLinkedListNode<T> current = _head;
            while (current != null)
            {
                SfLinkedListNode<T> nextNode = current.Next;
                current.Clear();
                current = nextNode;
            }
            _head = null;
            _tail = null;
            Count = 0;
        }

        /// <summary>Copies the elements of the list to an array starting at the specified index.</summary>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (arrayIndex + Count > array.Length) throw new ArgumentException("Destination array is not large enough.");

            SfLinkedListNode<T> current = _head;
            while (current != null)
            {
                array[arrayIndex++] = current.Value;
                current = current.Next;
            }
        }
        

        /// <summary>Adds an item at the beginning of the list.</summary>
        public void AddFirst(T item)
        {
            var newNode = new SfLinkedListNode<T>(item, this);
            if (Count == 0)
            {
                _head = _tail = newNode;
            }
            else
            {
                newNode.Next = _head;
                _head.Prev = newNode;
                _head = newNode;
            }
            Count++;
        }

        /// <summary>Adds an item at the end of the list.</summary>
        public void AddLast(T item)
        {
            var newNode = new SfLinkedListNode<T>(item, this);
            if (Count == 0)
            {
                _head = _tail = newNode;
            }
            else
            {
                newNode.Prev = _tail;
                _tail.Next = newNode;
                _tail = newNode;
            }
            Count++;
        }

        /// <summary>Removes and returns the first element of the list.</summary>
        /// <exception cref="InvalidOperationException">Thrown if the list is empty.</exception>
        public T RemoveFirst()
        {
            if (Count == 0) throw new InvalidOperationException("Collection is empty");

            T value = _head.Value;
            RemoveNode(_head);
            return value;
        }

        /// <summary>Removes and returns the last element of the list.</summary>
        /// <exception cref="InvalidOperationException">Thrown if the list is empty.</exception>
        public T RemoveLast()
        {
            if (Count == 0) throw new InvalidOperationException("Collection is empty");

            T value = _tail.Value;
            RemoveNode(_tail);
            return value;
        }

        /// <summary>Inserts a new item after the specified node.</summary>
        public void InsertAfter(SfLinkedListNode<T> node, T item)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (node.List != this) throw new ArgumentException("Node does not belong to this list.");

            if (node.Next == null)
            {
                AddLast(item);
                return;
            }

            var newNode = new SfLinkedListNode<T>(item, this)
            {
                Prev = node,
                Next = node.Next
            };
            node.Next.Prev = newNode;
            node.Next = newNode;
            Count++;
        }

        /// <summary>Inserts a new item before the specified node.</summary>
        public void InsertBefore(SfLinkedListNode<T> node, T item)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (node.List != this) throw new ArgumentException("Node does not belong to this list.");

            if (node.Prev == null)
            {
                AddFirst(item);
                return;
            }

            var newNode = new SfLinkedListNode<T>(item, this)
            {
                Prev = node.Prev,
                Next = node
            };
            node.Prev.Next = newNode;
            node.Prev = newNode;
            Count++;
        }

        /// <summary>Removes the first occurrence of the specified item.</summary>
        public bool Remove(T item)
        {
            SfLinkedListNode<T> current = _head;
            while (current != null)
            {
                if (EqualityComparer<T>.Default.Equals(current.Value, item))
                {
                    RemoveNode(current);
                    return true;
                }
                current = current.Next;
            }
            return false;
        }

        /// <summary>Reverses the list in-place.</summary>
        public void Reverse()
        {
            SfLinkedListNode<T> current = _head;
            SfLinkedListNode<T> temp = null;

            while (current != null)
            {
                temp = current.Prev;
                current.Prev = current.Next;
                current.Next = temp;
                current = current.Prev;
            }

            (_head, _tail) = (_tail, _head);
        }

        /// <summary>Finds the first node containing the specified item.</summary>
        public SfLinkedListNode<T> Find(T item)
        {
            SfLinkedListNode<T> current = _head;
            while (current != null)
            {
                if (EqualityComparer<T>.Default.Equals(current.Value, item))
                    return current;
                current = current.Next;
            }
            return null;
        }

        /// <summary>Finds the last node containing the specified item.</summary>
        public SfLinkedListNode<T> FindLast(T item)
        {
            SfLinkedListNode<T> current = _tail;
            while (current != null)
            {
                if (EqualityComparer<T>.Default.Equals(current.Value, item))
                    return current;
                current = current.Prev;
            }
            return null;
        }

        private void RemoveNode(SfLinkedListNode<T> node)
        {
            if (node.Prev == null) _head = node.Next;
            else node.Prev.Next = node.Next;

            if (node.Next == null) _tail = node.Prev;
            else node.Next.Prev = node.Prev;

            node.Clear();
            Count--;
        }
    }

    /// <summary>Represents a node in a doubly linked list.</summary>
    /// <typeparam name="T">Type of value stored in the node.</typeparam>
    public class SfLinkedListNode<T>
    {
        /// <summary>The value stored in the node.</summary>
        public T Value;

        /// <summary>Reference to the next node in the list.</summary>
        public SfLinkedListNode<T> Next { get; internal set; }

        /// <summary>Reference to the previous node in the list.</summary>
        public SfLinkedListNode<T> Prev { get; internal set; }

        /// <summary>The list that owns this node.</summary>
        public SfLinkedList<T> List { get; private set; }

        /// <summary>Provides a reference to the node's value.</summary>
        public ref T ValueRef => ref Value;

        internal SfLinkedListNode(T value, SfLinkedList<T> list)
        {
            Value = value;
            List = list;
        }

        internal void Clear()
        {
            Next = null;
            Prev = null;
            List = null;
        }
    }
}

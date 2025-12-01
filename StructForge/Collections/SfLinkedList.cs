using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using StructForge.Comparers;
using StructForge.Helpers;

namespace StructForge.Collections
{
    /// <summary>
    /// Represents a doubly linked list data structure.
    /// Supports adding/removing elements at both ends, insertion, deletion, and enumeration.
    /// </summary>
    /// <typeparam name="T">Type of elements stored in the list.</typeparam>
    public sealed class SfLinkedList<T> : ISfDataStructure<T>
    {
        private int _count;

        /// <inheritdoc/>
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _count;
        }

        /// <inheritdoc/>
        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _count == 0;
        }

        private SfLinkedListNode<T> _head;
        private SfLinkedListNode<T> _tail;

        /// <summary>Gets the first node in the list.</summary>
        public SfLinkedListNode<T> First
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _head;
        }

        /// <summary>Gets the last node in the list.</summary>
        public SfLinkedListNode<T> Last
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _tail;
        }

        /// <summary>Initializes an empty linked list.</summary>
        public SfLinkedList()
        {
            _head = null;
            _tail = null;
            _count = 0;
        }

        /// <summary>Initializes a linked list from an enumerable collection.</summary>
        /// <param name="enumerable">Collection of items to add to the list.</param>
        public SfLinkedList(IEnumerable<T> enumerable)
        {
            if (enumerable is null)
                SfThrowHelper.ThrowArgumentNull(nameof(enumerable));
            
            SfLinkedListNode<T> current = null;
            foreach (var item in enumerable)
            {
                if (IsEmpty)
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
                _count++;
            }
            _tail = current;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SfLinkListEnumerator GetEnumerator() => new(this);
        /// <inheritdoc/>
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        public void ForEach(Action<T> action)
        {
            SfLinkedListNode<T> current = _head;
            while (current is not null)
            {
                action(current.Value);
                current = current.Next;
            }
        }

        /// <summary>Executes the specified action for each element in backward order.</summary>
        public void ForEachBackward(Action<T> action)
        {
            SfLinkedListNode<T> current = _tail;
            while (current is not null)
            {
                action(current.Value);
                current = current.Prev;
            }
        }

        /// <inheritdoc/>
        public bool Contains(T item) => Contains(item, SfEqualityComparers<T>.Default);

        /// <inheritdoc/>
        public bool Contains(T item, IEqualityComparer<T> comparer)
        {
            SfLinkedListNode<T> current = _head;
            while (current is not null)
            {
                if (comparer.Equals(current.Value, item))
                    return true;
                current = current.Next;
            }
            return false;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            SfLinkedListNode<T> current = _head;
            while (current is not null)
            {
                SfLinkedListNode<T> nextNode = current.Next;
                current.Clear();
                current = nextNode;
            }
            _head = null;
            _tail = null;
            _count = 0;
        }

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array is null)
                SfThrowHelper.ThrowArgumentNull(nameof(array));
            if (arrayIndex < 0)
                SfThrowHelper.ThrowArgumentOutOfRange(nameof(arrayIndex));
            if (arrayIndex + _count > array.Length) 
                SfThrowHelper.ThrowArgument("Destination array is not large enough.");
            
            SfLinkedListNode<T> current = _head;
            while (current is not null)
            {
                array[arrayIndex++] = current.Value;
                current = current.Next;
            }
        }

        /// <inheritdoc/>
        public T[] ToArray()
        {
            T[] arr = new T[_count];
            CopyTo(arr, 0);
            return arr;
        }
        
        /// <summary>
        /// Adds an item at the beginning of the list.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void AddFirst(T item)
        {
            var newNode = new SfLinkedListNode<T>(item, this);
            if (IsEmpty)
            {
                _head = _tail = newNode;
            }
            else
            {
                newNode.Next = _head;
                _head.Prev = newNode;
                _head = newNode;
            }
            _count++;
        }

        /// <summary>
        /// Adds an item at the end of the list.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void AddLast(T item)
        {
            var newNode = new SfLinkedListNode<T>(item, this);
            if (IsEmpty)
            {
                _head = _tail = newNode;
            }
            else
            {
                newNode.Prev = _tail;
                _tail.Next = newNode;
                _tail = newNode;
            }
            _count++;
        }

        /// <summary>
        /// Removes and returns the first element of the list.
        /// </summary>
        /// <returns>The removed element.</returns>
        public T RemoveFirst()
        {
            if (IsEmpty) 
                SfThrowHelper.ThrowInvalidOperation("Collection is empty");

            T value = _head.Value;
            RemoveNode(_head);
            return value;
        }

        /// <summary>
        /// Removes and returns the last element of the list.
        /// </summary>
        /// <returns>The removed element.</returns>
        public T RemoveLast()
        {
            if (IsEmpty) 
                SfThrowHelper.ThrowInvalidOperation("Collection is empty");

            T value = _tail.Value;
            RemoveNode(_tail);
            return value;
        }

        /// <summary>
        /// Inserts an item after the specified node.
        /// </summary>
        /// <param name="node">The node after which to insert.</param>
        /// <param name="item">The item to insert.</param>
        public void InsertAfter(SfLinkedListNode<T> node, T item)
        {
            if (node is null) 
                SfThrowHelper.ThrowArgumentNull(nameof(node));
            
            if (node.List != this)
                SfThrowHelper.ThrowArgument("Node does not belong to this list.");

            if (node.Next is null)
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
            _count++;
        }

        /// <summary>
        /// Inserts an item before the specified node.
        /// </summary>
        /// <param name="node">The node before which to insert.</param>
        /// <param name="item">The item to insert.</param>
        public void InsertBefore(SfLinkedListNode<T> node, T item)
        {
            if (node is null) 
                SfThrowHelper.ThrowArgumentNull(nameof(node));
            
            if (node.List != this)
                SfThrowHelper.ThrowArgument("Node does not belong to this list.");

            if (node.Prev is null)
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
            _count++;
        }

        /// <summary>
        /// Removes the first occurrence of the specified item from the list.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>True if the item was found and removed; otherwise, false.</returns>
        public bool Remove(T item) => Remove(item, SfEqualityComparers<T>.Default);
        
        /// <summary>
        /// Removes the first occurrence of the specified item from the list.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <param name="comparer">Comparer</param>
        /// <returns>True if the item was found and removed; otherwise, false.</returns>
        public bool Remove(T item, IEqualityComparer<T> comparer)
        {
            SfLinkedListNode<T> current = _head;
            while (current is not null)
            {
                if (comparer.Equals(current.Value, item))
                {
                    RemoveNode(current);
                    return true;
                }
                current = current.Next;
            }
            return false;
        }

        /// <summary>
        /// Reverses the order of the elements in the list.
        /// </summary>
        public void Reverse()
        {
            SfLinkedListNode<T> current = _head;
            SfLinkedListNode<T> temp = null;

            while (current is not null)
            {
                temp = current.Prev;
                current.Prev = current.Next;
                current.Next = temp;
                current = current.Prev;
            }

            (_head, _tail) = (_tail, _head);
        }

        /// <summary>
        /// Finds the first node that contains the specified item.
        /// </summary>
        /// <param name="item">The item to find.</param>
        /// <returns>The first node containing the item, or null if not found.</returns>
        public SfLinkedListNode<T> Find(T item)
        {
            SfLinkedListNode<T> current = _head;
            while (current is not null)
            {
                if (SfEqualityComparers<T>.Default.Equals(current.Value, item))
                    return current;
                current = current.Next;
            }
            return null;
        }

        /// <summary>
        /// Finds the last node that contains the specified item.
        /// </summary>
        /// <param name="item">The item to find.</param>
        /// <returns>The last node containing the item, or null if not found.</returns>
        public SfLinkedListNode<T> FindLast(T item)
        {
            SfLinkedListNode<T> current = _tail;
            while (current is not null)
            {
                if (SfEqualityComparers<T>.Default.Equals(current.Value, item))
                    return current;
                current = current.Prev;
            }
            return null;
        }

        private void RemoveNode(SfLinkedListNode<T> node)
        {
            if (node.Prev is null) _head = node.Next;
            else node.Prev.Next = node.Next;

            if (node.Next is null) _tail = node.Prev;
            else node.Next.Prev = node.Prev;

            node.Clear();
            _count--;
        }
        
        public struct SfLinkListEnumerator : IEnumerator<T>
        {
            private SfLinkedListNode<T> _current;
            private readonly SfLinkedListNode<T> _head;
            private bool _started;

            public ref T Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => ref _current.Value;
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal SfLinkListEnumerator(SfLinkedList<T> list)
            {
                _current = null;
                _head = list.First;
                _started = false;
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                if (!_started)
                {
                    _current = _head;
                    _started = true;
                }
                else
                {
                    _current = _current?.Next;
                }
                return _current is not null;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Reset()
            {
                _current = null;
                _started = false;
            }

            T IEnumerator<T>.Current => _current.Value;

            object IEnumerator.Current => _current.Value;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Dispose() {}
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

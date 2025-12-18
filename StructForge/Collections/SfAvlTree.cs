using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using StructForge.Comparers;
using StructForge.Helpers;

namespace StructForge.Collections
{
    /// <summary>
    /// Represents a self-balancing binary search tree (AVL Tree) that maintains
    /// O(log n) complexity for insertion, deletion, and lookup operations.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the tree.</typeparam>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    [DebuggerTypeProxy(typeof(SfTreeDebugView<>))]
    public sealed class SfAvlTree<T> : ISfDataStructure<T>, ICollection<T>
    {
        private int _count;
        /// <inheritdoc cref="ICollection{T}.Count" />
        
        public int Count 
        { 
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _count; 
        }

        /// <inheritdoc/>
        public bool IsReadOnly
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => false;
        }

        /// <inheritdoc/>
        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _count == 0; 
        }

        /// <summary>
        /// Gets the height of the tree.
        /// </summary>
        public int Height => GetHeight(_root);
        
        private SfAvlTreeNode<T> _root;
        private readonly IComparer<T> _comparer;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SfAvlTree{T}"/> class using the specified comparer.
        /// </summary>
        /// <param name="comparer">The comparer used to order elements. If null, the default comparer is used.</param>
        public SfAvlTree(IComparer<T> comparer = null)
        {
            _comparer = comparer ?? SfComparers<T>.DefaultComparer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SfAvlTree{T}"/> class that contains elements
        /// copied from the specified collection and uses the specified comparer.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new tree.</param>
        /// <param name="comparer">The comparer used to order elements. If null, the default comparer is used.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collection"/> is null.</exception>
        public SfAvlTree(IEnumerable<T> collection, IComparer<T> comparer = null)
        {
            if (collection is null)
                SfThrowHelper.ThrowArgumentNull(nameof(collection));
            
            _comparer = comparer ?? SfComparers<T>.DefaultComparer;
            
            T[] arr = collection.ToArray();
            if (arr.Length == 0)
                return;
            
            foreach (var item in arr)
                TryAdd(item);
        }

        /// <summary>
        /// Returns an enumerator for iterating over the collection.
        /// Can be used by <c>foreach</c> loops.
        /// </summary>
        /// <returns>An enumerator for the collection.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SfAvlTreeInOrderEnumerator GetEnumerator() => new(this);
        /// <inheritdoc/>
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        /// <inheritdoc cref="ISfDataStructure{T}.Contains(T)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T item)
        {
            SfAvlTreeNode<T> current = _root;
            while (current is not null)
            {
                int cmp = _comparer.Compare(item, current.Value);
                if (cmp == 0) return true;
                current = cmp < 0 ? current.Left : current.Right;
            }
            return false;
        }

        /// <inheritdoc/>
        public bool Remove(T item)
        {
            int before = _count;
            _root = Delete(_root, item);
            return _count < before;
        }

        private SfAvlTreeNode<T> Delete(SfAvlTreeNode<T> node, T item, bool decrementCount = true)
        {
            if (node is null)
                return null;

            int cmp = _comparer.Compare(item, node.Value);
            switch (cmp)
            {
                case < 0:
                    node.Left = Delete(node.Left, item);
                    break;
                case > 0:
                    node.Right = Delete(node.Right, item);
                    break;
                default:
                {
                    if (decrementCount) 
                        _count--;

                    // Node with one or zero children
                    if (node.Left is null)
                        return node.Right;
                    if (node.Right is null)
                        return node.Left;

                    // Node with two children → replace with in-order successor
                    SfAvlTreeNode<T> successor = FindLeftmost(node.Right);
                    node.Value = successor.Value;
                    node.Right = Delete(node.Right, successor.Value, false);
                    break;
                }
            }

            node.Height = 1 + Math.Max(GetHeight(node.Left), GetHeight(node.Right));
            return Rebalance(node);
        }

        /// <inheritdoc/>
        public bool Contains(T item, IEqualityComparer<T> comparer)
        {
            foreach (T element in this)
            {
                if (comparer.Equals(item, element)) return true;
            }
            return false;
        }

        /// <inheritdoc/>
        public void Add(T item)
        {
            if (!TryAdd(item))
                SfThrowHelper.ThrowInvalidOperation("Duplicate item");
        }

        /// <inheritdoc cref="ICollection{T}.Clear" />
        public void Clear()
        {
            _root = null;
            _count = 0;
        }

        /// <inheritdoc cref="ISfDataStructure{T}.CopyTo" />
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array is null)
                SfThrowHelper.ThrowArgumentNull(nameof(array));
            if (arrayIndex < 0)
                SfThrowHelper.ThrowArgumentOutOfRange(nameof(arrayIndex));
            if (arrayIndex + _count > array.Length) 
                SfThrowHelper.ThrowArgument("Destination array is not large enough.");
            
            foreach (T item in InOrder())
                array[arrayIndex++] = item;
        }

        /// <inheritdoc/>
        public T[] ToArray()
        {
            T[] arr = new T[_count];
            CopyTo(arr, 0);
            return arr;
        }

        /// <summary>
        /// Attempts to add an item to the tree.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>True if added; false if duplicate or insertion failed.</returns>
        public bool TryAdd(T item)
        {
            bool added = false;
            _root = TryAdd(_root, item, ref added);
            if (added)
                _count++;
            return added;
        }

        private SfAvlTreeNode<T> TryAdd(SfAvlTreeNode<T> node, T item, ref bool added)
        {
            if (node is null)
            {
                added = true;
                return new SfAvlTreeNode<T>(item);
            }

            int cmp = _comparer.Compare(item, node.Value);
            switch (cmp)
            {
                case 0:
                    return node;
                case < 0:
                    node.Left = TryAdd(node.Left, item, ref added);
                    break;
                default:
                    node.Right = TryAdd(node.Right, item, ref added);
                    break;
            }

            node.Height = Math.Max(GetHeight(node.Left), GetHeight(node.Right)) + 1;
            return Rebalance(node);
        }

        /// <summary>
        /// Balances the AVL tree at the given node by performing appropriate rotations.
        /// </summary>
        private static SfAvlTreeNode<T> Rebalance(SfAvlTreeNode<T> node)
        {
            int balance = GetBalance(node);

            switch (balance)
            {
                case > 1:
                    if (GetBalance(node.Left) < 0)
                        node.Left = LeftRotate(node.Left); // LR case
                    return RightRotate(node); // LL case

                case < -1:
                    if (GetBalance(node.Right) > 0)
                        node.Right = RightRotate(node.Right); // RL case
                    return LeftRotate(node); // RR case

                default:
                    return node;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static SfAvlTreeNode<T> RightRotate(SfAvlTreeNode<T> pivot)
        {
            var newRoot = pivot.Left;
            var transferSubtree = newRoot.Right;

            newRoot.Right = pivot;
            pivot.Left = transferSubtree;

            pivot.Height = Math.Max(GetHeight(pivot.Left), GetHeight(pivot.Right)) + 1;
            newRoot.Height = Math.Max(GetHeight(newRoot.Left), GetHeight(newRoot.Right)) + 1;

            return newRoot;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static SfAvlTreeNode<T> LeftRotate(SfAvlTreeNode<T> pivot)
        {
            var newRoot = pivot.Right;
            var transferSubtree = newRoot.Left;
            
            newRoot.Left = pivot;
            pivot.Right = transferSubtree;
            
            pivot.Height = Math.Max(GetHeight(pivot.Left), GetHeight(pivot.Right)) + 1;
            newRoot.Height = Math.Max(GetHeight(newRoot.Left), GetHeight(newRoot.Right)) + 1;

            return newRoot;
        }

        /// <summary>
        /// Returns the minimum value in the tree.
        /// </summary>
        /// <returns>The minimum item.</returns>
        /// <exception cref="InvalidOperationException">Thrown if tree is empty.</exception>
        public T FindMin() => FindLeftmost(_root).Value;

        /// <summary>
        /// Returns the maximum value in the tree.
        /// </summary>
        /// <returns>The maximum item.</returns>
        /// <exception cref="InvalidOperationException">Thrown if tree is empty.</exception>
        public T FindMax() => FindRightmost(_root).Value;

        /// <summary>
        /// Returns a low-allocation enumerable that traverses the tree in <b>In-Order</b> (Left-Root-Right).
        /// </summary>
        /// <remarks>
        /// <para>
        /// This traversal yields elements in <b>sorted (ascending) order</b>.
        /// </para>
        /// <para>
        /// Use this when you need to iterate through the data from smallest to largest, 
        /// similar to a sorted list. This is the default behavior of the tree's enumerator.
        /// </para>
        /// </remarks>
        /// <returns>A struct wrapper for allocation-free iteration.</returns>
        public SfInOrderTraversal InOrder() => new(this);

        /// <summary>
        /// Returns a low-allocation enumerable that traverses the tree in <b>Pre-Order</b> (Root-Left-Right).
        /// </summary>
        /// <remarks>
        /// <para>
        /// This traversal visits the parent before its children.
        /// </para>
        /// <para>
        /// <b>Use Cases:</b>
        /// <list type="bullet">
        /// <item><description>Cloning/Copying the tree structure efficiently.</description></item>
        /// <item><description>Serialization (saving the tree to reconstruct it later).</description></item>
        /// <item><description>Prefix expression evaluation.</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <returns>A struct wrapper for allocation-free iteration.</returns>
        public SfPreOrderTraversal PreOrder() => new(this);

        /// <summary>
        /// Returns a low-allocation enumerable that traverses the tree in <b>Post-Order</b> (Left-Right-Root).
        /// </summary>
        /// <remarks>
        /// <para>
        /// This traversal visits children before their parent.
        /// </para>
        /// <para>
        /// <b>Use Cases:</b>
        /// <list type="bullet">
        /// <item><description>Deleting the tree (freeing children before the parent).</description></item>
        /// <item><description>Dependency resolution (processing leaves first).</description></item>
        /// <item><description>Evaluating mathematical expressions (postfix/Reverse Polish Notation).</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <returns>A struct wrapper for allocation-free iteration.</returns>
        public SfPostOrderTraversal PostOrder() => new(this);

        /// <summary>
        /// Tries to retrieve the actual stored value that is equal to the specified value
        /// based on the set's equality or comparison logic.
        /// </summary>
        /// <param name="equalValue">The value to search for in the set.</param>
        /// <param name="actualValue">The actual stored value if found; otherwise, the default value of T.</param>
        /// <returns>True if a matching value is found; otherwise, false.</returns>
        public bool TryGetValue(T equalValue, out T actualValue)
        {
            var node = _root;
            while (node is not null)
            {
                switch (_comparer.Compare(node.Value, equalValue))
                {
                    case > 0:
                        node = node.Left;
                        break;
                    case < 0:
                        node = node.Right;
                        break;
                    default:
                        actualValue = node.Value;
                        return true;
                }
            }

            actualValue = default;
            return false;
        }

        /// <inheritdoc/>
        public void ForEach(Action<T> action)
        {
            foreach (T item in this)
                action(item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetHeight(SfAvlTreeNode<T> node) => node?.Height ?? 0;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetBalance(SfAvlTreeNode<T> node) => GetHeight(node.Left) - GetHeight(node.Right);

        private static SfAvlTreeNode<T> FindLeftmost(SfAvlTreeNode<T> node)
        {
            if (node is null)
                SfThrowHelper.ThrowInvalidOperation("Tree is empty.");
                
            while (node.Left is not null) node = node.Left;
            return node;
        }

        private static SfAvlTreeNode<T> FindRightmost(SfAvlTreeNode<T> node)
        {
            if (node is null)
                SfThrowHelper.ThrowInvalidOperation("Tree is empty.");
            
            while (node.Right is not null) node = node.Right;
            return node;
        }

        #region Enumerators
        
        /// <inheritdoc />
        public readonly struct SfPreOrderTraversal: IEnumerable<T>
        {
            private readonly SfAvlTree<T> _tree;
            
            /// <summary>
            /// Creates a pre-order travarsel for given tree 
            /// </summary>
            /// <param name="tree">The given tree</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public SfPreOrderTraversal(SfAvlTree<T> tree) => _tree = tree;

            /// <summary>
            /// Returns an enumerator for iterating over the collection.
            /// Can be used by <c>foreach</c> loops.
            /// </summary>
            /// <returns>An enumerator for the collection.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public SfAvlTreePreOrderEnumerator GetEnumerator() => new(_tree);
            IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        
        /// <inheritdoc />
        public readonly struct SfInOrderTraversal : IEnumerable<T>
        {
            private readonly SfAvlTree<T> _tree;
            
            /// <summary>
            /// Creates an in-order travarsel for given tree 
            /// </summary>
            /// <param name="tree">The given tree</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public SfInOrderTraversal(SfAvlTree<T> tree) => _tree = tree;

            /// <summary>
            /// Returns an enumerator for iterating over the collection.
            /// Can be used by <c>foreach</c> loops.
            /// </summary>
            /// <returns>An enumerator for the collection.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public SfAvlTreeInOrderEnumerator GetEnumerator() => new(_tree);
            IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        
        /// <inheritdoc />
        public readonly struct SfPostOrderTraversal: IEnumerable<T>
        {
            private readonly SfAvlTree<T> _tree;
            
            /// <summary>
            /// Creates a post-order travarsel for given tree 
            /// </summary>
            /// <param name="tree">The given tree</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public SfPostOrderTraversal(SfAvlTree<T> tree) => _tree = tree;

            /// <summary>
            /// Returns an enumerator for iterating over the collection.
            /// Can be used by <c>foreach</c> loops.
            /// </summary>
            /// <returns>An enumerator for the collection.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public SfAvlTreePostOrderEnumerator GetEnumerator() => new(_tree);
            IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        /// <inheritdoc />
        public struct SfAvlTreePreOrderEnumerator : IEnumerator<T>
        {
            private readonly SfAvlTreeNode<T> _root;
            private readonly SfStack<SfAvlTreeNode<T>> _stack;
            
            private SfAvlTreeNode<T> _current;
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal SfAvlTreePreOrderEnumerator(SfAvlTree<T> tree)
            {
                _current = null;
                int capacity = Math.Max(16, tree.Height + 4);
                _stack = new SfStack<SfAvlTreeNode<T>>(capacity);
                _root = tree._root;
                _stack.Push(_root);
            }
            
            /// <inheritdoc />
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                if (_stack.IsEmpty)
                    return false;
                
                _current = _stack.Pop();
                
                if (_current.Right is not null)
                    _stack.Push(_current.Right);
                
                if (_current.Left is not null)
                    _stack.Push(_current.Left);
                
                return true;
            }

            /// <inheritdoc />
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Reset()
            {
                _stack.Clear();
                _stack.Push(_root);
                _current = null;
            }

            /// <summary>
            /// Gives the current element's reference
            /// </summary>
            public ref T Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => ref _current.Value;
            }
            
            T IEnumerator<T>.Current => _current.Value;
            object IEnumerator.Current => _current.Value;

            /// <inheritdoc />
            public void Dispose() { }
        }
        
        /// <inheritdoc />
        public struct SfAvlTreeInOrderEnumerator : IEnumerator<T>
        {
            private readonly SfAvlTreeNode<T> _root;
            private readonly SfStack<SfAvlTreeNode<T>> _stack;
            
            private SfAvlTreeNode<T> _current;
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal SfAvlTreeInOrderEnumerator(SfAvlTree<T> tree)
            {
                _root = tree._root;
                _current = null;
                
                int capacity = Math.Max(16, tree.Height + 4);
                _stack = new SfStack<SfAvlTreeNode<T>>(capacity);

                PreparePath(_root);
            }

            /// <inheritdoc />
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                if (_stack.IsEmpty)
                    return false;
                
                _current =  _stack.Pop();
                if (_current.Right != null)
                {
                    PreparePath(_current.Right);
                }

                return true;
            }


            /// <inheritdoc />
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Reset()
            {
                _stack.Clear();
                _current = null;
                PreparePath(_root);
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void PreparePath(SfAvlTreeNode<T> node)
            {
                while (node != null)
                {
                    _stack.Push(node);
                    node = node.Left;
                }
            }

            /// <summary>
            /// Gives the current element's reference
            /// </summary>
            public ref T Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => ref _current.Value;
            }
            
            T IEnumerator<T>.Current => _current.Value;
            object IEnumerator.Current => _current.Value;

            /// <inheritdoc />
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Dispose() { }
        }

        /// <inheritdoc />
        public struct SfAvlTreePostOrderEnumerator : IEnumerator<T>
        {
            private readonly SfAvlTreeNode<T> _root;
            private readonly SfStack<SfAvlTreeNode<T>> _stack;
            
            private SfAvlTreeNode<T> _current;
            private SfAvlTreeNode<T> _lastVisitedNode;
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal SfAvlTreePostOrderEnumerator(SfAvlTree<T> tree)
            {
                _root = tree._root;
                _current = null;
                _lastVisitedNode = null;
                
                int capacity = Math.Max(16, tree.Height + 4);
                _stack = new SfStack<SfAvlTreeNode<T>>(capacity);

                PreparePath(_root);
            }

            /// <inheritdoc />
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                if (_stack.IsEmpty)
                    return false;
                
                while (!_stack.IsEmpty)
                {
                    SfAvlTreeNode<T> peekNode = _stack.Peek();
                    
                    if (peekNode.Right != null && peekNode.Right != _lastVisitedNode)
                    {
                        PreparePath(peekNode.Right);
                    }
                    else
                    {
                        _current = _stack.Pop();
                        _lastVisitedNode = _current;
                        return true;
                    }
                }

                return false;
            }


            /// <inheritdoc />
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Reset()
            {
                _stack.Clear();
                _current = null;
                _lastVisitedNode = null;
                PreparePath(_root);
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void PreparePath(SfAvlTreeNode<T> node)
            {
                while (node != null)
                {
                    _stack.Push(node);
                    node = node.Left;
                }
            }

            /// <summary>
            /// Gives the current element's reference
            /// </summary>
            public ref T Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => ref _current.Value;
            }
            
            T IEnumerator<T>.Current => _current.Value;
            object IEnumerator.Current => _current.Value;

            /// <inheritdoc />
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Dispose() { }
        }

        #endregion
        
        private string DebuggerDisplay => $"SfAvlTree<{typeof(T).Name}> (Count = {Count})";
    }
    
    /// <summary>
    /// Represents a single node in the <see cref="SfAvlTree{T}"/>.
    /// </summary>
    [DebuggerDisplay("{Value} H={Height}")]
    public sealed class SfAvlTreeNode<T>
    {
        internal T Value;
        internal SfAvlTreeNode<T> Left;
        internal SfAvlTreeNode<T> Right;
        internal int Height;

        internal SfAvlTreeNode(T value)
        {
            Value = value;
            Left = null;
            Right = null;
            Height = 1;
        }
    }
    
    /// <summary>
    /// Provides a custom debugger view for <see cref="SfAvlTree{T}"/> displaying elements in-order.
    /// </summary>
    internal sealed class SfTreeDebugView<T>
    {
        private readonly SfAvlTree<T> _tree;
        public SfTreeDebugView(SfAvlTree<T> tree) { _tree = tree; }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => _tree.InOrder().ToArray();
    }
}

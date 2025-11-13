using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using StructForge.Comparers;

namespace StructForge.Collections
{
    /// <summary>
    /// Represents a self-balancing binary search tree (AVL Tree) that maintains
    /// O(log n) complexity for insertion, deletion, and lookup operations.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the tree.</typeparam>
    [DebuggerTypeProxy(typeof(SfTreeDebugView<>))]
    public class SfAvlTree<T> : ISfTree<T>, ICollection<T>
    {
        /// <summary>Gets the number of elements contained in the tree.</summary>
        public int Count { get; private set; }

        /// <summary>Gets a value indicating whether the collection is read-only.</summary>
        public bool IsReadOnly => false;

        /// <summary>Gets a value indicating whether the tree is empty.</summary>
        public bool IsEmpty => Count == 0;

        /// <summary>Gets the height of the tree.</summary>
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
            ArgumentNullException.ThrowIfNull(collection);
            _comparer = comparer ?? SfComparers<T>.DefaultComparer;
            
            T[] arr = collection.ToArray();
            if (arr.Length == 0)
                return;
            
            foreach (var item in arr)
                TryAdd(item);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the elements in in-order traversal.
        /// </summary>
        public IEnumerator<T> GetEnumerator() => InOrder().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        
        /// <summary>
        /// Determines whether the specified item exists in the tree using binary search.
        /// </summary>
        /// <param name="item">The element to locate.</param>
        /// <returns><c>true</c> if the item is found; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T item)
        {
            SfAvlTreeNode<T> current = _root;
            while (current != null)
            {
                int cmp = _comparer.Compare(item, current.Value);
                if (cmp == 0) return true;
                current = cmp < 0 ? current.Left : current.Right;
            }
            return false;
        }

        /// <summary>
        /// Removes the specified element from the tree.
        /// </summary>
        /// <param name="item">The element to remove.</param>
        /// <returns><c>true</c> if the item was removed; otherwise, <c>false</c>.</returns>
        public bool Remove(T item)
        {
            int before = Count;
            _root = Delete(_root, item);
            return Count < before;
        }

        private SfAvlTreeNode<T> Delete(SfAvlTreeNode<T> node, T item, bool decrementCount = true)
        {
            if (node == null)
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
                        Count--;

                    // Node with one or zero children
                    if (node.Left == null)
                        return node.Right;
                    if (node.Right == null)
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

        /// <summary>
        /// Determines whether the specified item exists in the tree using the provided equality comparer.
        /// Performs a linear search (O(n)).
        /// </summary>
        /// <param name="item">The element to locate.</param>
        /// <param name="comparer">The equality comparer to use.</param>
        /// <returns><c>true</c> if the item is found; otherwise, <c>false</c>.</returns>
        public bool Contains(T item, IEqualityComparer<T> comparer)
        {
            foreach (T element in this)
            {
                if (comparer.Equals(item, element)) return true;
            }
            return false;
        }

        /// <summary>
        /// Adds the specified item to the tree. Throws if the item already exists.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <exception cref="InvalidOperationException">Thrown when the item already exists.</exception>
        public void Add(T item)
        {
            if (!TryAdd(item))
                throw new InvalidOperationException("Duplicate item");
        }

        /// <summary>
        /// Removes all elements from the tree.
        /// </summary>
        public void Clear()
        {
            _root = null;
            Count = 0;
        }

        /// <summary>
        /// Copies the elements of the tree to an existing array, starting at the specified index.
        /// </summary>
        /// <param name="array">The destination array.</param>
        /// <param name="arrayIndex">The starting index in the destination array.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="array"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="arrayIndex"/> is negative.</exception>
        /// <exception cref="ArgumentException">If the destination array is too small.</exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null) 
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (arrayIndex + Count > array.Length)
                throw new ArgumentException("Destination array is not large enough.");

            foreach (T item in InOrder())
                array[arrayIndex++] = item;
        }

        /// <summary>
        /// Attempts to add an item to the tree.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns><c>true</c> if added successfully; <c>false</c> if a duplicate exists.</returns>
        public bool TryAdd(T item)
        {
            bool added = false;
            _root = TryAdd(_root, item, ref added);
            if (added)
                Count++;
            return added;
        }

        private SfAvlTreeNode<T> TryAdd(SfAvlTreeNode<T> node, T item, ref bool added)
        {
            if (node == null)
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
        /// Returns the smallest (minimum) value in the tree.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the tree is empty.</exception>
        public T FindMin() => FindLeftmost(_root).Value;

        /// <summary>
        /// Returns the largest (maximum) value in the tree.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the tree is empty.</exception>
        public T FindMax() => FindRightmost(_root).Value;

        /// <summary>
        /// Enumerates nodes in in-order traversal (Left → Root → Right).
        /// </summary>
        public IEnumerable<T> InOrder()
        {
            if (_root == null) yield break;

            SfStack<SfAvlTreeNode<T>> sfStack = new SfStack<SfAvlTreeNode<T>>(Count);
            SfAvlTreeNode<T> current = _root;

            while (current != null || sfStack.Count > 0)
            {
                while (current != null)
                {
                    sfStack.Push(current);
                    current = current.Left;
                }

                current = sfStack.Pop();
                yield return current.Value;
                current = current.Right;
            }
        }

        /// <summary>
        /// Enumerates nodes in pre-order traversal (Root → Left → Right).
        /// </summary>
        public IEnumerable<T> PreOrder()
        {
            if (_root == null) yield break;

            SfStack<SfAvlTreeNode<T>> sfStack = new SfStack<SfAvlTreeNode<T>>(Count);
            sfStack.Push(_root);

            while (sfStack.Count > 0)
            {
                SfAvlTreeNode<T> node = sfStack.Pop();
                yield return node.Value;

                if (node.Right != null) sfStack.Push(node.Right);
                if (node.Left != null) sfStack.Push(node.Left);
            }
        }

        /// <summary>
        /// Enumerates nodes in post-order traversal (Left → Right → Root).
        /// </summary>
        public IEnumerable<T> PostOrder()
        {
            if (_root == null) yield break;

            SfStack<SfAvlTreeNode<T>> stack1 = new SfStack<SfAvlTreeNode<T>>(Count);
            SfStack<SfAvlTreeNode<T>> stack2 = new SfStack<SfAvlTreeNode<T>>(Count);

            stack1.Push(_root);
            while (stack1.Count > 0)
            {
                SfAvlTreeNode<T> node = stack1.Pop();
                stack2.Push(node);

                if (node.Left != null) stack1.Push(node.Left);
                if (node.Right != null) stack1.Push(node.Right);
            }

            while (stack2.Count > 0)
                yield return stack2.Pop().Value;
        }

        /// <summary>
        /// Attempts to find the stored value equal to <paramref name="equalValue"/>.
        /// </summary>
        /// <param name="equalValue">The value to locate.</param>
        /// <param name="actualValue">When found, contains the actual stored value.</param>
        /// <returns><c>true</c> if found; otherwise, <c>false</c>.</returns>
        public bool TryGetValue(T equalValue, out T actualValue)
        {
            var node = _root;
            while (node != null)
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

        /// <summary>
        /// Executes the specified action for each element in the tree.
        /// </summary>
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
            if (node == null)
                throw new InvalidOperationException("Tree is empty.");
            while (node.Left != null) node = node.Left;
            return node;
        }

        private static SfAvlTreeNode<T> FindRightmost(SfAvlTreeNode<T> node)
        {
            if (node == null)
                throw new InvalidOperationException("Tree is empty.");
            while (node.Right != null) node = node.Right;
            return node;
        }
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
    internal class SfTreeDebugView<T>
    {
        private readonly SfAvlTree<T> _tree;
        public SfTreeDebugView(SfAvlTree<T> tree) { _tree = tree; }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => _tree.InOrder().ToArray();
    }
}

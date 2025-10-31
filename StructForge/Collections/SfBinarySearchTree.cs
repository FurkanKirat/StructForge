using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using StructForge.Comparers;

namespace StructForge.Collections
{
    /// <summary>
    /// A generic Binary Search Tree (BST) implementation.
    /// Supports insertion, deletion, search, and traversal.
    /// </summary>
    [Obsolete("Use SfAvlTree<T> instead. This class is kept for educational purposes.")]
    public class SfBinarySearchTree<T> : ISfTree<T>, ICollection<T>
    {
        private SfBinaryTreeNode<T> _root;
        private readonly IComparer<T> _comparer;

        /// <inheritdoc/>
        public int Count { get; private set; }

        /// <summary>
        /// Gets whether the tree is empty.
        /// </summary>
        public bool IsEmpty => Count == 0;
        
        /// <summary>
        /// Gets the height of the binary tree.
        /// </summary>
        public int Height => GetHeight(_root);

        /// <inheritdoc/>
        public bool IsReadOnly => false;
        
        /// <summary>
        /// Initializes a new instance of <see cref="SfBinarySearchTree{T}"/> using a custom comparer.
        /// </summary>
        public SfBinarySearchTree(IComparer<T> comparer = null)
        {
            _comparer = comparer ?? SfComparers<T>.DefaultComparer;
        }
        
        /// <summary>
        /// Initializes a new instance of <see cref="SfBinarySearchTree{T}"/> with an enumeration and a custom comparer.
        /// </summary>
        public SfBinarySearchTree(IEnumerable<T> enumerable, IComparer<T> comparer = null)
        {
            if (enumerable == null) 
                throw new ArgumentNullException(nameof(enumerable));

            _comparer = comparer ?? SfComparers<T>.DefaultComparer;

            T[] arr = enumerable.ToArray();
            Array.Sort(arr, _comparer);
            
            for (int i = 1; i < arr.Length; i++)
            {
                if (_comparer.Compare(arr[i - 1], arr[i]) == 0)
                    throw new ArgumentException("Enumerable contains duplicate values.", nameof(enumerable));
            }


            _root = BuildBalanced(arr, 0, arr.Length - 1);
            Count = arr.Length;
        }


        #region Traversal API

        /// <summary>
        /// Returns an enumerator that iterates the tree in-order.
        /// </summary>
        public IEnumerator<T> GetEnumerator() => InOrder().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Enumerates nodes using in-order traversal (Left → Root → Right).
        /// </summary>
        public IEnumerable<T> InOrder()
        {
            if (_root == null) yield break;

            SfStack<SfBinaryTreeNode<T>> sfStack = new SfStack<SfBinaryTreeNode<T>>(Count);
            SfBinaryTreeNode<T> current = _root;

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
        /// Enumerates nodes using pre-order traversal (Root → Left → Right).
        /// </summary>
        public IEnumerable<T> PreOrder()
        {
            if (_root == null) yield break;

            SfStack<SfBinaryTreeNode<T>> sfStack = new SfStack<SfBinaryTreeNode<T>>(Count);
            sfStack.Push(_root);

            while (sfStack.Count > 0)
            {
                SfBinaryTreeNode<T> node = sfStack.Pop();
                yield return node.Value;

                if (node.Right != null) sfStack.Push(node.Right);
                if (node.Left != null) sfStack.Push(node.Left);
            }
        }

        /// <summary>
        /// Enumerates nodes using post-order traversal (Left → Right → Root).
        /// </summary>
        public IEnumerable<T> PostOrder()
        {
            if (_root == null) yield break;

            SfStack<SfBinaryTreeNode<T>> stack1 = new SfStack<SfBinaryTreeNode<T>>(Count);
            SfStack<SfBinaryTreeNode<T>> stack2 = new SfStack<SfBinaryTreeNode<T>>(Count);

            stack1.Push(_root);
            while (stack1.Count > 0)
            {
                SfBinaryTreeNode<T> node = stack1.Pop();
                stack2.Push(node);

                if (node.Left != null) stack1.Push(node.Left);
                if (node.Right != null) stack1.Push(node.Right);
            }

            while (stack2.Count > 0)
                yield return stack2.Pop().Value;
        }

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

        public void ForEach(Action<T> action)
        {
            foreach (T item in this)
            {
                action(item);
            }
        }

        #endregion



        #region Core API

        /// <summary>
        /// Inserts an item into the tree.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when a duplicate item is added.</exception>
        public void Add(T item)
        {
           if (!TryAdd(item)) 
               throw new ArgumentException("Duplicate value is not allowed.", nameof(item));
        }
        
        /// <summary>
        /// Inserts an item into the tree without throwing an exception for duplicates.
        /// </summary>
        public bool TryAdd(T item)
        {
            if (_root == null)
            {
                _root = new SfBinaryTreeNode<T>(item);
                Count++;
                return true;
            }
            SfBinaryTreeNode<T> current = _root;
            SfBinaryTreeNode<T> parent = null;
            while (current != null)
            {
                parent = current;
                switch (_comparer.Compare(item, current.Value))
                {
                    case < 0:
                        current = current.Left;
                        break;
                    case > 0:
                        current = current.Right;
                        break;
                    default:
                        return false;
                }
            }
            
            int compareToParent = _comparer.Compare(item, parent!.Value);
            if (compareToParent < 0)
                parent.Left = new SfBinaryTreeNode<T>(item);
            else
                parent.Right = new SfBinaryTreeNode<T>(item);
            Count++;
            return true;
        }

        /// <summary>
        /// Removes all nodes from the tree.
        /// </summary>
        public void Clear()
        {
            _root = null;
            Count = 0;
        }

        /// <summary>
        /// Determines whether the specified item exists in the tree.
        /// </summary>
        public bool Contains(T item)
        {
            SfBinaryTreeNode<T> current = _root;
            while (current != null)
            {
                int cmp = _comparer.Compare(item, current.Value);
                if (cmp == 0) return true;
                current = cmp < 0 ? current.Left : current.Right;
            }
            return false;
        }
        
        /// <summary>
        /// Determines whether the specified item exists in the tree using a linear search.
        /// 
        /// Note: This method iterates over all elements in the tree, so it is less efficient
        /// than <see cref="Contains(T)"/>, which uses the tree's ordering for a binary search
        /// and performs faster on balanced trees.
        /// </summary>
        public bool Contains(T item, IEqualityComparer<T> comparer)
        {
            foreach (T element in this)
            {
                if (comparer.Equals(item, element)) return true;
            }
            return false;
        }

        /// <summary>
        /// Copies the elements of the tree into the given array.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when the target array is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the array index is negative.</exception>
        /// <exception cref="ArgumentException">Thrown when the array does not have enough space.</exception>
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
        /// Removes an item from the tree if it exists.
        /// </summary>
        public bool Remove(T item) => Delete(null, _root, item);

        /// <summary>
        /// Returns the minimum value in the tree.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the tree is empty.</exception>
        public T FindMin() => FindLeftmost(_root).Value;

        /// <summary>
        /// Returns the maximum value in the tree.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the tree is empty.</exception>
        public T FindMax() => FindRightmost(_root).Value;
        #endregion
   
        
        #region ICollection<T>
        int ICollection<T>.Count => Count;
        void ICollection<T>.Clear() => Clear();
        bool ICollection<T>.Contains(T item) => Contains(item);
        void ICollection<T>.CopyTo(T[] array, int arrayIndex) => CopyTo(array, arrayIndex);
        #endregion

        #region PrivateHelpers

        /// <summary>
        /// Iteratively searches for a node, then calls <see cref="DeleteNode"/> when found.
        /// </summary>
        private bool Delete(SfBinaryTreeNode<T> parent, SfBinaryTreeNode<T> node, T item)
        {
            while (true)
            {
                if (node == null) return false;

                int comparison = _comparer.Compare(item, node.Value);
                switch (comparison)
                {
                    case 0:
                        DeleteNode(parent, node);
                        return true;
                    case < 0:
                        parent = node;
                        node = node.Left;
                        continue;
                    default:
                        parent = node;
                        node = node.Right;
                        continue;
                }
            }
        }

        /// <summary>
        /// Handles node removal:
        /// - Two children → replace with inorder successor.
        /// - One child → parent points to child.
        /// - No children → parent reference is cleared.
        /// If parent is null, updates root.
        /// </summary>
        private void DeleteNode(SfBinaryTreeNode<T> parent, SfBinaryTreeNode<T> node)
        {
            if (node.Left != null && node.Right != null) // Two children
            {
                SfBinaryTreeNode<T> successor = FindLeftmost(node.Right);
                node.Value = successor.Value; // Copy successor value
                Delete(node, node.Right, successor.Value); // Delete successor node
                return;
            }

            SfBinaryTreeNode<T> child = node.Left ?? node.Right;

            if (parent == null) // Root deletion
            {
                _root = child;
            }
            else if (parent.Left == node)
            {
                parent.Left = child;    
            }
            else
            {
                parent.Right = child;
            }

            Count--;
        }
        /// <summary>
        /// Builds the tree as the most balanced version.
        /// </summary>
        private static SfBinaryTreeNode<T> BuildBalanced(T[] arr, int left, int right)
        {
            if (left > right) return null;

            int mid = (left + right) / 2;
            SfBinaryTreeNode<T> node = new SfBinaryTreeNode<T>(arr[mid])
            {
                Left = BuildBalanced(arr, left, mid - 1),
                Right = BuildBalanced(arr, mid + 1, right)
            };
            return node;
        }

        /// <summary>
        /// Finds the leftmost (minimum) node starting from the given node.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the tree is empty.</exception>
        private static SfBinaryTreeNode<T> FindLeftmost(SfBinaryTreeNode<T> node)
        {
            if (node == null)
                throw new InvalidOperationException("Tree is empty.");
            while (node.Left != null) node = node.Left;
            return node;
        }

        /// <summary>
        /// Finds the rightmost (maximum) node starting from the given node.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the tree is empty.</exception>
        private static SfBinaryTreeNode<T> FindRightmost(SfBinaryTreeNode<T> node)
        {
            if (node == null)
                throw new InvalidOperationException("Tree is empty.");
            while (node.Right != null) node = node.Right;
            return node;
        }
        /// <summary>
        /// Gives the height of the given node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static int GetHeight(SfBinaryTreeNode<T> node)
        {
            if (node == null) return 0;
            return 1 + Math.Max(GetHeight(node.Left), GetHeight(node.Right));
        }

        #endregion

        
    }
    
    [DebuggerDisplay("{Value}")]
    internal class SfBinaryTreeNode<T>
    {
        internal T Value;
        internal SfBinaryTreeNode<T> Left;
        internal SfBinaryTreeNode<T> Right;

        internal SfBinaryTreeNode(T value)
        {
            Value = value;
            Left = null;
            Right = null;
        }
    }
}

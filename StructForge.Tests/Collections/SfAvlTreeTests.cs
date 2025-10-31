using StructForge.Collections;
using StructForge.Comparers;

namespace StructForge.Tests.Collections
{
    public class SfAvlTreeTests
    {
        [Fact]
        public void Add_ShouldIncreaseCount_AndContainItems()
        {
            var tree = new SfAvlTree<int>();

            tree.Add(10);
            tree.Add(5);
            tree.Add(15);

            Assert.Equal(3, tree.Count);
            Assert.Contains(10, tree);
            Assert.Contains(5, tree);
            Assert.Contains(15, tree);
        }

        [Fact]
        public void InOrder_ShouldReturnSortedValues()
        {
            var tree = new SfAvlTree<int>();
            tree.Add(10);
            tree.Add(5);
            tree.Add(15);
            tree.Add(2);
            tree.Add(7);

            var result = tree.InOrder().ToArray();

            Assert.Equal(new[] { 2, 5, 7, 10, 15 }, result);
        }

        [Fact]
        public void PreOrder_ShouldReturnCorrectOrder()
        {
            var tree = new SfAvlTree<int>();
            tree.Add(10);
            tree.Add(5);
            tree.Add(15);

            var result = tree.PreOrder().ToArray();

            Assert.Equal(new[] { 10, 5, 15 }, result);
        }

        [Fact]
        public void PostOrder_ShouldReturnCorrectOrder()
        {
            var tree = new SfAvlTree<int>();
            tree.Add(10);
            tree.Add(5);
            tree.Add(15);

            var result = tree.PostOrder().ToArray();

            Assert.Equal(new[] { 5, 15, 10 }, result);
        }

        [Fact]
        public void FindMinMax_ShouldReturnCorrectValues()
        {
            var tree = new SfAvlTree<int>();
            tree.Add(10);
            tree.Add(5);
            tree.Add(15);
            tree.Add(2);
            tree.Add(20);

            Assert.Equal(2, tree.FindMin());
            Assert.Equal(20, tree.FindMax());
        }

        [Fact]
        public void Remove_ShouldDeleteLeafNode()
        {
            var tree = new SfAvlTree<int>();
            tree.Add(10);
            tree.Add(5);
            tree.Add(15);

            var result = tree.Remove(5);

            Assert.True(result);
            Assert.DoesNotContain(5, tree);
            Assert.Equal(2, tree.Count);
        }

        [Fact]
        public void Remove_ShouldDeleteNodeWithOneChild()
        {
            var tree = new SfAvlTree<int>();
            tree.Add(10);
            tree.Add(5);
            tree.Add(2); // child of 5

            var result = tree.Remove(5);

            Assert.True(result);
            Assert.DoesNotContain(5, tree);
            Assert.Contains(2, tree); // child should remain
            Assert.Equal(2, tree.Count);

        }

        [Fact]
        public void Remove_ShouldDeleteNodeWithTwoChildren()
        {
            var tree = new SfAvlTree<int>();
            tree.Add(10);
            tree.Add(5);
            tree.Add(15);
            tree.Add(12);
            tree.Add(20);

            var result = tree.Remove(15);

            Assert.True(result);
            Assert.DoesNotContain(15, tree);
            Assert.Contains(12, tree);
            Assert.Contains(20, tree);
        }

        [Fact]
        public void Remove_ShouldHandleRootDeletion()
        {
            var tree = new SfAvlTree<int>();
            tree.Add(10);

            var result = tree.Remove(10);

            Assert.True(result);
            Assert.DoesNotContain(10, tree);
            Assert.Empty(tree);
        }
        
        [Fact]
        public void Remove_ShouldDeleteRootNodeWithTwoChildren()
        {
            var tree = new SfAvlTree<int>();
            tree.Add(10);
            tree.Add(5);
            tree.Add(15);
            tree.Add(12);
            tree.Add(20);

            var result = tree.Remove(10);

            Assert.True(result);
            Assert.DoesNotContain(10, tree);
            Assert.Contains(12, tree);
            Assert.Contains(20, tree);
            Assert.Contains(5, tree);
            Assert.Contains(15, tree);
        }

        [Fact]
        public void Clear_ShouldEmptyTheTree()
        {
            var tree = new SfAvlTree<int>();
            tree.Add(10);
            tree.Add(20);

            tree.Clear();

            Assert.Empty(tree);
            Assert.DoesNotContain(10, tree);
        }
        
        [Fact]
        public void Add_ShouldIgnoreDuplicates()
        {
            var tree = new SfAvlTree<int>();
            tree.Add(10);

            Assert.Equal(new[]{10}, tree);
        }
        
        [Fact]
        public void InOrder_ShouldAlwaysReturnSortedSequence()
        {
            var tree = new SfAvlTree<int>();
            var rand = new Random(42);
            var numbers = Enumerable.Range(1, 100).OrderBy(_ => rand.Next()).ToArray();

            foreach (var n in numbers) tree.Add(n);

            var inOrder = tree.InOrder().ToArray();
            Assert.True(inOrder.SequenceEqual(Enumerable.Range(1, 100)));
        }
        
        [Fact]
        public void RandomInsertDelete_ShouldMaintainCorrectCount()
        {
            var tree = new SfAvlTree<int>();
            var rand = new Random(123);
            int count = 10000;
            var numbers = Enumerable.Range(1, count).OrderBy(_ => rand.Next()).ToArray();
            
            foreach (var n in numbers) tree.Add(n);
            Assert.Equal(count, tree.Count);
            
            foreach (var n in numbers.OrderBy(_ => rand.Next()))
                Assert.True(tree.Remove(n));

            Assert.Empty(tree);
            Assert.Empty(tree);
        }
        
        [Fact]
        public void CopyTo_ShouldCopyElementsInOrder()
        {
            var tree = new SfAvlTree<int>();
            tree.Add(10);
            tree.Add(5);
            tree.Add(15);

            var arr = new int[3];
            tree.CopyTo(arr, 0);

            Assert.Equal(new[] { 5, 10, 15 }, arr);
        }

        [Fact]
        public void FindMinMax_OnEmptyTree_ShouldThrow()
        {
            var tree = new SfAvlTree<int>();

            Assert.Throws<InvalidOperationException>(() => tree.FindMin());
            Assert.Throws<InvalidOperationException>(() => tree.FindMax());
        }

        [Fact]
        public void ICollection_Interface_ShouldWork()
        {
            ICollection<int> tree = new SfAvlTree<int>();
            tree.Add(1);
            tree.Add(2);

            Assert.Equal(2, tree.Count);
            Assert.False(tree.IsReadOnly);

            var arr = new int[2];
            tree.CopyTo(arr, 0);
            Assert.Contains(1, arr);
            Assert.Contains(2, arr);
        }

        [Fact]
        public void Constructor_WithComparer_ShouldRespectCustomOrder()
        {
            var comparer = SfComparers<int>.ReverseComparer;
            var tree = new SfAvlTree<int>(comparer);
            
            tree.Add(10);
            tree.Add(5);
            tree.Add(20);
            
            var result = tree.InOrder().ToArray();
            Assert.Equal(new[] { 20, 10, 5 }, result);
        }

        [Fact]
        public void Constructor_WithEnumerable_ShouldBuildBalancedTree()
        {
            var numbers = new[] { 1, 2, 3, 4, 5, 6, 7 };
            
            var tree = new SfAvlTree<int>(numbers);
            
            Assert.Equal(numbers.Length, tree.Count);
            Assert.Equal(numbers, tree.InOrder().ToArray());
            Assert.True(tree.Height <= (int)Math.Ceiling(Math.Log2(numbers.Length + 1)) + 1);
        }

        [Fact]
        public void Constructor_WithEnumerableAndComparer_ShouldRespectBoth()
        {
            var words = new[] { "pea", "apple", "banana", "kiwi" };
            var comparer = SfStringComparers.Length;
            
            var tree = new SfAvlTree<string>(words, comparer);
            
            Assert.Equal(words.Length, tree.Count);

            // In-order traversal should be sorted by length
            var result = tree.InOrder().ToArray();
            Assert.Equal(new[] { "pea", "kiwi", "apple", "banana" }, result);
        }

        [Fact]
        public void Constructor_WithNullEnumerable_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => new SfAvlTree<int>((IEnumerable<int>)null));
        }

        [Fact]
        public void Constructor_WithEmptyEnumerable_ShouldProduceEmptyTree()
        {
            var tree = new SfAvlTree<int>(Enumerable.Empty<int>());
            
            Assert.True(tree.IsEmpty);
            Assert.Empty(tree);
        }
    }
}

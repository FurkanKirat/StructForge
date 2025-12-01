using StructForge.Collections;

namespace StructForge.Tests.Collections
{
    public class SfEnumeratorTests
    {
        private void AssertEnumeratorContent<T>(IEnumerable<T> collection, T[] expectedItems)
        {
            if (collection is ICollection<T> col)
            {
                Assert.Equal(expectedItems.Length, col.Count);
            }

            int index = 0;
            foreach (var item in collection)
            {
                Assert.True(index < expectedItems.Length, $"Enumerator returned too many elements! Expected: {expectedItems.Length}, Coming Index: {index}");
                Assert.Equal(expectedItems[index], item);
                index++;
            }
            Assert.Equal(expectedItems.Length, index);

            using var enumerator = collection.GetEnumerator();
            if (expectedItems.Length > 0)
            {
                Assert.True(enumerator.MoveNext());
                Assert.Equal(expectedItems[0], enumerator.Current);
                
                enumerator.Reset();
                
                Assert.True(enumerator.MoveNext());
                Assert.Equal(expectedItems[0], enumerator.Current);
            }
            else
            {
                Assert.False(enumerator.MoveNext());
            }
        }


        [Fact]
        public void SfList_Enumerator_ShouldWork()
        {
            var list = new SfList<int> { 10, 20, 30 };
            AssertEnumeratorContent(list, new[] { 10, 20, 30 });
        }

        [Fact]
        public void SfLinkedList_Enumerator_ShouldWork()
        {
            var list = new SfLinkedList<int>();
            list.AddLast(1);
            list.AddLast(2);
            list.AddLast(3);
            AssertEnumeratorContent(list, new[] { 1, 2, 3 });
        }

        [Fact]
        public void SfStack_Enumerator_ShouldBeLIFO()
        {
            var stack = new SfStack<int>();
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
       
            AssertEnumeratorContent(stack, new[] { 3, 2, 1 });
        }

        [Fact]
        public void SfQueue_Enumerator_ShouldBeFIFO()
        {
            var queue = new SfQueue<int>();
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);
            AssertEnumeratorContent(queue, new[] { 1, 2, 3 });
        }

        [Fact]
        public void SfRingBuffer_Enumerator_ShouldHandleWrapAround()
        {
            var buffer = new SfRingBuffer<int>(5);
            buffer.Enqueue(1); buffer.Enqueue(2); buffer.Enqueue(3);
            buffer.Enqueue(4); buffer.Enqueue(5);
            
            buffer.Dequeue();
            buffer.Dequeue();
            
            buffer.Enqueue(6);
            buffer.Enqueue(7);

            AssertEnumeratorContent(buffer, new[] { 3, 4, 5, 6, 7 });
        }

        [Fact]
        public void SfGrid2D_Enumerator_ShouldIterateRowMajor()
        {
            // 3x3 Grid
            var grid = new SfGrid2D<int>(3, 3);
            int val = 0;
            for (int y = 0; y < 3; y++)
                for (int x = 0; x < 3; x++)
                    grid[x, y] = val++;
            
            var expected = Enumerable.Range(0, 9).ToArray();
            AssertEnumeratorContent(grid, expected);
        }

        [Fact]
        public void SfGrid3D_Enumerator_ShouldIterateLinear()
        {
            // 2x2x2 Grid
            var grid = new SfGrid3D<int>(2, 2, 2);
            int val = 0;
            
            for (int z = 0; z < 2; z++)
                for (int y = 0; y < 2; y++)
                    for (int x = 0; x < 2; x++)
                        grid[x, y, z] = val++;

            var expected = Enumerable.Range(0, 8).ToArray();
            AssertEnumeratorContent(grid, expected);
        }

        [Fact]
        public void SfBitArray_Enumerator_ShouldIterateBooleans()
        {
            var bits = new SfBitArray(5);
            bits[0] = true;
            bits[2] = true;
            bits[4] = true;
            // 1, 0, 1, 0, 1
            AssertEnumeratorContent(bits, new[] { true, false, true, false, true });
        }

        [Fact]
        public void SfBitArray2D_Enumerator_ShouldIterateCorrectly()
        {
            var bits = new SfBitArray2D(2, 2);
            bits[0, 0] = true; // Index 0
            bits[1, 0] = false; // Index 1
            bits[0, 1] = false; // Index 2
            bits[1, 1] = true;  // Index 3

            AssertEnumeratorContent(bits, new[] { true, false, false, true });
        }
        
        [Fact]
        public void SfBitArray3D_Enumerator_ShouldIterateCorrectly()
        {
            var bits = new SfBitArray3D(1, 1, 2);
            bits[0, 0, 0] = true;
            bits[0, 0, 1] = false;
            
            AssertEnumeratorContent(bits, new[] { true, false });
        }

        [Fact]
        public void SfAvlTree_Enumerator_ShouldBeInOrder()
        {
            var tree = new SfAvlTree<int>();
            tree.Add(50); tree.Add(30); tree.Add(70); tree.Add(20); tree.Add(40);

            AssertEnumeratorContent(tree, new[] { 20, 30, 40, 50, 70 });
        }

        [Fact]
        public void SfSortedSet_Enumerator_ShouldBeInOrder()
        {
            var set = new SfSortedSet<int>();
            set.Add(5); set.Add(1); set.Add(3);

            AssertEnumeratorContent(set, new[] { 1, 3, 5 });
        }
        
        [Fact]
        public void SfHashSet_Enumerator_ShouldContainAllItems()
        {
            var set = new SfHashSet<int>();
            set.Add(10); set.Add(20); set.Add(30);

            var list = new List<int>();
            foreach (var item in set) list.Add(item);
            
            Assert.Equal(3, list.Count);
            Assert.Contains(10, list);
            Assert.Contains(20, list);
            Assert.Contains(30, list);
        }

        [Fact]
        public void SfBinaryHeap_Enumerator_ShouldIterateRawStorage()
        {
            var heap = new SfBinaryHeap<int>();
            heap.Add(10);
            heap.Add(5);
            heap.Add(20);

            var items = new List<int>();
            foreach (var item in heap) items.Add(item);

            Assert.Equal(3, items.Count);
            Assert.Contains(5, items);
            Assert.Contains(10, items);
            Assert.Contains(20, items);
        }
        
        [Fact]
        public void SfPriorityQueue_Enumerator_ShouldIterateItems()
        {
            var pq = new SfPriorityQueue<string, int>();
            pq.Enqueue("A", 1);
            pq.Enqueue("B", 2);
            
            var items = new List<string>();
            foreach (var item in pq) items.Add(item);
            
            Assert.Contains("A", items);
            Assert.Contains("B", items);
        }
    }
}
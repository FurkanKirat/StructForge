#nullable disable
using StructForge.Collections;

namespace StructForge.Tests.Collections
{
    public class SfPriorityQueueTests
    {
        [Fact]
        public void Enqueue_Dequeue_ShouldFollowPriorityOrder_MaxHeap()
        {
            var pq = new SfPriorityQueue<string, int>(minHeap: false);

            pq.Enqueue("Low", 1);
            pq.Enqueue("Medium", 5);
            pq.Enqueue("High", 10);

            Assert.Equal("High", pq.Dequeue());
            Assert.Equal("Medium", pq.Dequeue());
            Assert.Equal("Low", pq.Dequeue());
            Assert.True(pq.IsEmpty);
        }

        [Fact]
        public void Enqueue_Dequeue_ShouldFollowPriorityOrder_MinHeap()
        {
            var pq = new SfPriorityQueue<string, int>(minHeap: true);

            pq.Enqueue("Low", 1);
            pq.Enqueue("Medium", 5);
            pq.Enqueue("High", 10);

            Assert.Equal("Low", pq.Dequeue());
            Assert.Equal("Medium", pq.Dequeue());
            Assert.Equal("High", pq.Dequeue());
        }

        [Fact]
        public void Peek_ShouldReturnWithoutRemoving()
        {
            var pq = new SfPriorityQueue<string, int>(minHeap: false);
            pq.Enqueue("A", 10);
            pq.Enqueue("B", 5);

            var peek = pq.Peek();

            Assert.Equal("A", peek);
            Assert.Equal(2, pq.Count);
        }

        [Fact]
        public void EnumerateByPriority_ShouldReturnSortedOrder()
        {
            var pq = new SfPriorityQueue<string, int>(minHeap: false);
            pq.Enqueue("Low", 1);
            pq.Enqueue("Medium", 5);
            pq.Enqueue("High", 10);

            var ordered = pq.EnumerateByPriority().ToArray();

            Assert.Equal(new[] { "High", "Medium", "Low" }, ordered);
            Assert.Equal(3, pq.Count); // Original queue not modified
        }

        [Fact]
        public void ToArray_ShouldReturnAllItems_Unordered()
        {
            var pq = new SfPriorityQueue<int, int>();
            pq.Enqueue(1, 1);
            pq.Enqueue(2, 2);
            pq.Enqueue(3, 3);

            var arr = pq.ToArray();

            Assert.Equal(3, arr.Length);
            Assert.Contains(1, arr);
            Assert.Contains(2, arr);
            Assert.Contains(3, arr);
        }

        [Fact]
        public void Contains_ShouldReturnTrueIfExists()
        {
            var pq = new SfPriorityQueue<int, int>();
            pq.Enqueue(10, 1);
            pq.Enqueue(20, 2);

            Assert.True(pq.Contains(10));
            Assert.False(pq.Contains(999));
        }

        [Fact]
        public void Clear_ShouldResetQueue()
        {
            var pq = new SfPriorityQueue<int, int>();
            pq.Enqueue(1, 1);
            pq.Enqueue(2, 2);

            pq.Clear();

            Assert.Empty(pq);
            Assert.True(pq.IsEmpty);
        }

        [Fact]
        public void CopyTo_ShouldCopyElements()
        {
            var pq = new SfPriorityQueue<string, int>();
            pq.Enqueue("A", 1);
            pq.Enqueue("B", 2);
            pq.Enqueue("C", 3);

            var target = new string[5];
            pq.CopyTo(target, 1);

            Assert.NotNull(target[1]);
            Assert.Equal(3, target.Count(x => x != null));
        }

        [Fact]
        public void EmptyQueue_Dequeue_ShouldThrow()
        {
            var pq = new SfPriorityQueue<int, int>();
            Assert.Throws<InvalidOperationException>(() => pq.Dequeue());
        }

        [Fact]
        public void EmptyQueue_Peek_ShouldThrow()
        {
            var pq = new SfPriorityQueue<int, int>();
            Assert.Throws<InvalidOperationException>(() => pq.Peek());
        }
    }
}

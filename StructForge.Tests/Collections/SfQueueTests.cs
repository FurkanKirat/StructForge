using StructForge.Collections;

namespace StructForge.Tests.Collections
{
    public class SfQueueTests
    {
        [Fact]
        public void Enqueue_IncreasesCount()
        {
            var queue = new SfQueue<int>();
            queue.Enqueue(1);
            queue.Enqueue(2);

            Assert.Equal(2, queue.Count);
        }

        [Fact]
        public void Dequeue_ReturnsItemsInOrder()
        {
            var queue = new SfQueue<int>();
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);

            Assert.Equal(1, queue.Dequeue());
            Assert.Equal(2, queue.Dequeue());
            Assert.Equal(3, queue.Dequeue());
            Assert.Empty(queue);
        }

        [Fact]
        public void Peek_ReturnsFirstItemWithoutRemoving()
        {
            var queue = new SfQueue<string>();
            queue.Enqueue("first");
            queue.Enqueue("second");

            var item = queue.Peek();
            Assert.Equal("first", item);
            Assert.Equal(2, queue.Count);
        }

        [Fact]
        public void Clear_EmptiesQueue()
        {
            var queue = new SfQueue<int>();
            queue.Enqueue(1);
            queue.Enqueue(2);

            queue.Clear();

            Assert.Empty(queue);
        }

        [Fact]
        public void Contains_ReturnsTrueIfItemExists()
        {
            var queue = new SfQueue<int>();
            queue.Enqueue(10);
            queue.Enqueue(20);

            Assert.Contains(10, queue);
            Assert.DoesNotContain(30, queue);
        }

        [Fact]
        public void Dequeue_OnEmptyQueue_ThrowsException()
        {
            var queue = new SfQueue<int>();
            Assert.Throws<InvalidOperationException>(() => queue.Dequeue());
        }

        [Fact]
        public void Peek_OnEmptyQueue_ThrowsException()
        {
            var queue = new SfQueue<int>();
            Assert.Throws<InvalidOperationException>(() => queue.Peek());
        }

        [Fact]
        public void Enqueue_Dequeue_MaintainsCorrectOrder()
        {
            var queue = new SfQueue<int>();
            for (int i = 1; i <= 5; i++)
                queue.Enqueue(i);

            for (int i = 1; i <= 5; i++)
                Assert.Equal(i, queue.Dequeue());
        }
        
        [Fact]
        public void Enqueue_Dequeue_WorksCorrectly()
        {
            var queue = new SfQueue<int>();
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);

            Assert.Equal(1, queue.Dequeue());
            Assert.Equal(2, queue.Dequeue());
            Assert.Equal(3, queue.Dequeue());
            Assert.Empty(queue);
        }

        [Fact]
        public void Peek_ReturnsFirstWithoutRemoving()
        {
            var queue = new SfQueue<string>();
            queue.Enqueue("first");
            queue.Enqueue("second");

            var item = queue.Peek();
            Assert.Equal("first", item);
            Assert.Equal(2, queue.Count);
        }
        
        [Fact]
        public void Dequeue_OnEmpty_Throws()
        {
            var queue = new SfQueue<int>();
            Assert.Throws<InvalidOperationException>(() => queue.Dequeue());
        }

        [Fact]
        public void Peek_OnEmpty_Throws()
        {
            var queue = new SfQueue<int>();
            Assert.Throws<InvalidOperationException>(() => queue.Peek());
        }

        [Fact]
        public void Enqueue_GrowsCapacity()
        {
            var queue = new SfQueue<int>(2);
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3); // triggers grow

            Assert.Equal(3, queue.Count);
        }

        [Fact]
        public void CircularBehavior_WrapsCorrectly()
        {
            var queue = new SfQueue<int>(3);
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);
            Assert.Equal(1, queue.Dequeue());

            queue.Enqueue(4);
            Assert.Equal(2, queue.Dequeue());
            Assert.Equal(3, queue.Dequeue());
            Assert.Equal(4, queue.Dequeue());
            Assert.True(queue.IsEmpty);
        }

        [Fact]
        public void RandomElements()
        {
            var queue = new SfQueue<int>();
            for (int i = 0; i < 50; i++)
            {
                queue.Enqueue(i);
            }

            for (int i = 0; i < 50; i++)
            {
                Assert.Equal(i, queue.Dequeue());
            }
            
            Assert.Empty(queue);
        }
        
        [Fact]
        public void PeekLast_ReturnsLastElement()
        {
            var queue = new SfQueue<int>();
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);

            int last = queue.PeekLast();

            Assert.Equal(3, last);
            Assert.Equal(3, queue.Count);
        }

        [Fact]
        public void TryPeekLast_WorksCorrectly()
        {
            var queue = new SfQueue<string>();
            queue.Enqueue("a");
            queue.Enqueue("b");

            bool result = queue.TryPeekLast(out string last);

            Assert.True(result);
            Assert.Equal("b", last);

            queue.Clear();
            result = queue.TryPeekLast(out last);
            Assert.False(result);
            Assert.Null(last);
        }

        [Fact]
        public void TrimExcess_ReducesCapacity()
        {
            var queue = new SfQueue<int>(10);
            for (int i = 0; i < 5; i++) queue.Enqueue(i);

            queue.TrimExcess();
            Assert.Equal(5, queue.Capacity);
            Assert.Equal(5, queue.Count);
            
            for (int i = 0; i < 5; i++)
                Assert.Equal(i, queue.Dequeue());
        }

        [Fact]
        public void ForEach_IteratesAllElements()
        {
            var queue = new SfQueue<int>();
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);

            int sum = 0;
            queue.ForEach(x => sum += x);

            Assert.Equal(6, sum);
        }
    }
    
}

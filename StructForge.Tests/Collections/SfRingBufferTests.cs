using StructForge.Collections;

namespace StructForge.Tests.Collections
{
    public class SfRingBufferTests
    {
        [Fact]
        public void Enqueue_Dequeue_WorksCorrectly()
        {
            var buffer = new SfRingBuffer<int>(3);

            // Initially empty
            Assert.True(buffer.IsEmpty);
            Assert.Empty(buffer);

            // Enqueue elements
            buffer.Enqueue(1);
            buffer.Enqueue(2);
            buffer.Enqueue(3);

            Assert.False(buffer.IsEmpty);
            Assert.Equal(3, buffer.Count);

            // Dequeue elements in FIFO order
            Assert.Equal(1, buffer.Dequeue());
            Assert.Equal(2, buffer.Dequeue());
            Assert.Equal(3, buffer.Dequeue());

            Assert.True(buffer.IsEmpty);
            Assert.Empty(buffer);
        }

        [Fact]
        public void TryDequeue_ReturnsFalse_WhenEmpty()
        {
            var buffer = new SfRingBuffer<int>(2);

            Assert.False(buffer.TryDequeue(out int item));
            Assert.Empty(buffer);
            Assert.Equal(0, item);
        }

        [Fact]
        public void Peek_And_PeekLast_WorkCorrectly()
        {
            var buffer = new SfRingBuffer<string>(3);
            buffer.Enqueue("a");
            buffer.Enqueue("b");
            buffer.Enqueue("c");

            // Peek front
            Assert.Equal("a", buffer.Peek());
            Assert.True(buffer.TryPeek(out string front));
            Assert.Equal("a", front);

            // Peek last
            Assert.Equal("c", buffer.PeekLast());
            Assert.True(buffer.TryPeekLast(out string last));
            Assert.Equal("c", last);
        }

        [Fact]
        public void OverwriteBehavior_WorksCorrectly()
        {
            var buffer = new SfRingBuffer<int>(3);

            buffer.Enqueue(1);
            buffer.Enqueue(2);
            buffer.Enqueue(3);

            // Queue is full, next enqueue overwrites oldest
            buffer.Enqueue(4);

            // Now the queue should contain 2, 3, 4
            Assert.Equal(3, buffer.Count);
            Assert.Equal(2, buffer.Dequeue());
            Assert.Equal(3, buffer.Dequeue());
            Assert.Equal(4, buffer.Dequeue());
        }

        [Fact]
        public void TryEnqueue_ReturnsFalse_WhenFull()
        {
            var buffer = new SfRingBuffer<int>(2);

            Assert.True(buffer.TryEnqueue(10));
            Assert.True(buffer.TryEnqueue(20));
            Assert.False(buffer.TryEnqueue(30)); // Full, cannot enqueue
        }

        [Fact]
        public void Clear_EmptiesBuffer()
        {
            var buffer = new SfRingBuffer<int>(3);
            buffer.Enqueue(1);
            buffer.Enqueue(2);

            buffer.Clear();

            Assert.True(buffer.IsEmpty);
            Assert.Empty(buffer);
            Assert.False(buffer.TryPeek(out _));
            Assert.False(buffer.TryPeekLast(out _));
        }
    }
}

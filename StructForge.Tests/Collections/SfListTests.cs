using StructForge.Collections;
using Xunit.Abstractions;

namespace StructForge.Tests.Collections
{
    public class SfListTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public SfListTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Add_IncreasesCount()
        {
            var list = new SfList<int>();
            list.Add(1);
            list.Add(2);

            Assert.Equal(2, list.Count);
            Assert.Equal(1, list[0]);
            Assert.Equal(2, list[1]);
        }

        [Fact]
        public void Clear_ResetsCountButKeepsCapacity()
        {
            var list = new SfList<int>(4);
            list.Add(1);
            list.Add(2);
            int oldCapacity = list.Capacity;

            list.Clear();
            
            Assert.Empty(list);
            Assert.Equal(oldCapacity, list.Capacity);
        }

        [Fact]
        public void Contains_ReturnsTrueIfExists()
        {
            var list = new SfList<string>();
            list.Add("a");
            list.Add("b");

            Assert.Contains("a", list);
            Assert.DoesNotContain("c", list);
        }

        [Fact]
        public void Indexer_GetAndSet_Works()
        {
            var list = new SfList<int>();
            list.Add(10);
            list[0] = 42;

            Assert.Equal(42, list[0]);
        }

        [Fact]
        public void Remove_DeletesItemAndShiftsArray()
        {
            var list = new SfList<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);

            bool removed = list.Remove(2);

            Assert.True(removed);
            Assert.Equal(2, list.Count);
            Assert.Equal(3, list[1]);
        }

        [Fact]
        public void RemoveAt_DeletesCorrectIndex()
        {
            var list = new SfList<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);

            list.RemoveAt(1);

            Assert.Equal(2, list.Count);
            Assert.Equal(1, list[0]);
            Assert.Equal(3, list[1]);
        }

        [Fact]
        public void Insert_AddsItemAtCorrectIndex()
        {
            var list = new SfList<int>();
            list.Add(1);
            list.Add(3);

            list.Insert(1, 2);

            Assert.Equal(3, list.Count);
            Assert.Equal(1, list[0]);
            Assert.Equal(2, list[1]);
            Assert.Equal(3, list[2]);
        }

        [Fact]
        public void ReGrowthIfNeeded_DoublesCapacity()
        {
            var list = new SfList<int>(2);
            int initialCapacity = list.Capacity;

            list.Add(1);
            list.Add(2);
            list.Add(3); // should trigger growth

            Assert.Equal(initialCapacity * 2, list.Capacity);
            Assert.Equal(3, list.Count);
        }

        [Fact]
        public void CopyTo_CopiesElementsCorrectly()
        {
            var list = new SfList<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);

            int[] array = new int[5];
            list.CopyTo(array, 1);

            Assert.Equal(0, array[0]);
            Assert.Equal(1, array[1]);
            Assert.Equal(2, array[2]);
            Assert.Equal(3, array[3]);
        }

        [Fact]
        public void Enumeration_YieldsCorrectItems()
        {
            var list = new SfList<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);

            var items = list.ToArray();
            Assert.Equal(new[] { 1, 2, 3 }.ToList(), items.ToList());
        }
        
         [Fact]
        public void Insert_ThrowsOnInvalidIndex()
        {
            var list = new SfList<int>();
            list.Add(1);

            Assert.Throws<ArgumentOutOfRangeException>(() => list.Insert(-1, 42));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.Insert(2, 42)); // Count = 1, valid indices: 0..1
        }

        [Fact]
        public void RemoveAt_ThrowsOnInvalidIndex()
        {
            var list = new SfList<int>();
            list.Add(1);

            Assert.Throws<ArgumentOutOfRangeException>(() => list.RemoveAt(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.RemoveAt(1)); // Count = 1
        }

        [Fact]
        public void Remove_ReturnsFalseIfItemNotFound()
        {
            var list = new SfList<string>();
            list.Add("a");
            list.Add("b");

            bool result = list.Remove("c");
            Assert.False(result);
            Assert.Equal(2, list.Count);
        }

        [Fact]
        public void Add_NullReference_AddsSuccessfully()
        {
            var list = new SfList<string>();
            list.Add(null);
            list.Add("test");

            Assert.Equal(2, list.Count);
            Assert.Null(list[0]);
            Assert.Equal("test", list[1]);
        }

        [Fact]
        public void InsertAtEnd_WorksCorrectly()
        {
            var list = new SfList<int>();
            list.Add(1);
            list.Add(2);

            list.Insert(2, 3); // Insert at Count index
            Assert.Equal(3, list.Count);
            Assert.Equal(3, list[2]);
        }

        [Fact]
        public void Clear_ThenAdd_Works()
        {
            var list = new SfList<int>();
            list.Add(1);
            list.Add(2);
            list.Clear();

            list.Add(3);
            Assert.Single(list);
            Assert.Equal(3, list[0]);
        }

        [Fact]
        public void CopyTo_ThrowsOnInvalidArguments()
        {
            var list = new SfList<int>();
            list.Add(1);
            list.Add(2);

            int[] arr = new int[1];
            Assert.Throws<ArgumentException>(() => list.CopyTo(arr, 0)); // Not enough space
            Assert.Throws<ArgumentOutOfRangeException>(() => list.CopyTo(new int[2], -1));
            Assert.Throws<ArgumentNullException>(() => list.CopyTo(null, 0));
        }

        [Fact]
        public void Enumeration_YieldsAllItemsAfterGrowth()
        {
            var list = new SfList<int>(2);
            list.Add(1);
            list.Add(2);
            list.Add(3); // triggers growth

            var items = list.ToArray();
            Assert.Equal(new[] { 1, 2, 3 }.ToList(), items.ToList());
        }
        
        [Fact]
        public void AddRange_AddsAllElements()
        {
            var list = new List<int>();
            list.Add(1);
            list.AddRange(new int[] { 2, 3, 4 });
            
            Assert.Equal(4, list.Count);
            Assert.Equal(new int[] { 1, 2, 3, 4 }, list.ToArray());
        }

        [Fact]
        public void TrimExcess_ReducesCapacity()
        {
            var list = new List<int>(32);
            for (int i = 0; i < 4; i++) list.Add(i);

            int oldCapacity = list.Capacity;
            list.TrimExcess();

            Assert.True(list.Capacity < oldCapacity);
            Assert.Equal(4, list.Count);
        }

        [Fact]
        public void Reverse_ReversesElements()
        {
            var list = new List<int>(new int[] { 1, 2, 3, 4 });
            list.Reverse();

            Assert.Equal(new int[] { 4, 3, 2, 1 }, list.ToArray());
        }

        [Fact]
        public void Exists_ReturnsTrueIfMatch()
        {
            var list = new List<int>(new int[] { 1, 2, 3 });
            Assert.True(list.Exists(x => x == 2));
            Assert.False(list.Exists(x => x == 5));
        }

        [Fact]
        public void Find_ReturnsCorrectElement()
        {
            var list = new List<int>(new int[] { 1, 2, 3 });
            int found = list.Find(x => x > 1);
            Assert.Equal(2, found);
        }

        [Fact]
        public void FindIndex_ReturnsCorrectIndex()
        {
            var list = new List<int>(new int[] { 5, 10, 15 });
            int index = list.FindIndex(x => x == 10);
            Assert.Equal(1, index);
            Assert.Equal(-1, list.FindIndex(x => x == 100));
        }

        [Fact]
        public void ForEach_AppliesActionToAll()
        {
            var list = new List<int>(new int[] { 1, 2, 3 });
            int sum = 0;
            list.ForEach(x => sum += x);

            Assert.Equal(6, sum);
        }
        
        [Fact]
        public void EnumeratorAllocationTest()
        {
            var myList = new SfList<int>();
            for (int i = 0; i < 1000; i++) myList.Add(i);

            foreach (var item in myList) { }
            
            long startBytes = GC.GetAllocatedBytesForCurrentThread();

            int total = 0;
            foreach (var item in myList)
            {
                total += item;
            }

            long endBytes = GC.GetAllocatedBytesForCurrentThread();
            long allocated = endBytes - startBytes;
            
            Assert.True(total > 0);
            Assert.Equal(0, allocated);
        }
    }
}

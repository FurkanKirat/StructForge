using StructForge.Collections;

namespace StructForge.Tests.Collections
{
    public class SfLinkedListTests
    {
        private SfLinkedList<int> CreateList()
        {
            return new SfLinkedList<int>();
        }

        [Fact]
        public void AddFirst_ShouldInsertElementAtHead()
        {
            var list = CreateList();
            list.AddFirst(10);

            Assert.Single(list);
            Assert.Equal(10, list.RemoveFirst());
        }

        [Fact]
        public void AddLast_ShouldInsertElementAtTail()
        {
            var list = CreateList();
            list.AddLast(1);
            list.AddLast(2);

            Assert.Equal(1, list.RemoveFirst());
            Assert.Equal(2, list.RemoveFirst());
        }

        [Fact]
        public void RemoveFirst_ShouldThrow_WhenEmpty()
        {
            var list = CreateList();
            Assert.Throws<InvalidOperationException>(() => list.RemoveFirst());
        }

        [Fact]
        public void RemoveLast_ShouldThrow_WhenEmpty()
        {
            var list = CreateList();
            Assert.Throws<InvalidOperationException>(() => list.RemoveLast());
        }

        [Fact]
        public void RemoveFirst_ShouldWorkCorrectly()
        {
            var list = CreateList();
            list.AddLast(1);
            list.AddLast(2);

            var removed = list.RemoveFirst();

            Assert.Equal(1, removed);
            Assert.Single(list);
        }

        [Fact]
        public void RemoveLast_ShouldWorkCorrectly()
        {
            var list = CreateList();
            list.AddLast(1);
            list.AddLast(2);

            var removed = list.RemoveLast();

            Assert.Equal(2, removed);
            Assert.Single(list);
        }

        [Fact]
        public void Find_ShouldReturnNode_WhenExists()
        {
            var list = CreateList();
            list.AddLast(5);
            list.AddLast(10);

            var node = list.Find(10);

            Assert.NotNull(node);
            Assert.Equal(10, node.Value);
        }

        [Fact]
        public void Find_ShouldReturnNull_WhenNotFound()
        {
            var list = CreateList();
            list.AddLast(1);
            list.AddLast(2);

            var node = list.Find(100);

            Assert.Null(node);
        }

        [Fact]
        public void Clear_ShouldRemoveAllElements()
        {
            var list = CreateList();
            list.AddLast(1);
            list.AddLast(2);
            list.Clear();

            Assert.True(list.IsEmpty);
            Assert.Empty(list);
        }

        [Fact]
        public void Contains_ShouldReturnTrue_WhenElementExists()
        {
            var list = CreateList();
            list.AddLast(1);
            list.AddLast(2);

            Assert.True(list.Contains(2));
        }

        [Fact]
        public void Contains_ShouldReturnFalse_WhenElementDoesNotExist()
        {
            var list = CreateList();
            list.AddLast(1);
            list.AddLast(2);

            Assert.False(list.Contains(100));
        }

        [Fact]
        public void ToArray_ShouldReturnCorrectOrder()
        {
            var list = CreateList();
            list.AddLast(1);
            list.AddLast(2);
            list.AddLast(3);

            var arr = list.ToArray();

            Assert.Equal(new[] { 1, 2, 3 }, arr);
        }

        [Fact]
        public void Enumerator_ShouldIterateCorrectly()
        {
            var list = CreateList();
            list.AddLast(1);
            list.AddLast(2);
            list.AddLast(3);

            int[] expected = { 1, 2, 3 };
            int index = 0;

            foreach (var item in list)
            {
                Assert.Equal(expected[index], item);
                index++;
            }
        }
        
        [Fact]
        public void AddFirstAndAddLast_ShouldMaintainOrder()
        {
            var list = CreateList();
            
            list.AddFirst(2);  // list: 2
            list.AddFirst(1);  // list: 1, 2
            list.AddLast(3);   // list: 1, 2, 3

            Assert.Equal(new[] { 1, 2, 3 }, list.ToArray());
            Assert.Equal(3, list.Count);
        }

        [Fact]
        public void RemoveFirstAndRemoveLast_ShouldUpdateHeadTailCorrectly()
        {
            var list = new SfLinkedList<int>(new[] { 10, 20, 30 });

            var first = list.RemoveFirst();  // removes 10
            var last = list.RemoveLast();    // removes 30

            Assert.Equal(10, first);
            Assert.Equal(30, last);

            Assert.Equal(new[] { 20 }, list.ToArray());
            Assert.Single(list);
        }

        [Fact]
        public void RemoveFromEmpty_ShouldThrow()
        {
            var list = CreateList();

            Assert.Throws<InvalidOperationException>(() => list.RemoveFirst());
            Assert.Throws<InvalidOperationException>(() => list.RemoveLast());
        }

        [Fact]
        public void ContainsAndFind_ShouldWorkWithCustomComparer()
        {
            var list = new SfLinkedList<string>(new[] { "apple", "banana", "cherry" });

            Assert.Contains("BANANA", list, StringComparer.OrdinalIgnoreCase);
            Assert.DoesNotContain("BANANA", list, StringComparer.Ordinal);

            var node = list.Find("cherry");
            Assert.NotNull(node);
            Assert.Equal("cherry", node.Value);
        }

        [Fact]
        public void ToArray_ShouldPreserveOrder()
        {
            var list = new SfLinkedList<char>();
            list.AddLast('a');
            list.AddLast('b');
            list.AddLast('c');

            var arr = list.ToArray();
            Assert.Equal(new[] { 'a', 'b', 'c' }, arr);
        }

        [Fact]
        public void Clear_ShouldEmptyListCompletely()
        {
            var list = new SfLinkedList<int>(Enumerable.Range(1, 5));

            list.Clear();

            Assert.Empty(list);
            Assert.True(list.IsEmpty);
            Assert.Equal(Array.Empty<int>(), list.ToArray());
        }

        [Fact]
        public void Enumerator_ShouldIterateInOrder()
        {
            var list = new SfLinkedList<int>(new[] { 100, 200, 300 });

            int[] result = list.ToArray();
            Assert.Equal(new[] { 100, 200, 300 }, result);

            // foreach test
            int sum = 0;
            foreach (var value in list)
                sum += value;

            Assert.Equal(600, sum);
        }

        [Fact]
        public void ComplexScenario_AddRemoveMix_ShouldMaintainIntegrity()
        {
            var list = new LinkedList<int>();
            
            // Add and remove in mixed order
            list.AddLast(1);       // 1
            list.AddLast(2);       // 1, 2
            list.AddFirst(0);      // 0, 1, 2
            list.RemoveFirst();    // remove 0 => 1, 2
            list.AddLast(3);       // 1, 2, 3
            list.RemoveLast();     // remove 3 => 1, 2
            list.AddFirst(-1);     // -1, 1, 2

            Assert.Equal(new[] { -1, 1, 2 }, list.ToArray());
            Assert.Equal(3, list.Count);
        }
        
        [Fact]
        public void Reverse_ShouldReverse()
        {
            var list = new SfLinkedList<int>();
            for (int i = 0; i < 50; i++)
            {
                list.AddLast(i);
            }
            list.Reverse();

            var list2 = new SfLinkedList<int>();
            for (int i = 0; i < 50; i++)
            {
                list2.AddFirst(i);
            }
            
            Assert.Equal(list, list2);    
            
        }
    
    }
}

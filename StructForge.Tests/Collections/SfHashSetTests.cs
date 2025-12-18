using StructForge.Collections;

namespace StructForge.Tests.Collections
{
    public class SfHashSetTests
    {
        [Fact]
        public void Add_SingleItem_ShouldContainItem()
        {
            var set = new SfHashSet<int>();
            set.Add(5);

            Assert.Contains(5, set);
            Assert.True(set.Count == 1);
            Assert.Single(set);
        }

        [Fact]
        public void Add_DuplicateItem_ShouldThrow()
        {
            var set = new SfHashSet<int>();
            set.Add(5);

            Assert.Throws<InvalidOperationException>(() => set.Add(5));
        }

        [Fact]
        public void TryAdd_DuplicateItem_ShouldReturnFalse()
        {
            var set = new SfHashSet<int>();
            Assert.True(set.TryAdd(5));
            Assert.False(set.TryAdd(5));
            Assert.Single(set);
        }

        [Fact]
        public void Remove_Item_ShouldDecreaseCount()
        {
            var set = new SfHashSet<int>();
            set.Add(5);
            var removed = set.Remove(5);

            Assert.True(removed);
            Assert.DoesNotContain(5, set);
            Assert.Empty(set);
        }

        [Fact]
        public void Clear_ShouldEmptySet()
        {
            var set = new SfHashSet<int>();
            set.Add(1);
            set.Add(2);

            set.Clear();

            Assert.Empty(set);
            Assert.DoesNotContain(1, set);
            Assert.DoesNotContain(2, set);
        }

        [Fact]
        public void UnionWith_ShouldContainAllItems()
        {
            var set1 = new SfHashSet<int> { 1, 2, 3 };
            var set2 = new SfHashSet<int> { 3, 4, 5 };

            set1.UnionWith(set2);

            for (int i = 1; i <= 5; i++)
                Assert.Contains(i, set1);
            Assert.Equal(5, set1.Count);
        }

        [Fact]
        public void IntersectWith_ShouldKeepOnlyCommonItems()
        {
            var set1 = new SfHashSet<int> { 1, 2, 3 };
            var set2 = new SfHashSet<int> { 2, 3, 4 };

            set1.IntersectWith(set2);

            Assert.Contains(2, set1);
            Assert.Contains(3, set1);
            Assert.DoesNotContain(1, set1);
            Assert.DoesNotContain(4, set1);
            Assert.Equal(2, set1.Count);
        }

        [Fact]
        public void ExceptWith_ShouldRemoveOtherItems()
        {
            var set1 = new SfHashSet<int> { 1, 2, 3 };
            var set2 = new SfHashSet<int> { 2, 3, 4 };

            set1.ExceptWith(set2);

            Assert.True(set1.Contains(1, null));
            Assert.DoesNotContain(2, set1);
            Assert.DoesNotContain(3, set1);
            Assert.Single(set1);
        }

        [Fact]
        public void SymmetricExceptWith_ComplexScenario_ShouldWorkCorrectly()
        {
            var set1 = new SfHashSet<int> { 1, 2, 3, 5, 7, 9 };
            var set2 = new SfHashSet<int> { 3, 4, 5, 6, 7 };
            
            set1.SymmetricExceptWith(set2);

            var expected = new HashSet<int> { 1, 2, 4, 6, 9 };

            foreach (var item in expected)
                Assert.True(set1.Contains(item), $"Set should contain {item}");

            Assert.Equal(expected.Count, set1.Count);

            Assert.DoesNotContain(3, set1);
            Assert.DoesNotContain(5, set1);
            Assert.DoesNotContain(7, set1);
        }


        [Fact]
        public void SetEquals_ShouldReturnTrueForSameItems()
        {
            var set1 = new SfHashSet<int> { 1, 2, 3 };
            var set2 = new SfHashSet<int> { 3, 2, 1 };

            Assert.True(set1.SetEquals(set2));
        }

        [Fact]
        public void Overlaps_ShouldDetectCommonItems()
        {
            var set1 = new SfHashSet<int> { 1, 2, 3 };
            var set2 = new SfHashSet<int> { 3, 4, 5 };

            Assert.True(set1.Overlaps(set2));
        }

        [Fact]
        public void IsSubsetOf_ShouldReturnTrueForSubset()
        {
            var set1 = new SfHashSet<int> { 1, 2 };
            var set2 = new SfHashSet<int> { 1, 2, 3 };

            Assert.True(set1.IsSubsetOf(set2));
            Assert.False(set2.IsSubsetOf(set1));
        }

        [Fact]
        public void IsSupersetOf_ShouldReturnTrueForSuperset()
        {
            var set1 = new SfHashSet<int> { 1, 2, 3 };
            var set2 = new SfHashSet<int> { 1, 2 };

            Assert.True(set1.IsSupersetOf(set2));
            Assert.False(set2.IsSupersetOf(set1));
        }
        

        [Fact]
        public void Remove_NonExistentItem_ShouldReturnFalse()
        {
            var set = new SfHashSet<int> { 1, 2, 3 };
            Assert.False(set.Remove(99));
            Assert.Equal(3, set.Count);
        }

        [Fact]
        public void Remove_WithCollisions_ShouldMaintainChain()
        {
            var comparer = new BadIntComparer(); 
            var set = new SfHashSet<int>(capacity: 10, comparer: comparer);

            set.Add(1);
            set.Add(2);
            set.Add(3);
            set.Add(4);
            set.Add(5);

            bool removed = set.Remove(3);

            Assert.True(removed);
            Assert.Equal(4, set.Count);
            Assert.False(set.Contains(3));
            
            Assert.True(set.Contains(1));
            Assert.True(set.Contains(2));
            Assert.True(set.Contains(4));
            Assert.True(set.Contains(5));
        }

        [Fact]
        public void Remove_HeadOfChain_ShouldUpdateBucket()
        {
            var comparer = new BadIntComparer();
            var set = new SfHashSet<int>(capacity: 10, comparer: comparer);
            
            set.Add(10);
            set.Add(20);
            set.Add(30);

            set.Remove(30);

            Assert.False(set.Contains(30));
            Assert.True(set.Contains(20));
            Assert.True(set.Contains(10));
        }

        [Fact]
        public void Remove_TailOfChain_ShouldUpdatePreviousNode()
        {
            var comparer = new BadIntComparer();
            var set = new SfHashSet<int>(capacity: 10, comparer: comparer);

            set.Add(10);
            set.Add(20);
            set.Add(30);

            set.Remove(10);

            Assert.False(set.Contains(10));
            Assert.True(set.Contains(20));
            Assert.True(set.Contains(30));
        }

        [Fact]
        public void Remove_RandomOperations_StressTest()
        {
            var set = new SfHashSet<int>();
            var tracker = new HashSet<int>();
            var random = new Random(42);

            for (int i = 0; i < 50; i++)
            {
                int val = random.Next(0, 500);
                bool add = random.NextDouble() > 0.5;

                if (add)
                {
                    bool added1 = set.TryAdd(val);
                    bool added2 = tracker.Add(val);
                    Assert.Equal(added2, added1);
                }
                else
                {
                    bool rem1 = set.Remove(val);
                    bool rem2 = tracker.Remove(val);
                    Assert.Equal(rem2, rem1);
                }
            }

            Assert.Equal(tracker.Count, set.Count);
            foreach (var item in tracker)
            {
                Assert.True(set.Contains(item));
            }
        }

        private class BadIntComparer : IEqualityComparer<int>
        {
            public bool Equals(int x, int y) => x == y;
            public int GetHashCode(int obj) => 1;
        }
    }
}

using StructForge.Collections;

namespace StructForge.Tests.Collections
{
    public class SfSortedDictionaryTests
    {
        private ISfDictionary<int, string> CreateDictionary()
        {
            return new SfSortedDictionary<int, string>(Comparer<int>.Default);
        }

        [Fact]
        public void Add_ShouldInsertKeyValue()
        {
            var dict = CreateDictionary();
            dict.Add(1, "One");

            Assert.True(dict.ContainsKey(1));
            Assert.Equal("One", dict[1]);
            Assert.Single(dict);
        }

        [Fact]
        public void TryAdd_ShouldReturnFalse_WhenKeyExists()
        {
            var dict = CreateDictionary();
            dict.Add(1, "One");

            bool added = dict.TryAdd(1, "Duplicate");

            Assert.False(added);
            Assert.Equal("One", dict[1]);
        }

        [Fact]
        public void TryAdd_ShouldAdd_WhenKeyDoesNotExist()
        {
            var dict = CreateDictionary();
            bool added = dict.TryAdd(2, "Two");

            Assert.True(added);
            Assert.Equal("Two", dict[2]);
        }

        [Fact]
        public void Remove_ShouldDeleteKey_WhenExists()
        {
            var dict = CreateDictionary();
            dict.Add(1, "One");

            bool removed = dict.Remove(1);

            Assert.True(removed);
            Assert.False(dict.ContainsKey(1));
        }

        [Fact]
        public void Remove_ShouldReturnFalse_WhenKeyNotExists()
        {
            var dict = CreateDictionary();

            bool removed = dict.Remove(999);

            Assert.False(removed);
        }

        [Fact]
        public void ContainsKey_ShouldDetectExistingKeys()
        {
            var dict = CreateDictionary();
            dict.Add(5, "Five");

            Assert.True(dict.ContainsKey(5));
            Assert.False(dict.ContainsKey(10));
        }

        [Fact]
        public void ContainsValue_ShouldDetectExistingValues()
        {
            var dict = CreateDictionary();
            dict.Add(1, "Apple");
            dict.Add(2, "Banana");

            Assert.True(dict.ContainsValue("Apple"));
            Assert.False(dict.ContainsValue("Orange"));
        }

        [Fact]
        public void TryGetValue_ShouldRetrieveValue_WhenKeyExists()
        {
            var dict = CreateDictionary();
            dict.Add(1, "One");

            bool found = dict.TryGetValue(1, out string value);

            Assert.True(found);
            Assert.Equal("One", value);
        }

        [Fact]
        public void TryGetValue_ShouldReturnFalse_WhenKeyMissing()
        {
            var dict = CreateDictionary();

            bool found = dict.TryGetValue(999, out string value);

            Assert.False(found);
            Assert.Null(value);
        }

        [Fact]
        public void Indexer_Get_ShouldThrow_WhenKeyMissing()
        {
            var dict = CreateDictionary();

            Assert.Throws<KeyNotFoundException>(() => { var _ = dict[123]; });
        }

        [Fact]
        public void Indexer_Set_ShouldAddOrUpdate()
        {
            var dict = CreateDictionary();

            dict[1] = "Hello";
            Assert.Equal("Hello", dict[1]);

            dict[1] = "Updated";
            Assert.Equal("Updated", dict[1]);
        }

        [Fact]
        public void ForEach_ShouldIterateAllItems()
        {
            var dict = CreateDictionary();
            dict.Add(1, "One");
            dict.Add(2, "Two");

            var list = new List<string>();
            dict.ForEach(kv => list.Add($"{kv.Key}:{kv.Value}"));

            Assert.Contains("1:One", list);
            Assert.Contains("2:Two", list);
        }

        [Fact]
        public void SfKeyValue_Deconstruct_WorksCorrectly()
        {
            var kv = new SfKeyValue<int, string>(42, "Answer");

            var (key, value) = kv; // Deconstruct

            Assert.Equal(42, key);
            Assert.Equal("Answer", value);
        }

        [Fact]
        public void CopyTo_ShouldCopyItemsToArray()
        {
            var dict = CreateDictionary();
            dict.Add(1, "One");
            dict.Add(2, "Two");

            var array = new SfKeyValue<int, string>[2];
            dict.CopyTo(array, 0);

            Assert.Contains(array, kv => kv.Key == 1 && kv.Value == "One");
            Assert.Contains(array, kv => kv.Key == 2 && kv.Value == "Two");
        }
    }
}

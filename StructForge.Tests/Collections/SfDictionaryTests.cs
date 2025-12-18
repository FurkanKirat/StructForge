namespace StructForge.Tests.Collections;

using Xunit;
using StructForge.Collections;

public class SfDictionaryTests
{
    [Fact]
    public void AddAndGet_ShouldWorkCorrectly()
    {
        var dict = new SfDictionary<string, int>();
        dict.Add("Bir", 1);
        dict.Add("İki", 2);

        Assert.Equal(1, dict["Bir"]);
        Assert.Equal(2, dict["İki"]);
        Assert.Equal(2, dict.Count);
    }

    [Fact]
    public void Indexer_Setter_ShouldUpdateExistingValue()
    {
        var dict = new SfDictionary<int, string>();
        dict.Add(10, "Eski");
        
        dict[10] = "Yeni";

        Assert.Equal("Yeni", dict[10]);
        Assert.Equal(1, dict.Count);
    }

    [Fact]
    public void Resize_ShouldPreserveData()
    {
        var dict = new SfDictionary<int, int>(capacity: 4);
        for (int i = 0; i < 100; i++)
        {
            dict.Add(i * 5, i * 10);
        }

        Assert.Equal(100, dict.Count);
        for (int i = 0; i < 100; i++)
        {
            Assert.True(dict.ContainsKey(i * 5));
            Assert.Equal(i * 10, dict[i * 5]);
        }
    }

    [Fact]
    public void Remove_ShouldWorkAndHandleCollisions()
    {
        var dict = new SfDictionary<int, int>();
        dict.Add(1, 10);
        dict.Add(2, 20);
        dict.Add(3, 30);

        bool removed = dict.Remove(2);
        
        Assert.True(removed);
        Assert.False(dict.ContainsKey(2));
        Assert.True(dict.ContainsKey(1));
        Assert.True(dict.ContainsKey(3));
        Assert.Equal(2, dict.Count);
    }
    
    [Fact]
    public void KeysAndValues_ShouldMatchContent()
    {
        var dict = new SfDictionary<string, int>
        {
            { "A", 1 },
            { "B", 2 }
        };
        
        var keys = dict.Keys.ToList();
        var values = dict.Values.ToList();

        Assert.Contains("A", keys);
        Assert.Contains("B", keys);
        Assert.Contains(1, values);
        Assert.Contains(2, values);
    }
    
    [Fact]
    public void Collision_Test_ShouldWorkCorrectly()
    {
        var keys = new[] { "A", "B", "C", "D", "E" };
        var dict = new SfDictionary<string, int>(
            comparer: new BadStringComparer()
        );

        foreach (var k in keys)
        {
            dict.Add(k, k.Length);
        }

        Assert.Equal(5, dict.Count);
        
        foreach (var k in keys)
        {
            Assert.True(dict.ContainsKey(k));
            Assert.Equal(k.Length, dict[k]);
        }

        dict.Remove("C");
        Assert.False(dict.ContainsKey("C"));
        Assert.True(dict.ContainsKey("A"));
        Assert.True(dict.ContainsKey("E"));
        Assert.Equal(4, dict.Count);
    }
    
    [Fact]
    public void Remove_WithCollisions_ShouldPreserveOtherValues()
    {
        var comparer = new BadStringComparer();
        var dict = new SfDictionary<string, int>(comparer: comparer);

        dict.Add("A", 10);
        dict.Add("B", 20);
        dict.Add("C", 30);
        dict.Add("D", 40);

        bool removed = dict.Remove("B");

        Assert.True(removed);
        Assert.Equal(3, dict.Count);
        Assert.False(dict.ContainsKey("B"));

        Assert.Equal(10, dict["A"]);
        Assert.Equal(30, dict["C"]);
        Assert.Equal(40, dict["D"]);
    }

    [Fact]
    public void Remove_And_AddAgain_ShouldWork()
    {
        var dict = new SfDictionary<int, int>();
        dict.Add(1, 100);
        dict.Remove(1);
        
        Assert.False(dict.ContainsKey(1));
        
        dict.Add(1, 200);
        Assert.True(dict.ContainsKey(1));
        Assert.Equal(200, dict[1]);
    }

    [Fact]
    public void Remove_StressTest_SwapLogic()
    {
        var dict = new SfDictionary<int, int>();
        int count = 10000;

        for (int i = 0; i < count; i++) dict.Add(i, i);

        for (int i = 0; i < count; i += 2)
        {
            Assert.True(dict.Remove(i));
        }

        Assert.Equal(count / 2, dict.Count);

        for (int i = 1; i < count; i += 2)
        {
            Assert.True(dict.ContainsKey(i), $"Key {i} should exist");
            Assert.Equal(i, dict[i]);
        }
    }
    
    [Fact]
    public void Remove_RandomOperations_StressTest()
    {
        var dictionary = new SfDictionary<int, int>();
        var tracker = new Dictionary<int, int>();
        var random = new Random(42);

        for (int i = 0; i < 5000; i++)
        {
            int val = random.Next(0, 500);
            bool add = random.NextDouble() > 0.5;

            if (add)
            {
                bool added1 = dictionary.TryAdd(val, val);
                bool added2 = tracker.TryAdd(val, val);
                Assert.Equal(added2, added1);
            }
            else
            {
                bool rem1 = dictionary.Remove(val);
                bool rem2 = tracker.Remove(val);
                Assert.Equal(rem2, rem1);
            }
        }

        Assert.Equal(tracker.Count, dictionary.Count);
        foreach (var item in tracker)
        {
            Assert.True(dictionary.TryGetValue(item.Key, out var val) && val == item.Value);
        }
    }
    
    public class BadStringComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y) => x == y;
        public int GetHashCode(string obj) => 1;
    }
    
}
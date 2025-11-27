using StructForge.Collections;

namespace StructForge.Tests.Collections;

public class SfDictionaryTests
{
    [Fact]
    public void Add_And_Get_Item_Works()
    {
        var dict = new SfDictionary<string, int>();
        dict.Add("one", 1);
        dict["two"] = 2;

        Assert.Equal(1, dict["one"]);
        Assert.Equal(2, dict["two"]);
    }

    [Fact]
    public void Update_Item_Using_Indexer_Works()
    {
        var dict = new SfDictionary<string, int>();
        dict.Add("a", 10);
        dict["a"] = 20;

        Assert.Equal(20, dict["a"]);
    }

    [Fact]
    public void TryGetValue_Works()
    {
        var dict = new SfDictionary<string, int>();
        dict.Add("x", 42);

        bool found = dict.TryGetValue("x", out int value);
        Assert.True(found);
        Assert.Equal(42, value);

        bool notFound = dict.TryGetValue("y", out _);
        Assert.False(notFound);
    }

    [Fact]
    public void Remove_Works()
    {
        var dict = new SfDictionary<string, int>();
        dict.Add("key", 99);

        bool removed = dict.Remove("key");
        Assert.True(removed);
        Assert.False(dict.ContainsKey("key"));
        Assert.Empty(dict);
    }

    [Fact]
    public void ContainsKey_And_ContainsValue_Works()
    {
        var dict = new SfDictionary<string, string>();
        dict.Add("k1", "v1");
        dict.Add("k2", "v2");

        Assert.True(dict.ContainsKey("k1"));
        Assert.False(dict.ContainsKey("k3"));

        Assert.True(dict.ContainsValue("v2"));
        Assert.False(dict.ContainsValue("v3"));
    }

    [Fact]
    public void Keys_And_Values_Enumeration_Works()
    {
        var dict = new SfDictionary<int, string>();
        dict.Add(1, "a");
        dict.Add(2, "b");

        var keys = dict.Keys;
        var values = dict.Values;

        Assert.Contains(1, keys);
        Assert.Contains(2, keys);
        Assert.Contains("a", values);
        Assert.Contains("b", values);
    }

    [Fact]
    public void Clear_Empties_Dictionary()
    {
        var dict = new SfDictionary<string, int>();
        dict.Add("x", 1);
        dict.Add("y", 2);

        dict.Clear();

        Assert.Empty(dict);
        Assert.False(dict.ContainsKey("x"));
        Assert.False(dict.ContainsKey("y"));
    }

    [Fact]
    public void TryAdd_Works_Correctly()
    {
        var dict = new SfDictionary<string, int>();
        bool firstAdd = dict.TryAdd("key", 100);
        bool secondAdd = dict.TryAdd("key", 200);

        Assert.True(firstAdd);
        Assert.False(secondAdd);
        Assert.Equal(100, dict["key"]);
    }
}
namespace StructForge.Tests.Collections;

using System;
using StructForge.Collections;
using Xunit;

public class SfStackTests
{
    [Fact]
    public void Push_IncreasesCount()
    {
        var stack = new SfStack<int>();
        stack.Push(1);
        stack.Push(2);

        Assert.Equal(2, stack.Count);
    }

    [Fact]
    public void Pop_ReturnsLastPushedItem()
    {
        var stack = new SfStack<int>();
        stack.Push(1);
        stack.Push(2);

        var item = stack.Pop();

        Assert.Equal(2, item);
        Assert.Single(stack);
    }

    [Fact]
    public void Peek_ReturnsLastPushedItemWithoutRemoving()
    {
        var stack = new SfStack<int>();
        stack.Push(1);
        stack.Push(2);

        var item = stack.Peek();

        Assert.Equal(2, item);
        Assert.Equal(2, stack.Count);
    }

    [Fact]
    public void IsEmpty_ReturnsTrueForEmptyStack()
    {
        var stack = new SfStack<int>();
        Assert.True(stack.IsEmpty);

        stack.Push(1);
        Assert.False(stack.IsEmpty);
    }

    [Fact]
    public void Contains_FindsItemUsingComparer()
    {
        var stack = new SfStack<string>();
        stack.Push("hello");
        stack.Push("world");

        bool contains = stack.Contains("world", StringComparer.Ordinal);
        Assert.True(contains);

        bool notContains = stack.Contains("test", StringComparer.Ordinal);
        Assert.False(notContains);
    }

    [Fact]
    public void CopyTo_CopiesElementsToArray()
    {
        var stack = new SfStack<int>();
        stack.Push(1);
        stack.Push(2);
        stack.Push(3);

        int[] array = new int[5];
        stack.CopyTo(array, 1);

        Assert.Equal(3, array[1]);
        Assert.Equal(2, array[2]);
        Assert.Equal(1, array[3]);
    }

    [Fact]
    public void ForEach_CallsActionOnAllItems()
    {
        var stack = new SfStack<int>();
        stack.Push(1);
        stack.Push(2);
        stack.Push(3);

        int sum = 0;
        
        stack.ForEach(x => sum += x);

        Assert.Equal(6, sum);
    }
}

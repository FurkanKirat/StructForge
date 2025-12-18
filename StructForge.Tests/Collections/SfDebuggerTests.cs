using Xunit.Abstractions;
using StructForge.Collections;

namespace StructForge.Tests.Collections;

public class SfDebuggerTests(ITestOutputHelper testOutputHelper)
{
    public enum SfTestEnum { Idle, Running, Paused, Stopped, Error }

    [Fact]
    public void RunSanityCheck()
    {
        testOutputHelper.WriteLine("Starting SfDebuggerTests...");

        // 1. SfAvlTree
        var avl = new SfAvlTree<int>();
        avl.Add(10); avl.Add(5); avl.Add(15);

        // 2. SfBinaryHeap
        var heap = new SfBinaryHeap<int>();
        heap.Add(10); heap.Add(20); heap.Add(5);

        // 3. SfBitArray
        var bitArray = new SfBitArray(8);
        bitArray[1] = true; bitArray[5] = true;

        // 4. SfBitArray2D
        var bitArray2D = new SfBitArray2D(4, 4);
        bitArray2D[1, 1] = true; bitArray2D[2, 3] = true;

        // 5. SfBitArray3D
        var bitArray3D = new SfBitArray3D(3, 3, 3);
        bitArray3D[1, 1, 1] = true;

        // 6. SfDictionary
        var dict = new SfDictionary<int, string>();
        dict.Add(1, "Struct"); dict.Add(2, "Forge");

        // 7. SfEnumSet
        var enumSet = new SfEnumSet<SfTestEnum>();
        enumSet.Add(SfTestEnum.Running); enumSet.Add(SfTestEnum.Error);

        // 8. SfGraph
        var graph = new SfGraph<int>();
         graph.AddEdge(1, 2, 1f);

        // 9. SfGrid2D
        var grid2D = new SfGrid2D<int>(3, 3);
        grid2D[0, 0] = 99; grid2D[1, 1] = 88;

        // 10. SfGrid3D
        var grid3D = new SfGrid3D<int>(2, 2, 2);
        grid3D[0, 0, 0] = 77;

        // 11. SfHashSet
        var hashSet = new SfHashSet<string>();
        hashSet.Add("Alpha"); hashSet.Add("Beta");

        // 12. SfLinkedList
        var linkedList = new SfLinkedList<int>();
        linkedList.AddLast(100); linkedList.AddLast(200);

        // 13. SfList
        var list = new SfList<double>();
        list.Add(3.14); list.Add(2.71);

        // 14. SfPriorityQueue
        var pq = new SfPriorityQueue<string, int>();
        pq.Enqueue("Critical Task", 1); pq.Enqueue("Low Task", 10);

        // 15. SfQueue
        var queue = new SfQueue<int>();
        queue.Enqueue(1); queue.Enqueue(2);

        // 16. SfRingBuffer
        var ringBuffer = new SfRingBuffer<int>(5);
        ringBuffer.Enqueue(10); ringBuffer.Enqueue(20);

        // 17. SfSortedSet
        var sortedSet = new SfSortedSet<int>();
        sortedSet.Add(5); sortedSet.Add(1); sortedSet.Add(10);

        // 18. SfStack
        var stack = new SfStack<string>();
        stack.Push("Bottom"); stack.Push("Top");

        testOutputHelper.WriteLine("Test Ended.");
    }
}
# StructForge

[![NuGet](https://img.shields.io/nuget/v/StructForge.svg)](https://www.nuget.org/packages/StructForge/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

**StructForge** is a lightweight, educational, and practical C# library for learning and experimenting with fundamental data structures and algorithms. It provides a range of generic collections, sorting algorithms, and interfaces for building your own high-level structures.

---

## Features

### Collections

* **Lists**: `SfList<T>`, `SfLinkedList<T>`
* **Stacks & Queues**: `SfStack<T>`, `SfQueue<T>`
* **Heaps & Priority Queues**: `SfBinaryHeap<T>`, `SfMaxHeap<T>`, `SfMinHeap<T>`, `SfPriorityQueue<TItem, TPriority>`
* **Binary Search Trees**: `SfBinarySearchTree<T>`
* **Trees**: `SfBinarySearchTree<T>, SfAvlTree<T>`
* **Sets & Dictionaries**: `SfSortedSet<T>, SfSortedDictionary<TKey, TValue>`

### Sorting Algorithms

* **QuickSort**
* **TreeSort**

### Interfaces

* `ISfDataStructure<T>`: Base interface for all collections
* `ISfList<T>`: List interface
* `ISfLinkedList<T>`: Doubly-linked list interface
* `ISfStack<T>`: Stack interface
* `ISfQueue<T>`: Queue interface
* `ISfHeap<T>`: Generic heap interface
* `ISfTree<T>`: Tree interface
* `ISfDictionary<T>`: Dictionary interface

###  Algorithms & Utilities

* Sorting

SfSorting.QuickSort(...)

SfSorting.TreeSort(...)

SfSorting.HeapSort(...)

* Searching

SfAlgorithms.BinarySearch(...)

* Randomization

SfAlgorithms.Shuffle(...)

* Comparers

SfComparers<T> – default comparer access

SfComparerUtils – helper utilities for key/value and custom comparers

### Spatial & Memory Optimized (New in v1.2.0)

* **High-Performance Grids**: `SfGrid2D<T>`, `SfGrid3D<T>` (Flattened array implementation for CPU cache locality)
* **Bit Manipulation**: `SfBitArray` (Stores booleans as bits, reducing memory usage by 8x)
* **Buffers**: `SfRingBuffer<T>` (Fixed-size circular queue for zero-allocation data streaming)

### Key Capabilities

* Fully generic implementations
* Iteration and enumeration support
* Custom comparers for heaps, priority queues, and sorting
* Educational and practical reference for learning C# data structures

---

## Installation

You can install the latest version via **NuGet**:

```bash
dotnet add package StructForge
```

Or clone the repository and include the `StructForge` project in your solution:

```bash
git clone https://github.com/FurkanKirat/StructForge.git
```

---

## Usage Examples

### 2D Grid
```csharp
// SfGrid2D uses a single 1D array internally for better CPU cache locality
var grid = new SfGrid2D<int>(width: 10, height: 10);

// Access like a standard 2D array
grid[5, 5] = 42;

// Fast iteration (Linear memory access)
foreach (var item in grid)
{
    // Process item...
}
```

### BitArray
```csharp
// Stores 1024 booleans using only ~128 bytes (8x memory saving)
var bits = new SfBitArray(1024);

bits[100] = true;
bits.Toggle(50);

Console.WriteLine($"True Bits: {bits.CountTrue()}"); // Optimized population count
```

### Ring Buffer
```csharp
// Fixed-size buffer that overwrites old data automatically
var buffer = new SfRingBuffer<string>(capacity: 3);

buffer.Enqueue("Log 1");
buffer.Enqueue("Log 2");
buffer.Enqueue("Log 3");
buffer.Enqueue("Log 4"); // Overwrites "Log 1"

// No resizing, no garbage collection pressure
foreach (var log in buffer)
    Console.WriteLine(log); // Output: Log 2, Log 3, Log 4
```
### Sorted Dictionary

```csharp
var dict = new SfSortedDictionary<int, string>();
dict.Add(3, "apple");
dict.Add(1, "banana");
dict.Add(2, "cherry");

foreach (var kv in dict)
Console.WriteLine($"{kv.Key}: {kv.Value}");
// Output:
// 1: banana
// 2: cherry
// 3: apple
```

### Shuffle and Binary Search

```csharp
int[] data = { 1, 2, 3, 4, 5, 6 };
int index = SfSearching.BinarySearch(data, 4);
Console.WriteLine(index);
// Output: 3

```
### AVL Tree

```csharp
var avl = new SfAvlTree<int>();
avl.Add(10);
avl.Add(5);
avl.Add(15);
avl.Add(7);

Console.WriteLine($"Min: {avl.FindMin()}, Max: {avl.FindMax()}, Count: {avl.Count}");
// Output: Min: 5, Max: 15, Count: 4
```

### Linked List

```csharp
var list = new SfLinkedList<int>();
list.AddLast(1);
list.AddLast(2);
list.AddFirst(0);

foreach (var item in list)
    Console.WriteLine(item); // 0, 1, 2
```

### Priority Queue

```csharp
var pq = new SfPriorityQueue<string, int>();
pq.Enqueue("low", 5);
pq.Enqueue("high", 1);
pq.Enqueue("medium", 3);

foreach (var item in pq.EnumerateByPriority())
    Console.WriteLine(item); // "high", "medium", "low"
```

### QuickSort

```csharp
int[] arr = { 5, 2, 9, 1, 5, 6 };
SfSorting.QuickSort(arr);
Console.WriteLine(string.Join(", ", arr)); // 1, 2, 5, 5, 6, 9
```

---

## Contribution

Contributions are welcome! Feel free to open issues, add features, or improve existing code. Keep in mind that this library is primarily **educational**.

---

## License

MIT License – see [LICENSE](LICENSE) for details.
# StructForge

**StructForge** is a lightweight, educational, and practical C# library for learning and experimenting with fundamental data structures and algorithms. It provides a range of generic collections, sorting algorithms, and interfaces for building your own high-level structures.

---

## Features

### Collections

* **Lists**: `SfList<T>`, `SfLinkedList<T>` (with nodes and full linked list operations)
* **Stacks & Queues**: `SfStack<T>`, `SfQueue<T>`
* **Heaps & Priority Queues**: `SfBinaryHeap<T>`, `SfMaxHeap<T>`, `SfMinHeap<T>`, `SfPriorityQueue<TItem, TPriority>`
* **Binary Search Trees**: `SfBinarySearchTree<T>`

### Sorting Algorithms

* **QuickSort**
* **TreeSort**

### Interfaces

* `IDataStructure<T>`: Base interface for all collections
* `IHeap<T>`: Generic heap interface
* `ILinkedList<T>`: Doubly-linked list interface
* `IQueue<T>`: Queue interface
* `ISequence<T>`: Sequence interface
* `IStack<T>`: Stack interface
* `ITree<T>`: Tree interface

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
git clone https://github.com/yourusername/StructForge.git
```

---

## Usage Examples

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
Quicksort.Sort(arr);
Console.WriteLine(string.Join(", ", arr)); // 1, 2, 5, 5, 6, 9
```

---

## Contribution

Contributions are welcome! Feel free to open issues, add features, or improve existing code. Keep in mind that this library is primarily **educational**.

---

## License

MIT License – see [LICENSE](LICENSE) for details.

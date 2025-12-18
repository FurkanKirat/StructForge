# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.5.0] - 2025-12-18

### 🚀 Added
- **SfDictionary:** Re-introduced allocation-free dictionary implementation using open addressing.
- **SfGraph:** Added core Graph data structure (Adjacency List).
- **Debugger Visualizers:** Added `DebuggerTypeProxy` for all collections. You can now inspect `SfBitArray`, `SfGraph`, and `SfDictionary` internals easily in Visual Studio.
- **SfEnumSet:** Added proper string formatting for debug views.

### ✨ Improved
- **SfList:** Optimized `RemoveAtSwap` performance.
- **General:** Added explicit `[MethodImpl(AggressiveInlining)]` to hot-path properties in spatial grids.
- **Documentation:** Updated README with new benchmarks, updated collection list, and usage examples.

---

## [1.4.0]
### 🚀 Added
- **Unity Support:** Added `package.json` and `.asmdef` definitions. The library is now fully compatible with Unity 2021.3+ via OpenUPM.
- **SfEnumSet<T>:** Introduced zero-allocation set specifically for Enums (~1.7x faster than HashSet).
- **Serialization:** Added `SfEnumSet(ulong[] buffer)` constructor for easy serialization.

### ⚡ Performance
- **SfBitArray & SfGrid:** Implemented `Unsafe` address arithmetic for `GetUnchecked` / `SetUnchecked` methods, bypassing JIT bounds checks.
- **SfList:** Reverted enumerator logic to standard array access to enable JIT optimizations (Loop Unrolling).

---

## [1.3.0]
### ⚠️ Removed
- **SfDictionary & SfSortedDictionary:** Removed temporarily to re-architect for strict zero-allocation standards.
- **Interfaces:** Cleaned up related interfaces and unused extension files.

### 🧹 Improved
- **Benchmarks:** Added new benchmarks for various collections and updated existing tests.
- **Refactor:** Streamlined the codebase to focus on core supported data structures.

---

## [1.2.0]
### 🚀 Added
- **SfGrid2D & SfGrid3D:** Added flattened arrays for cache locality optimization.
- **SfBitArray:** Added 1D optimized boolean storage using bit-packing.
- **SfRingBuffer:** Added Zero-allocation circular queue implementation.
- **Serialization:** Exposed `GetRawData()` methods for serialization support.

---

## [1.1.0]
### 🚀 Added
- **SfAvlTree<T>:** Added self-balancing binary search tree implementation.
- **SfSortedSet<T>:** Added ordered set based on AVL tree.
- **SfSortedDictionary<TKey, TValue>:** Added ordered key–value dictionary.
- **Interfaces:** Added `ISfSet<T>` and `ISfDictionary<TKey, TValue>` for standardized API.
- **Utilities:** Added `SfComparers`, `SfComparerUtils`, and `SfAlgorithms` (Shuffle, BinarySearch).

### 🧹 Improved
- **API:** Enhanced consistency in collection API design.
- **Documentation:** Improved type safety and XML documentation comments for all interfaces.

---

## [1.0.0]
- Initial release of the **StructForge** library.
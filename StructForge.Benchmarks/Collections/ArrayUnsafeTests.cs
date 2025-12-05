using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace StructForge.Benchmarks.Collections
{
    /// <summary>
    /// Isolates the performance impact of:
    /// 1. Bounds checking (if statements)
    /// 2. Unsafe code (Unsafe.Add)
    /// 3. Ref return pattern
    /// </summary>
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net80, warmupCount: 3, iterationCount: 10)]
    public class BoundsCheckIsolationBenchmark
    {
        private int[] _buffer;
        private int[] _randomIndices;
        private const int Size = 10000;
        private const int Iterations = 10000;

        [GlobalSetup]
        public void Setup()
        {
            var random = new Random(42);
            _buffer = new int[Size];
            _randomIndices = new int[Iterations];
            
            for (int i = 0; i < Iterations; i++)
            {
                _randomIndices[i] = random.Next(0, Size);
                _buffer[i % Size] = random.Next();
            }
        }

        // ========== DIRECT ARRAY ACCESS ==========

        [Benchmark(Description = "1. Direct Array (with bounds check)", Baseline = true)]
        public long DirectArray()
        {
            long sum = 0;
            for (int i = 0; i < Iterations; i++)
                sum += _buffer[_randomIndices[i]];
            return sum;
        }

        // ========== UNSAFE WITHOUT BOUNDS CHECK ==========

        [Benchmark(Description = "2. Unsafe.Add (no bounds check)")]
        public long UnsafeNoBoundsCheck()
        {
            long sum = 0;
            ref var baseRef = ref MemoryMarshal.GetReference(_buffer.AsSpan());
            for (int i = 0; i < Iterations; i++)
                sum += Unsafe.Add(ref baseRef, _randomIndices[i]);
            return sum;
        }

        // ========== UNSAFE WITH BOUNDS CHECK ==========

        [Benchmark(Description = "3. Unsafe.Add (WITH bounds check)")]
        public long UnsafeWithBoundsCheck()
        {
            long sum = 0;
            ref var baseRef = ref MemoryMarshal.GetReference(_buffer.AsSpan());
            for (int i = 0; i < Iterations; i++)
            {
                int index = _randomIndices[i];
                if ((uint)index >= (uint)Size)
                    throw new IndexOutOfRangeException();
                sum += Unsafe.Add(ref baseRef, index);
            }
            return sum;
        }

        // ========== DIRECT ARRAY WITHOUT JIT BOUNDS CHECK ELIMINATION ==========

        [Benchmark(Description = "4. Direct Array (forced bounds check)")]
        public long DirectArrayForcedBoundsCheck()
        {
            long sum = 0;
            for (int i = 0; i < Iterations; i++)
            {
                int index = _randomIndices[i];
                // Access with explicit check that JIT can't eliminate
                if ((uint)index >= (uint)_buffer.Length)
                    throw new IndexOutOfRangeException();
                sum += _buffer[index];
            }
            return sum;
        }
    }

    /// <summary>
    /// Tests ref return vs get+set pattern
    /// </summary>
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net80, warmupCount: 3, iterationCount: 10)]
    public class RefReturnIsolationBenchmark
    {
        private int[] _buffer;
        private int[] _randomIndices;
        private const int Size = 10000;
        private const int Iterations = 10000;

        [GlobalSetup]
        public void Setup()
        {
            var random = new Random(42);
            _buffer = new int[Size];
            _randomIndices = new int[Iterations];
            
            for (int i = 0; i < Iterations; i++)
            {
                _randomIndices[i] = random.Next(0, Size);
                _buffer[i % Size] = random.Next();
            }
        }

        // ========== REF RETURN (UNSAFE) ==========

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ref int GetRefUnsafe(int index)
        {
            ref var baseRef = ref MemoryMarshal.GetReference(_buffer.AsSpan());
            return ref Unsafe.Add(ref baseRef, index);
        }

        [Benchmark(Description = "Ref Return Unsafe (no bounds)", Baseline = true)]
        public void RefReturnUnsafe()
        {
            for (int i = 0; i < Iterations; i++)
                GetRefUnsafe(_randomIndices[i]) *= 2;
        }

        // ========== REF RETURN (SAFE) ==========

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ref int GetRefSafe(int index)
        {
            return ref _buffer[index];
        }

        [Benchmark(Description = "Ref Return Safe (with bounds)")]
        public void RefReturnSafe()
        {
            for (int i = 0; i < Iterations; i++)
                GetRefSafe(_randomIndices[i]) *= 2;
        }

        // ========== GET + SET PATTERN ==========

        [Benchmark(Description = "Get+Set Pattern (double access)")]
        public void GetSetPattern()
        {
            for (int i = 0; i < Iterations; i++)
            {
                int index = _randomIndices[i];
                _buffer[index] = _buffer[index] * 2;
            }
        }
    }

    /// <summary>
    /// Grid2D specific: 2D coordinate calculation overhead
    /// </summary>
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net80, warmupCount: 3, iterationCount: 10)]
    public class Grid2DCalculationBenchmark
    {
        private int[] _buffer;
        private int[] _randomX;
        private int[] _randomY;
        private const int Width = 100;
        private const int Height = 100;
        private const int Iterations = 10000;

        [GlobalSetup]
        public void Setup()
        {
            var random = new Random(42);
            _buffer = new int[Width * Height];
            _randomX = new int[Iterations];
            _randomY = new int[Iterations];
            
            for (int i = 0; i < Iterations; i++)
            {
                _randomX[i] = random.Next(0, Width);
                _randomY[i] = random.Next(0, Height);
                _buffer[i % (Width * Height)] = random.Next();
            }
        }

        // ========== SAFE: BOUNDS CHECK + CALCULATION ==========

        [Benchmark(Description = "Safe: Bounds + Calc + Array Access", Baseline = true)]
        public long SafeWithBoundsCheck()
        {
            long sum = 0;
            for (int i = 0; i < Iterations; i++)
            {
                int x = _randomX[i];
                int y = _randomY[i];
                
                // Bounds check
                if ((uint)x >= (uint)Width || (uint)y >= (uint)Height)
                    throw new IndexOutOfRangeException();
                
                // Calculate and access
                sum += _buffer[y * Width + x];
            }
            return sum;
        }

        // ========== UNSAFE: NO BOUNDS CHECK + CALCULATION ==========

        [Benchmark(Description = "Unsafe: Calc + Unsafe.Add")]
        public long UnsafeNoBoundsCheck()
        {
            long sum = 0;
            ref var baseRef = ref MemoryMarshal.GetReference(_buffer.AsSpan());
            for (int i = 0; i < Iterations; i++)
            {
                int x = _randomX[i];
                int y = _randomY[i];
                sum += Unsafe.Add(ref baseRef, y * Width + x);
            }
            return sum;
        }

        // ========== SAFE: NO BOUNDS CHECK + CALCULATION ==========

        [Benchmark(Description = "Safe: Calc + Array Access (no check)")]
        public long SafeNoBoundsCheck()
        {
            long sum = 0;
            for (int i = 0; i < Iterations; i++)
            {
                int x = _randomX[i];
                int y = _randomY[i];
                // No explicit bounds check - trust the data
                sum += _buffer[y * Width + x];
            }
            return sum;
        }

        // ========== PRE-CALCULATED INDEX ==========

        [Benchmark(Description = "Pre-calculated Index + Array Access")]
        public long PreCalculatedIndex()
        {
            long sum = 0;
            for (int i = 0; i < Iterations; i++)
            {
                int index = _randomY[i] * Width + _randomX[i];
                sum += _buffer[index];
            }
            return sum;
        }
    }

    /// <summary>
    /// BitArray specific: bit manipulation overhead
    /// </summary>
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net80, warmupCount: 3, iterationCount: 10)]
    public class BitArrayCalculationBenchmark
    {
        private ulong[] _bits;
        private int[] _randomIndices;
        private const int Size = 10000;
        private const int Iterations = 10000;

        [GlobalSetup]
        public void Setup()
        {
            var random = new Random(42);
            _bits = new ulong[(Size + 63) / 64];
            _randomIndices = new int[Iterations];
            
            for (int i = 0; i < Iterations; i++)
            {
                _randomIndices[i] = random.Next(0, Size);
            }
        }

        // ========== SAFE GET ==========

        [Benchmark(Description = "BitArray Safe Get", Baseline = true)]
        public int SafeGet()
        {
            int count = 0;
            for (int i = 0; i < Iterations; i++)
            {
                int index = _randomIndices[i];
                int blockIndex = index >> 6;
                int bitIndex = index & 63;
                if ((_bits[blockIndex] & (1UL << bitIndex)) != 0)
                    count++;
            }
            return count;
        }

        // ========== UNSAFE GET ==========

        [Benchmark(Description = "BitArray Unsafe Get")]
        public int UnsafeGet()
        {
            int count = 0;
            ref ulong baseRef = ref MemoryMarshal.GetReference(_bits.AsSpan());
            for (int i = 0; i < Iterations; i++)
            {
                int index = _randomIndices[i];
                ulong block = Unsafe.Add(ref baseRef, index >> 6);
                if ((block & (1UL << (index & 63))) != 0)
                    count++;
            }
            return count;
        }

        // ========== SAFE SET ==========

        [Benchmark(Description = "BitArray Safe Set")]
        public void SafeSet()
        {
            for (int i = 0; i < Iterations; i++)
            {
                int index = _randomIndices[i];
                int blockIndex = index >> 6;
                int bitIndex = index & 63;
                bool value = (i & 1) == 0;
                
                if (value)
                    _bits[blockIndex] |= (1UL << bitIndex);
                else
                    _bits[blockIndex] &= ~(1UL << bitIndex);
            }
        }

        // ========== UNSAFE SET ==========

        [Benchmark(Description = "BitArray Unsafe Set")]
        public void UnsafeSet()
        {
            ref ulong baseRef = ref MemoryMarshal.GetReference(_bits.AsSpan());
            for (int i = 0; i < Iterations; i++)
            {
                int index = _randomIndices[i];
                ref ulong blockRef = ref Unsafe.Add(ref baseRef, index >> 6);
                bool value = (i & 1) == 0;
                ulong mask = 1UL << (index & 63);
                
                if (value)
                    blockRef |= mask;
                else
                    blockRef &= ~mask;
            }
        }
    }
}
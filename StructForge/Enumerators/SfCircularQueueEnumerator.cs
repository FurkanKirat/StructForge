using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace StructForge.Enumerators
{
    /// <inheritdoc/>
    public struct SfCircularQueueEnumerator<T> : IEnumerator<T>
    {
        private readonly T[] _buffer;
        
        private readonly int _capacity;
        private readonly int _count;
        private readonly int _head;
        
        private int _index;
        private int _currentOffset;
        
        /// <summary>
        /// Gives the current element's reference
        /// </summary>
        public ref T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                ref var baseRef = ref _buffer[0];
                return ref Unsafe.Add(ref baseRef, _currentOffset);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal SfCircularQueueEnumerator(T[] buffer, int head, int count)
        {
            _buffer = buffer;
            _head = head;
            _count = count;
            _capacity = buffer.Length;
            _index = -1;
            _currentOffset = -1;
        }
        
        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            if (++_index >= _count) return false;
            
            if (_index == 0)
                _currentOffset = _head;
            else
            {
                _currentOffset++;
                if (_currentOffset == _capacity) _currentOffset = 0;
            }
            return true;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            _index = -1;
            _currentOffset = -1;
        }
        
        T IEnumerator<T>.Current => Current;
        object IEnumerator.Current => Current;

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        { }
    }
}
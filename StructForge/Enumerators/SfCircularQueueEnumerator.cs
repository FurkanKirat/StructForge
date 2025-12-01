using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace StructForge.Enumerators
{
    public struct SfCircularQueueEnumerator<T> : IEnumerator<T>
    {
        private readonly T[] _buffer;
        
        private readonly int _capacity;
        private readonly int _count;
        private readonly int _head;
        
        private int _index;
        private int _currentOffset;
        
        public ref T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _buffer[_currentOffset];
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            _index = -1;
            _currentOffset = -1;
        }
        
        T IEnumerator<T>.Current => _buffer[_currentOffset];
        object IEnumerator.Current => _buffer[_currentOffset];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        { }
    }
}
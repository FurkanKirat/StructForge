using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace StructForge.Enumerators
{
    /// <summary>
    /// A high-performance, allocation-free enumerator shared by SfGrid structures.
    /// Works on any backing flat array.
    /// </summary>
    public struct SfArrayEnumerator<T> : IEnumerator<T>
    {
        private readonly T[] _buffer;
        private readonly int _length;
        private int _index;
        
        /// <summary>
        /// Gives the current element's reference
        /// </summary>
        public ref T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _buffer[_index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal SfArrayEnumerator(T[] buffer, int count)
        {
            _buffer = buffer;
            _length = count;
            _index = -1;
        }
        
        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            int nextIndex = _index + 1;
            if (nextIndex < _length)
            {
                _index = nextIndex;
                return true;
            }
            return false;
        }

        T IEnumerator<T>.Current => Current;
        object IEnumerator.Current => Current;

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset() => _index = -1;
        /// <inheritdoc/>
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() { }
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace StructForge.Enumerators
{
    /// <inheritdoc/>
    public struct SfReverseArrayEnumerator<T> : IEnumerator<T>
    {
        private readonly T[] _buffer;
        private readonly int _count;
        private int _index;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal SfReverseArrayEnumerator(T[] buffer, int count)
        {
            _buffer = buffer;
            _count = count;
            _index = count; 
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            if (--_index >= 0)
            {
                return true;
            }
            _index = -1; 
            return false;
        }

        /// <summary>
        /// Gives the current element's reference
        /// </summary>
        public ref T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _buffer[_index];
        }

        T IEnumerator<T>.Current => Current;
        object IEnumerator.Current => Current;

        /// <inheritdoc/>
        public void Reset() => _index = _count;

        /// <inheritdoc/>
        public void Dispose() { }
    }
    
}
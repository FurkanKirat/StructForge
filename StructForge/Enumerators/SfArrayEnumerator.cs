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
        private readonly T[] _array;
        private readonly int _length;
        private int _index;
        
        public ref T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _array[_index];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal SfArrayEnumerator(T[] array, int count)
        {
            _array = array;
            _length = count;
            _index = -1;
        }
        
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

        T IEnumerator<T>.Current => _array[_index];
        object IEnumerator.Current => _array[_index];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset() => _index = -1;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() { }
    }
}
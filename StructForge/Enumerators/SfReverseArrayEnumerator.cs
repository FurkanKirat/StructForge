using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace StructForge.Enumerators
{
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

        public ref T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _buffer[_index];
        }

        T IEnumerator<T>.Current => _buffer[_index];
        object IEnumerator.Current => _buffer[_index];

        public void Reset() => _index = _count;

        public void Dispose() { }
    }
    
}
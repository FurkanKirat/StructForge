using System;
using System.Diagnostics;
using StructForge.Comparers;

namespace StructForge.Collections
{
    /// <summary>
    /// Represents a key-value pair used internally by StructForge collections such as
    /// <see cref="SfSortedDictionary{TKey, TValue}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    [DebuggerDisplay("{Key} => {Value}")]
    public class SfKeyValue<TKey, TValue> : IEquatable<SfKeyValue<TKey, TValue>>
    {
        /// <summary>
        /// Gets the key associated with this key-value pair.
        /// </summary>
        public TKey Key { get; internal set; }

        /// <summary>
        /// Gets or sets the value associated with the key.
        /// </summary>
        public TValue Value { get; internal set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SfKeyValue{TKey, TValue}"/> class
        /// using the specified key and value.
        /// </summary>
        /// <param name="key">The key component of the pair.</param>
        /// <param name="value">The value component of the pair.</param>
        public SfKeyValue(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        /// <summary>
        /// Deconstructs the key-value pair into separate key and value variables.
        /// </summary>
        /// <param name="key">Receives the key.</param>
        /// <param name="value">Receives the value.</param>
        public void Deconstruct(out TKey key, out TValue value)
        {
            key = Key;
            value = Value;
        }

        /// <summary>
        /// Returns a string representation of the key and value pair in the format "key => value".
        /// </summary>
        public override string ToString() => $"{Key} => {Value}";

        /// <summary>
        /// Determines whether this instance is equal to another <see cref="SfKeyValue{TKey, TValue}"/> instance.
        /// </summary>
        /// <param name="other">The other <see cref="SfKeyValue{TKey, TValue}"/> to compare with.</param>
        public bool Equals(SfKeyValue<TKey, TValue> other)
        {
            return SfEqualityComparers<TKey>.Default.Equals(Key, other.Key) &&
                   SfEqualityComparers<TValue>.Default.Equals(Value, other.Value);
        }

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is SfKeyValue<TKey, TValue> other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Key, Value);

        /// <summary>
        /// Compares two <see cref="SfKeyValue{TKey, TValue}"/> instances for equality.
        /// </summary>
        public static bool operator ==(SfKeyValue<TKey, TValue> left, SfKeyValue<TKey, TValue> right) =>
            left?.Equals(right) == true;

        /// <summary>
        /// Compares two <see cref="SfKeyValue{TKey, TValue}"/> instances for inequality.
        /// </summary>
        public static bool operator !=(SfKeyValue<TKey, TValue> left, SfKeyValue<TKey, TValue> right) =>
            !(left == right);
    }
}

namespace StructForge.Collections
{
    /// <summary>
    /// Represents a key-value mapping structure that provides fast lookups,
    /// insertions, and deletions based on keys.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    public interface ISfDictionary<in TKey, TValue>
    {
        /// <summary>
        /// Determines whether the dictionary contains the specified key.
        /// </summary>
        bool ContainsKey(TKey key);

        /// <summary>
        /// Determines whether the dictionary contains at least one entry
        /// with the specified value.
        /// </summary>
        bool ContainsValue(TValue value);

        /// <summary>
        /// Attempts to get the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <param name="value">
        /// When this method returns, contains the value associated with the specified key,
        /// if the key is found; otherwise, the default value for the type of the value parameter.
        /// </param>
        /// <returns><see langword="true"/> if the key was found; otherwise, <see langword="false"/>.</returns>
        bool TryGetValue(TKey key, out TValue value);

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        TValue this[TKey key] { get; set; }

        /// <summary>
        /// Removes the element with the specified key from the dictionary.
        /// </summary>
        bool Remove(TKey key);

        /// <summary>
        /// Attempts to add a key/value pair to the dictionary.
        /// Returns <see langword= "true"/> if added successfully,
        /// or <see langword="false"/> if the key already exists.
        /// </summary>
        bool TryAdd(TKey key, TValue value);

        /// <summary>
        /// Adds a key/value pair to the dictionary.
        /// Throws an exception if the key already exists.
        /// </summary>
        void Add(TKey key, TValue value);

        /// <summary>
        /// Gets a reference to the value associated with the specified key, adding a default value if it doesn't exist.
        /// </summary>
        /// <param name="key">Given key</param>
        /// <param name="exists">Returns true if the key was already present.</param>
        /// <returns>A reference to the value. Can be assigned to directly (e.g., <c>val = 123</c>).</returns>
        ref TValue GetValueRefOrAddDefault(TKey key, out bool exists);
    }
}

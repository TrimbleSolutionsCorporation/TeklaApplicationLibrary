namespace Tekla.Structures
{
    using System;
    using System.Collections.Generic;

    /// <summary>Generic item cache.</summary>
    /// <typeparam name="TItem">Item type.</typeparam>
    /// <remarks>Cache is used to hold cached items with multiple key types.</remarks>
    public static class Cache<TItem>
    {
        #region Public Methods and Operators

        /// <summary>Adds an item to the cache.</summary>
        /// <typeparam name="TKey">The Key type.</typeparam>
        /// <param name="key">The Item key.</param>
        /// <param name="item">Item to add.</param>
        /// <exception cref="ArgumentException">Thrown if the key already exists in the cache.</exception>
        public static void Add<TKey>(TKey key, TItem item)
        {
            Container<TKey>.Add(key, item);
        }

        /// <summary>Determines whether the specified key exists in the cache.</summary>
        /// <typeparam name="TKey">The Key type.</typeparam>
        /// <param name="key">The Item key.</param>
        /// <returns>A boolean value indicating whether the key exists in the cache.</returns>
        public static bool ContainsKey<TKey>(TKey key)
        {
            return Container<TKey>.ContainsKey(key);
        }

        /// <summary>Gets an item from the cache.</summary>
        /// <typeparam name="TKey">The Key type.</typeparam>
        /// <param name="key">The Item key.</param>
        /// <returns>Retrieved item.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the key does not exist in the cache.</exception>
        public static TItem GetItem<TKey>(TKey key)
        {
            return Container<TKey>.GetItem(key);
        }

        /// <summary>Gets an item or default value from the cache.</summary>
        /// <typeparam name="TKey">The Key type.</typeparam>
        /// <param name="key">The Item key.</param>
        /// <returns>Retrieved item or default value.</returns>
        public static TItem GetItemOrDefault<TKey>(TKey key)
        {
            return Container<TKey>.GetItemOrDefault(key);
        }

        /// <summary>Gets an item or the specified default item from the cache.</summary>
        /// <typeparam name="TKey">The Key type.</typeparam>
        /// <param name="key">The Item key.</param>
        /// <param name="defaultItem">Default item that is returned if the key does not exist in the cache.</param>
        /// <returns>Retrieved item or the specified default item.</returns>
        public static TItem GetItemOrDefault<TKey>(TKey key, TItem defaultItem)
        {
            return Container<TKey>.GetItemOrDefault(key, defaultItem);
        }

        /// <summary>Attempts to get an item from the cache.</summary>
        /// <typeparam name="TKey">The Key type.</typeparam>
        /// <param name="key">The Item key.</param>
        /// <param name="item">Output variable for the item.</param>
        /// <returns>A boolean value indicating whether the item was retrieved.</returns>
        public static bool TryGetItem<TKey>(TKey key, out TItem item)
        {
            return Container<TKey>.TryGetItem(key, out item);
        }

        #endregion

        /// <summary>Static container.</summary>
        /// <typeparam name="TKey">The Key type.</typeparam>
        private static class Container<TKey>
        {
            #region Static Fields

            /// <summary>
            /// Cache dictionary.
            /// </summary>
            private static readonly Dictionary<TKey, TItem> Cache = new Dictionary<TKey, TItem>();

            #endregion

            #region Public Methods and Operators

            /// <summary>Adds an item to the cache.</summary>
            /// <param name="key">The Item key.</param>
            /// <param name="item">The Item to add.</param>
            /// <exception cref="ArgumentException">Thrown if the key already exists in the cache.</exception>
            public static void Add(TKey key, TItem item)
            {
                Cache.Add(key, item);
            }

            /// <summary>Determines whether the specified key exists in the cache.</summary>
            /// <param name="key">The Item key.</param>
            /// <returns>A boolean value indicating whether the key exists in the cache.</returns>
            public static bool ContainsKey(TKey key)
            {
                return Cache.ContainsKey(key);
            }

            /// <summary>Gets an item from the cache.</summary>
            /// <param name="key">The Item key.</param>
            /// <returns>Retrieved item.</returns>
            /// <exception cref="KeyNotFoundException">Thrown if the key does not exist in the cache.</exception>
            public static TItem GetItem(TKey key)
            {
                return Cache[key];
            }

            /// <summary>Gets an item or default value from the cache.</summary>
            /// <param name="key">The Item key.</param>
            /// <returns>Retrieved item or default value.</returns>
            public static TItem GetItemOrDefault(TKey key)
            {
                TItem item;

                if (Cache.TryGetValue(key, out item))
                {
                    return item;
                }
                else
                {
                    return default(TItem);
                }
            }

            /// <summary>Gets an item or the specified default item from the cache.</summary>
            /// <param name="key">The Item key.</param>
            /// <param name="defaultItem">Default item that is returned if the key does not exist in the cache.</param>
            /// <returns>Retrieved item or the specified default item.</returns>
            public static TItem GetItemOrDefault(TKey key, TItem defaultItem)
            {
                TItem item;

                if (Cache.TryGetValue(key, out item))
                {
                    return item;
                }
                else
                {
                    return defaultItem;
                }
            }

            /// <summary>Attempts to get an item from the cache.</summary>
            /// <param name="key">The Item key.</param>
            /// <param name="item">Output variable for the item.</param>
            /// <returns>A boolena value indicating whether the item was retrieved.</returns>
            public static bool TryGetItem(TKey key, out TItem item)
            {
                return Cache.TryGetValue(key, out item);
            }

            #endregion
        }
    }
}
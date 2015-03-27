namespace Tekla.Structures
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    /// <summary>
    /// The cache tests.
    /// </summary>
    [TestFixture]
    public class CacheTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// The add should insert item.
        /// </summary>
        [Test]
        public void AddShouldInsertItem()
        {
            Cache<int>.Add("AddedOnce", 10);

            Assert.IsTrue(Cache<int>.ContainsKey("AddedOnce"));
        }

        /// <summary>
        /// The add should throw on double insert.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void AddShouldThrowOnDoubleInsert()
        {
            Cache<int>.Add("AddedTwice", 1);
            Cache<int>.Add("AddedTwice", 2);
        }

        /// <summary>
        /// The cache is sealed class.
        /// </summary>
        [Test]
        public void CacheIsSealedClass()
        {
            Assert.IsTrue(typeof(Cache<>).IsSealed);
        }

        /// <summary>
        /// The contains key should return false if not in cache.
        /// </summary>
        [Test]
        public void ContainsKeyShouldReturnFalseIfNotInCache()
        {
            Assert.IsFalse(Cache<int>.ContainsKey("NotInCache"));
        }

        /// <summary>
        /// The contains key should return true if in cache.
        /// </summary>
        [Test]
        public void ContainsKeyShouldReturnTrueIfInCache()
        {
            Cache<int>.Add("AddedInCache", 1);

            Assert.IsTrue(Cache<int>.ContainsKey("AddedInCache"));
        }

        /// <summary>
        /// The get item or default should return cached item.
        /// </summary>
        [Test]
        public void GetItemOrDefaultShouldReturnCachedItem()
        {
            Cache<int>.Add("GetItemOrDefault_1", 10);

            Assert.AreEqual(10, Cache<int>.GetItemOrDefault("GetItemOrDefault_1"));
        }

        /// <summary>
        /// The get item or default should return default if not in cache.
        /// </summary>
        [Test]
        public void GetItemOrDefaultShouldReturnDefaultIfNotInCache()
        {
            Assert.AreEqual(default(int), Cache<int>.GetItemOrDefault("NotInCache"));
        }

        /// <summary>
        /// The get item or default should return specified default if not in cache.
        /// </summary>
        [Test]
        public void GetItemOrDefaultShouldReturnSpecifiedDefaultIfNotInCache()
        {
            Assert.AreEqual(12, Cache<int>.GetItemOrDefault("NotInCache", 12));
        }

        /// <summary>
        /// The get item should return cached item.
        /// </summary>
        [Test]
        public void GetItemShouldReturnCachedItem()
        {
            Cache<int>.Add("GetItem", 10);

            Assert.AreEqual(10, Cache<int>.GetItem("GetItem"));
        }

        /// <summary>
        /// The get item should throw if not in cache.
        /// </summary>
        [Test]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void GetItemShouldThrowIfNotInCache()
        {
            Cache<int>.GetItem("NotInCache");
        }

        /// <summary>
        /// The try get item should output cached item.
        /// </summary>
        [Test]
        public void TryGetItemShouldOutputCachedItem()
        {
            int value;

            Cache<int>.Add("TryGetItem", 10);

            Assert.IsTrue(Cache<int>.TryGetItem("TryGetItem", out value));
            Assert.AreEqual(10, value);
        }

        /// <summary>
        /// The try get item should return false if not in cache.
        /// </summary>
        [Test]
        public void TryGetItemShouldReturnFalseIfNotInCache()
        {
            int value;

            Assert.IsFalse(Cache<int>.TryGetItem("NotInCache", out value));
        }

        #endregion
    }
}
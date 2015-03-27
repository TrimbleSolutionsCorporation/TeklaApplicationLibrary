namespace Tekla.Structures
{
    using System;

    using NUnit.Framework;

    /// <summary>
    /// The configuration set tests.
    /// </summary>
    [TestFixture]
    public class ConfigurationSetTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// The add should include all items in set.
        /// </summary>
        [Test]
        public void AddShouldIncludeAllItemsInSet()
        {
            var target = ConfigurationSet.Empty;

            target.Add(Configuration.Developer, Configuration.Educational);

            Assert.IsTrue(target.Contains(Configuration.Developer));
            Assert.IsTrue(target.Contains(Configuration.Educational));
        }

        /// <summary>
        /// The add should include item in set.
        /// </summary>
        [Test]
        public void AddShouldIncludeItemInSet()
        {
            var target = ConfigurationSet.Empty;

            target.Add(Configuration.Developer);

            Assert.IsTrue(target.Contains(Configuration.Developer));
        }

        /// <summary>
        /// The configuration set is sealed class.
        /// </summary>
        [Test]
        public void ConfigurationSetIsSealedClass()
        {
            Assert.IsTrue(typeof(ConfigurationSet).IsSealed);
        }

        /// <summary>
        /// The contains all should return false if at least one is missing.
        /// </summary>
        [Test]
        public void ContainsAllShouldReturnFalseIfAtLeastOneIsMissing()
        {
            var target = ConfigurationSet.Full;

            target.Remove(Configuration.Developer);

            Assert.IsFalse(target.ContainsAll(Enum.GetValues(typeof(Configuration)) as Configuration[]));
        }

        /// <summary>
        /// The contains all should return true for empty list.
        /// </summary>
        [Test]
        public void ContainsAllShouldReturnTrueForEmptyList()
        {
            var target = ConfigurationSet.Empty;

            Assert.IsTrue(target.ContainsAll());
        }

        /// <summary>
        /// The contains all should return true for full set.
        /// </summary>
        [Test]
        public void ContainsAllShouldReturnTrueForFullSet()
        {
            var target = ConfigurationSet.Full;

            Assert.IsTrue(target.ContainsAll(Enum.GetValues(typeof(Configuration)) as Configuration[]));
        }

        /// <summary>
        /// The contains any should return false for empty list.
        /// </summary>
        [Test]
        public void ContainsAnyShouldReturnFalseForEmptyList()
        {
            var target = ConfigurationSet.Full;

            Assert.IsFalse(target.ContainsAny());
        }

        /// <summary>
        /// The contains any should return false for empty set.
        /// </summary>
        [Test]
        public void ContainsAnyShouldReturnFalseForEmptySet()
        {
            var target = ConfigurationSet.Empty;

            Assert.IsFalse(target.ContainsAny(Enum.GetValues(typeof(Configuration)) as Configuration[]));
        }

        /// <summary>
        /// The contains any should return true if at least one matches.
        /// </summary>
        [Test]
        public void ContainsAnyShouldReturnTrueIfAtLeastOneMatches()
        {
            var target = ConfigurationSet.Empty;

            target.Add(Configuration.Developer);

            Assert.IsTrue(target.ContainsAny(Enum.GetValues(typeof(Configuration)) as Configuration[]));
        }

        /// <summary>
        /// The contains should return false for empty set.
        /// </summary>
        [Test]
        public void ContainsShouldReturnFalseForEmptySet()
        {
            var target = ConfigurationSet.Empty;

            foreach (Configuration value in Enum.GetValues(typeof(Configuration)))
            {
                Assert.IsFalse(target.Contains(value), value.ToString());
            }
        }

        /// <summary>
        /// The contains should return true for full set.
        /// </summary>
        [Test]
        public void ContainsShouldReturnTrueForFullSet()
        {
            var target = ConfigurationSet.Full;

            foreach (Configuration value in Enum.GetValues(typeof(Configuration)))
            {
                Assert.IsTrue(target.Contains(value));
            }
        }

        /// <summary>
        /// The remove should exclude all items from set.
        /// </summary>
        [Test]
        public void RemoveShouldExcludeAllItemsFromSet()
        {
            var target = ConfigurationSet.Full;

            target.Remove(Configuration.Developer, Configuration.Educational);

            Assert.IsFalse(target.Contains(Configuration.Developer));
            Assert.IsFalse(target.Contains(Configuration.Educational));
        }

        /// <summary>
        /// The remove should exclude item from set.
        /// </summary>
        [Test]
        public void RemoveShouldExcludeItemFromSet()
        {
            var target = ConfigurationSet.Full;

            target.Remove(Configuration.Developer);

            Assert.IsFalse(target.Contains(Configuration.Developer));
        }

        #endregion
    }
}
namespace Tekla.Structures
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Enumerable utility methods.
    /// </summary>
    internal static class Enumerable
    {
        #region Public Methods and Operators

        /// <summary>Wraps a naked enumerator inside an enumerable sequence.</summary>
        /// <param name="enumerator">Source enumerator.</param>
        /// <returns>Enumerable sequence.</returns>
        public static IEnumerable From(IEnumerator enumerator)
        {
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }

        /// <summary>Wraps a naked enumerator inside an enumerable sequence.</summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="enumerator">Source enumerator.</param>
        /// <returns>Enumerable sequence.</returns>
        public static IEnumerable<T> From<T>(IEnumerator<T> enumerator)
        {
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }

        #endregion
    }
}
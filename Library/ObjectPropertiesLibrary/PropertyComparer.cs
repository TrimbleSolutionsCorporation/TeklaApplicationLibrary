namespace Tekla.Structures.ObjectPropertiesLibrary
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>The property comparer.</summary>
    /// <typeparam name="T">The T value.</typeparam>
    public class PropertyComparer<T> : IComparer<T>
    {
        #region Fields

        /// <summary>
        /// The _direction.
        /// </summary>
        private readonly ListSortDirection direction;

        /// <summary>
        /// The _property.
        /// </summary>
        private readonly PropertyDescriptor property;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="PropertyComparer{T}"/> class.</summary>
        /// <param name="property">The property.</param>
        /// <param name="direction">The direction.</param>
        public PropertyComparer(PropertyDescriptor property, ListSortDirection direction)
        {
            this.property = property;
            this.direction = direction;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The compare.</summary>
        /// <param name="wordX">The x word.</param>
        /// <param name="wordY">The y word.</param>
        /// <returns>The System.Int32.</returns>
        public int Compare(T wordX, T wordY)
        {
            // Get property values
            var valueX = this.GetPropertvalueY(wordX, this.property.Name);
            var valueY = this.GetPropertvalueY(wordY, this.property.Name);

            // Determine sort order
            if (this.direction == ListSortDirection.Ascending)
            {
                return this.CompareAscending(valueX, valueY);
            }
            else
            {
                return this.CompareDescending(valueX, valueY);
            }
        }

        /// <summary>The equals.</summary>
        /// <param name="wordX">The x word.</param>
        /// <param name="wordY">The y word.</param>
        /// <returns>The System.Boolean.</returns>
        public bool Equals(T wordX, T wordY)
        {
            return wordX.Equals(wordY);
        }

        /// <summary>The get hash code.</summary>
        /// <param name="obj">The obj value.</param>
        /// <returns>The System.Int32.</returns>
        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }

        #endregion

        // Compare two property values of any isDayOfWeekSchedule
        #region Methods

        /// <summary>The compare ascending.</summary>
        /// <param name="valueX">The x value.</param>
        /// <param name="valueY">The y value.</param>
        /// <returns>The System.Int32.</returns>
        private int CompareAscending(object valueX, object valueY)
        {
            int result;

            // The one not null is greater
            if (valueX == null && valueY == null)
            {
                return 0;
            }
            else if (valueX == null && valueY != null)
            {
                return -1;
            }
            else if (valueX != null && valueY == null)
            {
                return 1;
            }

            // IF     : Values implement IComparer
            // ELSE IF: Values don't implement IComparer but are equivalent
            // ELSE   : Values don't implement IComparer and are not equivalent, so compare as string values
            if (valueX is IComparable)
            {
                result = ((IComparable)valueX).CompareTo(valueY);
            }
            else if (valueX.Equals(valueY))
            {
                result = 0;
            }
            else
            {
                result = valueX.ToString().CompareTo(valueY.ToString());
            }

            // Return _predecessorsString
            return result;
        }

        /// <summary>The compare descending.</summary>
        /// <param name="valueX">The x value.</param>
        /// <param name="valueY">The y value.</param>
        /// <returns>The System.Int32.</returns>
        private int CompareDescending(object valueX, object valueY)
        {
            // Return _predecessorsString adjusted for ascending or descending sort order ie
            // multiplied by 1 for ascending or -1 for descending
            return this.CompareAscending(valueX, valueY) * -1;
        }

        /// <summary>The get property value.</summary>
        /// <param name="value">The value.</param>
        /// <param name="property">The property.</param>
        /// <returns>The System.Object.</returns>
        private object GetPropertvalueY(T value, string property)
        {
            // Get property
            var propertyInfo = value.GetType().GetProperty(property);

            // Return value
            return propertyInfo.GetValue(value, null);
        }

        #endregion
    }
}
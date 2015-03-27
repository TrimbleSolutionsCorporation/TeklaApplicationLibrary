namespace Tekla.Structures.ObjectPropertiesLibrary
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    /// <summary>The searchable sortable binding list.</summary>
    /// <typeparam name="T">The T value.</typeparam>
    public class SearchableSortableBindingList<T> : BindingList<T>, IBindingList
    {
        #region Fields

        /// <summary>
        /// The _is sorted.
        /// </summary>
        private bool isSorted;

        /// <summary>
        /// The _sort direction.
        /// </summary>
        private ListSortDirection sortDirection;

        /// <summary>
        /// The _sort property.
        /// </summary>
        private PropertyDescriptor sortProperty;

        #endregion

        #region Properties

        /// <summary>Gets a value indicating whether is sorted core.</summary>
        /// <value>The is sorted core.</value>
        protected override bool IsSortedCore
        {
            get
            {
                return this.isSorted;
            }
        }

        // Missing from Part 2

        /// <summary>Gets the sort direction core.</summary>
        /// <value>The sort direction core.</value>
        protected override ListSortDirection SortDirectionCore
        {
            get
            {
                return this.sortDirection;
            }
        }

        // Missing from Part 2

        /// <summary>Gets the sort property core.</summary>
        /// <value>The sort property core.</value>
        protected override PropertyDescriptor SortPropertyCore
        {
            get
            {
                return this.sortProperty;
            }
        }

        /// <summary>Gets a value indicating whether supports searching core.</summary>
        /// <value>The supports searching core.</value>
        protected override bool SupportsSearchingCore
        {
            get
            {
                return true;
            }
        }

        /// <summary>Gets a value indicating whether supports sorting core.</summary>
        /// <value>The supports sorting core.</value>
        protected override bool SupportsSortingCore
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The load value.</summary>
        /// <param name="filename">The filename.</param>
        public void Load(string filename)
        {
            this.ClearItems();

            if (File.Exists(filename))
            {
                var formatter = new BinaryFormatter();
                using (var stream = new FileStream(filename, FileMode.Open))
                {
                    // Deserialize data list items
                    ((List<T>)this.Items).AddRange((IEnumerable<T>)formatter.Deserialize(stream));
                }
            }

            // Let bound controls know they should refresh their views
            this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        /// <summary>The save value.</summary>
        /// <param name="filename">The filename.</param>
        public void Save(string filename)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new FileStream(filename, FileMode.Create))
            {
                // Serialize data list items
                formatter.Serialize(stream, (List<T>)this.Items);
            }
        }

        /// <summary>The sort value.</summary>
        /// <param name="property">The property.</param>
        /// <param name="direction">The direction.</param>
        public void Sort(PropertyDescriptor property, ListSortDirection direction)
        {
            this.ApplySortCore(property, direction);
        }

        #endregion

        #region Methods

        /// <summary>The apply sort core.</summary>
        /// <param name="property">The property.</param>
        /// <param name="direction">The direction.</param>
        protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
        {
            // Get list to sort
            // Note: this.Items is a non-sortable ICollection<T>
            var items = this.Items as List<T>;

            // Apply and set the sort, if items to sort
            if (items != null)
            {
                var pc = new PropertyComparer<T>(property, direction);
                items.Sort(pc);
                this.isSorted = true;
            }
            else
            {
                this.isSorted = false;
            }

            this.sortProperty = property;
            this.sortDirection = direction;

            this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        /// <summary>The find core.</summary>
        /// <param name="property">The property.</param>
        /// <param name="key">The key value.</param>
        /// <returns>The System.Int32.</returns>
        protected override int FindCore(PropertyDescriptor property, object key)
        {
            // Specify search columns
            if (property == null)
            {
                return -1;
            }

            // Get list to search
            var items = this.Items as List<T>;

            // Traverse list for value
            foreach (var item in items)
            {
                // Test column search value
                var value = (string)property.GetValue(item);

                // If value is the search value, return the 
                // index of the data item
                if ((string)key == value)
                {
                    return this.IndexOf(item);
                }
            }

            return -1;
        }

        /// <summary>
        /// The remove sort core.
        /// </summary>
        protected override void RemoveSortCore()
        {
            this.isSorted = false;
            this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        #endregion
    }
}
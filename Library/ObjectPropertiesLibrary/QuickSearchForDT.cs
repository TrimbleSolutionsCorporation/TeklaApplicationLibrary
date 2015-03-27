namespace Tekla.Structures.ObjectPropertiesLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// The search start delegate.
    /// </summary>
    public delegate void SearchStartDelegate();

    /// <summary>
    /// The search ended delegate.
    /// </summary>
    /// <param name="resultTable">
    /// The result table.
    /// </param>
    public delegate void SearchEndedDelegate(DataTable resultTable);

    /// <summary>
    /// The quick search for data table.
    /// </summary>
    public partial class QuickSearchForDataTable : UserControl
    {
        #region Constants

        /// <summary>
        /// The delay before search.
        /// </summary>
        private const int DelayBeforeSearch = 1000;

        #endregion

        #region Fields

        /// <summary>
        /// The clear button image.
        /// </summary>
        private readonly Image clearButtonImage;

        /// <summary>
        /// The quick search timer.
        /// </summary>
        private readonly Timer quickSearchTimer = new Timer();

        /// <summary>
        /// The data table to search.
        /// </summary>
        private DataTable dataTableToSearch;

        /// <summary>
        /// The do search ended.
        /// </summary>
        private SearchEndedDelegate doSearchEnded;

        /// <summary>
        /// The do search started.
        /// </summary>
        private SearchStartDelegate doSearchStarted;

        /// <summary>
        /// The hidden rows.
        /// </summary>
        private int hiddenRows;

        /// <summary>
        /// The previous search string.
        /// </summary>
        private string previousSearchString = string.Empty;

        /// <summary>
        /// The result data table.
        /// </summary>
        private DataTable resultDataTable;

        /// <summary>
        /// The row is selected.
        /// </summary>
        private List<bool> rowIsSelected;

        /// <summary>
        /// The search string.
        /// </summary>
        private string searchString = string.Empty;

        /// <summary>
        /// The visible rows.
        /// </summary>
        private int visibleRows;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuickSearchForDataTable"/> class.
        /// </summary>
        public QuickSearchForDataTable()
        {
            this.InitializeComponent();
            this.quickSearchTimer.Interval = DelayBeforeSearch;
            this.quickSearchTimer.Tick += this.OnQuickSearchTimerTick;
            this.clearButtonImage = this.ClearButton.Image;
            this.resultDataTable = new DataTable();
        }

        /// <summary>Initializes a new instance of the <see cref="QuickSearchForDataTable"/> class.</summary>
        /// <param name="newDataTableToSearch">The dt to search.</param>
        /// <param name="searchStarted">The search started.</param>
        /// <param name="searchEnded">The search ended.</param>
        public QuickSearchForDataTable(ref DataTable newDataTableToSearch, SearchStartDelegate searchStarted, SearchEndedDelegate searchEnded)
        {
            this.InitializeComponent();
            this.quickSearchTimer.Interval = DelayBeforeSearch;
            this.quickSearchTimer.Tick += this.OnQuickSearchTimerTick;
            this.clearButtonImage = this.ClearButton.Image;
            this.dataTableToSearch = newDataTableToSearch;
            this.doSearchStarted = searchStarted;
            this.doSearchEnded = searchEnded;
            this.resultDataTable = new DataTable();
        }

        #endregion

        #region Public Events

        /// <summary>
        /// Fired when quick search is done. Registered handler had to get updated rows count by GetVisibleRows() and GetHiddenRows().
        /// </summary>
        public event EventHandler UpdateRowsCount;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Return count of hidden rows.
        /// </summary>
        /// <returns>
        /// The System.Int32.
        /// </returns>
        public int GetHiddenRows()
        {
            return this.hiddenRows;
        }

        /// <summary>
        /// Return count of visible rows.
        /// </summary>
        /// <returns>
        /// The System.Int32.
        /// </returns>
        public int GetVisibleRows()
        {
            return this.visibleRows;
        }

        /// <summary>
        /// Show only rows that match search pattern.
        /// </summary>
        public void QuickSearchExecute()
        {
            if (this.doSearchStarted != null)
            {
                this.doSearchStarted();
            }

            this.visibleRows = 0;
            this.hiddenRows = 0;

            this.previousSearchString = this.searchString;

            // 0. get columns and headers from source data table
            this.resultDataTable.Columns.Clear();
            this.resultDataTable.Clear();

            this.resultDataTable = this.dataTableToSearch.Clone();

            // 1. make table 
            var rowsAsStrings = new string[this.dataTableToSearch.Rows.Count];
            var count = 0;

            this.rowIsSelected = new List<bool>();

            foreach (DataRow thisRow in this.dataTableToSearch.Rows)
            {
                for (var i = 1; i < this.dataTableToSearch.Columns.Count; i++)
                {
                    rowsAsStrings[count] += thisRow[i].ToString().ToLower() + " ";
                }

                count++;
            }

            // 2. search and set visible
            var searchTermsOr = this.GetOrSearchTerms(this.searchString);

            for (var ii = 0; ii < rowsAsStrings.Length; ii++)
            {
                if (this.QuickSearchIsTrue(rowsAsStrings[ii], searchTermsOr))
                {
                    this.resultDataTable.ImportRow(this.dataTableToSearch.Rows[ii]);
                    this.visibleRows++;
                }
                else
                {
                    this.hiddenRows++;
                }
            }

            this.UpdateRows();

            if (this.doSearchEnded != null)
            {
                this.doSearchEnded(this.resultDataTable);
            }
        }

        /// <summary>Assignes dataGridView for control to search.</summary>
        /// <param name="newDataTableToSearch">DataTable to search.</param>
        /// <param name="searchStarted">Delegate method for giving possibility for parent do some actions just before search is started.</param>
        /// <param name="searchEnded">Delegate method for giving possibility for parent do some actions just after search is completed.</param>
        public void SetReferenceProperties(ref DataTable newDataTableToSearch, SearchStartDelegate searchStarted, SearchEndedDelegate searchEnded)
        {
            this.dataTableToSearch = newDataTableToSearch;
            this.doSearchStarted = searchStarted;
            this.doSearchEnded = searchEnded;

            if (this.dataTableToSearch != null && this.dataTableToSearch.Rows != null)
            {
                this.visibleRows = this.dataTableToSearch.Rows.Count;
            }
        }

        /// <summary>
        /// Empties Search text box without doing search.
        /// </summary>
        public void SilentlyEmptySearchBox()
        {
            this.QuickSearchTB.TextChanged -= this.QuickSearchTbTextChanged;
            this.QuickSearchTB.Text = this.searchString = string.Empty;
            this.HideClearButton();
            this.QuickSearchTB.TextChanged += this.QuickSearchTbTextChanged;
        }

        #endregion

        #region Methods

        /// <summary>The on update rows count.</summary>
        /// <param name="e">The e value.</param>
        protected virtual void OnUpdateRowsCount(EventArgs e)
        {
            if (this.UpdateRowsCount != null)
            {
                this.UpdateRowsCount(this, e);
            }
        }

        /// <summary>The clear button_ click.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e value.</param>
        private void ClearButtonClick(object sender, EventArgs e)
        {
            this.QuickSearchTB.Text = this.searchString = string.Empty;
            this.HideClearButton();
        }

        /// <summary>The get or search terms.</summary>
        /// <param name="rawSearchString">The raw search string.</param>
        /// <returns>The System.Collections.Generic.List`1[T -&gt; System.String[]].</returns>
        private List<string[]> GetOrSearchTerms(string rawSearchString)
        {
            var tmpResultString = string.Empty;
            var searchTermsOr = new List<string[]>();

            for (var ii = 0; ii < rawSearchString.Length; ii++)
            {
                tmpResultString = rawSearchString.Replace("+ ", "+");

                if (!tmpResultString.Contains("+ "))
                {
                    break;
                }
            }

            for (var ii = 0; ii < rawSearchString.Length; ii++)
            {
                tmpResultString = tmpResultString.Replace(" +", "+");

                if (!tmpResultString.Contains(" +"))
                {
                    break;
                }
            }

            var charSeparators = new[] { ' ' };

            var stringsOr = tmpResultString.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);

            foreach (var stringOr in stringsOr)
            {
                searchTermsOr.Add(stringOr.Split('+'));
            }

            return searchTermsOr;
        }

        /// <summary>
        /// The hide clear button.
        /// </summary>
        private void HideClearButton()
        {
            if (this.ClearButton.Image != null)
            {
                this.ClearButton.Image = null;
            }
        }

        /// <summary>The on quick search timer tick.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e value.</param>
        private void OnQuickSearchTimerTick(object sender, EventArgs e)
        {
            this.quickSearchTimer.Stop();
            this.searchString = this.QuickSearchTB.Text.ToLower();

            if (this.previousSearchString != this.searchString)
            {
                this.QuickSearchExecute();
            }
        }

        /// <summary>The quick search is true.</summary>
        /// <param name="rowString">The row string.</param>
        /// <param name="searchTermsOr">The or search terms.</param>
        /// <returns>The System.Boolean.</returns>
        private bool QuickSearchIsTrue(string rowString, List<string[]> searchTermsOr)
        {
            var isTrue = false;

            if (searchTermsOr.Count == 0)
            {
                isTrue = true;
            }
            else
            {
                foreach (var termOr in searchTermsOr)
                {
                    foreach (var andTerm in termOr)
                    {
                        var trueAndTerm = andTerm;
                        var not = false;

                        if (andTerm.StartsWith("!"))
                        {
                            trueAndTerm = andTerm.Substring(1);
                            not = true;
                        }

                        if ((rowString.Contains(trueAndTerm) && !not) || (!rowString.Contains(trueAndTerm) && not))
                        {
                            isTrue = true;
                        }
                        else
                        {
                            isTrue = false;
                            break;
                        }
                    }

                    if (isTrue)
                    {
                        break;
                    }
                }
            }

            return isTrue;
        }

        /// <summary>The quick search t b_ text changed.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e value.</param>
        private void QuickSearchTbTextChanged(object sender, EventArgs e)
        {
            this.searchString = this.QuickSearchTB.Text.ToLower();

            // Hide cross when empty
            if (string.IsNullOrEmpty(this.searchString))
            {
                this.HideClearButton();
            }
            else
            {
                this.ShowClearButton();
            }

            this.quickSearchTimer.Stop();
            this.quickSearchTimer.Start();
        }

        /// <summary>
        /// The show clear button.
        /// </summary>
        private void ShowClearButton()
        {
            if (this.ClearButton.Image == null)
            {
                this.ClearButton.Image = this.clearButtonImage;
            }
        }

        /// <summary>
        /// The update rows.
        /// </summary>
        private void UpdateRows()
        {
            this.OnUpdateRowsCount(null);
        }

        #endregion
    }
}
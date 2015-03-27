namespace Tekla.Structures.ObjectPropertiesLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// Textbox that performs search for set datagridview and changes visibility of row accordingly. OR- " ", AND - "+", NOT - "!". 
    /// NOTE: DGW performance bug => 4000 rows takes ~10s. 8000 35s. etc.
    /// </summary>
    public partial class QuickSearchForDGW : UserControl
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
        /// The dgw to search.
        /// </summary>
        private DataGridView dgwToSearch;

        /// <summary>
        /// The hidden rows.
        /// </summary>
        private int hiddenRows;

        /// <summary>
        /// The previous search string.
        /// </summary>
        private string previousSearchString = string.Empty;

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
        /// Initializes a new instance of the <see cref="QuickSearchForDGW"/> class.
        /// </summary>
        public QuickSearchForDGW()
        {
            this.InitializeComponent();
            this.quickSearchTimer.Interval = DelayBeforeSearch;
            this.quickSearchTimer.Tick += this.OnQuickSearchTimerTick;
            this.clearButtonImage = this.ClearButton.Image;
            this.ClearButton.Image = null;
        }

        /// <summary>Initializes a new instance of the <see cref="QuickSearchForDGW"/> class.</summary>
        /// <param name="dgw">The dgw value.</param>
        public QuickSearchForDGW(ref DataGridView dgw)
        {
            this.InitializeComponent();
            this.quickSearchTimer.Interval = DelayBeforeSearch;
            this.quickSearchTimer.Tick += this.OnQuickSearchTimerTick;
            this.clearButtonImage = this.ClearButton.Image;
            this.ClearButton.Image = null;
            this.dgwToSearch = dgw;
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
        /// Empty Search Box.
        /// </summary>
        public void EmptySearchBox()
        {
            this.QuickSearchTB.Text = this.searchString = string.Empty;
        }

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
            this.visibleRows = 0;
            this.hiddenRows = 0;

            this.previousSearchString = this.searchString;

            // 1. make table (try to do realtime hide/show using it, if possible)
            var rowsAsStrings = new string[this.dgwToSearch.Rows.Count];
            var count = 0;
            var ifFirstSelected = true;
            this.rowIsSelected = new List<bool>();

            foreach (DataGridViewRow thisRow in this.dgwToSearch.Rows)
            {
                foreach (DataGridViewCell thisCell in thisRow.Cells)
                {
                    if (this.dgwToSearch.Columns[thisCell.ColumnIndex].Visible && thisCell.Value != null)
                    {
                        rowsAsStrings[count] += thisCell.Value.ToString().ToLower() + " ";
                    }
                }

                this.rowIsSelected.Add(thisRow.Selected);

                count++;
            }

            if (this.dgwToSearch.CurrentCell != null)
            {
                this.dgwToSearch.CurrentCell = null;
            }

            this.dgwToSearch.SuspendLayout();
            DrawingControl.SuspendDrawing(this.dgwToSearch);
            this.dgwToSearch.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.Never;

            // 2. search and set visible
            var searchTermsOr = this.GetOrSearchTerms(this.searchString);

            for (var ii = 0; ii < rowsAsStrings.Length; ii++)
            {
                // uncommitted row can have all null values
                if (rowsAsStrings[ii] == null || this.QuickSearchIsTrue(rowsAsStrings[ii], searchTermsOr))
                {
                    this.dgwToSearch.Rows[ii].Visible = true;
                    this.visibleRows++;
                }
                else
                {
                    try
                    {
                        this.dgwToSearch.Rows[ii].Visible = false;
                        this.hiddenRows++;
                    }
                    catch (Exception)
                    {
                        // Same uncommitted row in edit mode can have values, but cannot be made invisible
                    }
                }

                if (this.dgwToSearch.Rows[ii].Visible && this.rowIsSelected[ii])
                {
                    this.dgwToSearch.Rows[ii].Selected = true;

                    if (ifFirstSelected)
                    {
                        // Try to set first selected to current, otherwise gantt will select it itself after focus change and all other selections are lost.
                        try
                        {
                            this.dgwToSearch.CurrentCell = this.dgwToSearch.Rows[ii].Cells[0];
                        }
                        catch (Exception ee)
                        {
                            Debug.WriteLine(ee);
                        }

                        ifFirstSelected = false;
                    }
                }
            }

            this.dgwToSearch.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            DrawingControl.ResumeDrawing(this.dgwToSearch);
            this.dgwToSearch.ResumeLayout();
            this.UpdateRows();
        }

        /// <summary>Assignes dataGridView for control to search.</summary>
        /// <param name="dgw">The dgw value.</param>
        public void SetReferenceProperties(ref DataGridView dgw)
        {
            this.dgwToSearch = dgw;

            if (this.dgwToSearch != null && this.dgwToSearch.Rows != null)
            {
                this.visibleRows = this.dgwToSearch.Rows.Count;
            }
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
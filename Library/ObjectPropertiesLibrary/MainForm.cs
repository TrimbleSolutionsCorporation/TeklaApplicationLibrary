using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Tekla.Structures.Dialog;
using Tekla.Structures.Model;
using System.Runtime.InteropServices;
using Tekla.Structures.Model.Collaboration;

namespace ObjectBrowserProto
{
    public partial class MainForm : ApplicationFormBase
    {
        private TSConnection ThisConnection = null;
        private DataTable ObjectsTable = null;
        private DataTable OriginalObjectsTable = null;
        private DataTable AllObjectsTable = null;
        private DataTable SumsTable = null;
        private PresentedPropertiesXml AllPropertiesXml = null;
        private PresentedPropertiesXml ShowablePropertiesXml = null;
        private ArrayList SelectedObjects = null;
        private    BackgroundWorker UpdateListWorker = new BackgroundWorker();
        private BackgroundWorker LoadListWorker = new BackgroundWorker();
        private bool AutoSelectOn = false;
        private bool DoAnotherUpdate = false;
        private bool AutoUpdateOn = false;
        delegate void DelegateAsynSelectionChanged();
        private int ColumnDragged = -1;
        private bool SelectEventActivated = false;
        private int TypeColumnWidth = PresentedProperties.DefaultWidth;
        private bool GotNewObjects = false;
        private DateTime DateNull = new DateTime(1970, 1, 1);
        private bool AllwaysSearch = false;
        private string LoadFileName = "";
        private string NotFoundsList = "";
        MenuItem[] SumMenuItems = new MenuItem[4];
        private int OrigSplitterDist = 0;
        private bool FirstTime = true;
        private int SumDGWRowHeight = 14; 

        public MainForm()
        {
            InitializeComponent();
            CustomInitialize();
            InitializeTsConnection();
            SetEvents();
            InitializeSumItemList();

            if (ThisConnection != null)
            {
                ThisConnection.LocalizationForm.Localize(this);

                //Context menu items dont localize?
                this.alblZoomtoselectedobjectsToolStripMenuItem.Text = ThisConnection.LocalizationForm.GetText("albl_Zoom_to_selected_objects");
                this.alblRemovefromlistToolStripMenuItem.Text = ThisConnection.LocalizationForm.GetText("albl_Remove_from_list");
            }
            else
            {
                throw new ApplicationException(
                    ThisConnection.LocalizationForm.GetText("albl_Connection_to_Tekla_Structures_could_not_be_established."));
            }

            InitializeDataClasses();
            FormBorders.RestoreFormSizeAndLocation(this);

            toolStripButtonGet_Click(null, null);

            OrigSplitterDist = splitContainer1.SplitterDistance;
        }

        private void CustomInitialize()
        {
            ObjectsTable = new DataTable();
            OriginalObjectsTable = new DataTable();
            AllObjectsTable = new DataTable();

            QuickSearchUserCntrl = new ObjectBrowserProto.QuickSearchForDataTable();
            QuickSearchUserCntrl.SetReferenceProperties(ref AllObjectsTable, new SearchStartDelegate(BeforeSearch), new SearchEndedDelegate(AfterSearch));
            //QuickSearchUserCntrl.UpdateRowsCount += new EventHandler(QuickSearchUserCntrl_UpdateRowsCount);

            ToolStripControlHost QuickSearchUserCntrlControlHost = new ToolStripControlHost(QuickSearchUserCntrl);
            QuickSearchUserCntrlControlHost.Alignment = ToolStripItemAlignment.Right;
            QuickSearchUserCntrl.Width = 140;
            toolStrip1.SuspendLayout();
            toolStrip1.Items.Add(QuickSearchUserCntrlControlHost);

            toolStrip1.ResumeLayout();

            UpdateListWorker.WorkerSupportsCancellation = true;
            UpdateListWorker.WorkerReportsProgress = true;
            UpdateListWorker.DoWork +=
                new DoWorkEventHandler(DoWork);
            UpdateListWorker.ProgressChanged +=
                new ProgressChangedEventHandler(ProgressChanged);
            UpdateListWorker.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(RunWorkerCompleted);

            LoadListWorker.WorkerSupportsCancellation = true;
            LoadListWorker.WorkerReportsProgress = true;
            LoadListWorker.DoWork +=
                new DoWorkEventHandler(LoadList_DoWork);
            LoadListWorker.ProgressChanged +=
                new ProgressChangedEventHandler(LoadList_ProgressChanged);
            LoadListWorker.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(LoadList_RunWorkerCompleted);


            ObjectsGrid.AutoGenerateColumns = true;
            
            //toolStripButtonAuto.Image = imageList1.Images["autoselect_objects_passive_big.ico"];
            //AutoUpdateTSButton.Image = imageList1.Images["autoselect_objects_passive_big.ico"];
            AutoUpdateTSButton.Checked = false;
            toolStripButtonGet.Image = imageList1.Images["move_down_24.ico"];
            toolStripButtonProperties.Image = imageList1.Images["keep_selection_16.ico"];
        }

        private void SetEvents()
        {
            toolStripButtonAuto.Click +=new EventHandler(toolStripButtonAuto_Click);
            toolStripButtonProperties.Click += new EventHandler(toolStripButtonProperties_Click);
            toolStripButtonGet.Click +=new EventHandler(toolStripButtonGet_Click);

            ObjectsGrid.KeyDown += new KeyEventHandler(MainForm_KeyDown);
            //ObjectsGrid.ContextMenuStrip = contextMenuStrip1;
            ObjectsGrid.SelectionChanged += new EventHandler(ObjectsGrid_SelectionChanged);
            ObjectsGrid.ColumnWidthChanged += new DataGridViewColumnEventHandler(ObjectsGrid_ColumnWidthChanged);
            ObjectsGrid.ColumnDisplayIndexChanged += new DataGridViewColumnEventHandler(ObjectsGrid_ColumnDisplayIndexChanged);
            ObjectsGrid.MouseClick += new MouseEventHandler(ObjectsGrid_MouseClick);
            ObjectsGrid.GotFocus += new EventHandler(MainForm_Activated);
            //ObjectsGrid.MouseDown += new MouseEventHandler(ObjectsGrid_MouseDown);
            //ObjectsGrid.MouseUp += new MouseEventHandler(ObjectsGrid_MouseUp);
            Closing += new CancelEventHandler(MainForm_Closing);
            SumDGW.KeyDown+=new KeyEventHandler(MainForm_KeyDown);
            //SumDGW.ColumnWidthChanged += new DataGridViewColumnEventHandler(SumDGW_ColumnWidthChanged);
            SumDGW.Scroll += new ScrollEventHandler(SumDGW_Scroll);
            SumDGW.SelectionChanged += new EventHandler(SumDGW_SelectionChanged);
            SumDGW.MouseClick += new MouseEventHandler(SumDGW_MouseClick);
            this.Activated += new EventHandler(MainForm_Activated);
            this.Deactivate+= new EventHandler(MainForm_Deactivate);
        }

        void SumDGW_MouseClick(object sender, MouseEventArgs e)
        {
            ObjectsGrid.ClearSelection();
        }

        void SumDGW_SelectionChanged(object sender, EventArgs e)
        {
            //DGW is user friendly...
            SumDGW.ClearSelection();
            SumDGW.CurrentCell = null;
        }

        void MainForm_Closing(object sender, CancelEventArgs e)
        {
            SaveAndUpdateList(GetChangedColumnOrderList(), false);
        }

        void MainForm_Activated(object sender, EventArgs e)
        {
            ObjectsGrid.DefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
            ObjectsGrid.DefaultCellStyle.SelectionForeColor = Color.White;

            if (AutoUpdateOn)
            {
                SelectEventActivated = false;
                ThisConnection.TeklaEvents.SelectionChange -= new Events.SelectionChangeDelegate(TeklaEvents_SelectionChange);
            }

            Point MouseInFormPoint = PointToClient(MousePosition);

            if (PointIsInControl(MouseInFormPoint, toolStripButtonProperties))
                toolStripButtonProperties.PerformClick();
                //else if (PointIsInControl(MouseInFormPoint, toolStripButtonAuto))
                //    toolStripButtonAuto.PerformClick();
            else if (PointIsInControl(MouseInFormPoint, AutoUpdateTSButton))
                AutoUpdateTSButton.PerformClick();
            else if (PointIsInControl(MouseInFormPoint, toolStripButtonGet))
                toolStripButtonGet.PerformClick();

        }

        private bool PointIsInControl(Point ThisPoint, ToolStripButton ThisControl)
        {
            bool Result = false;

                        if ((ThisPoint.X >= ThisControl.Bounds.X && ThisPoint.X <= ThisControl.Bounds.X + ThisControl.Bounds.Width) &&
                (ThisPoint.Y >= ThisControl.Bounds.Y && ThisPoint.Y <= ThisControl.Bounds.Y + ThisControl.Bounds.Height))
                            Result = true;

            return Result;
        }

        void MainForm_Deactivate(object sender, EventArgs e)
        {

            //ObjectsGrid.SelectionChanged -= new EventHandler(ObjectsGrid_SelectionChanged);
            ////ObjectsGrid.ClearSelection();
            ////ObjectsGrid.CurrentCell = null;
            ////SumDGW.ClearSelection();
            ////SumDGW.CurrentCell = null;
            //ObjectsGrid.SelectionChanged += new EventHandler(ObjectsGrid_SelectionChanged);

            if (AutoUpdateOn && !SelectEventActivated)
            {
                SelectEventActivated = true;
                ThisConnection.TeklaEvents.SelectionChange += new Events.SelectionChangeDelegate(TeklaEvents_SelectionChange);
            }

            if(!FirstTime)
            {
                ObjectsGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(240, 240, 240);
                ObjectsGrid.DefaultCellStyle.SelectionForeColor = Color.Black; 
            }
        }

        void TeklaEvents_SelectionChange()
        {
            if (InvokeRequired)
            {
                DelegateAsynSelectionChanged d = new DelegateAsynSelectionChanged(TeklaEvents_SelectionChange);
                Invoke(d);
            }
            else
            {
                //Auto update only when not focused.
                if (AutoUpdateOn && !Focused && !ObjectsGrid.Focused && !SumDGW.Focused)
                {
                    if (UpdateListWorker.IsBusy)
                    {
                        DoAnotherUpdate = true;
                        UpdateListWorker.CancelAsync();
                    }
                    else
                    {
                        DoAnotherUpdate = false;
                        SelectedObjects = ThisConnection.GetSelectedObjects();

                        if (SelectedObjects.Count > 0)
                        {
                            GotNewObjects = true;
                            UpdateList();
                        }
                    }
                }
            }
        }

        void ObjectsGrid_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (e.Y < ObjectsGrid.ColumnHeadersHeight)
                {
                    DataGridView.HitTestInfo Info = ObjectsGrid.HitTest(e.X, e.Y);
                    
                    if(Info.ColumnIndex > 0 && Info.ColumnIndex < ObjectsGrid.Columns.Count)
                    {
                        int DraggedTo = ObjectsGrid.Columns[Info.ColumnIndex].DisplayIndex;
                        if (ColumnDragged != DraggedTo) 
                        {
                            //ColumnOrderChanged(DraggedTo);
                        }
                    }
                }
            }
        }

        void ObjectsGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (e.Y < ObjectsGrid.ColumnHeadersHeight)
                {
                    //DataGridView.HitTestInfo Info = ObjectsGrid.HitTest(e.X, e.Y);

                    //if(Info.ColumnIndex == 1)
                        

                    //int fuck = ObjectsGrid.Columns[Info.ColumnIndex].DisplayIndex;
                    //ColumnDragged = Info.ColumnIndex;
                }
            }
        }

        void ObjectsGrid_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.Y < ObjectsGrid.ColumnHeadersHeight)
                {
                    if (e.Button == MouseButtons.Right)
                    {
                        ShowHideColumns(e);
                    }
                }
                else
                {
                    contextMenuStrip1.Show(MousePosition);
                }
            }
            else if (e.Button == MouseButtons.Left)
            {
                DataGridView.HitTestInfo Info = ObjectsGrid.HitTest(e.X, e.Y);

                if(e.Y > ObjectsGrid.ColumnHeadersHeight && Info.RowIndex == -1)
                {
                    ObjectsGrid.ClearSelection();
                }
            }
        }

        void SumDGW_Scroll(object sender, ScrollEventArgs e)
        {
            if(e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                ObjectsGrid.HorizontalScrollingOffset = SumDGW.HorizontalScrollingOffset;
                ObjectsGrid.Refresh();
            }
        }

        void SumDGW_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            try
            {
                ObjectsGrid.Columns[e.Column.Index].Width = e.Column.Width;
                ObjectsGrid.Refresh();
                SumDGW.Refresh();
            }
            catch (Exception ee)
            {
                System.Diagnostics.Debug.WriteLine(ee.ToString());
            }
        }

        void ObjectsGrid_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            try
            {
                SumDGW.Columns[e.Column.Index].Width = e.Column.Width;
                ObjectsGrid.Refresh();
                SumDGW.Refresh();
            }
            catch (Exception ee)
            {
                System.Diagnostics.Debug.WriteLine(ee.ToString());
            }
        }

        void ObjectsGrid_ColumnDisplayIndexChanged(object sender, DataGridViewColumnEventArgs e)
        {
            try
            {
                if (SumDGW.Columns.Count > 0)
                {
                    SumDGW.Columns[e.Column.Index].DisplayIndex = e.Column.DisplayIndex;
                    ObjectsGrid.Refresh();
                    SumDGW.Refresh();
                }
            }
            catch (Exception ee)
            {
                System.Diagnostics.Debug.WriteLine(ee.ToString());
            }
        }

        void ObjectsGrid_SelectionChanged(object sender, EventArgs e)
        {
            if(AutoSelectOn)
                alblSelectinmodelToolStripMenuItem_Click(null, null);


            if(ObjectsGrid.SelectedRows.Count > 1)
            {
                DataTable SelectedRowCol = new DataTable();
                SelectedRowCol = ObjectsTable.Clone();

                foreach (DataGridViewRow Row in ObjectsGrid.SelectedRows)
                {
                    SelectedRowCol.ImportRow((((DataRowView) Row.DataBoundItem).Row));
                }

                CountSums(SelectedRowCol.Rows);
            }
            else
            {
                CountSums(ObjectsTable.Rows);
            }

            toolStripStatusLabelCount.Text = string.Format(ThisConnection.LocalizationForm.GetText("albl_{0}/{1}_Objects_({2}_Hidden)"), ObjectsGrid.SelectedRows.Count, ObjectsGrid.Rows.Count, QuickSearchUserCntrl.GetHiddenRows());
        }

        void QuickSearchUserCntrl_UpdateRowsCount(object sender, EventArgs e)
        {
            toolStripStatusLabelCount.Text = string.Format(ThisConnection.LocalizationForm.GetText("albl_{0}/{1}_Objects_({2}_Hidden)"), 1, QuickSearchUserCntrl.GetVisibleRows(), QuickSearchUserCntrl.GetHiddenRows());
        }

        void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    if (UpdateListWorker.IsBusy)
                    {
                        UpdateListWorker.CancelAsync();
                    }
                    break;
                case Keys.A:
                    if (e.Control && e.Shift)
                    {
                        AllwaysSearch = !AllwaysSearch;
                    }
                    break;

#if DEBUG
                case Keys.Q:
                    //FolderBrowserDialog asdf = new FolderBrowserDialog();
                    //asdf.ShowNewFolderButton = true;
                    //asdf.RootFolder

                    break;
                case Keys.S:

                    SaveFileDialog SaveDialog = new SaveFileDialog();
                    SaveDialog.InitialDirectory = ThisConnection.OpenModel.GetInfo().ModelPath;
                    SaveDialog.FileName = "ObjectsTable.obr";
                    

                    if (SaveDialog.ShowDialog() == DialogResult.OK)
                    {
                        DataTable WritableTable = AllObjectsTable.Copy();
                        WritableTable.TableName = "ObjectBrowser write test";
                        WritableTable.Columns.Add("guid", typeof(string));

                        foreach (DataRow row in WritableTable.Rows)
                        {
                            row[WritableTable.Columns.Count - 1] = ThisConnection.OpenModel.GetGUIDByIdentifier((row[0] as ModelObject).Identifier);
                        }

                        WritableTable.Columns.RemoveAt(0);

                        WritableTable.WriteXml(SaveDialog.FileName, XmlWriteMode.WriteSchema);
                    }
                    break;

                case Keys.L:

                    if (!UpdateListWorker.IsBusy)
                    {
                        OpenFileDialog OpenDialog = new OpenFileDialog();
                        OpenDialog.InitialDirectory = ThisConnection.OpenModel.GetInfo().ModelPath;
                        OpenDialog.FileName = "ObjectsTable.obr";

                        if (OpenDialog.ShowDialog() == DialogResult.OK)
                        {
                            LoadFileName = OpenDialog.FileName;
                            toolStripStatusLabelStatus.Text = "albl_Loading_list_-_Please_wait";
                            LoadListWorker.RunWorkerAsync();
                        }

                    }
                    else
                    {
                        LoadListWorker.CancelAsync();
                    }

                    break;

#endif
            }
        }

        private void InitializeTsConnection()
        {
            ThisConnection = new TSConnection();
        }

        private void InitializeDataClasses()
        {
            AllPropertiesXml = new PresentedPropertiesXml("ObjectBrowserAllProperties.xml", ThisConnection.OpenModel, false);
            ShowablePropertiesXml = new PresentedPropertiesXml("ObjectBrowserShowableProperties.xml", ThisConnection.OpenModel, true);
        }

        private void UpdateList()
        {
            AutoSelectOn = false;

            if (!HackForNotCheckingColumnOrderAnyMore)
                SaveAndUpdateList(GetChangedColumnOrderList(), false);
            else
                HackForNotCheckingColumnOrderAnyMore = false;


            DrawingControl.SuspendDrawing(ObjectsGrid);
            DrawingControl.SuspendDrawing(SumDGW);
            SumDGW.SuspendLayout();
            ObjectsGrid.SuspendLayout();

            ObjectsGrid.CurrentCell = null;
            ObjectsGrid.DataSource = null;

            SumDGW.CurrentCell = null;
            SumDGW.DataSource = null;

            if (ObjectsTable == null)
                ObjectsTable = new DataTable();

            if (SumsTable == null)
                SumsTable = new DataTable();

            CreateObjectsColumns();
            CreateSumsColumns();

            toolStripButtonGet.Image = imageList1.Images["delete_big.ico"];
            this.toolStripButtonGet.ToolTipText = ThisConnection.LocalizationForm.GetText("albl_Cancel");

            toolStripStatusLabelStatus.Text = ThisConnection.LocalizationForm.GetText("albl_Updating_-_please wait");

            QuickSearchUserCntrl.Enabled = false;
            toolStripButtonProperties.Enabled = false;
            AutoUpdateTSButton.Enabled = false;
            ObjectsGrid.Enabled = false;
            SumDGW.Enabled = false;

            UpdateListWorker.RunWorkerAsync();
        }

        private void CreateObjectsColumns()
        {
            ObjectsTable.Columns.Clear();
            ObjectsTable.Clear();

            ObjectsTable.Columns.Add("ModelObject", typeof(Tekla.Structures.Model.ModelObject));
            ObjectsTable.Columns.Add(ThisConnection.LocalizationForm.GetText("albl_Object"), typeof(string));

            foreach (PresentedProperties ShownProperty in ShowablePropertiesXml.VisiblePropertiesList)
            {
                if (PresentedPropertiesManage.GetBaseType(ShownProperty.PropertyType) == PresentedPropertiesManage.PropertyTypes.String)
                    ObjectsTable.Columns.Add(ShownProperty.Name, typeof(string));
                else if (PresentedPropertiesManage.GetBaseType(ShownProperty.PropertyType) == PresentedPropertiesManage.PropertyTypes.Int)
                    ObjectsTable.Columns.Add(ShownProperty.Name, typeof(int));
                else if (PresentedPropertiesManage.GetBaseType(ShownProperty.PropertyType) == PresentedPropertiesManage.PropertyTypes.Double)
                    ObjectsTable.Columns.Add(ShownProperty.Name, typeof(double));
                else if (PresentedPropertiesManage.GetBaseType(ShownProperty.PropertyType) == PresentedPropertiesManage.PropertyTypes.Date)
                    ObjectsTable.Columns.Add(ShownProperty.Name, typeof(DateTime));

                //NO Display unit
                //if (PresentedPropertiesManage.GetBaseType(ShownProperty.PropertyType) == PresentedPropertiesManage.PropertyTypes.String)
                //    ObjectsTable.Columns.Add(ShownProperty.Name + " / " + ShownProperty.DisplayType, typeof(string));
                //else if (PresentedPropertiesManage.GetBaseType(ShownProperty.PropertyType) == PresentedPropertiesManage.PropertyTypes.Int)
                //    ObjectsTable.Columns.Add(ShownProperty.Name + " / " + ShownProperty.DisplayType, typeof(int));
                //else if (PresentedPropertiesManage.GetBaseType(ShownProperty.PropertyType) == PresentedPropertiesManage.PropertyTypes.Double)
                //    ObjectsTable.Columns.Add(ShownProperty.Name + " / " + ShownProperty.DisplayType, typeof(double));
                //else if (PresentedPropertiesManage.GetBaseType(ShownProperty.PropertyType) == PresentedPropertiesManage.PropertyTypes.Date)
                //    ObjectsTable.Columns.Add(ShownProperty.Name + " / " + ShownProperty.DisplayType, typeof (DateTime));
            }
        }
        //TODO: refactor to same method
        private void CreateAllObjectsColumns()
        {
            AllObjectsTable.Columns.Clear();
            AllObjectsTable.Clear();

            AllObjectsTable.Columns.Add("ModelObject", typeof(Tekla.Structures.Model.ModelObject));
            AllObjectsTable.Columns.Add(ThisConnection.LocalizationForm.GetText("albl_Object"), typeof(string));

            foreach (PresentedProperties ShownProperty in ShowablePropertiesXml.PropertiesList)
            {
                if (PresentedPropertiesManage.GetBaseType(ShownProperty.PropertyType) == PresentedPropertiesManage.PropertyTypes.String)
                    AllObjectsTable.Columns.Add(ShownProperty.Name, typeof(string));
                else if (PresentedPropertiesManage.GetBaseType(ShownProperty.PropertyType) == PresentedPropertiesManage.PropertyTypes.Int)
                    AllObjectsTable.Columns.Add(ShownProperty.Name, typeof(int));
                else if (PresentedPropertiesManage.GetBaseType(ShownProperty.PropertyType) == PresentedPropertiesManage.PropertyTypes.Double)
                    AllObjectsTable.Columns.Add(ShownProperty.Name, typeof(double));
                else if (PresentedPropertiesManage.GetBaseType(ShownProperty.PropertyType) == PresentedPropertiesManage.PropertyTypes.Date)
                    AllObjectsTable.Columns.Add(ShownProperty.Name, typeof(DateTime));

                //NO display type
                //if (PresentedPropertiesManage.GetBaseType(ShownProperty.PropertyType) == PresentedPropertiesManage.PropertyTypes.String)
                //    AllObjectsTable.Columns.Add(ShownProperty.Name + " / " + ShownProperty.DisplayType, typeof(string));
                //else if (PresentedPropertiesManage.GetBaseType(ShownProperty.PropertyType) == PresentedPropertiesManage.PropertyTypes.Int)
                //    AllObjectsTable.Columns.Add(ShownProperty.Name + " / " + ShownProperty.DisplayType, typeof(int));
                //else if (PresentedPropertiesManage.GetBaseType(ShownProperty.PropertyType) == PresentedPropertiesManage.PropertyTypes.Double)
                //    AllObjectsTable.Columns.Add(ShownProperty.Name + " / " + ShownProperty.DisplayType, typeof(double));
                //else if (PresentedPropertiesManage.GetBaseType(ShownProperty.PropertyType) == PresentedPropertiesManage.PropertyTypes.Date)
                //    AllObjectsTable.Columns.Add(ShownProperty.Name + " / " + ShownProperty.DisplayType, typeof(DateTime));
            }
        }

        private void CreateSumsColumns()
        {
            SumsTable.Columns.Clear();
            SumsTable.Clear();

            SumsTable.Columns.Add("ModelObject", typeof(Tekla.Structures.Model.ModelObject));
            SumsTable.Columns.Add(ThisConnection.LocalizationForm.GetText("albl_Object"), typeof(string));

            foreach (PresentedProperties ShownProperty in ShowablePropertiesXml.VisiblePropertiesList)
            {
                SumsTable.Columns.Add(ShownProperty.Name /* + " / " + ShownProperty.DisplayType*/, typeof(string));
            }
        }

        private void alblSelectinmodelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ObjectsGrid != null)
            {
                ArrayList SelectedRowObjects = new ArrayList();

                foreach (DataGridViewRow Row in ObjectsGrid.SelectedRows)
                {
                    SelectedRowObjects.Add((ModelObject)Row.Cells[0].Value);
                }

                if (!ThisConnection.SelectObjectsInModel(SelectedRowObjects))
                    System.Diagnostics.Debug.WriteLine("Selection in model failed!");
            }
        }

        private void alblZoomtoselectedobjectsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ThisConnection.ZoomToSelectedObjects();
        }


        private void alblRemovefromlistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ObjectsGrid.SelectedRows.Count > 0)
            {
                try
                {
                    SaveColumnWidths();

                    DataTable SelectedRowCol = new DataTable();
                    DataTable NewObjectsTable = new DataTable();
                    SelectedRowCol = ObjectsTable.Clone();
                    NewObjectsTable = ObjectsTable.Copy();

                    foreach (DataGridViewRow Row in ObjectsGrid.SelectedRows)
                    {
                        SelectedRowCol.ImportRow((((DataRowView)Row.DataBoundItem).Row));
                    }

                    foreach (DataRow Row in SelectedRowCol.Rows)
                    {
                        for (int ii = 0; ii < ObjectsTable.Rows.Count; ii++)
                        {
                            if ((Row[0] as ModelObject).Identifier.ID == (ObjectsTable.Rows[ii][0] as ModelObject).Identifier.ID)
                                NewObjectsTable.Rows.RemoveAt(ii);
                        }
                    }

                    ObjectsTable = NewObjectsTable;

                    DrawingControl.SuspendDrawing(ObjectsGrid);
                    ObjectsGrid.DataSource = null;
                    ObjectsGrid.DataSource = ObjectsTable;
                    ObjectsGrid.Columns[0].Visible = false;
                    //ObjectsGrid.Columns[ObjectsGrid.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    UpdateColumnWidths();
                    DrawingControl.ResumeDrawing(ObjectsGrid);

                    CountSums(ObjectsTable.Rows);

                    toolStripStatusLabelCount.Text = string.Format(ThisConnection.LocalizationForm.GetText("albl_{0}/{1}_Objects_({2}_Hidden)"),ObjectsGrid.SelectedRows.Count, ObjectsGrid.Rows.Count, QuickSearchUserCntrl.GetHiddenRows());
                }
                catch (Exception ee)
                {
                    System.Diagnostics.Debug.WriteLine(ee.ToString(), "Remove from list failed");

                    DrawingControl.SuspendDrawing(ObjectsGrid);
                    ObjectsGrid.DataSource = null;
                    ObjectsTable.Rows.Clear();
                    ObjectsGrid.DataSource = ObjectsTable;
                    ObjectsGrid.Columns[0].Visible = false;
                    //ObjectsGrid.Columns[ObjectsGrid.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    UpdateColumnWidths();
                    DrawingControl.ResumeDrawing(ObjectsGrid);
                }
            }
        }

        private void toolStripButtonAuto_Click(object sender, EventArgs e)
        {
            AutoSelectOn = !AutoSelectOn;

            if (AutoSelectOn)
            {
                toolStripButtonAuto.Image = imageList1.Images["autoselect_objects_active_big.ico"];
            }
            else
            {
                toolStripButtonAuto.Image = imageList1.Images["autoselect_objects_passive_big.ico"];
            }

        }

        private void AutoUpdateTSButton_Click(object sender, EventArgs e)
        {
            AutoUpdateOn = !AutoUpdateOn;

            if (AutoUpdateOn)
            {
                AutoUpdateTSButton.Checked = true;
                toolStripButtonGet_Click(null, null);
            }
            else
            {
                AutoUpdateTSButton.Checked = false;

                if (SelectEventActivated)
                {
                    SelectEventActivated = false;
                    ThisConnection.TeklaEvents.SelectionChange -= new Events.SelectionChangeDelegate(TeklaEvents_SelectionChange);
                }
            }
        }

        private void toolStripButtonProperties_Click(object sender, EventArgs e)
        {
            ShowablePropertiesXml.PropertiesList = GetChangedColumnOrderList();
            SaveAndUpdateList(ShowablePropertiesXml.PropertiesList, false);
            PropertiesForm ShownProps = new PropertiesForm(this, ref AllPropertiesXml, ref ShowablePropertiesXml, ThisConnection);
            ShownProps.ShowDialog();
            HackForNotCheckingColumnOrderAnyMore = true;

            CreateAllObjectsColumns();
            OriginalObjectsTable.Clear();

            if (UpdateListWorker.IsBusy)
            {
                DoAnotherUpdate = true;
                UpdateListWorker.CancelAsync();
            }
            else
            {
                DoAnotherUpdate = false;
                UpdateList();
            }
        }

        private void toolStripButtonGet_Click(object sender, EventArgs e)
        {
            if (UpdateListWorker.IsBusy)
            {
                DoAnotherUpdate = false;
                UpdateListWorker.CancelAsync();
            }
            else
            {
                CreateAllObjectsColumns();
                DoAnotherUpdate = false;
                SelectedObjects = ThisConnection.GetSelectedObjects();
                GotNewObjects = true;
                UpdateList();
            }
        }


        #region BackGround get objects
        private void DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            System.Diagnostics.Stopwatch WorkWatch = new Stopwatch();
            //System.Diagnostics.Stopwatch DebugWatch = new Stopwatch();
            WorkWatch.Start();
            int Counter = 0;

            if (SelectedObjects != null)
            {
                ObjectsTable.DefaultView.Sort = string.Empty;
                AllObjectsTable.Clear();

                for (int ii = 0; ii < SelectedObjects.Count; ii++)
                {
                    //DebugWatch.Start();

                    if ((worker.CancellationPending == true))
                    {
                        e.Cancel = true;
                        break;
                    }
                    else
                    {
                        string ObjectType = "";

                        ObjectsTable.Rows.Add();
                        ObjectsTable.Rows[ii][0] = SelectedObjects[ii];

                        if (SelectedObjects[ii] != null)
                            ObjectType = SelectedObjects[ii].GetType().ToString();

                        if (ObjectType.Contains("Tekla.Structures.Model.") && ObjectType.Length > 23)
                            ObjectsTable.Rows[ii][1] = ObjectType.Substring(23);

                        AllObjectsTable.Rows.Add();
                        AllObjectsTable.Rows[ii][0] = SelectedObjects[ii];

                        if (SelectedObjects[ii] != null)
                            ObjectType = SelectedObjects[ii].GetType().ToString();

                        if (ObjectType.Contains("Tekla.Structures.Model.") && ObjectType.Length > 23)
                            AllObjectsTable.Rows[ii][1] = ObjectType.Substring(23);

                        int VisibleCounter = 2;

                        for (int jj = 0; jj < ShowablePropertiesXml.PropertiesList.Count; jj++)
                        {
                            if (ShowablePropertiesXml.PropertiesList[jj].Visible)
                            {
                                string PropertyName = "";

                                if (!string.IsNullOrEmpty(ShowablePropertiesXml.PropertiesList[jj].ReportPropertyName))
                                {
                                    PropertyName = ShowablePropertiesXml.PropertiesList[jj].ReportPropertyName;
                                }
                                else if (!string.IsNullOrEmpty(ShowablePropertiesXml.PropertiesList[jj].UdaPropertyName))
                                {
                                    PropertyName = ShowablePropertiesXml.PropertiesList[jj].UdaPropertyName;
                                }

                                if (!string.IsNullOrEmpty(PropertyName))
                                {
                                    object PropertyValue = null;

                                    //Get from model only if new objects fetched or a new property column
                                    if (!GotNewObjects && OriginalObjectsTable.Rows.Count > 0)
                                    {
                                        if (OriginalObjectsTable.Rows[ii][jj+2] != DBNull.Value) 
                                        {
                                            PropertyValue = OriginalObjectsTable.Rows[ii][jj+2];
                                        }
                                        else
                                        {
                                            PropertyValue = PresentedPropertiesManage.GetPartPropertyValue((ModelObject) SelectedObjects[ii],
                                                                                                           ShowablePropertiesXml.
                                                                                                               PropertiesList[jj].Name,
                                                                                                           PropertyName,
                                                                                                           ShowablePropertiesXml);
                                        }
                                    }
                                    else
                                    {
                                        PropertyValue = PresentedPropertiesManage.GetPartPropertyValue((ModelObject) SelectedObjects[ii],
                                                                                                       ShowablePropertiesXml.
                                                                                                           PropertiesList[jj].Name,
                                                                                                       PropertyName,
                                                                                                       ShowablePropertiesXml);
                                    }

                                    if (PropertyValue != null)
                                    {
                                        if (ShowablePropertiesXml.PropertiesList[jj].PropertyType == PresentedPropertiesManage.PropertyTypes.Date && (DateTime)PropertyValue == DateNull)
                                        {
                                            ObjectsTable.Rows[ii][VisibleCounter] = DBNull.Value; 
                                        }
                                        else
                                        {
                                            ObjectsTable.Rows[ii][VisibleCounter] = PropertyValue;
                                        }

                                        AllObjectsTable.Rows[ii][jj+2] = PropertyValue;
                                    }
                                    else
                                    {
                                        ObjectsTable.Rows[ii][VisibleCounter] = DBNull.Value;

                                        if (ShowablePropertiesXml.PropertiesList[jj].PropertyType == PresentedPropertiesManage.PropertyTypes.Date)
                                        {
                                            AllObjectsTable.Rows[ii][jj + 2] = DateNull;
                                        }
                                        else
                                        {
                                            AllObjectsTable.Rows[ii][jj + 2] = DBNull.Value;
                                        }
                                    }
                                }

                                VisibleCounter++;
                            }
                            else
                            {
                                //Update allprops with old value
                                if (!GotNewObjects && OriginalObjectsTable.Rows.Count > 0)
                                {
                                    AllObjectsTable.Rows[ii][jj + 2] = OriginalObjectsTable.Rows[ii][jj + 2];
                                }
                            }
                        }
                    }

                    if (WorkWatch.ElapsedMilliseconds > 2000)
                    {
                        if (WorkWatch.IsRunning)
                            WorkWatch.Stop();

                        double Count1 = (double)ii / SelectedObjects.Count;
                        int Count2 = (int)(100.0 * Count1);

                        if (Count2 > Counter * 10)
                        {
                            Counter++;
                            worker.ReportProgress(Count2);
                        }
                    }

                    //DebugWatch.Stop();
                    //System.Diagnostics.Debug.WriteLine("2: " + DebugWatch.ElapsedMilliseconds);
                }

                OriginalObjectsTable = AllObjectsTable.Copy();

                GotNewObjects = false;
            }
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (!toolStripProgressBar1.Visible)
                toolStripProgressBar1.Visible = true;

            this.toolStripProgressBar1.Value = e.ProgressPercentage;
        }

        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Cancelled))
            {
                this.toolStripStatusLabelStatus.Text = ThisConnection.LocalizationForm.GetText("albl_Cancelled");
            }

            else if (!(e.Error == null))
            {
                this.toolStripStatusLabelStatus.Text = (ThisConnection.LocalizationForm.GetText("albl_Error") + " " + e.Error.Message);
                CreateAllObjectsColumns();
                CreateObjectsColumns();
                CreateSumsColumns();
            }

            else
            {
                this.toolStripStatusLabelStatus.Text = ThisConnection.LocalizationForm.GetText("albl_Ready");
            }


            try
            {
                if (toolStripProgressBar1.Visible)
                {
                    toolStripProgressBar1.Visible = false;
                }

                CountSums(ObjectsTable.Rows);

                SumDGW.DataSource = SumsTable;
                SumDGW.Columns[0].Visible = false;

                ObjectsGrid.DataSource = ObjectsTable;

                if (AllwaysSearch)
                    QuickSearchUserCntrl.QuickSearchExecute();
                else
                    QuickSearchUserCntrl.SilentlyEmptySearchBox();

                ObjectsGrid.Columns[0].Visible = false;
                ObjectsGrid.Columns[ObjectsGrid.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                if (!AllwaysSearch)
                    UpdateColumnWidths();

                ObjectsGrid.ResumeLayout();
                DrawingControl.ResumeDrawing(ObjectsGrid);
                SumDGW.ResumeLayout();
                DrawingControl.ResumeDrawing(SumDGW);

                toolStripButtonGet.Image = imageList1.Images["move_down_24.ico"];

                QuickSearchUserCntrl.Enabled = true;
                toolStripButtonGet.Enabled = true;
                ObjectsGrid.Enabled = true;
                SumDGW.Enabled = true;
                ObjectsGrid.Columns[0].Frozen = true;
                ObjectsGrid.Columns[1].Frozen = true;
                SumDGW.Columns[0].Frozen = true;
                SumDGW.Columns[1].Frozen = true;
                toolStripButtonProperties.Enabled = true;
                AutoUpdateTSButton.Enabled = true;
                toolStripButtonGet.ToolTipText = ThisConnection.LocalizationForm.GetText("albl_Get_selected_objects_from_model");

                if (!AllwaysSearch)
                    toolStripStatusLabelCount.Text = string.Format(ThisConnection.LocalizationForm.GetText("albl_{0}/{1}_Objects_({2}_Hidden)"), ObjectsGrid.SelectedRows.Count, ObjectsGrid.Rows.Count, 0);

                MainForm_Deactivate(null, null);
                AutoSelectOn = true;

                if (DoAnotherUpdate)
                {
                    DoAnotherUpdate = false;
                    toolStripButtonGet_Click(null, null);
                }

                if(FirstTime)
                {
                    FirstTime = false;
                    SumDGWRowHeight = SumDGW.Rows[0].Height;
                }
            }
            catch (Exception ee)
            {
                System.Diagnostics.Debug.WriteLine(ee.ToString());
            }
        }

        #endregion get object background

        #region background load list

        private void LoadList_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            System.Diagnostics.Stopwatch WorkWatch = new Stopwatch();
            WorkWatch.Start();
            int Counter = 0;

            DataTable ReadTable = new DataTable();
            DataTable TmpTable = new DataTable();

            string NotFounds = "";
            
            ReadTable.TableName = "ObjectBrowser write test";
            ReadTable.ReadXmlSchema(LoadFileName);
            ReadTable.ReadXml(LoadFileName);

            TmpTable.Columns.Add("ModelObject", typeof(Tekla.Structures.Model.ModelObject));

            Dictionary<string, ModelObject> AllModelObjects = ThisConnection.GetAllObjects();

            SelectedObjects.Clear();

            for (int ii = 0; ii < ReadTable.Columns.Count - 1; ii++)
            {
                TmpTable.Columns.Add(ReadTable.Columns[ii].ColumnName, ReadTable.Columns[ii].DataType);
            }

            foreach (DataRow row in ReadTable.Rows)
            {
                string Guid = (string)row[ReadTable.Columns.Count - 1];

                try
                {
                    if (AllModelObjects.ContainsKey(Guid))
                    {
                        TmpTable.Rows.Add();
                        TmpTable.Rows[TmpTable.Rows.Count - 1][0] = AllModelObjects[Guid];

                        for (int ii = 0; ii < ReadTable.Columns.Count - 1; ii++)
                        {
                            TmpTable.Rows[TmpTable.Rows.Count - 1][ii + 1] = row[ii];
                        }

                        SelectedObjects.Add(AllModelObjects[Guid]);
                    }
                    else
                    {
                        NotFounds += Guid + " : " + row[0] + " ; ";
                    }
                }
                catch (Exception)
                {
                    NotFounds += Guid + " : " + row[0] + " ; ";
                }

                if (WorkWatch.ElapsedMilliseconds > 2000)
                {
                    WorkWatch.Stop();
                    int Count1 = (int)(100 * ((double)TmpTable.Rows.Count / ReadTable.Rows.Count));

                    if (Count1 > Counter * 10)
                    {
                        Counter++;
                        worker.ReportProgress(Count1);
                    }
                }
            }

            NotFoundsList = NotFounds;

            OriginalObjectsTable = TmpTable.Copy();
            AllObjectsTable = TmpTable.Copy();
        }

        private void LoadList_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (!toolStripProgressBar1.Visible)
                toolStripProgressBar1.Visible = true;

            this.toolStripProgressBar1.Value = e.ProgressPercentage;
        }

        private void LoadList_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Cancelled))
            {
                this.toolStripStatusLabelStatus.Text = ThisConnection.LocalizationForm.GetText("albl_Cancelled");
            }

            else if (!(e.Error == null))
            {
                this.toolStripStatusLabelStatus.Text = (ThisConnection.LocalizationForm.GetText("albl_Error") + " " + e.Error.Message);
            }

            else
            {
                this.toolStripStatusLabelStatus.Text = ThisConnection.LocalizationForm.GetText("albl_Ready");
            }


            try
            {
                if (toolStripProgressBar1.Visible)
                {
                    toolStripProgressBar1.Visible = false;
                }

                if(!string.IsNullOrEmpty(NotFoundsList))
                {
                    if (!string.IsNullOrEmpty(NotFoundsList))
                        MessageBox.Show(NotFoundsList, ThisConnection.LocalizationForm.GetText("albl_Following_objects_were_not_found_in_model"), MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                }

                UpdateList();

            }
            catch (Exception ee)
            {
                System.Diagnostics.Debug.WriteLine(ee.ToString());
            }
        }

        #endregion background load list

        #region Helpers

        private void CountSums(DataRowCollection RowsToCount)
        {
            try
            {
                DataTable TmpSumsTable = new DataTable();
                TmpSumsTable = SumsTable.Clone();

                TmpSumsTable.Rows.Add();
                TmpSumsTable.Rows[0][1] = ThisConnection.LocalizationForm.GetText("albl_Sum");
                TmpSumsTable.Rows.Add();
                TmpSumsTable.Rows[1][1] = ThisConnection.LocalizationForm.GetText("albl_Average");
                TmpSumsTable.Rows.Add();
                TmpSumsTable.Rows[2][1] = ThisConnection.LocalizationForm.GetText("albl_Minim");
                TmpSumsTable.Rows.Add();
                TmpSumsTable.Rows[3][1] = ThisConnection.LocalizationForm.GetText("albl_Max");

                for (int ii = 2; ii < ObjectsTable.Columns.Count; ii++)
                {
                    double Sum = 0.0;
                    double Max = double.MinValue;
                    double Min = double.MaxValue;
                    double Avg = 0.0;

                    DateTime MaxDate = DateTime.MinValue;
                    DateTime MinDate = DateTime.MaxValue;

                    if (((ObjectsTable.Columns[ii].DataType == System.Type.GetType("System.Int")) || (ObjectsTable.Columns[ii].DataType == System.Type.GetType("System.Double")))
                          && ObjectsTable.Rows.Count > 0)
                    {
                        int ShownDecimals = ShowablePropertiesXml.VisiblePropertiesList[ii - 2].Decimals;

                        foreach (DataRow CurrentRow in RowsToCount)
                        {
                            Sum = Math.Round(Sum + (double)CurrentRow[ii], ShownDecimals);

                            if ((double)CurrentRow[ii] < Min)
                                Min = Math.Round((double)CurrentRow[ii], ShownDecimals);

                            if ((double)CurrentRow[ii] > Max)
                                Max = Math.Round((double)CurrentRow[ii], ShownDecimals);
                        }

                        TmpSumsTable.Rows[0][ii] = Sum.ToString();
                        TmpSumsTable.Rows[2][ii] = Min.ToString();
                        TmpSumsTable.Rows[3][ii] = Max.ToString();

                        Avg = Math.Round((double)Sum / (double)ObjectsTable.Rows.Count, ShownDecimals);

                        TmpSumsTable.Rows[1][ii] = Avg.ToString();
                    }
                    else if ((ObjectsTable.Columns[ii].DataType == System.Type.GetType("System.DateTime"))
                          && ObjectsTable.Rows.Count > 0)
                    {
                        TmpSumsTable.Rows[0][ii] = "-";

                        foreach (DataRow CurrentRow in RowsToCount)
                        {
                            if (!(CurrentRow[ii] is DBNull))
                            {
                                DateTime ThisValue = (DateTime)CurrentRow[ii];

                                if (ThisValue < MinDate)
                                    MinDate = (DateTime)ThisValue;

                                if (ThisValue > MaxDate)
                                    MaxDate = ThisValue;
                            }
                        }

                        if (MinDate == DateTime.MaxValue)
                            TmpSumsTable.Rows[2][ii] = "-";
                        else
                            TmpSumsTable.Rows[2][ii] = MinDate;

                        if (MaxDate == DateTime.MinValue)
                            TmpSumsTable.Rows[3][ii] = "-";
                        else
                            TmpSumsTable.Rows[3][ii] = MaxDate;

                        TmpSumsTable.Rows[1][ii] = "-";

                    }
                    else
                    {
                        TmpSumsTable.Rows[0][ii] = "-";
                        TmpSumsTable.Rows[1][ii] = "-";
                        TmpSumsTable.Rows[2][ii] = "-";
                        TmpSumsTable.Rows[3][ii] = "-";
                    }
                }

                SumsTable.Rows.Clear();

                for (int ii = 0; ii < 4; ii++)
                {
                    if (SumMenuItems[ii].Checked)
                        SumsTable.ImportRow(TmpSumsTable.Rows[ii]);
                }
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("I have absolutely no idea why there is sometimes invalid cast exception here.");
            }
        }

        private void ShowHideColumns(MouseEventArgs e)
        {
            ShowablePropertiesXml.PropertiesList = GetChangedColumnOrderList();

            MenuItem[] Items = new MenuItem[ShowablePropertiesXml.PropertiesList.Count + SumMenuItems.Length + 1];
            int count = 0;

            foreach (PresentedProperties ShownProperty in ShowablePropertiesXml.PropertiesList)
            {
                MenuItem Item = new MenuItem(ShownProperty.Name, new EventHandler(ShowHideOnClick));

                if (ShownProperty.Visible)
                    Item.Checked = true;

                Items[count] = Item;
                count++;
            }

            Items[count] = new MenuItem("-");
            count++;

            for (int ii = 0; ii < 4; ii++)
            {
                Items[count] = SumMenuItems[ii];
                count++;
            }

            System.Windows.Forms.ContextMenu ShowHide = new ContextMenu(Items);
            ShowHide.Show(this, e.Location);

        }

        private bool HackForNotCheckingColumnOrderAnyMore = false;

        void ShowHideOnClick(object sender, EventArgs e)
        {
            MenuItem ThisItem = sender as MenuItem;
            int OrigSumsChecked = 0;

            for(int ii = 0; ii < 4; ii++)
                if(SumMenuItems[ii].Checked)
                    OrigSumsChecked++;

            if (ThisItem != null)
            {
                ThisItem.Checked = !ThisItem.Checked;

                foreach (PresentedProperties CurrentProperty in ShowablePropertiesXml.PropertiesList)
                {
                    if (CurrentProperty.Name == ThisItem.Text)
                    {
                        CurrentProperty.Visible = ThisItem.Checked;
                        break;
                    }
                }

                int SumsChecked = 0;

                for (int ii = 0; ii < 4; ii++)
                    if (SumMenuItems[ii].Checked)
                        SumsChecked++;

                if (SumsChecked > OrigSumsChecked)
                    splitContainer1.SplitterDistance = splitContainer1.SplitterDistance - SumDGWRowHeight;
                else if (SumsChecked < OrigSumsChecked)
                    splitContainer1.SplitterDistance = splitContainer1.SplitterDistance + SumDGWRowHeight;


                HackForNotCheckingColumnOrderAnyMore = true;
                SaveAndUpdateList(ShowablePropertiesXml.PropertiesList, true);
            }
        }

        private SearchableSortableBindingList<PresentedProperties> GetChangedColumnOrderList()
        {
            SearchableSortableBindingList<PresentedProperties> Result = ShowablePropertiesXml.PropertiesList;
            SearchableSortableBindingList<PresentedProperties> NewShownPropertiesList = new SearchableSortableBindingList<PresentedProperties>();
            SearchableSortableBindingList<PresentedProperties> TmpVisiblePropertiesList = new SearchableSortableBindingList<PresentedProperties>();
            PresentedProperties TmpProperty = null;
            int DisplayedPropertyCounter = 0;

            try
            {
                if (ObjectsGrid.Columns.Count > 0)
                {
                    //Get properties in order they are in DataGridView
                    for (int ii = 2; ii < ObjectsGrid.Columns.Count; ii++)
                    {
                        for (int jj = 2; jj < ObjectsGrid.Columns.Count; jj++)
                        {
                            if (ObjectsGrid.Columns[jj].DisplayIndex == (ii))
                            {
                                //NO Display units
                                //int LenghtOfName = ObjectsGrid.Columns[jj].HeaderText.IndexOf("/") - 1;
                                //string PropertyName = ObjectsGrid.Columns[jj].HeaderText.Substring(0, LenghtOfName);
                                string PropertyName = ObjectsGrid.Columns[jj].HeaderText;

                                ShowablePropertiesXml.GetPropertyByName(PropertyName, ref TmpProperty);
                                TmpVisiblePropertiesList.Add(TmpProperty);
                            }
                        }
                    }

                    //Replace visible properties in order they are in DataGridView
                    foreach (PresentedProperties OldProperty in ShowablePropertiesXml.PropertiesList)
                    {
                        if (OldProperty.Visible)
                        {
                            NewShownPropertiesList.Add(TmpVisiblePropertiesList[DisplayedPropertyCounter]);
                            DisplayedPropertyCounter++;
                        }
                        else
                        {
                            NewShownPropertiesList.Add(OldProperty);
                        }
                    }

                    Result = NewShownPropertiesList;

                }

                SaveColumnWidths();
            }
            catch (Exception)
            {
                Result = ShowablePropertiesXml.PropertiesList;
            }

            return Result;
        }

        private void SaveAndUpdateList(SearchableSortableBindingList<PresentedProperties> NewShownPropertiesList, bool DoUpdate)
        {
            ShowablePropertiesXml.XmlWriteProperties(NewShownPropertiesList);
            ShowablePropertiesXml.ForceReadFile();

            if (DoUpdate)
            {
                if (UpdateListWorker.IsBusy)
                {
                    DoAnotherUpdate = true;
                    UpdateListWorker.CancelAsync();
                }
                else
                {
                    DoAnotherUpdate = false;
                    UpdateList();
                }
            }
        }

        private void UpdateColumnWidths()
        {
            if (ObjectsGrid.Columns != null && ObjectsGrid.Columns.Count > 0)
            {
                DrawingControl.SuspendDrawing(ObjectsGrid);
                DrawingControl.SuspendDrawing(SumDGW);

                ObjectsGrid.Columns[1].Width = TypeColumnWidth;
                SumDGW.Columns[1].Width = TypeColumnWidth;

                for (int ii = 2; ii < ObjectsGrid.Columns.Count; ii++)
                {
                    ObjectsGrid.Columns[ii].Width = SumDGW.Columns[ii].Width = ShowablePropertiesXml.VisiblePropertiesList[ii - 2].Width;
                }

                DrawingControl.ResumeDrawing(ObjectsGrid);
                DrawingControl.ResumeDrawing(SumDGW);
            }
        }

        private void SaveColumnWidths()
        {
            if (ObjectsGrid.Columns != null && ObjectsGrid.Columns.Count > 0)
            {
                TypeColumnWidth = ObjectsGrid.Columns[1].Width;

                for (int ii = 2; ii < ObjectsGrid.Columns.Count; ii++)
                {
                    ShowablePropertiesXml.VisiblePropertiesList[ii - 2].Width = ObjectsGrid.Columns[ii].Width;
                }
            }
        }

        private void BeforeSearch()
        {
            SaveColumnWidths();

            DrawingControl.SuspendDrawing(ObjectsGrid);
            DrawingControl.SuspendDrawing(SumDGW);

            ObjectsGrid.DataSource = null;
        }

        private void AfterSearch(DataTable ResultTable)
        {
            ObjectsGrid.DataSource = ObjectsTable = ResultTable;
            ObjectsGrid.Columns[ObjectsGrid.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            DrawingControl.ResumeDrawing(ObjectsGrid);
            DrawingControl.ResumeDrawing(SumDGW);

            UpdateColumnWidths();

            if(ObjectsGrid.Columns.Count>0)
                ObjectsGrid.Columns[0].Visible = false;

            CountSums(ObjectsTable.Rows);

            toolStripStatusLabelCount.Text = string.Format(ThisConnection.LocalizationForm.GetText("albl_{0}/{1}_Objects_({2}_Hidden)"), 1, ObjectsGrid.Rows.Count, QuickSearchUserCntrl.GetHiddenRows());
        }

        private void InitializeSumItemList()
        {
            string []Names = new string[4];

            Names[0] = ThisConnection.LocalizationForm.GetText("albl_Sum");
            Names[1] = ThisConnection.LocalizationForm.GetText("albl_Average");
            Names[2] = ThisConnection.LocalizationForm.GetText("albl_Minim");
            Names[3] = ThisConnection.LocalizationForm.GetText("albl_Max");

            for(int ii = 0 ; ii<4; ii++)
            {
                MenuItem MItem = new MenuItem(Names[ii], new EventHandler(ShowHideOnClick));
                MItem.Checked = true;
                SumMenuItems[ii] = MItem;
            }
        }
        #endregion
    }
}
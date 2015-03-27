namespace Tekla.Structures
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Remoting;

    using Tekla.Structures.Drawing;

    using TSDrawing = Tekla.Structures.Drawing.Drawing;
    using TSDrawingConnection = Tekla.Structures.Drawing.DrawingHandler;
    using TSDrawingEnumerator = Tekla.Structures.Drawing.DrawingEnumerator;
    using TSDrawingEvents = Tekla.Structures.Drawing.UI.Events;
    using TSDrawingObject = Tekla.Structures.Drawing.DrawingObject;
    using TSDrawingObjectEnumerator = Tekla.Structures.Drawing.DrawingObjectEnumerator;
    using TSDrawingObjectSelector = Tekla.Structures.Drawing.UI.DrawingObjectSelector;
    using TSDrawingPicker = Tekla.Structures.Drawing.UI.Picker;
    using TSDrawingViewBase = Tekla.Structures.Drawing.ViewBase;

    /// <summary>
    /// Drawing connection.
    /// </summary>
    internal class DrawingConnection : IDrawing
    {
        #region Constants

        /// <summary>
        /// Timeout for long operations.
        /// </summary>
        private const int LongOperation = 60000;

        #endregion

        #region Fields

        /// <summary>
        /// Model connection.
        /// </summary>
        private readonly ModelConnection model;

        /// <summary>
        /// Drawing connection object.
        /// </summary>
        private DrawingHandler connection;

        /// <summary>
        /// Drawing event dispatcher.
        /// </summary>
        private TSDrawingEvents events;

        /// <summary>
        /// Indicates whether event dispatcher has been registered.
        /// </summary>
        private bool eventsRegistered;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DrawingConnection"/> class. 
        /// Initializes a new instance of the class.
        /// </summary>
        public DrawingConnection()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="DrawingConnection"/> class. 
        /// Initializes a new instance of the class.</summary>
        /// <param name="model">Model connection.</param>
        public DrawingConnection(ModelConnection model)
        {
            this.model = model;
        }

        #endregion

        #region Public Events

        /// <summary>
        /// Announces that the changes have been committed.
        /// </summary>
        public event EventHandler ChangesCommitted;

        /// <summary>
        /// Announces that the changes have been discarded.
        /// </summary>
        public event EventHandler ChangesDiscarded;

        /// <summary>
        /// Announces that a drawing has been loaded.
        /// </summary>
        public event EventHandler DrawingLoaded
        {
            add
            {
                this.UnregisterEvents();
                this.DrawingLoadedEvent += value;
                this.RegisterEvents();
            }

            remove
            {
                this.UnregisterEvents();
                this.DrawingLoadedEvent -= value;
                this.RegisterEvents();
            }
        }

        /// <summary>
        /// Announces that the drawing editor has been closed.
        /// </summary>
        public event EventHandler EditorClosed
        {
            add
            {
                this.UnregisterEvents();
                this.EditorClosedEvent += value;
                this.RegisterEvents();
            }

            remove
            {
                this.UnregisterEvents();
                this.EditorClosedEvent -= value;
                this.RegisterEvents();
            }
        }

        /// <summary>
        /// Announces that the drawing editor has been opened.
        /// </summary>
        public event EventHandler EditorOpened
        {
            add
            {
                this.UnregisterEvents();
                this.EditorOpenedEvent += value;
                this.RegisterEvents();
            }

            remove
            {
                this.UnregisterEvents();
                this.EditorOpenedEvent -= value;
                this.RegisterEvents();
            }
        }

        /// <summary>
        /// Announces that the selection has changed.
        /// </summary>
        public event EventHandler SelectionChanged
        {
            add
            {
                this.UnregisterEvents();
                this.SelectionChangedEvent += value;
                this.RegisterEvents();
            }

            remove
            {
                this.UnregisterEvents();
                this.SelectionChangedEvent -= value;
                this.RegisterEvents();
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Announces that a drawing has been loaded.
        /// </summary>
        private event EventHandler DrawingLoadedEvent;

        /// <summary>
        /// Announces that the drawing editor has been closed.
        /// </summary>
        private event EventHandler EditorClosedEvent;

        /// <summary>
        /// Announces that the drawing editor has been opened.
        /// </summary>
        private event EventHandler EditorOpenedEvent;

        /// <summary>
        /// Announces that the selection has changed.
        /// </summary>
        private event EventHandler SelectionChangedEvent;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets all objects in the view.
        /// </summary>
        /// <value>
        /// Enumerable collection of objects.
        /// </value>
        public IEnumerable<object> AllObjects
        {
            get
            {
                var snapshot = new List<object>();

                try
                {
                    if (this.IsEditorOpen)
                    {
                        SeparateThread.Execute(
                            LongOperation, 
                            delegate
                                {
                                    foreach (var item in this.connection.GetActiveDrawing().GetSheet().GetAllObjects())
                                    {
                                        if (item != null)
                                        {
                                            snapshot.Add(item);
                                        }
                                    }
                                });
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);

                    snapshot.Clear();
                }

                return snapshot;
            }
        }

        /// <summary>
        /// Gets the current drawing.
        /// </summary>
        /// <value>Current drawing.</value>
        public TSDrawing Current
        {
            get
            {
                if (this.IsActive)
                {
                    return SeparateThread.Execute<TSDrawing>(this.connection.GetActiveDrawing);
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the drawings.
        /// </summary>
        /// <value>
        /// Enumerable collection of drawings.
        /// </value>
        public ICollection<TSDrawing> Drawings
        {
            get
            {
                return new DrawingCollection(this);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the object is active.
        /// </summary>
        /// <value>
        /// Indicates whether the object is active.
        /// </value>
        public bool IsActive
        {
            get
            {
                return this.connection != null && this.connection.GetConnectionStatus();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the drawing editor is open.
        /// </summary>
        /// <value>
        /// Indicates whether the drawing editor is open.
        /// </value>
        public bool IsEditorOpen
        {
            get
            {
                return this.connection != null && SeparateThread.Execute<TSDrawing>(this.connection.GetActiveDrawing) != null;
            }
        }

        /// <summary>
        /// Gets the selected objects in the view.
        /// </summary>
        /// <value>
        /// Enumerable collection of objects.
        /// </value>
        public IEnumerable<object> SelectedObjects
        {
            get
            {
                var snapshot = new List<object>();

                try
                {
                    if (this.IsEditorOpen)
                    {
                        SeparateThread.Execute(
                            LongOperation, 
                            delegate
                                {
                                    foreach (var item in this.connection.GetDrawingObjectSelector().GetSelected())
                                    {
                                        if (item != null)
                                        {
                                            snapshot.Add(item);
                                        }
                                    }
                                });
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);

                    snapshot.Clear();
                }

                return snapshot;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Closes the drawing editor.
        /// </summary>
        /// <returns>A boolean value indicating whether the drawing editor was closed.</returns>
        public bool Close()
        {
            if (this.IsEditorOpen)
            {
                return SeparateThread.Execute<bool>(this.connection.CloseActiveDrawing);
            }

            return false;
        }

        /// <summary>Closes the drawing editor.</summary>
        /// <param name="saveBeforeClosing">Indicates whether the drawing is saved before closing.</param>
        /// <returns>A boolean value indicating whether the drawing editor was closed.</returns>
        public bool Close(bool saveBeforeClosing)
        {
            if (this.IsEditorOpen)
            {
                return
                    SeparateThread.Execute<bool>(delegate { return this.connection.CloseActiveDrawing(saveBeforeClosing); });
            }

            return false;
        }

        /// <summary>Commits the changes made to the objects.</summary>
        /// <param name="objects">Enumerable collection of modified objects.</param>
        public void CommitChanges(IEnumerable<object> objects)
        {
            if (this.IsEditorOpen)
            {
                try
                {
                    foreach (var item in objects)
                    {
                        var drawingObject = item as TSDrawingObject;

                        if (drawingObject != null)
                        {
                            drawingObject.Modify();
                        }
                    }

                    if (SeparateThread.Execute<bool>(delegate { return this.connection.GetActiveDrawing().CommitChanges(); }))
                    {
                        if (this.ChangesCommitted != null)
                        {
                            this.ChangesCommitted(this, EventArgs.Empty);
                        }
                    }
                }
                catch (RemotingException e)
                {
                    Debug.WriteLine(e.ToString());
                    this.DiscardChanges(objects);
                }
            }
        }

        /// <summary>
        /// Connects the drawing interface.
        /// </summary>
        /// <returns>A boolean value indicating whether the interface is connected.</returns>
        public bool Connect()
        {
            try
            {
                if (this.connection == null)
                {
                    this.connection = new TSDrawingConnection();

                    SeparateThread.Execute(
                        delegate
                            {
                                TSDrawingConnection.SetMessageExecutionStatus(
                                    TSDrawingConnection.MessageExecutionModeEnum.BY_COMMIT);
                            });
                }

                if (this.events == null)
                {
                    this.events = new TSDrawingEvents();

                    this.RegisterEvents();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);

                this.Disconnect();
            }

            return this.IsActive;
        }

        /// <summary>Discards the changed made to the objects.</summary>
        /// <param name="objects">Enumerable collection of modified objects.</param>
        public void DiscardChanges(IEnumerable<object> objects)
        {
            if (this.ChangesDiscarded != null)
            {
                this.ChangesDiscarded(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Disconnects the drawing interface.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                this.UnregisterEvents();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                this.events = null;
                this.connection = null;
            }
        }

        /// <summary>Opens a drawing in the drawing editor.</summary>
        /// <param name="drawing">Drawing to open.</param>
        /// <returns>A boolean value indicating whether the drawing was opened.</returns>
        public bool Open(TSDrawing drawing)
        {
            if (this.IsActive)
            {
                return SeparateThread.Execute<bool>(delegate { return this.connection.SetActiveDrawing(drawing, true); });
            }

            return false;
        }

        /// <summary>Picks an object.</summary>
        /// <param name="prompt">Pick prompt.</param>
        /// <returns>Picked object or null if no object was picked.</returns>
        public object PickObject(string prompt)
        {
            if (this.IsEditorOpen)
            {
                ViewBase viewBase;
                DrawingObject drawingObject;

                try
                {
                    // NOTE: Picking can not be placed into separate thread as the picking
                    // action may take arbitrary long time if the user does not select anything.
                    this.connection.GetPicker().PickObject(prompt, out drawingObject, out viewBase);
                    return drawingObject;
                }
                catch (RemotingException e)
                {
                    Debug.WriteLine(e.ToString());
                }
                catch (PickerInterruptedException)
                {
                    // No action.
                }
            }

            return null;
        }

        /// <summary>Executes a macro in the view.</summary>
        /// <param name="macroName">Macro name.</param>
        public void RunMacro(string macroName)
        {
            if (this.IsEditorOpen && this.model != null)
            {
                this.model.RunMacro(@"..\drawings\" + macroName);
            }
        }

        /// <summary>
        /// Saves the current drawing.
        /// </summary>
        /// <returns>A boolean value indicating whether the drawing was saved.</returns>
        public bool Save()
        {
            if (this.IsEditorOpen)
            {
                return SeparateThread.Execute<bool>(this.connection.SaveActiveDrawing);
            }

            return false;
        }

        /// <summary>Selects object by identifier.</summary>
        /// <param name="identifier">Object identifier.</param>
        /// <returns>Selected object or null if no matching object was found.</returns>
        public object SelectObjectByIdentifier(Identifier identifier)
        {
            return null;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raises the DrawingLoaded event.
        /// </summary>
        private void OnDrawingLoaded()
        {
            this.DrawingLoadedEvent.BeginInvoke(this, EventArgs.Empty, null, null);
        }

        /// <summary>
        /// Raises the EditorClosed event.
        /// </summary>
        private void OnEditorClosed()
        {
            this.EditorClosedEvent.BeginInvoke(this, EventArgs.Empty, null, null);
        }

        /// <summary>
        /// Raises the EditorOpened event.
        /// </summary>
        private void OnEditorOpened()
        {
            this.EditorOpenedEvent.BeginInvoke(this, EventArgs.Empty, null, null);
        }

        /// <summary>
        /// Raises the SelectionChanged event.
        /// </summary>
        private void OnSelectionChanged()
        {
            this.SelectionChangedEvent.BeginInvoke(this, EventArgs.Empty, null, null);
        }

        /// <summary>
        /// Registers the event handlers.
        /// </summary>
        private void RegisterEvents()
        {
            if (this.events != null && !this.eventsRegistered)
            {
                SeparateThread.Execute(
                    delegate
                        {
                            var registeredHandlers = 0;

                            if (this.EditorOpenedEvent != null)
                            {
                                this.events.DrawingEditorOpened += this.OnEditorOpened;
                                registeredHandlers++;
                            }

                            if (this.EditorClosedEvent != null)
                            {
                                this.events.DrawingEditorClosed += this.OnEditorClosed;
                                registeredHandlers++;
                            }

                            if (this.DrawingLoadedEvent != null)
                            {
                                this.events.DrawingLoaded += this.OnDrawingLoaded;
                                registeredHandlers++;
                            }

                            if (this.SelectionChangedEvent != null)
                            {
                                this.events.SelectionChange += this.OnSelectionChanged;
                                registeredHandlers++;
                            }

                            if (registeredHandlers > 0)
                            {
                                this.events.Register();

                                this.eventsRegistered = true;
                            }
                        });
            }
        }

        /// <summary>
        /// Removes the event handler registration.
        /// </summary>
        private void UnregisterEvents()
        {
            if (this.events != null && this.eventsRegistered)
            {
                SeparateThread.Execute(
                    delegate
                        {
                            this.eventsRegistered = false;

                            this.events.UnRegister();

                            if (this.EditorOpenedEvent != null)
                            {
                                this.events.DrawingEditorOpened -= this.OnEditorOpened;
                            }

                            if (this.EditorClosedEvent != null)
                            {
                                this.events.DrawingEditorClosed -= this.OnEditorClosed;
                            }

                            if (this.DrawingLoadedEvent != null)
                            {
                                this.events.DrawingLoaded -= this.OnDrawingLoaded;
                            }

                            if (this.SelectionChangedEvent != null)
                            {
                                this.events.SelectionChange -= this.OnSelectionChanged;
                            }
                        });
            }
        }

        #endregion

        /// <summary>
        /// Read-only collection of drawings.
        /// </summary>
        private sealed class DrawingCollection : ICollection<TSDrawing>
        {
            #region Fields

            /// <summary>
            /// Parent connection.
            /// </summary>
            private readonly DrawingConnection parent;

            #endregion

            #region Constructors and Destructors

            /// <summary>Initializes a new instance of the <see cref="DrawingCollection"/> class. 
            /// Initializes a new instance of the class.</summary>
            /// <param name="parent">The parent.</param>
            public DrawingCollection(DrawingConnection parent)
            {
                this.parent = parent;
            }

            #endregion

            #region Public Properties

            /// <summary>
            /// Gets the number of drawings in the collection.
            /// </summary>
            /// <returns>The number of drawings in the collection.</returns>
            public int Count
            {
                get
                {
                    var count = 0;

                    try
                    {
                        if (this.parent.IsActive)
                        {
                            SeparateThread.Execute(
                                LongOperation, 
                                delegate
                                    {
                                        var enumerator = this.parent.connection.GetDrawings();
                                        enumerator.SelectInstances = false;

                                        foreach (TSDrawing item in enumerator)
                                        {
                                            if (item != null)
                                            {
                                                count++;
                                            }
                                        }
                                    });
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }

                    return count;
                }
            }

            /// <summary>
            /// Gets a value indicating whether the collection is read-only.
            /// </summary>
            /// <returns>A boolean value indicating whether the collection is read-only.</returns>
            public bool IsReadOnly
            {
                get
                {
                    return true;
                }
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>Not supported.</summary>
            /// <param name="item">The item value.</param>
            public void Add(TSDrawing item)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Not supported.
            /// </summary>
            public void Clear()
            {
                throw new NotSupportedException();
            }

            /// <summary>Determines whether the collection contains the specified drawing.</summary>
            /// <param name="item">Drawing to match.</param>
            /// <returns>A boolean value indicating whether the collection contains the specified drawing.</returns>
            [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed. Suppression is OK here.")]
            public bool Contains(TSDrawing item)
            {
                foreach (var drawing in this)
                {
                    if (Equals(drawing, item))
                    {
                        return true;
                    }
                }

                return false;
            }

            /// <summary>Copies the drawings to an array.</summary>
            /// <param name="array">Target array.</param>
            /// <param name="arrayIndex">Starting index in the array.</param>
            public void CopyTo(TSDrawing[] array, int arrayIndex)
            {
                foreach (var drawing in this)
                {
                    array[arrayIndex++] = drawing;
                }
            }

            /// <summary>
            /// Gets the enumerator for the collection.
            /// </summary>
            /// <returns>Enumerator for the collection.</returns>
            public IEnumerator<TSDrawing> GetEnumerator()
            {
                var snapshot = new List<TSDrawing>();

                try
                {
                    if (this.parent.IsActive)
                    {
                        SeparateThread.Execute(
                            LongOperation, 
                            delegate
                                {
                                    foreach (TSDrawing item in this.parent.connection.GetDrawings())
                                    {
                                        if (item != null)
                                        {
                                            snapshot.Add(item);
                                        }
                                    }
                                });
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);

                    snapshot.Clear();
                }

                return snapshot.GetEnumerator();
            }

            /// <summary>Not supported.</summary>
            /// <param name="item">The item value.</param>
            /// <returns>The System.Boolean.</returns>
            public bool Remove(TSDrawing item)
            {
                throw new NotSupportedException();
            }

            #endregion

            #region Explicit Interface Methods

            /// <summary>
            /// Gets the enumerator for the collection.
            /// </summary>
            /// <returns>Enumerator for the collection.</returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            #endregion
        }
    }
}
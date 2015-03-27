namespace Tekla.Structures
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Connection interface implementation.
    /// </summary>
    internal class Connection : IConnection
    {
        #region Fields

        /// <summary>
        /// Drawing connection instance.
        /// </summary>
        private readonly DrawingConnection drawing;

        /// <summary>
        /// Model connection instance.
        /// </summary>
        private readonly ModelConnection model;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Connection"/> class. 
        /// Initializes a new instance of the class.
        /// </summary>
        public Connection()
        {
            this.model = new ModelConnection();
            this.drawing = new DrawingConnection(this.model);
        }

        #endregion

        #region Public Events

        /// <summary>
        /// Announces that the changes have been committed.
        /// </summary>
        public event EventHandler ChangesCommitted
        {
            add
            {
                this.model.ChangesCommitted += value;
                this.drawing.ChangesCommitted += value;
            }

            remove
            {
                this.model.ChangesCommitted -= value;
                this.drawing.ChangesCommitted -= value;
            }
        }

        /// <summary>
        /// Announces that the changes have been discarded.
        /// </summary>
        public event EventHandler ChangesDiscarded
        {
            add
            {
                this.model.ChangesDiscarded += value;
                this.drawing.ChangesDiscarded += value;
            }

            remove
            {
                this.model.ChangesDiscarded -= value;
                this.drawing.ChangesDiscarded -= value;
            }
        }

        /// <summary>
        /// Announces that the selection has changed.
        /// </summary>
        public event EventHandler SelectionChanged
        {
            add
            {
                this.model.SelectionChanged += value;
                this.drawing.SelectionChanged += value;
            }

            remove
            {
                this.model.SelectionChanged -= value;
                this.drawing.SelectionChanged -= value;
            }
        }

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
                if (this.drawing.IsEditorOpen)
                {
                    return this.drawing.AllObjects;
                }
                else
                {
                    return this.model.AllObjects;
                }
            }
        }

        /// <summary>
        /// Gets the drawing connection instance.
        /// </summary>
        /// <value>
        /// Drawing connection instance.
        /// </value>
        public DrawingConnection Drawing
        {
            get
            {
                return this.drawing;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the view is active.
        /// </summary>
        /// <value>
        /// Indicates whether the view is active.
        /// </value>
        public bool IsActive
        {
            get
            {
                return this.model.IsActive && this.drawing.IsActive;
            }
        }

        /// <summary>
        /// Gets the model connection instance.
        /// </summary>
        /// <value>
        /// Model connection instance.
        /// </value>
        public ModelConnection Model
        {
            get
            {
                return this.model;
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
                if (this.drawing.IsEditorOpen)
                {
                    return this.drawing.SelectedObjects;
                }
                else
                {
                    return this.model.SelectedObjects;
                }
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Commits the changes made to the objects.</summary>
        /// <param name="objects">Enumerable collection of modified objects.</param>
        public void CommitChanges(IEnumerable<object> objects)
        {
            if (this.drawing.IsEditorOpen)
            {
                this.drawing.CommitChanges(objects);
            }
            else
            {
                this.model.CommitChanges(objects);
            }
        }

        /// <summary>
        /// Connects the interface.
        /// </summary>
        /// <returns>A boolean value indicating whether the interface was connected.</returns>
        public bool Connect()
        {
            return this.model.Connect() && this.drawing.Connect();
        }

        /// <summary>Discards the changed made to the objects.</summary>
        /// <param name="objects">Enumerable collection of modified objects.</param>
        public void DiscardChanges(IEnumerable<object> objects)
        {
            if (this.drawing.IsEditorOpen)
            {
                this.drawing.DiscardChanges(objects);
            }
            else
            {
                this.model.DiscardChanges(objects);
            }
        }

        /// <summary>
        /// Disconnects the interface.
        /// </summary>
        public void Disconnect()
        {
            this.model.Disconnect();
            this.drawing.Disconnect();
        }

        /// <summary>Picks an object.</summary>
        /// <param name="prompt">Pick prompt.</param>
        /// <returns>Picked object or null if no object was picked.</returns>
        public object PickObject(string prompt)
        {
            if (this.drawing.IsEditorOpen)
            {
                return this.drawing.PickObject(prompt);
            }
            else
            {
                return this.model.PickObject(prompt);
            }
        }

        /// <summary>Executes a macro.</summary>
        /// <param name="macroName">Macro name.</param>
        public void RunMacro(string macroName)
        {
            if (this.drawing.IsEditorOpen)
            {
                this.drawing.RunMacro(macroName);
            }
            else
            {
                this.model.RunMacro(macroName);
            }
        }

        /// <summary>Selects object by identifier.</summary>
        /// <param name="identifier">Object identifier.</param>
        /// <returns>Selected object or null if no matching object was found.</returns>
        public object SelectObjectByIdentifier(Identifier identifier)
        {
            return this.model.SelectObjectByIdentifier(identifier);
        }

        #endregion
    }
}
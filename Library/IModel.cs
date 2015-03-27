namespace Tekla.Structures
{
    using System;

    /// <summary>
    /// Model connection interface.
    /// </summary>
    public interface IModel : IConnection
    {
        #region Public Events

        /// <summary>
        /// Announces that the model has been changed.
        /// </summary>
        event EventHandler Changed;

        /// <summary>
        /// Announces that the model has been loaded.
        /// </summary>
        event EventHandler Loaded;

        /// <summary>
        /// Announces that the model is being numbered.
        /// </summary>
        event EventHandler Numbering;

        /// <summary>
        /// Announces that the model has been saved.
        /// </summary>
        event EventHandler Saved;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the model folder.
        /// </summary>
        /// <value>
        /// Model folder.
        /// </value>
        ModelFolder Folder { get; }

        /// <summary>
        /// Gets the model name.
        /// </summary>
        /// <value>
        /// Model name.
        /// </value>
        string Name { get; }

        #endregion
    }
}
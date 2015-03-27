namespace Tekla.UI.HyperToolTips
{
    using System;
    using System.Diagnostics;
    using System.Windows.Forms;

    /// <summary>
    /// Helper class to prepare a form for HyperToolTips support. Allow to attache HyperToolTips to all UI elements on the form.
    /// Enumerates all controls and toolbars inside form and attachs nesessary handles to show/hide tooltips to each control.
    /// Controls Tag property is used as a TipContextId. For all <see cref="Control"/>s and <see cref="ToolStripItem"/>s
    /// with not empty tags tooltip is attached.
    /// 
    /// These helpers can be used to add HyperToolTips programmically on runtime,
    /// but it is not recomended, design time support in <seealso cref="HyperToolTipProvider"/> should be used instead.
    /// </summary>
    [Obsolete]
    public static class TipLoader
    {
        #region Public Methods and Operators

        /// <summary>Populate hyper tooltips provider with all <see cref="Control"/>s and <see cref="ToolStripItem"/>s found on the form.
        /// UI element's Tag property is used as HyperToolTipId.</summary>
        /// <param name="form"><see cref="Form"/> to enumerate UI elements in.</param>
        /// <param name="provider"><see cref="HyperToolTipProvider"/> instance to populate</param>
        public static void PopulateWithAllControls(Form form, HyperToolTipProvider provider)
        {
            AddEventHandlers(form, provider);
        }

        /// <summary>Populate hyper tooltips provider with all toolbars found on the form.<see cref="ToolStripItem"/>'s Tag property is used as HyperToolTipId.
        /// Attaches tooltips only to toolbar on the form.</summary>
        /// <param name="form"><see cref="Form"/> to enumerate UI elements in.</param>
        /// <param name="provider"><see cref="HyperToolTipProvider"/> instance to populate</param>
        public static void PopulateWithToolbars(Form form, HyperToolTipProvider provider)
        {
            foreach (Control child in form.Controls)
            {
                if (child is ToolStrip)
                {
                    AddEventHandlers(child as ToolStrip, provider);
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>The add event handlers.</summary>
        /// <param name="control">The control.</param>
        /// <param name="provider">The provider.</param>
        private static void AddEventHandlers(Control control, HyperToolTipProvider provider)
        {
            Debug.Assert(control != null);
            if (control.Tag != null && !string.IsNullOrEmpty(control.Tag.ToString()))
            {
                // Console.WriteLine(control.Tag.ToString());
                // control.MouseHover += parentform.button1_Hover;

                // attach handler
                provider.SetHyperToolTipId(control, control.Tag.ToString());
            }

            if (control is ToolStrip)
            {
                AddEventHandlers(control as ToolStrip, provider);
            }
            else
            {
                // iterate thru child controls
                foreach (Control child in control.Controls)
                {
                    AddEventHandlers(child, provider);
                }
            }
        }

        /// <summary>The add event handlers.</summary>
        /// <param name="control">The control.</param>
        /// <param name="provider">The provider.</param>
        private static void AddEventHandlers(ToolStrip control, HyperToolTipProvider provider)
        {
            foreach (ToolStripItem item in control.Items)
            {
                if (item is ToolStripDropDownItem)
                {
                    AddEventHandlers(item as ToolStripDropDownItem, provider);
                }
                else
                {
                    if (item.Tag != null && !string.IsNullOrEmpty(item.Tag.ToString()))
                    {
                        // attach handler
                        provider.SetHyperToolTipId(item, item.Tag.ToString());
                    }
                }
            }
        }

        /// <summary>The add event handlers.</summary>
        /// <param name="control">The control.</param>
        /// <param name="provider">The provider.</param>
        private static void AddEventHandlers(ToolStripItem control, HyperToolTipProvider provider)
        {
            if (control.Tag != null && !string.IsNullOrEmpty(control.Tag.ToString()))
            {
                // attach handler
                provider.SetHyperToolTipId(control, control.Tag.ToString());
            }

            if (control is ToolStripDropDownItem)
            {
                foreach (ToolStripDropDownItem item in (control as ToolStripDropDownItem).DropDownItems)
                {
                    if (item is ToolStripDropDownItem)
                    {
                        // attach handler
                        AddEventHandlers(item as ToolStripDropDownItem, provider);
                    }
                }
            }
        }

        #endregion
    }
}
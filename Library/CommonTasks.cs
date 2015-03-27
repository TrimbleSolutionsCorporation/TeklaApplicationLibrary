namespace Tekla.Structures
{
    /// <summary>
    /// Common tasks library.
    /// </summary>
    internal class CommonTasks : ICommonTasks
    {
        #region Public Methods and Operators

        /// <summary>Creates a general arrangement drawing from template.</summary>
        /// <param name="name">Template name.</param>
        public void CreateGeneralArrangementDrawingFromTemplate(string name)
        {
            new MacroBuilder().Callback("acmd_create_dim_general_assembly_drawing", string.Empty, "main_frame").PushButton("Pushbutton", "Create GA-drawing").ValueChange("gdraw_dial", "gr_gdraw_get_menu", name).PushButton("gr_gdraw_get", "gdraw_dial").PushButton("gr_gdraw_ok", "gdraw_dial").Run();
        }

        /// <summary>Opens assembly drawing properties dialog.</summary>
        /// <param name="name">Drawing name.</param>
        public void OpenAssemblyDrawingProperties(string name)
        {
            new MacroBuilder().Callback("acmd_display_attr_dialog", "adraw_dial", "main_frame").ValueChange(
                "adraw_dial", "gr_adraw_get_menu", name).PushButton("gr_adraw_get", "adraw_dial").Run();
        }

        /// <summary>Opens auto drawing script editor.</summary>
        /// <param name="name">Script name.</param>
        public void OpenAutoDrawingScript(string name)
        {
            new MacroBuilder().Callback("acmd_create_drawings_auto", string.Empty, "main_frame").ListSelect(
                "dia_auto_drawings", "auto_drawings_list", name).PushButton("Pushbutton_133", "dia_auto_drawings").Run();
        }

        /// <summary>Opens cast unit drawing properties dialog.</summary>
        /// <param name="name">Drawing name.</param>
        public void OpenCastUnitDrawingProperties(string name)
        {
            new MacroBuilder().Callback("acmd_display_attr_dialog", "cudraw_dial", "main_frame").ValueChange(
                "cudraw_dial", "gr_cudraw_get_menu", name).PushButton("gr_cudraw_get", "cudraw_dial").Run();
        }

        /// <summary>
        /// Opens the drawing list.
        /// </summary>
        public void OpenDrawingList()
        {
            new MacroBuilder().Callback("gdr_menu_select_active_draw", string.Empty, "main_frame").Run();
        }

        /// <summary>Opens general arrangement drawing properties dialog.</summary>
        /// <param name="name">Drawing name.</param>
        public void OpenGeneralArrangementDrawingProperties(string name)
        {
            new MacroBuilder().Callback("acmd_display_attr_dialog", "gdraw_dial", "main_frame").ValueChange(
                "gdraw_dial", "gr_gdraw_get_menu", name).PushButton("gr_gdraw_get", "gdraw_dial").Run();
        }

        /// <summary>
        /// Opens the numbering settings dialog.
        /// </summary>
        public void OpenNumberingSettings()
        {
            new MacroBuilder().Callback("acmd_display_partnumbers_set_options", string.Empty, "main_frame").Run();
        }

        /// <summary>Opens single part drawing properties dialog.</summary>
        /// <param name="name">Drawing name.</param>
        public void OpenSinglePartDrawingProperties(string name)
        {
            new MacroBuilder().Callback("acmd_display_attr_dialog", "wdraw_dial", "main_frame").ValueChange(
                "wdraw_dial", "gr_wdraw_get_menu", name).PushButton("gr_wdraw_get", "wdraw_dial").Run();
        }

        /// <summary>Performs numbering.</summary>
        /// <param name="numberAllModified">Indicates whether to perform numbering for all modified parts
        /// or only to selected modified parts.</param>
        public void PerformNumbering(bool numberAllModified)
        {
            if (numberAllModified)
            {
                new MacroBuilder().Callback("acmd_partnumbers_all", string.Empty, "main_frame").Run();
            }
            else
            {
                new MacroBuilder().Callback("acmd_partnumbers_selected", string.Empty, "main_frame").Run();
            }
        }

        #endregion
    }
}
namespace Tekla.Structures.InpParser
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The key words.
    /// DOT NOT RENAME!!!
    /// </summary>
    public enum KeyWords
    {
        /// <summary>
        /// The value.
        /// </summary>
        value, 

        /// <summary>
        /// The attribute.
        /// </summary>
        attribute, 

        /// <summary>
        /// The unique_attribute.
        /// </summary>
        unique_attribute, 

        /// <summary>
        /// The picture.
        /// </summary>
        picture, 

        /// <summary>
        /// The tab_page.
        /// </summary>
        tab_page, 

        /// <summary>
        /// The modify.
        /// </summary>
        modify
    }

    /// <summary>
    /// The ts model object types.
    /// DOT NOT RENAME!!!
    /// </summary>
    public enum TSModelObjectTypes
    {
        /// <summary>
        /// The part value.
        /// </summary>
        part, 

        /// <summary>
        /// The beam value.
        /// </summary>
        beam, 

        /// <summary>
        /// The column value.
        /// </summary>
        column, 

        /// <summary>
        /// The beamortho.
        /// </summary>
        beamortho, 

        /// <summary>
        /// The twinprofile.
        /// </summary>
        twinprofile, 

        /// <summary>
        /// The contourplate.
        /// </summary>
        contourplate, 

        /// <summary>
        /// The foldedplate.
        /// </summary>
        foldedplate, 

        /// <summary>
        /// The concrete_beam.
        /// </summary>
        concrete_beam, 

        /// <summary>
        /// The concrete_column.
        /// </summary>
        concrete_column, 

        /// <summary>
        /// The pad_footing.
        /// </summary>
        pad_footing, 

        /// <summary>
        /// The strip_footing.
        /// </summary>
        strip_footing, 

        /// <summary>
        /// The concrete_panel.
        /// </summary>
        concrete_panel, 

        /// <summary>
        /// The concrete_slab.
        /// </summary>
        concrete_slab, 

        /// <summary>
        /// The rebar.
        /// </summary>
        rebar, 

        /// <summary>
        /// The surfacing.
        /// </summary>
        surfacing, 

        /// <summary>
        /// The welding.
        /// </summary>
        welding, 

        /// <summary>
        /// The bolt value.
        /// </summary>
        bolt, 

        /// <summary>
        /// The singledrawing.
        /// </summary>
        singledrawing, 

        /// <summary>
        /// The assemblydrawing.
        /// </summary>
        assemblydrawing, 

        /// <summary>
        /// The gadrawing.
        /// </summary>
        gadrawing, 

        /// <summary>
        /// The multidrawing.
        /// </summary>
        multidrawing, 

        /// <summary>
        /// The castunitdrawing.
        /// </summary>
        castunitdrawing, 

        /// <summary>
        /// The project.
        /// </summary>
        project, 

        /// <summary>
        /// The phase.
        /// </summary>
        phase, 

        /// <summary>
        /// The reference.
        /// </summary>
        reference, 

        /// <summary>
        /// The reference_part.
        /// </summary>
        reference_part, 

        /// <summary>
        /// The steelassembly.
        /// </summary>
        steelassembly, 

        /// <summary>
        /// The precastassembly.
        /// </summary>
        precastassembly, 

        /// <summary>
        /// The insituassembly.
        /// </summary>
        insituassembly, 

        /// <summary>
        /// The grid value.
        /// </summary>
        grid, 

        /// <summary>
        /// The grid_plane.
        /// </summary>
        grid_plane, 

        /// <summary>
        /// The task value.
        /// </summary>
        task, 

        /// <summary>
        /// The pour_object.
        /// </summary>
        pour_object, 

        /// <summary>
        /// The pour_break.
        /// </summary>
        pour_break
    }

    /// <summary>
    /// The uda types.
    /// DOT NOT RENAME!!!
    /// </summary>
    public enum UDATypes
    {
        /// <summary>
        /// The integer.
        /// </summary>
        Integer, 

        /// <summary>
        /// The float.
        /// </summary>
        Float, 

        /// <summary>
        /// The string.
        /// </summary>
        String, 

        /// <summary>
        /// The date value.
        /// </summary>
        Date, 

        /// <summary>
        /// The option.
        /// </summary>
        Option, 

        /// <summary>
        /// The label.
        /// </summary>
        Label, 

        /// <summary>
        /// The material.
        /// </summary>
        Material, 

        /// <summary>
        /// The profile.
        /// </summary>
        Profile, 

        /// <summary>
        /// The file_in.
        /// </summary>
        File_in, 

        /// <summary>
        /// The file_out.
        /// </summary>
        File_out, 

        /// <summary>
        /// The bolt_standard.
        /// </summary>
        Bolt_standard, 

        /// <summary>
        /// The bolt_size.
        /// </summary>
        Bolt_size, 

        /// <summary>
        /// The ratio.
        /// </summary>
        Ratio, 

        /// <summary>
        /// The strain.
        /// </summary>
        Strain, 

        /// <summary>
        /// The angle.
        /// </summary>
        Angle, 

        /// <summary>
        /// The deformation.
        /// </summary>
        Deformation, 

        /// <summary>
        /// The dimension.
        /// </summary>
        Dimension, 

        /// <summary>
        /// The radiusofinertia.
        /// </summary>
        Radiusofinertia, 

        /// <summary>
        /// The area value.
        /// </summary>
        Area, 

        /// <summary>
        /// The areaperlength.
        /// </summary>
        Areaperlength, 

        /// <summary>
        /// The sectionmodulus.
        /// </summary>
        Sectionmodulus, 

        /// <summary>
        /// The momentofinertia.
        /// </summary>
        Momentofinertia, 

        /// <summary>
        /// The torsionconstant.
        /// </summary>
        Torsionconstant, 

        /// <summary>
        /// The warpingconstant.
        /// </summary>
        Warpingconstant, 

        /// <summary>
        /// The force.
        /// </summary>
        Force, 

        /// <summary>
        /// The weight.
        /// </summary>
        Weight, 

        /// <summary>
        /// The distribload.
        /// </summary>
        Distribload, 

        /// <summary>
        /// The springconstant.
        /// </summary>
        Springconstant, 

        /// <summary>
        /// The surfaceload.
        /// </summary>
        Surfaceload, 

        /// <summary>
        /// The strength.
        /// </summary>
        Strength, 

        /// <summary>
        /// The modulus.
        /// </summary>
        Modulus, 

        /// <summary>
        /// The density.
        /// </summary>
        Density, 

        /// <summary>
        /// The moment.
        /// </summary>
        Moment, 

        /// <summary>
        /// The distribmoment.
        /// </summary>
        Distribmoment, 

        /// <summary>
        /// The rotspringconst.
        /// </summary>
        Rotspringconst, 

        /// <summary>
        /// The temperature.
        /// </summary>
        Temperature, 

        /// <summary>
        /// The thermdilatcoeff.
        /// </summary>
        Thermdilatcoeff, 

        /// <summary>
        /// The distance.
        /// </summary>
        Distance, 

        /// <summary>
        /// The distance_list.
        /// </summary>
        Distance_list, 

        /// <summary>
        /// The integer_no_toggle.
        /// </summary>
        integer_no_toggle, 

        /// <summary>
        /// The float_no_toggle.
        /// </summary>
        float_no_toggle, 

        /// <summary>
        /// The string_no_toggle.
        /// </summary>
        string_no_toggle, 

        /// <summary>
        /// The distance_list_no_toggle.
        /// </summary>
        distance_list_no_toggle, 

        /// <summary>
        /// The distance_no_toggle.
        /// </summary>
        distance_no_toggle, 

        /// <summary>
        /// The label 2.
        /// </summary>
        label2, 

        /// <summary>
        /// The label 3.
        /// </summary>
        label3, 

        /// <summary>
        /// The toggle.
        /// </summary>
        toggle, 

        /// <summary>
        /// The toggle_no_toggle.
        /// </summary>
        toggle_no_toggle, 

        /// <summary>
        /// The optionstring.
        /// </summary>
        optionstring, 

        /// <summary>
        /// The radiobutton.
        /// </summary>
        radiobutton, 

        /// <summary>
        /// The weld_type.
        /// </summary>
        weld_type, 

        /// <summary>
        /// The chamfer_type.
        /// </summary>
        chamfer_type, 

        /// <summary>
        /// The welding_site.
        /// </summary>
        welding_site, 

        /// <summary>
        /// The bolt_type.
        /// </summary>
        bolt_type, 

        /// <summary>
        /// The hole_type.
        /// </summary>
        hole_type, 

        /// <summary>
        /// The hole_direction.
        /// </summary>
        hole_direction, 

        /// <summary>
        /// The stud_standard.
        /// </summary>
        stud_standard, 

        /// <summary>
        /// The stud_size.
        /// </summary>
        stud_size, 

        /// <summary>
        /// The stud_length.
        /// </summary>
        stud_length, 

        /// <summary>
        /// The rebar_size.
        /// </summary>
        rebar_size, 

        /// <summary>
        /// The rebar_main.
        /// </summary>
        rebar_main, 

        /// <summary>
        /// The rebar_stirrup.
        /// </summary>
        rebar_stirrup, 

        /// <summary>
        /// The rebar_mesh.
        /// </summary>
        rebar_mesh, 

        /// <summary>
        /// The rebar_grade.
        /// </summary>
        rebar_grade, 

        /// <summary>
        /// The rebar_radius.
        /// </summary>
        rebar_radius, 

        /// <summary>
        /// The date_time_min.
        /// </summary>
        date_time_min, 

        /// <summary>
        /// The date_time_sec.
        /// </summary>
        date_time_sec, 

        /// <summary>
        /// The factor.
        /// </summary>
        factor, 

        /// <summary>
        /// The component name.
        /// </summary>
        ComponentName, 

        /// <summary>
        /// The yes no.
        /// </summary>
        YesNo, 

        /// <summary>
        /// The component attribute file.
        /// </summary>
        ComponentAttributeFile, 
    }

    /// <summary>
    /// The check switch values.
    /// DOT NOT RENAME!!!
    /// </summary>
    public enum CheckSwitchValues
    {
        /// <summary>
        /// The none value.
        /// </summary>
        none = 0, 

        /// <summary>
        /// The check_max.
        /// </summary>
        check_max, 

        /// <summary>
        /// The check_min.
        /// </summary>
        check_min, 

        /// <summary>
        /// The check_maxmin.
        /// </summary>
        check_maxmin
    }

    /// <summary>
    ///        Describes attribute value.
    /// </summary>
    public class UDAValue
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="UDAValue"/> class. 
        ///         Initializes new instance of <see cref="UDAValue"/> class
        ///         and sets <see cref="DefaultSwitch"/> property to 0.</summary>
        /// <param name="attributeValue">UDA value.</param>
        public UDAValue(string attributeValue)
        {
            this.Value = attributeValue;
            this.DefaultSwitch = 0;
        }

        /// <summary>Initializes a new instance of the <see cref="UDAValue"/> class. 
        ///         Initializes new instance of <see cref="UDAValue"/> class.</summary>
        /// <param name="attributeValue">UDA value.</param>
        /// <param name="defaultSwitch">Default switch.
        ///         0 - no default
        ///         1 - default, which is stored to the database
        ///         2 - default, which is not stored to the database.
        /// </param>
        public UDAValue(string attributeValue, int defaultSwitch)
        {
            this.Value = attributeValue;
            this.DefaultSwitch = defaultSwitch;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets a value wheather this value is default in case of option menu.
        ///        Can be set to    0 - no default
        ///                        1 - default, which is stored to the database
        ///                        2 - default, which is not stored to the database.</summary>
        /// <value>The default switch.</value>
        public int DefaultSwitch { get; set; }

        /// <summary>Gets or sets the value of attribute.</summary>
        /// <value>The value.</value>
        public string Value { get; set; }

        #endregion
    }
}
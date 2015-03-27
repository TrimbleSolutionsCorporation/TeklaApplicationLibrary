namespace Tekla.Structures
{
    using TSConfiguration = Tekla.Structures.ModuleManager.ProgramConfigurationEnum;

    /// <summary>
    /// Application configuration.
    /// </summary>
    public enum Configuration
    {
        /// <summary>
        /// Viewer configuration.
        /// </summary>
        Viewer = TSConfiguration.CONFIGURATION_VIEWER, 

        /// <summary>
        /// Drafter configuration.
        /// </summary>
        Drafter = TSConfiguration.CONFIGURATION_DRAFTER, 

        /// <summary>
        /// Project Management configuration.
        /// </summary>
        ProjectManagement = TSConfiguration.CONFIGURATION_PROJECT_MANAGEMENT, 

        /// <summary>
        /// Construction Management configuration.
        /// </summary>
        ConstructionManagement = TSConfiguration.CONFIGURATION_CONSTRUCTION_MANAGEMENT, 

        /// <summary>
        /// Engineering configuration.
        /// </summary>
        Engineering = TSConfiguration.CONFIGURATION_ENGINEERING, 

        /// <summary>
        /// Reinforced conrete detailing configuration.
        /// </summary>
        ReinforcedConcreteDetailing = TSConfiguration.CONFIGURATION_REINFORCED_CONCRETE_DETAILING, 

        /// <summary>
        /// Precast concrete detailing configuration.
        /// </summary>
        PrecastConcreteDetailing = TSConfiguration.CONFIGURATION_PRECAST_CONCRETE_DETAILING, 

        /// <summary>
        /// Steel detailing configuration.
        /// </summary>
        SteelDetailing = TSConfiguration.CONFIGURATION_STEEL_DETAILING, 

        /// <summary>
        /// Full detailing configuration.
        /// </summary>
        Full = TSConfiguration.CONFIGURATION_FULL, 

        /// <summary>
        /// Steel detailing limited configuration.
        /// </summary>
        Primary = TSConfiguration.CONFIGURATION_PRIMARY, 

        /// <summary>
        /// Educational configuration.
        /// </summary>
        Educational = TSConfiguration.CONFIGURATION_EDUCATIONAL, 

        /// <summary>
        /// Developer configuration.
        /// </summary>
        Developer = TSConfiguration.CONFIGURATION_DEVELOPER, 
    }
}
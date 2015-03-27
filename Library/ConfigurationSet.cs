namespace Tekla.Structures
{
    using System;
    using System.Collections;

    /// <summary>
    /// Configuration set.
    /// </summary>
    public sealed class ConfigurationSet
    {
        #region Fields

        /// <summary>
        /// Configuration set storage.
        /// </summary>
        private readonly BitArray storage = new BitArray(Enum.GetValues(typeof(Configuration)).Length, false);

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationSet"/> class. 
        /// Initializes a new instance of the class.
        /// </summary>
        public ConfigurationSet()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ConfigurationSet"/> class. 
        /// Initializes a new instance of the class.</summary>
        /// <param name="configurations">Initial configurations.</param>
        public ConfigurationSet(params Configuration[] configurations)
        {
            this.Add(configurations);
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the empty configuration set.</summary>
        /// <value>The empty.</value>
        public static ConfigurationSet Empty
        {
            get
            {
                return new ConfigurationSet();
            }
        }

        /// <summary>Gets the full configuration set.</summary>
        /// <value>The full value.</value>
        public static ConfigurationSet Full
        {
            get
            {
                return new ConfigurationSet(
                    Configuration.ConstructionManagement, 
                    Configuration.Developer, 
                    Configuration.Drafter, 
                    Configuration.Educational, 
                    Configuration.Full, 
                    Configuration.PrecastConcreteDetailing, 
                    Configuration.ProjectManagement, 
                    Configuration.ReinforcedConcreteDetailing, 
                    Configuration.Engineering, 
                    Configuration.SteelDetailing, 
                    Configuration.Primary, 
                    Configuration.Viewer);
            }
        }

        #endregion

        #region Indexers

        /// <summary>Gets or sets a boolean value indicating whether the set contains the specified configuration.</summary>
        /// <param name="configuration">The configuration.</param>
        /// <value>A boolean value indicating whether the set contains the specified configuration.</value>
        /// <returns>The System.Boolean.</returns>
        private bool this[Configuration configuration]
        {
            get
            {
                return this.storage[(int)configuration];
            }

            set
            {
                this.storage[(int)configuration] = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Adds the specified configuration to the configuration set.</summary>
        /// <param name="configuration">The configuration.</param>
        public void Add(Configuration configuration)
        {
            this[configuration] = true;
        }

        /// <summary>Adds the specified configurations to the configuration set.</summary>
        /// <param name="configurations">The configurations.</param>
        public void Add(params Configuration[] configurations)
        {
            foreach (var item in configurations)
            {
                this[item] = true;
            }
        }

        /// <summary>Determines whether the configuration set contains the specified configuration.</summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>A boolean value indicating whether the set contains the specified configuration.</returns>
        public bool Contains(Configuration configuration)
        {
            return this[configuration];
        }

        /// <summary>Determines whether the configuration set contains all of the specified configurations.</summary>
        /// <param name="configurations">The configurations.</param>
        /// <returns>A boolean value indicating whether the set contains all of the specified configurations.</returns>
        public bool ContainsAll(params Configuration[] configurations)
        {
            foreach (var item in configurations)
            {
                if (!this.Contains(item))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>Determines whether the configuration set contains any of the specified configurations.</summary>
        /// <param name="configurations">The configurations.</param>
        /// <returns>A boolean value indicating whether the set contains any of the specified configurations.</returns>
        public bool ContainsAny(params Configuration[] configurations)
        {
            foreach (var item in configurations)
            {
                if (this.Contains(item))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>Removes the specified configuration from the configuration set.</summary>
        /// <param name="configuration">The configuration.</param>
        public void Remove(Configuration configuration)
        {
            this[configuration] = false;
        }

        /// <summary>Removes the specified configurations from the configuration set.</summary>
        /// <param name="configurations">The configurations.</param>
        public void Remove(params Configuration[] configurations)
        {
            foreach (var item in configurations)
            {
                this[item] = false;
            }
        }

        #endregion
    }
}
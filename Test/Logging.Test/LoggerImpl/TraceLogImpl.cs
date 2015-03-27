#region Apache License

// Licensed to the Apache Software Foundation (ASF) under one or more 
// contributor license agreements. See the NOTICE file distributed with
// this work for additional information regarding copyright ownership. 
// The ASF licenses this file to you under the Apache License, Version 2.0
// (the "License"); you may not use this file except in compliance with 
// the License. You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

namespace log4net.Ext.Trace
{
    using System;
    using System.Globalization;

    using log4net.Core;
    using log4net.Repository;
    using log4net.Util;

    /// <summary>
    /// The trace log impl.
    /// </summary>
    public class TraceLogImpl : LogImpl, ITraceLog
    {
        #region Static Fields

        /// <summary>
        /// The fully qualified name of this declaring type not the type of any subclass.
        /// </summary>
        private static readonly Type ThisDeclaringType = typeof(TraceLogImpl);

        /// <summary>The default value for the TRACE level.</summary>
        private static readonly Level DefaultLevelTrace = new Level(20000, "TRACE");

        #endregion

        #region Fields

        /// <summary>
        /// The current value for the TRACE level.
        /// </summary>
        private Level levelTrace;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceLogImpl"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public TraceLogImpl(ILogger logger)
            : base(logger)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>Gets a value indicating whether is trace enabled.</summary>
        /// <value>The is trace enabled.</value>
        public bool IsTraceEnabled
        {
            get
            {
                return this.Logger.IsEnabledFor(this.levelTrace);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The trace.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Trace(object message)
        {
            this.Logger.Log(ThisDeclaringType, this.levelTrace, message, null);
        }

        /// <summary>
        /// The trace.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="t">
        /// The t value.
        /// </param>
        public void Trace(object message, Exception t)
        {
            this.Logger.Log(ThisDeclaringType, this.levelTrace, message, t);
        }

        /// <summary>
        /// The trace format.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args value.
        /// </param>
        public void TraceFormat(string format, params object[] args)
        {
            if (this.IsTraceEnabled)
            {
                this.Logger.Log(
                    ThisDeclaringType, 
                    this.levelTrace, 
                    new SystemStringFormat(CultureInfo.InvariantCulture, format, args), 
                    null);
            }
        }

        /// <summary>
        /// The trace format.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args value.
        /// </param>
        public void TraceFormat(IFormatProvider provider, string format, params object[] args)
        {
            if (this.IsTraceEnabled)
            {
                this.Logger.Log(
                    ThisDeclaringType, this.levelTrace, new SystemStringFormat(provider, format, args), null);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Lookup the current value of the TRACE level.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        protected override void ReloadLevels(ILoggerRepository repository)
        {
            base.ReloadLevels(repository);

            this.levelTrace = repository.LevelMap.LookupWithDefault(DefaultLevelTrace);
        }

        #endregion
    }
}
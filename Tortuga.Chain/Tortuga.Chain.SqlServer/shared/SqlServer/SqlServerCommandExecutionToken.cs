using System.Collections.Generic;
using System.Data;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;

#if !SqlDependency_Missing

using System;

#endif

#if SQL_SERVER_SDS

using System.Data.SqlClient;

#elif SQL_SERVER_MDS

using Microsoft.Data.SqlClient;

#endif

namespace Tortuga.Chain.SqlServer
{
    /// <summary>
    /// An execution token specific to Sql Server.
    /// </summary>
    public sealed class SqlServerCommandExecutionToken : CommandExecutionToken<SqlCommand, SqlParameter>
    {
#if !SqlDependency_Missing
        OnChangeEventHandler? m_OnChangeEventHandler;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExecutionToken{TCommand, TParameter}"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="operationName">Name of the operation. This is used for logging.</param>
        /// <param name="commandText">The SQL to be executed.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="commandType">Type of the command.</param>
        public SqlServerCommandExecutionToken(ICommandDataSource<SqlCommand, SqlParameter> dataSource, string operationName, string commandText, IReadOnlyList<SqlParameter> parameters, CommandType commandType = CommandType.Text)
            : base(dataSource, operationName, commandText, parameters, commandType)
        {
        }

#if !SqlDependency_Missing

        /// <summary>
        /// Adds a SQL Dependency based change listener.
        /// </summary>
        /// <param name="eventHandler">The event handler.</param>
        /// <remarks>This requires SQL Dependency to be active.</remarks>
        public void AddChangeListener(OnChangeEventHandler eventHandler)
        {
            if (eventHandler == null)
                throw new ArgumentNullException(nameof(eventHandler), $"{nameof(eventHandler)} is null.");

            var disp = DataSource as SqlServerDataSource;
            if (disp == null)
                throw new InvalidOperationException("Change listeners can only be added to non-transactional data sources.");
            if (!disp.IsSqlDependencyActive)
                throw new InvalidOperationException("SQL Dependency is not active on the associated data source.");

            m_OnChangeEventHandler += eventHandler;

            return;
        }

#endif

        /// <summary>
        /// Subclasses can override this method to change the command object after the command text and parameters are loaded.
        /// </summary>
        protected override void OnBuildCommand(SqlCommand command)
        {
            base.OnBuildCommand(command);
#if !SqlDependency_Missing
            if (m_OnChangeEventHandler != null)
            {
                var sd = new SqlDependency(command);
                sd.OnChange += m_OnChangeEventHandler;
            }
#endif
        }
    }
}

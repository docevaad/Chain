using System.Linq;
using System.Text;
using Tortuga.Chain.Metadata;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Core
{
    public class MetadataTests : TestBase
    {
        public static BasicData Prime = new BasicData(s_PrimaryDataSource);
        public static BasicData Basic = new BasicData(s_DataSources.Values);
        public static TableData Tables = new TableData(s_DataSources.Values);
        public static TableData TablesNormal = new TableData(s_DataSources.Values, DataSourceType.Normal);
        public static ViewData Views = new ViewData(s_DataSources.Values);
        public static ViewData ViewsNormal = new ViewData(s_DataSources.Values, DataSourceType.Normal);

        public MetadataTests(ITestOutputHelper output) : base(output)
        {
        }

#if SQL_SERVER || ACCESS || SQLITE || POSTGRESQL || MYSQL

        [Theory, MemberData(nameof(Tables))]
        public void TableIndexes(string dataSourceName, DataSourceType mode, string tableName)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
                var indexes = table.GetIndexes();
                Assert.IsTrue(indexes.Where(i => i.IsPrimaryKey).Count() <= 1, "No more than one primary key");

                if (table.Columns.Any(c => c.IsPrimaryKey))
                    Assert.IsTrue(indexes.Where(i => i.IsPrimaryKey).Count() == 1, "A column is marked as primary, so there should be a primary index.");

                foreach (var index in indexes)
                {
                    if (index.IndexType != IndexType.Heap)
                    {
                        Assert.IsFalse(string.IsNullOrWhiteSpace(index.Name), $"Indexes should have names. Table name {table.Name}");
                        Assert.IsTrue(index.Columns.Count > 0, $"Indexes should have columns. Table name {table.Name} Index name {index.Name}");
                    }
                }
            }
            finally
            {
                Release(dataSource);
            }
        }

#endif

#if SQL_SERVER

        [Theory, MemberData(nameof(Views))]
        public void ViewIndexes(string dataSourceName, DataSourceType mode, string tableName)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
                var indexes = table.GetIndexes();
                Assert.IsTrue(indexes.Where(i => i.IsPrimaryKey).Count() <= 1, "No more than one primary key");

                if (table.Columns.Any(c => c.IsPrimaryKey))
                    Assert.IsTrue(indexes.Where(i => i.IsPrimaryKey).Count() <= 1, "A column is marked as primary, so there should be a primary index.");

                foreach (var index in indexes)
                {
                    //Assert.IsTrue(index.Columns.Count > 0, "Indexes should have columns");
                    Assert.IsFalse(string.IsNullOrWhiteSpace(index.Name), "indexes should have names");
                }
            }
            finally
            {
                Release(dataSource);
            }
        }

#endif

#if SQL_SERVER || OLE_SQL_SERVER || POSTGRESQL

        [Theory, MemberData(nameof(Basic))]
        public void DatabaseName(string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var databaseName = dataSource.DatabaseMetadata.DatabaseName;
                Assert.IsFalse(string.IsNullOrWhiteSpace(databaseName), "Database name wasn't returned");
            }
            finally
            {
                Release(dataSource);
            }
        }

#endif

#if SQL_SERVER || MySQL || OLE_SQL_SERVER

        [Theory, MemberData(nameof(Basic))]
        public void DefaultSchema(string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var defaultSchema = dataSource.DatabaseMetadata.DefaultSchema;
                Assert.IsFalse(string.IsNullOrWhiteSpace(defaultSchema), "Default schema name wasn't returned");
            }
            finally
            {
                Release(dataSource);
            }
        }

#endif

        [Theory, MemberData(nameof(Basic))]
        public void Preload(string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                dataSource.DatabaseMetadata.Preload();
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData(nameof(Basic))]
        public void SqlTypeNameToDbType_Tables(string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                dataSource.DatabaseMetadata.PreloadTables();

                foreach (var table in dataSource.DatabaseMetadata.GetTablesAndViews().Where(t => t.IsTable))
                {
                    foreach (var column in table.Columns)
                    {
                        if (!string.IsNullOrEmpty(column.TypeName) && !dataSource.DatabaseMetadata.UnsupportedSqlTypeNames.Contains(column.TypeName))
                        {
                            Assert.IsTrue(column.DbType.HasValue, $"Unable to map '{column.TypeName}' to a DbType in table {table.Name} column {column.SqlName}");
                        }
                    }
                }
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData(nameof(Basic))]
        public void SqlTypeNameToDbType_Views(string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                dataSource.DatabaseMetadata.PreloadViews();

                foreach (var view in dataSource.DatabaseMetadata.GetTablesAndViews().Where(t => !t.IsTable))
                {
                    foreach (var column in view.Columns)
                    {
                        if (!string.IsNullOrEmpty(column.TypeName) && !dataSource.DatabaseMetadata.UnsupportedSqlTypeNames.Contains(column.TypeName))
                        {
                            Assert.IsTrue(column.DbType.HasValue, $"Unable to map '{column.TypeName}' to a DbType in view {view.Name} column {column.SqlName}");
                        }
                    }
                }
            }
            finally
            {
                Release(dataSource);
            }
        }

#if SQL_SERVER || OLE_SQL_SERVER || POSTGRESQL

        [Theory, MemberData(nameof(Basic))]
        public void SqlTypeNameToDbType_TableFunctions(string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                dataSource.DatabaseMetadata.PreloadTableFunctions();

                foreach (var function in dataSource.DatabaseMetadata.GetTableFunctions())
                {
                    foreach (var parameter in function.Parameters)
                    {
                        if (!string.IsNullOrEmpty(parameter.TypeName) && !dataSource.DatabaseMetadata.UnsupportedSqlTypeNames.Contains(parameter.TypeName))
                        {
                            Assert.IsTrue(parameter.DbType.HasValue, $"Unable to map '{parameter.TypeName}' to a DbType in TableFunction {function.Name} parameter {parameter.SqlParameterName}");
                        }
                    }
                    foreach (var column in function.Columns)
                    {
                        if (!string.IsNullOrEmpty(column.TypeName) && !dataSource.DatabaseMetadata.UnsupportedSqlTypeNames.Contains(column.TypeName))
                        {
                            Assert.IsTrue(column.DbType.HasValue, $"Unable to map '{column.TypeName}' to a DbType in TableFunction {function.Name} column {column.SqlName}");
                        }
                    }
                }
            }
            finally
            {
                Release(dataSource);
            }
        }

#endif

#if SQL_SERVER || OLE_SQL_SERVER || POSTGRESQL || MYSQL

        [Theory, MemberData(nameof(Basic))]
        public void SqlTypeNameToDbType_StoredProcedures(string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                dataSource.DatabaseMetadata.PreloadStoredProcedures();

                foreach (var function in dataSource.DatabaseMetadata.GetStoredProcedures())
                {
                    foreach (var parameter in function.Parameters)
                    {
                        if (!string.IsNullOrEmpty(parameter.TypeName) && !dataSource.DatabaseMetadata.UnsupportedSqlTypeNames.Contains(parameter.TypeName))
                        {
                            Assert.IsTrue(parameter.DbType.HasValue, $"Unable to map '{parameter.TypeName}' to a DbType in Stored Procedure {function.Name} parameter {parameter.SqlParameterName}");
                        }
                    }
                }
            }
            finally
            {
                Release(dataSource);
            }
        }

#endif

#if SQL_SERVER || OLE_SQL_SERVER || POSTGRESQL || MYSQL

        [Theory, MemberData(nameof(Basic))]
        public void SqlTypeNameToDbType_ScalarFunctions(string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                dataSource.DatabaseMetadata.PreloadScalarFunctions();

                foreach (var function in dataSource.DatabaseMetadata.GetScalarFunctions())
                {
                    foreach (var parameter in function.Parameters)
                    {
                        if (!string.IsNullOrEmpty(parameter.TypeName) && !dataSource.DatabaseMetadata.UnsupportedSqlTypeNames.Contains(parameter.TypeName))
                        {
                            Assert.IsTrue(parameter.DbType.HasValue, $"Unable to map '{parameter.TypeName}' to a DbType in Table Function {function.Name} parameter {parameter.SqlParameterName}");
                        }
                    }

                    if (!string.IsNullOrEmpty(function.TypeName) && !dataSource.DatabaseMetadata.UnsupportedSqlTypeNames.Contains(function.TypeName))
                    {
                        Assert.IsTrue(function.DbType.HasValue, $"Unable to map '{function.TypeName}' to a DbType in Scalar Function {function.Name} return type");
                    }
                }
            }
            finally
            {
                Release(dataSource);
            }
        }

#endif

        [Theory, MemberData(nameof(Tables))]
        public void TryGetTable(string dataSourceName, DataSourceType mode, string tableName)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                dataSource.DatabaseMetadata.Reset();
                var result = dataSource.DatabaseMetadata.TryGetTableOrView(tableName, out var table);
                Assert.IsTrue(result);
                Assert.Equal(tableName, table.Name.ToString());
                Assert.NotEmpty(table.Columns);
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData(nameof(Prime))]
        public void TryGetTable_Failed(string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                dataSource.DatabaseMetadata.Reset();
                var result = dataSource.DatabaseMetadata.TryGetTableOrView("XXXX", out var table);
                Assert.IsFalse(result, "No object should have been found");
                Assert.IsNull(table, "No object should have been found");
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData(nameof(Tables))]
        public void GetTable(string dataSourceName, DataSourceType mode, string tableName)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                dataSource.DatabaseMetadata.Reset();
                var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
                Assert.Equal(tableName, table.Name.ToString());
                Assert.NotEmpty(table.Columns);
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData(nameof(TablesNormal))]
        public void BuildDto(string dataSourceName, DataSourceType mode, string tableName)
        {
            //this isn't building a real DTO, just exercising some of the functionality
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                dataSource.DatabaseMetadata.Reset();
                var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

                var targetFolder = GetOutputFolder("DTO\\");
                var output = new StringBuilder();
                output.AppendLine("namespace Test.Dto");
                output.AppendLine("{");
                output.AppendLine("    public class " + table.Name);
                output.AppendLine("    {");

                foreach (var column in table.Columns)
                {
                    output.AppendLine();

                    if (column.IsPrimaryKey)
                        output.AppendLine("    " + "    " + "[Key]");

                    output.AppendLine("    " + "    " + $"[Column(\"{column.SqlName}\", TypeName = \"{column.FullTypeName}\")]");
                    if (column.MaxLength > 0)
                        output.AppendLine("    " + "    " + $"[MaxLength({column.MaxLength})]");

                    output.AppendLine("    " + "    " + $"public {column.ClrTypeName(NameGenerationOptions.CSharp)} {column.ClrName} " + "{ get; set; }");

                    output.AppendLine();
                }

                output.AppendLine("    }");
                output.AppendLine("}");

                System.IO.File.WriteAllText(System.IO.Path.Combine(targetFolder.FullName, table.Name.ToString() + ".cs"), output.ToString());
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData(nameof(Tables))]
        public void GetTable_LowerCase(string dataSourceName, DataSourceType mode, string tableName)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                dataSource.DatabaseMetadata.Reset();
                var table = dataSource.DatabaseMetadata.GetTableOrView(tableName.ToLower());
                Assert.Equal(tableName, table.Name.ToString());
                Assert.NotEmpty(table.Columns);
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData(nameof(Tables))]
        public void GetTable_UpperCase(string dataSourceName, DataSourceType mode, string tableName)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                dataSource.DatabaseMetadata.Reset();
                var table = dataSource.DatabaseMetadata.GetTableOrView(tableName.ToUpper());
                Assert.Equal(tableName, table.Name.ToString());
                Assert.NotEmpty(table.Columns);
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData(nameof(Views))]
        public void GetView(string dataSourceName, DataSourceType mode, string viewName)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                dataSource.DatabaseMetadata.Reset();
                var view = dataSource.DatabaseMetadata.GetTableOrView(viewName);
                Assert.Equal(viewName, view.Name.ToString());
                Assert.NotEmpty(view.Columns);
            }
            finally
            {
                Release(dataSource);
            }
        }

#if SQL_SERVER || POSTGRESQL || OLE_SQL_SERVER

        [Theory, MemberData(nameof(Prime))]
        public void VerifyFunction1(string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                dataSource.DatabaseMetadata.PreloadTableFunctions();
                var function = dataSource.DatabaseMetadata.GetTableFunction(TableFunction1Name);
                Assert.IsNotNull(function, $"Error reading function {TableFunction1Name}");
            }
            finally
            {
                Release(dataSource);
            }
        }

#endif

#if POSTGRESQL

        [Theory, MemberData(nameof(Prime))]
        public void GetTableWithDefaultSchema(string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var table = dataSource.DatabaseMetadata.GetTableOrView("employee");

                Assert.AreEqual(DefaultSchema, table.Name.Schema, "Incorrect default schema was used");
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData(nameof(Prime))]
        public void GetSchemaList(string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var schemaList = dataSource.DatabaseMetadata.DefaultSchemaList;

                Assert.IsNotNull(schemaList, "Schema list is null");
                Assert.NotEmpty(schemaList, "Schema list is empty");

                foreach (var schema in schemaList)
                {
                    Assert.IsFalse(string.IsNullOrWhiteSpace(schema), "Empty schema name found in list");
                    Assert.IsFalse(schema.StartsWith(" "), "Leading space in schema name");
                    Assert.IsFalse(schema.EndsWith(" "), "Trailing space in schema name");
                }
            }
            finally
            {
                Release(dataSource);
            }
        }

#endif

#if SQL_SERVER || OLE_SQL_SERVER

        [Theory, MemberData(nameof(Prime))]
        public void VerifyFunction2(string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                dataSource.DatabaseMetadata.PreloadTableFunctions();
                var function = dataSource.DatabaseMetadata.GetTableFunction(TableFunction2Name);
                Assert.IsNotNull(function, $"Error reading function {TableFunction2Name}");
            }
            finally
            {
                Release(dataSource);
            }
        }

#endif
    }
}

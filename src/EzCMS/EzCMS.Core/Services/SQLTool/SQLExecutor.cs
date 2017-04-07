using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using Ez.Framework.Configurations;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Models.SQLTool;
using EzCMS.Core.Services.SiteSettings;

namespace EzCMS.Core.Services.SQLTool
{
    public class SQLExecutor
    {
        private readonly IDbConnection _connection;
        private readonly ISQLCommandService _isqlCommandService;

        public SQLExecutor()
        {
            _isqlCommandService = HostContainer.GetInstance<ISQLCommandService>();
            HostContainer.GetInstance<ISiteSettingService>();
            _connection = _isqlCommandService.GetConnection();
        }

        /// <summary>
        /// Convert current data pointed by IDataReader to a DataResult
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>a DataResult containing all column names as well as all data rows.</returns>
        private DataResult getResult(IDataReader reader)
        {
            //Get schema
            var schema = reader.GetSchemaTable();
            if (schema == null)
            {
                return null;
            }

            var dataResult = new DataResult
            {
                ColumnNames = new List<string>(),
                Data = new List<List<object>>()
            };
            foreach (DataRow row in schema.Rows)
            {
                dataResult.ColumnNames.Add(row["ColumnName"].ToString());
            }
            //Get all data rows
            while (reader.Read())
            {
                var rowData = new List<object>(dataResult.ColumnNames.Count);
                rowData.AddRange(dataResult.ColumnNames.Select(columnName => reader[columnName]));
                dataResult.Data.Add(rowData);
            }
            if (dataResult.ColumnNames.Count == 1 && dataResult.Data.Count == 1 && dataResult.ColumnNames[0].Length == 0)
            {
                dataResult.ColumnNames[0] = "Scalar Value";
            }
            return dataResult;
        }

        /// <summary>
        /// Executing a SQL query against current DB
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public SQLResult Execute(SQLRequest request)
        {
            //save executing query into history
            if (request.SaveToHistory)
            {
                _isqlCommandService.SaveCommand(request);
            }

            //Prepare the result object based on basic information from request
            var result = new SQLResult
            {
                ConnectionString = _connection.ConnectionString,
                Query = request.Query,
                ReadOnly = request.ReadOnly,
                HtmlEncode = request.HtmlEncode
            };
            var returnData = new List<DataResult>();
            var startTime = DateTime.UtcNow.Ticks;

            IDbTransaction transaction = null;
            var openConnection = false;
            try
            {
                //open connection if needed
                if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                    openConnection = true;
                }
                var command = _connection.CreateCommand();
                command.CommandText = request.Query;
                command.CommandType = CommandType.Text;
                //if executing in read only, we put all SQL statement into a DB transaction
                //then roll back after then
                if (request.ReadOnly)
                {
                    transaction = _connection.BeginTransaction();
                    command.Transaction = transaction;
                }
                //Excuting and parse result
                var reader = command.ExecuteReader();
                do
                {
                    var dataResult = getResult(reader);
                    if (dataResult != null)
                    {
                        returnData.Add(dataResult);
                    }
                } while (reader.NextResult());
                result.RecordsAffected = reader.RecordsAffected;
            }
            catch (Exception ex)
            {
                result.Error = ex;
            }
            finally
            {
                //Roll back transaction if needed
                if (transaction != null)
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch
                    {
                        //Do nothing
                    }
                }
                //Close connection if it was opened by us
                if (openConnection)
                {
                    try
                    {
                        _connection.Close();
                    }
                    catch
                    {
                        //Do nothing
                    }
                }
            }
            //Other properties for resutl object
            result.Tables = GetTableNames();
            var endTime = DateTime.UtcNow.Ticks;
            result.ReturnData = returnData;
            result.ProcessTime = (long) new TimeSpan(endTime - startTime).TotalMilliseconds;

            result.Histories = _isqlCommandService.GetHistories(EzCMSContants.DefaultHistoryStart,
                EzCMSContants.DefaultHistoryLength);
            return result;
        }

        /// <summary>
        /// Get all table names in current DB.
        /// This current solution does not work with all DB ADO.net drivers.
        /// This only works with driver that inherit DbConnection.
        /// Generally, drivers that are implemented by MS are supported.
        /// <see cref="http://msdn.microsoft.com/en-us/library/system.data.common.dbconnection.aspx" />
        /// </summary>
        /// <returns>List of table names in current DB</returns>
        public List<string> GetTableNames()
        {
            var dbConnection = _connection as DbConnection;
            if (dbConnection == null)
            {
                return null;
            }
            var openConnection = false;
            try
            {
                //Open connection if needed
                if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                    openConnection = true;
                }
                var tables = new List<string>();
                var dt = dbConnection.GetSchema("Tables");
                foreach (DataRow row in dt.Rows)
                {
                    tables.Add(row["TABLE_NAME"] as string);
                }
                tables.Sort();
                return tables;
            }
            finally
            {
                //Close connection if it was opened by this method.
                if (openConnection)
                {
                    try
                    {
                        _connection.Close();
                    }
                    catch
                    {
                        //Do nothing
                    }
                }
            }
        }

        /// <summary>
        /// Get simple schema for a table in current DB
        /// </summary>
        /// <param name="tablename">name of table</param>
        /// <returns>list of FieldInfo describing column attributes</returns>
        public List<FieldInfo> GetSchema(string tablename)
        {
            var fields = new List<FieldInfo>();
            var openConnection = false;
            try
            {
                //Open connectino if needed
                if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                    openConnection = true;
                }
                if (!GetTableNames().Any(i => i.Equals(tablename, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return null;
                }
                var command = _connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT TOP 1 * FROM [" + tablename + "]";
                //Execute query without return data.
                var reader = command.ExecuteReader(CommandBehavior.KeyInfo | CommandBehavior.SchemaOnly);
                var dt = reader.GetSchemaTable();
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        var field = new FieldInfo
                        {
                            FieldName = string.Format("[{0}]", row["ColumnName"] as string),
                            IsKey = (bool) row["IsKey"],
                            IsAutoIncrement = (bool) row["IsAutoIncrement"],
                            Size = (int) row["ColumnSize"],
                            IsReadOnly = (bool) row["IsReadOnly"],
                            FieldType = row["DataType"] as Type,
                            IsAllowNull = (bool) row["AllowDBNull"]
                        };
                        fields.Add(field);
                    }
                }
                return fields;
            }
            finally
            {
                //Close connection if it was opened by this method
                if (openConnection)
                {
                    try
                    {
                        _connection.Close();
                    }
                    catch
                    {
                        //Do nothing
                    }
                }
            }
        }

        /// <summary>
        /// Generate a simple select command template based on schema of a table.
        /// </summary>
        /// <param name="tablename">name of table</param>
        /// <returns>select command template</returns>
        public string GenerateSelectCommand(string tablename)
        {
            //For security, we check if passed tablename exists.
            var schema = GetSchema(tablename);
            if (schema == null)
            {
                return null;
            }
            var statement = new StringBuilder("SELECT ");
            foreach (var field in schema)
            {
                statement.Append(field.FieldName).Append(", ");
            }
            statement.Remove(statement.Length - 2, 2);
            statement.Append(" FROM [" + tablename + "]");
            return statement.ToString();
        }

        /// <summary>
        /// Generate a simple insert command template based on schema of a table.
        /// </summary>
        /// <param name="tablename">name of table</param>
        /// <returns>insert command template</returns>
        public string GenerateInsertCommand(string tablename)
        {
            //For security, we check if passed tablename exists.
            var schema = GetSchema(tablename);
            if (schema == null)
            {
                return null;
            }
            var statement = new StringBuilder("INSERT INTO [");
            statement.Append(tablename).Append("] \n(");
            foreach (var field in schema)
            {
                if (!field.IsReadOnly)
                {
                    statement.Append(field.FieldName).Append(", ");
                }
            }
            statement.Remove(statement.Length - 2, 2);
            statement.Append(") \nVALUES \n(");
            foreach (var field in schema)
            {
                if (!field.IsReadOnly)
                {
                    if (field.FieldType == typeof (int))
                    {
                        statement.Append(field.FieldName + "_int, ");
                    }
                    else if (field.FieldType == typeof (bool))
                    {
                        statement.Append(field.FieldName + "_TRUE/FALSE, ");
                    }
                    else if (field.FieldType == typeof (DateTime))
                    {
                        statement.Append("'" + field.FieldName + "_yyyy/mm/dd', ");
                    }
                    else if (field.FieldType == typeof (string))
                    {
                        statement.Append("'" + field.FieldName + "_String(" + field.Size + ")', ");
                    }
                    else
                    {
                        statement.Append(field.FieldName).Append('_').Append(field.FieldType.Name).Append(", ");
                    }
                }
            }
            statement.Remove(statement.Length - 2, 2);
            statement.Append(")");
            return statement.ToString();
        }

        /// <summary>
        /// Generate simple update command template for a table
        /// </summary>
        /// <param name="tablename">name of table</param>
        /// <returns>update command template</returns>
        public string GenerateUpdateCommand(string tablename)
        {
            //For security, we check if passed tablename exists.
            var schema = GetSchema(tablename);
            if (schema == null)
            {
                return null;
            }
            var statement = new StringBuilder("UPDATE [");
            statement.Append(tablename).Append("] \n");

            foreach (var field in schema)
            {
                if (!field.IsReadOnly && !field.IsKey)
                {
                    statement.Append("SET ");
                    if (field.FieldType == typeof (int))
                    {
                        statement.Append(field.FieldName + " = int, ");
                    }
                    else if (field.FieldType == typeof (bool))
                    {
                        statement.Append(field.FieldName + " = TRUE/FALSE, ");
                    }
                    else if (field.FieldType == typeof (DateTime))
                    {
                        statement.Append(field.FieldName + " = 'yyyy/dd/mm', ");
                    }
                    else if (field.FieldType == typeof (string))
                    {
                        statement.Append(field.FieldName + " = 'String(" + field.Size + ")', ");
                    }
                    else
                    {
                        statement.Append(field.FieldName + " = " + field.FieldType.Name + ", ");
                    }

                    statement.Append('\n');
                }
            }
            statement.Remove(statement.Length - 3, 3);
            statement.Append("\n WHERE \n");
            var first = true;
            foreach (var field in schema)
            {
                if (field.IsKey)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        statement.Append("AND ");
                    }

                    if (field.FieldType == typeof (int))
                    {
                        statement.Append(field.FieldName + " = int ");
                    }
                    else if (field.FieldType == typeof (bool))
                    {
                        statement.Append(field.FieldName + " = TRUE/FALSE ");
                    }
                    else if (field.FieldType == typeof (DateTime))
                    {
                        statement.Append(field.FieldName + " = 'yyyy/dd/mm' ");
                    }
                    else if (field.FieldType == typeof (string))
                    {
                        statement.Append(field.FieldName + " = 'String(" + field.Size + ")' ");
                    }
                    else
                    {
                        statement.Append(field.FieldName + " = " + field.FieldType.Name + " ");
                    }
                    statement.Append('\n');
                }
            }
            return statement.ToString();
        }

        /// <summary>
        /// Generate a simple delete command template for a table
        /// </summary>
        /// <param name="tablename">name of table</param>
        /// <returns>delete command template</returns>
        public string GenerateDeleteCommand(string tablename)
        {
            //For security, we check if passed tablename exists.
            var schema = GetSchema(tablename);
            if (schema == null)
            {
                return null;
            }
            var statement = new StringBuilder("DELETE [");
            statement.Append(tablename).Append("]");
            statement.Append("\n WHERE \n");
            var first = true;
            foreach (var field in schema)
            {
                if (field.IsKey)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        statement.Append("AND ");
                    }

                    if (field.FieldType == typeof (int))
                    {
                        statement.Append(field.FieldName + " = int ");
                    }
                    else if (field.FieldType == typeof (bool))
                    {
                        statement.Append(field.FieldName + " = TRUE/FALSE ");
                    }
                    else if (field.FieldType == typeof (DateTime))
                    {
                        statement.Append(field.FieldName + " = 'yyyy/dd/mm' ");
                    }
                    else if (field.FieldType == typeof (string))
                    {
                        statement.Append(field.FieldName + " = 'String(" + field.Size + ")' ");
                    }
                    else
                    {
                        statement.Append(field.FieldName + " = " + field.FieldType.Name + " ");
                    }
                    statement.Append('\n');
                }
            }
            return statement.ToString();
        }

        /// <summary>
        /// Generate simple create command template for a table
        /// </summary>
        /// <param name="tablename">name of table</param>
        /// <returns>create command template</returns>
        public string GenerateCreateCommand(string tablename)
        {
            //For security, we check if passed tablename exists.
            var schema = GetSchema(tablename);
            if (schema == null)
            {
                return null;
            }
            var statement = new StringBuilder("CREATE TABLE [");
            statement.Append(tablename).Append("]\n ( \n");
            foreach (var field in schema)
            {
                if (field.FieldType == typeof (int))
                {
                    statement.Append(field.FieldName + " int ");
                }
                else if (field.FieldType == typeof (bool))
                {
                    statement.Append(field.FieldName + " bit ");
                }
                else if (field.FieldType == typeof (DateTime))
                {
                    statement.Append(field.FieldName + " DateTime ");
                }
                else if (field.FieldType == typeof (string))
                {
                    statement.Append(field.FieldName + " nvarchar(" +
                                     (field.Size == int.MaxValue
                                         ? "max"
                                         : field.Size.ToString(CultureInfo.InvariantCulture)) + ") ");
                }
                else if (field.FieldType == typeof (double))
                {
                    statement.Append(field.FieldName + " float ");
                }
                else if (field.FieldType == typeof (decimal))
                {
                    statement.Append(field.FieldName + " Decimal(" + field.NumericScale + "," + field.NumericPrecision +
                                     ") ");
                }
                else
                {
                    statement.Append(field.FieldName + " " + field.FieldType.Name + " ");
                }
                statement.Append(field.IsAllowNull ? " NULL " : " NOT NULL ");
                statement.Append(",\n");
            }
            statement.Remove(statement.Length - 2, 2);
            statement.Append(')');
            return statement.ToString();
        }

        /// <summary>
        /// Generate a simple alter command template
        /// </summary>
        /// <param name="tablename">name of table</param>
        /// <returns>simple alter table command template</returns>
        public string GenerateAlterCommand(string tablename)
        {
            //For security, we check if passed tablename exists.
            var schema = GetSchema(tablename);
            if (schema == null)
            {
                return null;
            }
            var statement = new StringBuilder("ALTER TABLE [");
            statement.Append(tablename).Append("]\n ");
            foreach (var field in schema)
            {
                statement.Append("ADD COLUMN ");
                if (field.FieldType == typeof (int))
                {
                    statement.Append(field.FieldName + " int ");
                }
                else if (field.FieldType == typeof (bool))
                {
                    statement.Append(field.FieldName + " bit ");
                }
                else if (field.FieldType == typeof (DateTime))
                {
                    statement.Append(field.FieldName + " DateTime ");
                }
                else if (field.FieldType == typeof (string))
                {
                    statement.Append(field.FieldName + " nvarchar(" +
                                     (field.Size == int.MaxValue
                                         ? "max"
                                         : field.Size.ToString(CultureInfo.InvariantCulture)) + ") ");
                }
                else if (field.FieldType == typeof (double))
                {
                    statement.Append(field.FieldName + " float ");
                }
                else if (field.FieldType == typeof (decimal))
                {
                    statement.Append(field.FieldName + " Decimal(" + field.NumericScale + "," + field.NumericPrecision +
                                     ") ");
                }
                else
                {
                    statement.Append(field.FieldName + " " + field.FieldType.Name + " ");
                }
                statement.Append(field.IsAllowNull ? " NULL " : " NOT NULL ");
                statement.Append(",\n");
            }
            statement.Remove(statement.Length - 2, 2);
            return statement.ToString();
        }
    }
}
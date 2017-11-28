using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

namespace CodeGenerator
{
	internal sealed class Utility {
		private Utility() {}
		
		/// <summary>
		/// Creates the specified sub-directory, if it doesn't exist.
		/// </summary>
		/// <param name="name">The name of the sub-directory to be created.</param>
		internal static void CreateSubDirectory(string name) {
			if (Directory.Exists(name) == false) {
				Directory.CreateDirectory(name);
			}
		}
		
		
		/// <summary>
		/// Creates the specified sub-directory, if it doesn't exist.
		/// </summary>
		/// <param name="name">The name of the sub-directory to be created.</param>
		/// <param name="deleteIfExists">Indicates if the directory should be deleted if it exists.</param>
		internal static void CreateSubDirectory(string name, bool deleteIfExists) {
			if (Directory.Exists(name)) {
				Directory.Delete(name, true);
			}

			Directory.CreateDirectory(name);
		}
		
		
		/// <summary>
		/// Retrieves the specified manifest resource stream from the executing assembly as a string.
		/// </summary>
		/// <param name="name">Name of the resource to retrieve.</param>
		/// <returns>The value of the specified manifest resource.</returns>
		internal static string GetResource(string name) {
			using (StreamReader streamReader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(name))) {
				return streamReader.ReadToEnd();
			}
		}
		
		
		/// <summary>
		/// Retrieves the specified manifest resource stream from the executing assembly as a string, replacing the specified old value with the specified new value.
		/// </summary>
		/// <param name="name">Name of the resource to retrieve.</param>
		/// <param name="oldValue">A string to be replaced.</param>
		/// <param name="newValue">A string to replace all occurrences of oldValue.</param>
		/// <returns>The value of the specified manifest resource, with all instances of oldValue replaced with newValue.</returns>
		internal static string GetResource(string name, string oldValue, string newValue) {
			string returnValue = GetResource(name);
			return returnValue.Replace(oldValue, newValue);
		}

		/// <summary>
		/// Returns the query that should be used for retrieving the list of tables for the specified database.
		/// </summary>
		/// <param name="databaseName">The database to be queried for.</param>
		/// <returns>The query that should be used for retrieving the list of tables for the specified database.</returns>
		internal static string GetTableQuery(string databaseName) {
            return GetResource("CodeGenerator.TableQuery.sql", "#DatabaseName#", databaseName);
		}
		
		
		/// <summary>
		/// Returns the query that should be used for retrieving column information for the specified table.
		/// </summary>
		/// <param name="tableName">The table to be queried for.</param>
		/// <returns>The query that should be used for retrieving column information for the specified table.</returns>
		internal static string GetColumnQuery(string tableName) {
            return GetResource("CodeGenerator.ColumnQuery.sql", "#TableName#", tableName);
		}
		
		
		/// <summary>
		/// Retrieves the foreign key information for the specified table.
		/// </summary>
		/// <param name="connection">The SqlConnection to be used when querying for the table information.</param>
		/// <param name="tableName">Name of the table that foreign keys should be checked for.</param>
		/// <returns>DataReader containing the foreign key information for the specified table.</returns>
		internal static DataTable GetForeignKeyList(SqlConnection connection, string tableName) {
			SqlParameter parameter;
			
			using (SqlCommand command = new SqlCommand("sp_fkeys", connection)) {
				command.CommandType = CommandType.StoredProcedure;
				
				parameter = new SqlParameter("@pktable_name", SqlDbType.NVarChar, 128, ParameterDirection.Input, true, 0, 0, "pktable_name", DataRowVersion.Current, DBNull.Value);
				command.Parameters.Add(parameter);
				parameter = new SqlParameter("@pktable_owner", SqlDbType.NVarChar, 128, ParameterDirection.Input, true, 0, 0, "pktable_owner", DataRowVersion.Current, DBNull.Value);
				command.Parameters.Add(parameter);
				parameter = new SqlParameter("@pktable_qualifier", SqlDbType.NVarChar, 128, ParameterDirection.Input, true, 0, 0, "pktable_qualifier", DataRowVersion.Current, DBNull.Value);
				command.Parameters.Add(parameter);
				parameter = new SqlParameter("@fktable_name", SqlDbType.NVarChar, 128, ParameterDirection.Input, true, 0, 0, "fktable_name", DataRowVersion.Current, tableName);
				command.Parameters.Add(parameter);
				parameter = new SqlParameter("@fktable_owner", SqlDbType.NVarChar, 128, ParameterDirection.Input, true, 0, 0, "fktable_owner", DataRowVersion.Current, DBNull.Value);
				command.Parameters.Add(parameter);
				parameter = new SqlParameter("@fktable_qualifier", SqlDbType.NVarChar, 128, ParameterDirection.Input, true, 0, 0, "fktable_qualifier", DataRowVersion.Current, DBNull.Value);
				command.Parameters.Add(parameter);
				
				SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
				DataTable dataTable = new DataTable();
				dataAdapter.Fill(dataTable);
				
				return dataTable;
			}
		}
		
		
		/// <summary>
		/// Retrieves the primary key information for the specified table.
		/// </summary>
		/// <param name="connection">The SqlConnection to be used when querying for the table information.</param>
		/// <param name="tableName">Name of the table that primary keys should be checked for.</param>
		/// <returns>DataReader containing the primary key information for the specified table.</returns>
		internal static DataTable GetPrimaryKeyList(SqlConnection connection, string tableName) {
			SqlParameter parameter;
			
			using (SqlCommand command = new SqlCommand("sp_pkeys", connection)) {
				command.CommandType = CommandType.StoredProcedure;
				
				parameter = new SqlParameter("@table_name", SqlDbType.NVarChar, 128, ParameterDirection.Input, false, 0, 0, "table_name", DataRowVersion.Current, tableName);
				command.Parameters.Add(parameter);
				parameter = new SqlParameter("@table_owner", SqlDbType.NVarChar, 128, ParameterDirection.Input, true, 0, 0, "table_owner", DataRowVersion.Current, DBNull.Value);
				command.Parameters.Add(parameter);
				parameter = new SqlParameter("@table_qualifier", SqlDbType.NVarChar, 128, ParameterDirection.Input, true, 0, 0, "table_qualifier", DataRowVersion.Current, DBNull.Value);
				command.Parameters.Add(parameter);
				
				SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
				DataTable dataTable = new DataTable();
				dataAdapter.Fill(dataTable);
				
				return dataTable;
			}
		}
		
		
		/// <summary>
		/// Creates a string containing the parameter declaration for a stored procedure based on the parameters passed in.
		/// </summary>
		/// <param name="column">Object that stores the information for the column the parameter represents.</param>
		/// <returns>String containing parameter information of the specified column for a stored procedure.</returns>
		internal static string CreateParameterString(Column column, bool checkForOutput) {
			string columnName;
			string parameter;

			columnName = column.ProgrammaticAlias;
		
			switch (column.Type.ToLower()) {
				case "binary":
					parameter = "@" + columnName + " " + column.Type + "(" + column.Length + ")";
					break;
				case "bigint":
					parameter = "@" + columnName + " " + column.Type;
					break;
				case "bit":
					parameter = "@" + columnName + " " + column.Type;
					break;
				case "char":
					parameter = "@" + columnName + " " + column.Type + "(" + column.Length + ")";
					break;
				case "datetime":
					parameter = "@" + columnName + " " + column.Type;
					break;
				case "decimal":
					if (column.Scale.Length == 0)
						parameter = "@" + columnName + " " + column.Type + "(" + column.Precision + ")";
					else
						parameter = "@" + columnName + " " + column.Type + "(" + column.Precision + ", "+ column.Scale + ")";
					break;
				case "float":
					parameter = "@" + columnName + " " + column.Type + "(" + column.Precision + ")";
					break;
				case "image":
					parameter = "@" + columnName + " " + column.Type;
					break;
				case "int":
					parameter = "@" + columnName + " " + column.Type;
					break;
				case "money":
					parameter = "@" + columnName + " " + column.Type;
					break;
				case "nchar":
					parameter = "@" + columnName + " " + column.Type + "(" + column.Length + ")";
					break;
				case "ntext":
					parameter = "@" + columnName + " " + column.Type;
					break;
				case "nvarchar":
					parameter = "@" + columnName + " " + column.Type + "(" + column.Length + ")";
					break;
				case "numeric":
					if (column.Scale.Length == 0)
						parameter = "@" + columnName + " " + column.Type + "(" + column.Precision + ")";
					else
						parameter = "@" + columnName + " " + column.Type + "(" + column.Precision + ", "+ column.Scale + ")";
					break;
				case "real":
					parameter = "@" + columnName + " " + column.Type;
					break;
				case "smalldatetime":
					parameter = "@" + columnName + " " + column.Type;
					break;
				case "smallint":
					parameter = "@" + columnName + " " + column.Type;
					break;
				case "smallmoney":
					parameter = "@" + columnName + " " + column.Type;
					break;
				case "sql_variant":
					parameter = "@" + columnName + " " + column.Type;
					break;
				case "sysname":
					parameter = "@" + columnName + " " + column.Type;
					break;
				case "text":
					parameter = "@" + columnName + " " + column.Type;
					break;
				case "timestamp":
					parameter = "@" + columnName + " " + column.Type;
					break;
				case "tinyint":
					parameter = "@" + columnName + " " + column.Type;
					break;
				case "varbinary":
					parameter = "@" + columnName + " " + column.Type + "(" + column.Length + ")";
					break;
				case "varchar":
					parameter = "@" + columnName + " " + column.Type + "(" + column.Length + ")";
					break;
				case "uniqueidentifier":
					parameter = "@" + columnName + " " + column.Type;
					break;
				default:  // Unknow data type
					throw(new Exception("Invalid SQL Server data type specified: " + column.Type));
			}
			
			// Is the parameter an output parameter?
			if (checkForOutput) {
				if (column.IsRowGuidCol || column.IsIdentity) {
					parameter += " output";
				}
			}
			
			// Return the new parameter string
			return parameter;
		}
		

		/// <summary>
		/// Creates a string for a method parameter representing the specified column.
		/// </summary>
		/// <param name="column">Object that stores the information for the column the parameter represents.</param>
		/// <returns>String containing parameter information of the specified column for a method call.</returns>
		internal static string CreateMethodParameter(Column column) {
			string parameter;
			string columnName;

			// Format the column name
			columnName = column.ProgrammaticAlias;
			columnName = FormatCamel(columnName);
//			columnName = column.t
		
			switch (column.Type.ToLower()) {
				case "binary":
					parameter = "byte[] " + columnName;
					break;
				case "bigint":
					parameter = "Int64 " + columnName;
					break;
				case "bit":
					parameter = "bool " + columnName;
					break;
				case "char":
					parameter = "string " + columnName;
					break;
				case "datetime":
					parameter = "DateTime " + columnName;
					break;
				case "decimal":
					parameter = "decimal " + columnName;
					break;
				case "float":
					parameter = "double " + columnName;
					break;
				case "image":
					parameter = "byte[] " + columnName;
					break;
				case "int":
					parameter = "int " + columnName;
					break;
				case "money":
					parameter = "decimal " + columnName;
					break;
				case "nchar":
					parameter = "string " + columnName;
					break;
				case "ntext":
					parameter = "string " + columnName;
					break;
				case "nvarchar":
					parameter = "string " + columnName;
					break;
				case "numeric":
					parameter = "decimal " + columnName;
					break;
				case "real":
					parameter = "float " + columnName;
					break;
				case "smalldatetime":
					parameter = "DateTime " + columnName;
					break;
				case "smallint":
					parameter = "short " + columnName;
					break;
				case "smallmoney":
					parameter = "decimal " + columnName;
					break;
				case "sql_variant":
					parameter = "object " + columnName;
					break;
				case "sysname":
					parameter = "string " + columnName;
					break;
				case "text":
					parameter = "string " + columnName;
					break;
				case "timestamp":
					parameter = "DateTime " + columnName;
					break;
				case "tinyint":
					parameter = "byte " + columnName;
					break;
				case "varbinary":
					parameter = "byte[] " + columnName;
					break;
				case "varchar":
					parameter = "string " + columnName;
					break;
				case "uniqueidentifier":
					parameter = "Guid " + columnName;
					break;
				default:  // Unknow data type
					throw(new Exception("Invalid SQL Server data type specified: " + column.Type));
			}
			
			// Return the new parameter string
			return parameter;
		}


		/// <summary>
		/// Formats a string in Camel case (the first letter is in lower case).
		/// </summary>
		/// <param name="sqlDbType">A string to be formatted.</param>
		/// <returns>A string in Camel case.</returns>
		internal static string FormatCamel(string original) {
			if (original.Length > 0) {
				return original.Substring(0, 1).ToLower() + original.Substring(1);
			} else {
				return String.Empty;
			}
		}


		/// <summary>
		/// Formats a string in Pascal case (the first letter is in upper case).
		/// </summary>
		/// <param name="sqlDbType">A string to be formatted.</param>
		/// <returns>A string in Pascal case.</returns>
		internal static string FormatPascal(string original) {
			if (original.Length > 0) {
				return original.Substring(0, 1).ToUpper() + original.Substring(1);
			} else {
				return String.Empty;
			}
		}

		/// <summary>
		/// 根据数据库字段类型获取类类型
		/// </summary>
		/// <param name="sqlDbType">A string representing a SQL Server data type.</param>
		/// <returns></returns>
		internal static string GetClassType(string sqlDbType) 
		{
			string parameter ;
			switch (sqlDbType.ToLower()) 
			{
				case "binary":
                case "byte[]":
                case "image":
                case "varbinary":
					parameter = "byte[] " ;
					break;
				case "bigint":
                case "int64":
					parameter = "Int64 " ;
					break;
				case "bit":
                case "bool":
                case "boolean":
					parameter = "bool ";
					break;
				case "char":
                case "string":
                case "nchar":
                case "text":
                case "ntext":
                case "varchar":
                case "nvarchar":
                case "sysname":
					parameter = "string ";
					break;
				case "datetime":
                case "smalldatetime":
                case "timestamp":
					parameter = "DateTime ";
					break;
                case "double":
					parameter = "double ";
					break;
				case "int":
                case "int16":
                case "int32":
					parameter = "int ";
					break;
				case "money":
                case "numeric":
                case "decimal":
                case "smallmoney":
					parameter = "decimal ";
					break;
				case "real":
                case "float":
					parameter = "float ";
					break;
                case "short":
                case "smallint":
					parameter = "short ";
					break;
                case "object":
                case "sql_variant":
					parameter = "object ";
					break;
				case "tinyint":
                case "byte":
					parameter = "byte ";
					break;
				case "uniqueidentifier":
                case "guid":
					parameter = "Guid ";
					break;
				default:  // Unknow data type
					throw(new Exception("Invalid SQL Server data type specified: " + sqlDbType));
			}

			return parameter;
		}

		/// <summary>
		/// Matches a SQL Server data type to a SqlClient.SqlDbType.
		/// </summary>
		/// <param name="sqlDbType">A string representing a SQL Server data type.</param>
		/// <returns>A string representing a SqlClient.SqlDbType.</returns>
		internal static string GetSqlDbType(string sqlDbType) {
			switch (sqlDbType.ToLower()) {
				case "binary":
					return "Binary";
				case "bigint":
					return "BigInt";
				case "bit":
					return "Bit";
				case "char":
					return "Char";
				case "datetime":
					return "DateTime";
				case "decimal":
					return "Decimal";
				case "float":
					return "Float";
				case "image":
					return "Image";
				case "int":
					return "Int";
				case "money":
					return "Money";
				case "nchar":
					return "NChar";
				case "ntext":
					return "NText";
				case "nvarchar":
					return "NVarChar";
				case "numeric":
					return "Decimal";
				case "real":
					return "Real";
				case "smalldatetime":
					return "SmallDateTime";
				case "smallint":
					return "SmallInt";
				case "smallmoney":
					return "SmallMoney";
				case "sql_variant":
					return "Variant";
				case "sysname":
					return "VarChar";
				case "text":
					return "Text";
				case "timestamp":
					return "Timestamp";
				case "tinyint":
					return "TinyInt";
				case "varbinary":
					return "VarBinary";
				case "varchar":
					return "VarChar";
				case "uniqueidentifier":
					return "UniqueIdentifier";
				default:  // Unknow data type
					throw(new Exception("Invalid SQL Server data type specified: " + sqlDbType));
			}
		}


		/// <summary>
		/// Creates a string for a SqlParameter representing the specified column.
		/// </summary>
		/// <param name="column">Object that stores the information for the column the parameter represents.</param>
		/// <param name="checkForOutputParameter">Indicates if a check should be performed to see if the parameter being created is an output parameter.</param>
		/// <returns>String containing SqlParameter information of the specified column for a method call.</returns>
		internal static string CreateSqlParameter(Column column, bool checkForOutputParameter) {
			byte bytePrecision;
			byte byteScale;
			string[] methodParameter;
			
			// Get an array of data types and variable names
			methodParameter = CreateMethodParameter(column).Split(' ');
			
			// Convert the precision value
			if (column.Precision.Length > 0) {
				bytePrecision = byte.Parse(column.Precision);
			} else {
				bytePrecision = 0;
			}

			// Convert the scale value
			if (column.Scale.Length > 0) {
				byteScale = byte.Parse(column.Scale);
			} else {
				byteScale = 0;
			}

			// Is the parameter used for input or output
			if (checkForOutputParameter && (column.IsRowGuidCol || column.IsIdentity)) {
				return "command.Parameters.Add(new SqlParameter(\"@" + column.ProgrammaticAlias + "\", SqlDbType." + GetSqlDbType(column.Type) + ", " + column.Length + ", ParameterDirection.Output, false, " + bytePrecision + ", " + byteScale + ", \"" + column.Name + "\", DataRowVersion.Proposed, null));";
			} else {
				return "command.Parameters.Add(new SqlParameter(\"@" + column.ProgrammaticAlias + "\", SqlDbType." + GetSqlDbType(column.Type) + ", " + column.Length + ", ParameterDirection.Input, false, " + bytePrecision + ", " + byteScale + ", \"" + column.Name + "\", DataRowVersion.Proposed, " + methodParameter[1] + "));";
			}
		}

		/// <summary>
		/// Creates a string for a SqlParameter representing the specified column.
		/// </summary>
		/// <param name="column">Object that stores the information for the column the parameter represents.</param>
		/// <param name="checkForOutputParameter">Indicates if a check should be performed to see if the parameter being created is an output parameter.</param>
		/// <returns>String containing SqlParameter information of the specified column for a method call.</returns>
		internal static string CreateSqlParameterNoCommand(Table table, Column column, bool checkForOutputParameter) 
		{
			byte bytePrecision;
			byte byteScale;
			string[] methodParameter;
			
			// Get an array of data types and variable names
			methodParameter = CreateMethodParameter(column).Split(' ');
			
			// Convert the precision value
			if (column.Precision.Length > 0) 
			{
				bytePrecision = byte.Parse(column.Precision);
			} 
			else 
			{
				bytePrecision = 0;
			}

			// Convert the scale value
			if (column.Scale.Length > 0) 
			{
				byteScale = byte.Parse(column.Scale);
			} 
			else 
			{
				byteScale = 0;
			}

			// Is the parameter used for input or output
			if (checkForOutputParameter && (column.IsRowGuidCol || column.IsIdentity)) 
			{
				return "new SqlParameter(\"@" + column.ProgrammaticAlias + "\", SqlDbType." + GetSqlDbType(column.Type) + ", " + column.Length + ", ParameterDirection.Output, false, " + bytePrecision + ", " + byteScale + ", \"" + column.Name + "\", DataRowVersion.Proposed, null)";
			} 
			else 
			{
				return "new SqlParameter(\"@" + column.ProgrammaticAlias + "\", SqlDbType." + GetSqlDbType(column.Type) + ", " + column.Length + ", ParameterDirection.Input, false, " + bytePrecision + ", " + byteScale + ", \"" + column.Name + "\", DataRowVersion.Proposed, " + table.ProgrammaticAlias.ToLower() + "." + Utility.FormatPascal(column.ProgrammaticAlias) + ")";
			}
		}
	}
}

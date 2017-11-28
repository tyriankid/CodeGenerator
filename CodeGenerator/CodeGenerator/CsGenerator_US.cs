using System;
using System.Collections;
using System.IO;
using System.Text;

namespace CodeGenerator
{
	internal class CsGenerator_US {
		private CsGenerator_US() {}

		/// <summary>
		/// 创建Module类
		/// </summary>
		/// <param name="table">表对象</param>
		/// <param name="storedProcedurePrefix">存储过程前缀</param>
		/// <param name="path">类文件路径</param>
		internal static void CreateComponentClass(Table table, string targetNamespace, string storedProcedurePrefix, string path) {
//			string className = Utility.FormatPascal(table.Name);
			string className = Utility.FormatPascal(table.ProgrammaticAlias);
			
			//创建类文件
			StreamWriter streamWriter = new StreamWriter(path + className + ".cs");
			// 创建 class头
			streamWriter.WriteLine("using System;");
			streamWriter.WriteLine("using System.Data;");
			streamWriter.WriteLine("using System.Collections;");
			streamWriter.WriteLine();
			if (targetNamespace.Length > 0) 
			{
				streamWriter.WriteLine("namespace " + targetNamespace + ".Component {");
			}
			else
			{
				streamWriter.WriteLine("namespace Component {");
			}
			
			streamWriter.WriteLine("\t/// <summary>");
			streamWriter.WriteLine("\t/// " + className + "类");
			streamWriter.WriteLine("\t/// </summary>");
			streamWriter.WriteLine("\t[Serializable]");
			streamWriter.WriteLine("\tpublic  class " + className + " {");
			streamWriter.WriteLine();

			//定义每列数据库字段名
			streamWriter.WriteLine("\t\t#region 字段名");
			for (int i = 0; i < table.Columns.Count; i++) 
			{
				Column column = (Column) table.Columns[i];
				streamWriter.WriteLine("\t\tpublic static string Field" + Utility.FormatPascal(column.ProgrammaticAlias) + " = \"" + Utility.FormatPascal(column.ProgrammaticAlias) + "\";");
			}
			streamWriter.WriteLine("\t\t#endregion");
			streamWriter.WriteLine();
			
			//为每列创建属性
			streamWriter.WriteLine("\t\t#region 属性");
			string privateName, publicName;
			for (int i = 0; i < table.Columns.Count; i++) {
				Column column = (Column) table.Columns[i];
				privateName = "_" + Utility.FormatCamel(column.ProgrammaticAlias);
				publicName = Utility.FormatPascal(column.ProgrammaticAlias);

				streamWriter.WriteLine("\t\tprivate " + Utility.GetClassType(column.Type) +  " "  + privateName + ";");
				streamWriter.WriteLine("\t\tpublic " + Utility.GetClassType(column.Type) + " "  + publicName );
				streamWriter.WriteLine("\t\t{");
				streamWriter.WriteLine("\t\t\tget{ return " + privateName + ";}");
				streamWriter.WriteLine("\t\t\tset{ " + privateName + "=value;}");
				streamWriter.WriteLine("\t\t}");

			}
			streamWriter.WriteLine("\t\t#endregion");
			streamWriter.WriteLine();

			//添加构造函数
			streamWriter.WriteLine("\t\t#region 构造函数");
			streamWriter.WriteLine("\t\tpublic " + className + "(){}");
			
			streamWriter.WriteLine();

			streamWriter.WriteLine("\t\tpublic " + className + "(DataRow dr)");
			streamWriter.WriteLine("\t\t{");
			for (int i = 0; i < table.Columns.Count; i++)
			{
				Column column = (Column) table.Columns[i];
				streamWriter.WriteLine("\t\t\tif (dr[Field" + Utility.FormatPascal(column.ProgrammaticAlias) + "] != DBNull.Value)");
				streamWriter.WriteLine("\t\t\t{");
				streamWriter.WriteLine("\t\t\t\t_" + Utility.FormatCamel(column.ProgrammaticAlias) + " = (" + Utility.GetClassType(column.Type) + ")dr[Field" + Utility.FormatPascal(column.ProgrammaticAlias) + "];");
				streamWriter.WriteLine("\t\t\t}");
			}
			streamWriter.WriteLine("\t\t}");

			streamWriter.WriteLine("\t\t#endregion");

			streamWriter.WriteLine();


			// Close out the class and namespace
			streamWriter.WriteLine("\t}");

			//创建集合类
			CreateCollectionClass(table, streamWriter);

			streamWriter.WriteLine("}");

			// Flush and close the stream
			streamWriter.Flush();
			streamWriter.Close();
		}

		/// <summary>
		/// 创建Business类
		/// </summary>
		/// <param name="table">表对象</param>
		/// <param name="storedProcedurePrefix">存储过程前缀</param>
		/// <param name="path">类文件路径</param>
		internal static void CreateBusinessClass(Table table, string targetNamespace, string storedProcedurePrefix, string path) 
		{
			string className = Utility.FormatPascal(table.ProgrammaticAlias) + "Manager";
			
			// 创建 class头
			StreamWriter streamWriter = new StreamWriter(path + className + ".cs");
			streamWriter.WriteLine("using System;");
			streamWriter.WriteLine("using System.Data;");
			streamWriter.WriteLine("using System.Data.SqlClient;");
			streamWriter.WriteLine("using System.Collections;");
			streamWriter.WriteLine("using DataAccess;");
			if (targetNamespace.Length > 0)
			{
				streamWriter.WriteLine("using " + targetNamespace + ".Component;");
			}
			else				
			{
				streamWriter.WriteLine("using Component;");
			}

			streamWriter.WriteLine();
			if (targetNamespace.Length > 0) 
			{
				streamWriter.WriteLine("namespace " + targetNamespace + ".Business {");
			}
			else
			{
				streamWriter.WriteLine("namespace " + "Business {");
			}
			
			streamWriter.WriteLine("\t/// <summary>");
			streamWriter.WriteLine("\t/// " + className + "类");
			streamWriter.WriteLine("\t/// </summary>");
			streamWriter.WriteLine("\t[Serializable]");
			streamWriter.WriteLine("\tpublic  class " + className + " {");
			streamWriter.WriteLine();

			// 创建公共方法
			streamWriter.WriteLine("\t\t#region 公共方法");

			CreateInsertMethod(table, storedProcedurePrefix, streamWriter);
			CreateUpdateMethod(table, storedProcedurePrefix, streamWriter);
			CreateDeleteMethod(table, storedProcedurePrefix, streamWriter);
			CreateDeleteByMethods(table, storedProcedurePrefix, streamWriter);
			CreateSelectMethod(table, storedProcedurePrefix, streamWriter);
			CreateSelectByWhereMethod(table, storedProcedurePrefix, streamWriter);
			CreateSelectByPaging(table, storedProcedurePrefix, streamWriter);
			//			CreateSelectByMethods(table, storedProcedurePrefix, streamWriter);
			streamWriter.WriteLine("\t\t#endregion");
			
			// Close out the class and namespace
			streamWriter.WriteLine("\t}");

			streamWriter.WriteLine("}");


			// Flush and close the stream
			streamWriter.Flush();
			streamWriter.Close();
		}

		
		/// <summary>
		/// 创建新增方法
		/// </summary>
		/// <param name="table">表对象</param>
		/// <param name="storedProcedurePrefix">存储过程前缀</param>
		/// <param name="streamWriter">写入文件流</param>
		private static void CreateInsertMethod(Table table, string storedProcedurePrefix, StreamWriter streamWriter) {
			// Append the method header
			streamWriter.WriteLine("\t\t/// <summary>");
			streamWriter.WriteLine("\t\t/// 新增记录");
			streamWriter.WriteLine("\t\t/// </summary>");
		

			streamWriter.Write("\t\tpublic static void Insert(" + Utility.FormatPascal(table.ProgrammaticAlias) +  " " + table.ProgrammaticAlias.ToLower() + ")");
			streamWriter.WriteLine("\t\t{");
			streamWriter.WriteLine("\t\t\tString strSpName = \"" + storedProcedurePrefix + table.Name + "Insert\";");
			streamWriter.WriteLine("\t\t\tSqlParameter[] sqlSpParaArray = new SqlParameter[" + table.Columns.Count.ToString() +  "];");
			
			// 创建参数
			streamWriter.WriteLine("\t\t\t// 创建参数");
			for (int i = 0; i < table.Columns.Count; i++) 
			{
				Column column = (Column) table.Columns[i];
				streamWriter.WriteLine("\t\t\t" + "sqlSpParaArray[" + i.ToString() +  "] =" + Utility.CreateSqlParameterNoCommand(table, column, true) + ";");
			}
			streamWriter.WriteLine("\t\t\t" + "DataProvider.Instance.ExecuteSp(strSpName,sqlSpParaArray);");
			streamWriter.WriteLine("");

			streamWriter.WriteLine("\t\t}");
		}


		/// <summary>
		/// 创建更新方法
		/// </summary>
		/// <param name="table">表对象</param>
		/// <param name="storedProcedurePrefix">存储过程前缀</param>
		/// <param name="streamWriter">写入文件流</param>
		private static void CreateUpdateMethod(Table table, string storedProcedurePrefix, StreamWriter streamWriter) {
			if (table.PrimaryKeys.Count > 0 && table.Columns.Count != table.PrimaryKeys.Count && table.Columns.Count != table.ForeignKeys.Count) {
				// 插入空行
				streamWriter.WriteLine();
				streamWriter.WriteLine();

				// 头
				streamWriter.WriteLine("\t\t/// <summary>");
				streamWriter.WriteLine("\t\t/// 更新记录");
				streamWriter.WriteLine("\t\t/// </summary>");
				
				streamWriter.Write("\t\tpublic static void Update(" + Utility.FormatPascal(table.ProgrammaticAlias) +  " " + table.ProgrammaticAlias.ToLower() + ")");
				streamWriter.WriteLine("\t\t{");
				streamWriter.WriteLine("\t\t\tString strSpName = \"" + storedProcedurePrefix + table.Name + "Update\";");
				streamWriter.WriteLine("\t\t\tSqlParameter[] sqlSpParaArray = new SqlParameter[" + table.Columns.Count.ToString() +  "];");
			
				// 创建参数
				streamWriter.WriteLine("\t\t\t// 创建参数");
				for (int i = 0; i < table.Columns.Count; i++) 
				{
					Column column = (Column) table.Columns[i];
					streamWriter.WriteLine("\t\t\t" + "sqlSpParaArray[" + i.ToString() +  "] =" + Utility.CreateSqlParameterNoCommand(table, column, false) + ";");
				}
				streamWriter.WriteLine("\t\t\t" + "DataProvider.Instance.ExecuteSp(strSpName,sqlSpParaArray);");
				streamWriter.WriteLine("");

				streamWriter.WriteLine("\t\t}");
			}
		}

		/// <summary>
		/// 创建集合类
		/// </summary>
		/// <param name="table">表对象</param>
		/// <param name="streamWriter">写入文件流</param>
		private static void CreateCollectionClass(Table table, StreamWriter streamWriter)
		{
			string className = Utility.FormatPascal(table.ProgrammaticAlias);

			streamWriter.WriteLine("\t/// <summary>");
			streamWriter.WriteLine("\t/// " + className + "集合类。");
			streamWriter.WriteLine("\t/// </summary>");
			streamWriter.WriteLine("\t[Serializable]");
			streamWriter.WriteLine("\tpublic class " + className + "Collection : ArrayList");
			streamWriter.WriteLine("\t{");
			streamWriter.WriteLine("\t\tpublic new " + className +  " this[int index]");
			streamWriter.WriteLine("\t\t{");
			streamWriter.WriteLine("\t\t\tget{return (" + className + ")base[index];}");
			streamWriter.WriteLine("\t\t\tset{base[index] = value;}");
			streamWriter.WriteLine("\t\t}");
			streamWriter.WriteLine("\t}");
		}

		/// <summary>
		/// 创建根据Key删除方法
		/// </summary>
		/// <param name="table">表对象</param>
		/// <param name="storedProcedurePrefix">存储过程前缀</param>
		/// <param name="streamWriter">写入文件流</param>
		private static void CreateDeleteMethod(Table table, string storedProcedurePrefix, StreamWriter streamWriter) {
			if (table.PrimaryKeys.Count > 0) {
				// 插入空行
				streamWriter.WriteLine();
				streamWriter.WriteLine();

				// 创建根据Key删除方法
				// 方法头
				streamWriter.WriteLine("\t\t/// <summary>");
				streamWriter.WriteLine("\t\t/// 通过主键删除 " + table.Name + "记录");
				streamWriter.WriteLine("\t\t/// </summary>");
				
				streamWriter.Write("\t\tpublic static void Delete(");
				for(int i = 0; i < table.PrimaryKeys.Count; i++)
				{
					Column column = (Column)table.PrimaryKeys[i];
					if (i != table.PrimaryKeys.Count - 1)
						streamWriter.Write(Utility.CreateMethodParameter(column) + ", ");
					else
						streamWriter.Write(Utility.CreateMethodParameter(column));
				}
				streamWriter.Write(")");
				streamWriter.WriteLine("\t\t{");
				
				streamWriter.WriteLine("\t\t\tString strSpName = " + "\"" + storedProcedurePrefix + table.Name + "Delete\";");
				streamWriter.WriteLine("\t\t\tSqlParameter[] sqlSpParaArray = new SqlParameter[" + (table.PrimaryKeys.Count).ToString() + "];");
				for(int i = 0; i < table.PrimaryKeys.Count; i++)
				{
					Column column = (Column)table.PrimaryKeys[i];
					streamWriter.WriteLine("\t\t\tsqlSpParaArray[" + i.ToString() + "] = new SqlParameter(\"@" +  
						Utility.FormatPascal(column.ProgrammaticAlias) + "\", " + Utility.FormatCamel(column.ProgrammaticAlias) + ");");
				}
	
				streamWriter.WriteLine("\t\t\tDataProvider.Instance.ExecuteSp(strSpName,sqlSpParaArray);");				

				// Append the method footer
				streamWriter.WriteLine("\t\t}");
			}
		}

		/// <summary>
		/// 创建根据外键删除的方法
		/// </summary>
		/// <param name="table">表对象</param>
		/// <param name="storedProcedurePrefix">存储过程前缀.</param>
		/// <param name="streamWriter">写入文件流</param>
		private static void CreateDeleteByMethods(Table table, string storedProcedurePrefix, StreamWriter streamWriter) {
			// 根据每个外键创建删除方法
			foreach (ArrayList compositeKeyList in table.ForeignKeys.Values) {
				// 插入空行
				streamWriter.WriteLine();
				streamWriter.WriteLine();

				// 设置存储过程名
				StringBuilder stringBuilder = new StringBuilder(255);
				stringBuilder.Append(storedProcedurePrefix + table.Name + "DeleteAllBy");
				for (int i = 0; i < compositeKeyList.Count; i++) {
					Column column = (Column) compositeKeyList[i];
					
					if (i > 0) {
						stringBuilder.Append("_" + Utility.FormatPascal(column.Name));
					} else {
						stringBuilder.Append(Utility.FormatPascal(column.Name));
					}
				}
				string procedureName = stringBuilder.ToString();

				// 设置方法名
				stringBuilder = new StringBuilder(255);
				stringBuilder.Append("DeleteAllBy");
				for (int i = 0; i < compositeKeyList.Count; i++) {
					Column column = (Column) compositeKeyList[i];
					
					if (i > 0) {
						stringBuilder.Append("_" + Utility.FormatPascal(column.ProgrammaticAlias));
					} else {
						stringBuilder.Append(Utility.FormatPascal(column.ProgrammaticAlias));
					}
				}
				string methodName = stringBuilder.ToString();

				// 创建根据外键删除功能
				// 方法头
				streamWriter.WriteLine("\t\t/// <summary>");
				streamWriter.WriteLine("\t\t/// 根据外键删除 " + table.Name + " 表中记录");
				streamWriter.WriteLine("\t\t/// </summary>");
				
				streamWriter.Write("\t\tpublic static void " + methodName + "(");
				for(int i = 0; i < compositeKeyList.Count; i++)
				{
					Column column = (Column)compositeKeyList[i];
					if (i != compositeKeyList.Count - 1)
						streamWriter.Write(Utility.CreateMethodParameter(column) + ", ");
					else
						streamWriter.Write(Utility.CreateMethodParameter(column));
				}
				streamWriter.Write(")");
				streamWriter.WriteLine("\t\t{");

				streamWriter.WriteLine("\t\t\tString strSpName = " + "\"" + procedureName + "\";");
				streamWriter.WriteLine("\t\t\tSqlParameter[] sqlSpParaArray = new SqlParameter[" + (compositeKeyList.Count).ToString() + "];");
				for(int i = 0; i < compositeKeyList.Count; i++)
				{
					Column column = (Column)compositeKeyList[i];
					streamWriter.WriteLine("\t\t\tsqlSpParaArray[" + i.ToString() + "] = new SqlParameter(\"@" +  
						Utility.FormatPascal(column.ProgrammaticAlias) + "\", " + Utility.FormatCamel(column.ProgrammaticAlias) + ");");
				}
	
				streamWriter.WriteLine("\t\t\tDataProvider.Instance.ExecuteSp(strSpName,sqlSpParaArray);");
			
				// Append the method footer
				streamWriter.WriteLine("\t\t}");
			}
		}


		/// <summary>
		/// 创建根据主键获取对象方法
		/// </summary>
		/// <param name="table">表对象</param>
		/// <param name="storedProcedurePrefix">存储过程前缀.</param>
		/// <param name="streamWriter">写入文件流</param>
		private static void CreateSelectMethod(Table table, string storedProcedurePrefix, StreamWriter streamWriter) 
		{
			if (table.PrimaryKeys.Count > 0 && table.Columns.Count != table.ForeignKeys.Count) {
				#region 添加根据主键返回类对象方法
				// 插入空行
				streamWriter.WriteLine();
				streamWriter.WriteLine();

				// 方法头
				streamWriter.WriteLine("\t\t/// <summary>");
				streamWriter.WriteLine("\t\t/// 根据主键从 " + table.Name + " 表中获取单个记录");
				streamWriter.WriteLine("\t\t/// </summary>");
				
				string className = Utility.FormatPascal(table.ProgrammaticAlias);

				streamWriter.Write("\t\tpublic static " + className + " Get(");
				for(int i = 0; i < table.PrimaryKeys.Count; i++)
				{
					Column column = (Column)table.PrimaryKeys[i];
					if (i != table.PrimaryKeys.Count - 1)
						streamWriter.Write(Utility.CreateMethodParameter(column) + ", ");
					else
						streamWriter.Write(Utility.CreateMethodParameter(column));
				}
				streamWriter.Write(")");
				streamWriter.WriteLine("\t\t{");
				streamWriter.WriteLine("\t\t\t" + className + " " + className.ToLower() + " = new " + className + "();");
				streamWriter.WriteLine("\t\t\tString strSpName = " + "\"" + storedProcedurePrefix + table.Name + "Select\";");
				streamWriter.WriteLine("\t\t\tSqlParameter[] sqlSpParaArray = new SqlParameter[" + (table.PrimaryKeys.Count).ToString() + "];");
				for(int i = 0; i < table.PrimaryKeys.Count; i++)
				{
					Column column = (Column)table.PrimaryKeys[i];
					streamWriter.WriteLine("\t\t\tsqlSpParaArray[" + i.ToString() + "] = new SqlParameter(\"@" +  
						Utility.FormatPascal(column.ProgrammaticAlias) + "\", " + Utility.FormatCamel(column.ProgrammaticAlias) + ");");
				}
	
				streamWriter.WriteLine("\t\t\tSqlDataReader reader = (SqlDataReader)DataProvider.Instance.GetReaderBySp(strSpName,sqlSpParaArray);");
				streamWriter.WriteLine("\t\t\ttry");
				streamWriter.WriteLine("\t\t\t{");
				streamWriter.WriteLine("\t\t\t\tif(reader.HasRows)");
				streamWriter.WriteLine("\t\t\t\t{");
				streamWriter.WriteLine("\t\t\t\t\treader.Read();");
				streamWriter.WriteLine("\t\t\t\t\tDataProvider.Instance.SetReaderToObject(reader, " + className.ToLower() + ");");
				streamWriter.WriteLine("\t\t\t\t}");
				streamWriter.WriteLine("\t\t\t}");
				streamWriter.WriteLine("\t\t\tfinally");
				streamWriter.WriteLine("\t\t\t{");
				streamWriter.WriteLine("\t\t\t\tif (!reader.IsClosed)");
				streamWriter.WriteLine("\t\t\t\t{");
				streamWriter.WriteLine("\t\t\t\t\treader.Close();");
				streamWriter.WriteLine("\t\t\t\t}");
				streamWriter.WriteLine("\t\t\t}");
				streamWriter.WriteLine("\t\t\treturn " + className.ToLower() + ";"); 
				streamWriter.WriteLine("\t\t}");

				#endregion

				#region 添加根据主键返回DataSet方法
				// 插入空行
				streamWriter.WriteLine();
				streamWriter.WriteLine();

				// 方法头
				streamWriter.WriteLine("\t\t/// <summary>");
				streamWriter.WriteLine("\t\t/// 根据主键从 " + table.Name + " 表中获取单个记录，返回DataSet");
				streamWriter.WriteLine("\t\t/// </summary>");
				
//				string className = Utility.FormatPascal(table.ProgrammaticAlias);

				streamWriter.Write("\t\tpublic static DataSet "  + " GetDataSet(");
				for(int i = 0; i < table.PrimaryKeys.Count; i++)
				{
					Column column = (Column)table.PrimaryKeys[i];
					if (i != table.PrimaryKeys.Count - 1)
						streamWriter.Write(Utility.CreateMethodParameter(column) + ", ");
					else
						streamWriter.Write(Utility.CreateMethodParameter(column));
				}
				streamWriter.Write(")");
				streamWriter.WriteLine("\t\t{");
				streamWriter.WriteLine("\t\t\t" + className + " " + className.ToLower() + " = new " + className + "();");
				streamWriter.WriteLine("\t\t\tString strSpName = " + "\"" + storedProcedurePrefix + table.Name + "Select\";");
				streamWriter.WriteLine("\t\t\tSqlParameter[] sqlSpParaArray = new SqlParameter[" + (table.PrimaryKeys.Count).ToString() + "];");
				for(int i = 0; i < table.PrimaryKeys.Count; i++)
				{
					Column column = (Column)table.PrimaryKeys[i];
					streamWriter.WriteLine("\t\t\tsqlSpParaArray[" + i.ToString() + "] = new SqlParameter(\"@" +  
						Utility.FormatPascal(column.ProgrammaticAlias) + "\", " + Utility.FormatCamel(column.ProgrammaticAlias) + ");");
				}
				streamWriter.WriteLine("\t\t\tDataSet ds = DataProvider.Instance.GetDatasetBySp(strSpName, sqlSpParaArray);");
				streamWriter.WriteLine("\t\t\treturn ds;");
				streamWriter.WriteLine("\t\t}");
				streamWriter.WriteLine();

				#endregion
				
			}
		}


		/// <summary>
		/// 创建根据条件返回记录的方法
		/// </summary>
		/// <param name="table">表对象</param>
		/// <param name="storedProcedurePrefix">存储过程前缀.</param>
		/// <param name="streamWriter">写入文件流</param>
		private static void CreateSelectByWhereMethod(Table table, string storedProcedurePrefix, StreamWriter streamWriter) 
		{
			if (table.Columns.Count != table.PrimaryKeys.Count && table.Columns.Count != table.ForeignKeys.Count) {
				
				#region 添加根据Where返回类对象方法
				// 插入空行
				streamWriter.WriteLine();
				streamWriter.WriteLine();

				// 方法头
				streamWriter.WriteLine("\t\t/// <summary>");
				streamWriter.WriteLine("\t\t/// 从 " + table.Name + " 表中获取所有记录");
				streamWriter.WriteLine("\t\t/// </summary>");

				string className = Utility.FormatPascal(table.ProgrammaticAlias);

				string classCollectionName = className + "Collection";
				string classCollection = className.ToLower() + "s";

				streamWriter.WriteLine("\t\tpublic static " + classCollectionName + " SelectWhere(string where){");
				streamWriter.WriteLine("\t\t\t" +  classCollectionName + " " + classCollection + " = new " + classCollectionName + "();");
				streamWriter.WriteLine("\t\t\t" + "String selectSql = \"select * from " + table.Name + "\";");
				streamWriter.WriteLine("\t\t\t" + "if (where != string.Empty) selectSql += \" Where \" + where; ");
				streamWriter.WriteLine("\t\t\t" + "SqlDataReader reader = (SqlDataReader)DataProvider.Instance.GetReader(selectSql);");

				streamWriter.WriteLine("\t\t\ttry");
				streamWriter.WriteLine("\t\t\t{");
				streamWriter.WriteLine("\t\t\t\twhile (reader.Read())");
				streamWriter.WriteLine("\t\t\t\t{");
				streamWriter.WriteLine("\t\t\t\t\t" + className + " " + className.ToLower() + " = new " + className + "();");
				streamWriter.WriteLine("\t\t\t\t\t" + "DataProvider.Instance.SetReaderToObject(reader, " + className.ToLower() + " );");
				streamWriter.WriteLine("\t\t\t\t\t" + classCollection + ".Add(" + className.ToLower() + ");");
				streamWriter.WriteLine("\t\t\t\t}");
				streamWriter.WriteLine("\t\t\t}");
				streamWriter.WriteLine("\t\t\tfinally");
				streamWriter.WriteLine("\t\t\t{");
				streamWriter.WriteLine("\t\t\t\t" + "if (!reader.IsClosed)");
				streamWriter.WriteLine("\t\t\t\t{");
				streamWriter.WriteLine("\t\t\t\t\t" + "reader.Close();");
				streamWriter.WriteLine("\t\t\t\t}");
				streamWriter.WriteLine("\t\t\t}");
				streamWriter.WriteLine("\t\t\treturn " + classCollection + ";");
				// Append the method footer
				streamWriter.WriteLine("\t\t}");

				streamWriter.WriteLine();
				streamWriter.WriteLine();
				#endregion

				#region 添加根据Where返回DataSet方法
				// 插入空行
				streamWriter.WriteLine();
				streamWriter.WriteLine();

				// 方法头
				streamWriter.WriteLine("\t\t/// <summary>");
				streamWriter.WriteLine("\t\t/// 从 " + table.Name + " 表中获取所有记录,返回DataSet");
				streamWriter.WriteLine("\t\t/// </summary>");

				streamWriter.WriteLine("\t\tpublic static DataSet " + " SelectWhereDataSet(string where){");
				streamWriter.WriteLine("\t\t\t" + "String selectSql = \"select * from " + table.Name + "\";");
				streamWriter.WriteLine("\t\t\t" + "if (where != string.Empty) selectSql += \" Where \" + where; ");
				streamWriter.WriteLine("\t\t\t" + "DataSet ds = DataProvider.Instance.GetDataset(selectSql);");
				streamWriter.WriteLine("\t\t\t" + "return ds;");
				// Append the method footer
				streamWriter.WriteLine("\t\t}");

				streamWriter.WriteLine();
				streamWriter.WriteLine();
				#endregion

				
			}
		}

		#region old SelectByPaging
//		/// <summary>
//		/// 创建根据条件返回分页记录的方法
//		/// </summary>
//		/// <param name="table">表对象</param>
//		/// <param name="storedProcedurePrefix">存储过程前缀.</param>
//		/// <param name="streamWriter">写入文件流</param>
//		private static void CreateSelectByPaging(Table table, string storedProcedurePrefix, StreamWriter streamWriter)
//		{
//			if (table.Columns.Count != table.PrimaryKeys.Count && table.Columns.Count != table.ForeignKeys.Count) 
//			{
//
//				#region 添加按条件分页返回类对象集合方法
//				// 插入空行
//				streamWriter.WriteLine();
//				streamWriter.WriteLine();
//
//				// 方法头
//				streamWriter.WriteLine("\t\t/// <summary>");
//				streamWriter.WriteLine("\t\t/// 从 " + table.Name + " 表中根据条件获取分页记录");
//				streamWriter.WriteLine("\t\t/// </summary>");
//
//				string className = Utility.FormatPascal(table.ProgrammaticAlias);
//
//				string classCollectionName = className + "Collection";
//				string classCollection = className.ToLower() + "s";
//
//				//		create procedure pagination3
//				//		@tblname   varchar(255),       -- 表名
//				//		@strgetfields varchar(1000) = ''*'',  -- 需要返回的列 
//				//		@fldname varchar(255)='''',      -- 排序的字段名
//				//		@pagesize   int = 10,          -- 页尺寸
//				//		@pageindex  int = 1,           -- 页码
//				//		@docount  int = 0,   -- 返回记录总数, 非 0 值则返回
//				//		@ordertype int = 0,  -- 设置排序类型, 非 0 值则降序
//				//		@strwhere  varchar(1500) = ''''  -- 查询条件 (注意: 不要加 where)
//
//				streamWriter.WriteLine("\t\tpublic static " + classCollectionName + " SelectPaging(string getFields, string sortField, int pageSize, int pageIndex, bool orderType, string where, ref int count){");
//				streamWriter.WriteLine("\t\t\t" +  classCollectionName + " " + classCollection + " = new " + classCollectionName + "();");
//				streamWriter.WriteLine("\t\t\tString strSpName = \"Proc_GetPagging\";");
//				streamWriter.WriteLine("\t\t\tSqlParameter[] sqlSpParaArray = new SqlParameter[8];");
//				streamWriter.WriteLine("\t\t\tsqlSpParaArray[0] = new SqlParameter(\"@tblname\", \"" + table.Name + "\");");
//				streamWriter.WriteLine("\t\t\tsqlSpParaArray[1] = new SqlParameter(\"@strgetfields\", getFields);");
//				streamWriter.WriteLine("\t\t\tsqlSpParaArray[2] = new SqlParameter(\"@fldname\", sortField);");
//				streamWriter.WriteLine("\t\t\tsqlSpParaArray[3] = new SqlParameter(\"@pagesize\", pageSize);");
//				streamWriter.WriteLine("\t\t\tsqlSpParaArray[4] = new SqlParameter(\"@pageindex\", pageIndex);");
//				streamWriter.WriteLine("\t\t\tsqlSpParaArray[5] = new SqlParameter(\"@docount\", 1);");
//				streamWriter.WriteLine("\t\t\tsqlSpParaArray[6] = new SqlParameter(\"@ordertype\", 0);");
//				streamWriter.WriteLine("\t\t\tsqlSpParaArray[7] = new SqlParameter(\"@strwhere\", where);");
//
//				streamWriter.WriteLine("\t\t\t//返回记录总数");
//				streamWriter.WriteLine("\t\t\tcount = (int)DataProvider.Instance.GetScalarBySp(strSpName, sqlSpParaArray);");
//
//
//				streamWriter.WriteLine("\t\t\tsqlSpParaArray[5] = new SqlParameter(\"@docount\", 0);");
//				streamWriter.WriteLine("\t\t\tsqlSpParaArray[6] = new SqlParameter(\"@ordertype\", orderType?1:0);");
//
//				streamWriter.WriteLine("\t\t\t//返回记录");
//				streamWriter.WriteLine("\t\t\tSqlDataReader reader = (SqlDataReader)DataProvider.Instance.GetReaderBySp(strSpName, sqlSpParaArray);");
//				streamWriter.WriteLine("\t\t\ttry");
//				streamWriter.WriteLine("\t\t\t{");
//				streamWriter.WriteLine("\t\t\t\twhile (reader.Read())");
//				streamWriter.WriteLine("\t\t\t\t{");
//				streamWriter.WriteLine("\t\t\t\t\t" + className + " " + className.ToLower() + " = new " + className + "();");
//				streamWriter.WriteLine("\t\t\t\t\t" + "DataProvider.Instance.SetReaderToObject(reader, " + className.ToLower() + " );");
//				streamWriter.WriteLine("\t\t\t\t\t" + classCollection + ".Add(" + className.ToLower() + ");");
//				streamWriter.WriteLine("\t\t\t\t}");
//				streamWriter.WriteLine("\t\t\t}");
//				streamWriter.WriteLine("\t\t\tfinally");
//				streamWriter.WriteLine("\t\t\t{");
//				streamWriter.WriteLine("\t\t\t\t" + "if (!reader.IsClosed)");
//				streamWriter.WriteLine("\t\t\t\t{");
//				streamWriter.WriteLine("\t\t\t\t\t" + "reader.Close();");
//				streamWriter.WriteLine("\t\t\t\t}");
//				streamWriter.WriteLine("\t\t\t}");
//				streamWriter.WriteLine("\t\t\treturn " + classCollection + ";");
//
//				// Append the method footer
//				streamWriter.WriteLine("\t\t}");
//
//				#endregion
//
//				#region 添加按条件分页返回DataSet方法
//				// 插入空行
//				streamWriter.WriteLine();
//				streamWriter.WriteLine();
//
//				// 方法头
//				streamWriter.WriteLine("\t\t/// <summary>");
//				streamWriter.WriteLine("\t\t/// 从 " + table.Name + " 表中根据条件获取分页记录，返回DataSet");
//				streamWriter.WriteLine("\t\t/// </summary>");
//
//				streamWriter.WriteLine("\t\tpublic static DataSet " + " SelectPagingDataSet(string getFields, string sortField, int pageSize, int pageIndex, bool orderType, string where, ref int count){");
//				streamWriter.WriteLine("\t\t\tString strSpName = \"Proc_GetPagging\";");
//				streamWriter.WriteLine("\t\t\tSqlParameter[] sqlSpParaArray = new SqlParameter[8];");
//				streamWriter.WriteLine("\t\t\tsqlSpParaArray[0] = new SqlParameter(\"@tblname\", \"" + table.Name + "\");");
//				streamWriter.WriteLine("\t\t\tsqlSpParaArray[1] = new SqlParameter(\"@strgetfields\", getFields);");
//				streamWriter.WriteLine("\t\t\tsqlSpParaArray[2] = new SqlParameter(\"@fldname\", sortField);");
//				streamWriter.WriteLine("\t\t\tsqlSpParaArray[3] = new SqlParameter(\"@pagesize\", pageSize);");
//				streamWriter.WriteLine("\t\t\tsqlSpParaArray[4] = new SqlParameter(\"@pageindex\", pageIndex);");
//				streamWriter.WriteLine("\t\t\tsqlSpParaArray[5] = new SqlParameter(\"@docount\", 1);");
//				streamWriter.WriteLine("\t\t\tsqlSpParaArray[6] = new SqlParameter(\"@ordertype\", 0);");
//				streamWriter.WriteLine("\t\t\tsqlSpParaArray[7] = new SqlParameter(\"@strwhere\", where);");
//
//				streamWriter.WriteLine("\t\t\t//返回记录总数");
//				streamWriter.WriteLine("\t\t\tcount = (int)DataProvider.Instance.GetScalarBySp(strSpName, sqlSpParaArray);");
//
//
//				streamWriter.WriteLine("\t\t\tsqlSpParaArray[5] = new SqlParameter(\"@docount\", 0);");
//				streamWriter.WriteLine("\t\t\tsqlSpParaArray[6] = new SqlParameter(\"@ordertype\", orderType?1:0);");
//
//				streamWriter.WriteLine("\t\t\t//返回记录");
//				streamWriter.WriteLine("\t\t\tDataSet ds = DataProvider.Instance.GetDatasetBySp(strSpName, sqlSpParaArray);");
//				streamWriter.WriteLine("\t\t\treturn ds;");
//				// Append the method footer
//				streamWriter.WriteLine("\t\t}");
//				#endregion
//			}
//		}
		#endregion

		#region new SelectByPaging
		/// <summary>
		/// 创建根据条件返回分页记录的方法
		/// </summary>
		/// <param name="table">表对象</param>
		/// <param name="storedProcedurePrefix">存储过程前缀.</param>
		/// <param name="streamWriter">写入文件流</param>
		private static void CreateSelectByPaging(Table table, string storedProcedurePrefix, StreamWriter streamWriter)
		{
			if (table.Columns.Count != table.PrimaryKeys.Count && table.Columns.Count != table.ForeignKeys.Count) 
			{

				#region 添加按条件分页返回类对象集合方法
				// 插入空行
				streamWriter.WriteLine();
				streamWriter.WriteLine();

				// 方法头
				streamWriter.WriteLine("\t\t/// <summary>");
				streamWriter.WriteLine("\t\t/// 从 " + table.Name + " 表中根据条件获取分页记录");
				streamWriter.WriteLine("\t\t/// </summary>");

				string className = Utility.FormatPascal(table.ProgrammaticAlias);

				string classCollectionName = className + "Collection";
				string classCollection = className.ToLower() + "s";

				//		create procedure pagination3
				//		@tblname   varchar(255),       -- 表名
				//		@strgetfields varchar(1000) = ''*'',  -- 需要返回的列 
				//		@fldname varchar(255)='''',      -- 排序的字段名
				//		@pagesize   int = 10,          -- 页尺寸
				//		@pageindex  int = 1,           -- 页码
				//		@docount  int = 0,   -- 返回记录总数, 非 0 值则返回
				//		@ordertype int = 0,  -- 设置排序类型, 非 0 值则降序
				//		@strwhere  varchar(1500) = ''''  -- 查询条件 (注意: 不要加 where)

				streamWriter.WriteLine("\t\tpublic static " + classCollectionName + " SelectPaging(string getFields, string sortField, int pageSize, int pageIndex, string where, ref int count){");
				streamWriter.WriteLine("\t\t\t" +  classCollectionName + " " + classCollection + " = new " + classCollectionName + "();");
				streamWriter.WriteLine("\t\t\tString strSpName = \"Proc_GetPagging\";");
				streamWriter.WriteLine("\t\t\tSqlParameter[] sqlSpParaArray = new SqlParameter[6];");
				streamWriter.WriteLine("\t\t\tsqlSpParaArray[0] = new SqlParameter(\"@tblname\", \"" + table.Name + "\");");
				streamWriter.WriteLine("\t\t\tsqlSpParaArray[1] = new SqlParameter(\"@strgetfields\", getFields);");
				streamWriter.WriteLine("\t\t\tsqlSpParaArray[2] = new SqlParameter(\"@fldname\", sortField);");
				streamWriter.WriteLine("\t\t\tsqlSpParaArray[3] = new SqlParameter(\"@pagesize\", pageSize);");
				streamWriter.WriteLine("\t\t\tsqlSpParaArray[4] = new SqlParameter(\"@pageindex\", pageIndex);");
				streamWriter.WriteLine("\t\t\tsqlSpParaArray[5] = new SqlParameter(\"@strwhere\", where);");

				streamWriter.WriteLine("\t\t\t//返回记录");
				streamWriter.WriteLine("\t\t\tDataSet ds = DataProvider.Instance.GetDatasetBySp(strSpName, sqlSpParaArray);");
				streamWriter.WriteLine("\t\t\t//返回记录总数");
				streamWriter.WriteLine("\t\t\tcount = (int)ds.Tables[1].Rows[0][0];");


				streamWriter.WriteLine("\t\t\t\tforeach(DataRow row in ds.Tables[0].Rows)");
				streamWriter.WriteLine("\t\t\t\t{");
				streamWriter.WriteLine("\t\t\t\t\t" + className + " " + className.ToLower() + " = new " + className + "(row);");
				streamWriter.WriteLine("\t\t\t\t\t" + classCollection + ".Add(" + className.ToLower() + ");");
				streamWriter.WriteLine("\t\t\t\t}");
				streamWriter.WriteLine("\t\t\treturn " + classCollection + ";");

				// Append the method footer
				streamWriter.WriteLine("\t\t}");

				#endregion

				#region 添加按条件分页返回DataSet方法
				// 插入空行
				streamWriter.WriteLine();
				streamWriter.WriteLine();

				// 方法头
				streamWriter.WriteLine("\t\t/// <summary>");
				streamWriter.WriteLine("\t\t/// 从 " + table.Name + " 表中根据条件获取分页记录，返回DataSet");
				streamWriter.WriteLine("\t\t/// </summary>");

				streamWriter.WriteLine("\t\tpublic static DataSet " + " SelectPagingDataSet(string getFields, string sortField, int pageSize, int pageIndex,  string where, ref int count){");
				streamWriter.WriteLine("\t\t\tString strSpName = \"Proc_GetPagging\";");
				streamWriter.WriteLine("\t\t\tSqlParameter[] sqlSpParaArray = new SqlParameter[6];");
				streamWriter.WriteLine("\t\t\tsqlSpParaArray[0] = new SqlParameter(\"@tblname\", \"" + table.Name + "\");");
				streamWriter.WriteLine("\t\t\tsqlSpParaArray[1] = new SqlParameter(\"@strgetfields\", getFields);");
				streamWriter.WriteLine("\t\t\tsqlSpParaArray[2] = new SqlParameter(\"@fldname\", sortField);");
				streamWriter.WriteLine("\t\t\tsqlSpParaArray[3] = new SqlParameter(\"@pagesize\", pageSize);");
				streamWriter.WriteLine("\t\t\tsqlSpParaArray[4] = new SqlParameter(\"@pageindex\", pageIndex);");
				streamWriter.WriteLine("\t\t\tsqlSpParaArray[5] = new SqlParameter(\"@strwhere\", where);");

				streamWriter.WriteLine("\t\t\t//返回记录");
				streamWriter.WriteLine("\t\t\tDataSet ds = DataProvider.Instance.GetDatasetBySp(strSpName, sqlSpParaArray);");
				streamWriter.WriteLine("\t\t\t//返回记录总数");
				streamWriter.WriteLine("\t\t\tcount = (int)ds.Tables[1].Rows[0][0];");

				streamWriter.WriteLine("\t\t\treturn ds;");
				// Append the method footer
				streamWriter.WriteLine("\t\t}");
				#endregion
			}
		}
		#endregion

//		/// <summary>
//		/// Creates a string that represents the "select by" functionality of the data access class.
//		/// </summary>
//		/// <param name="table">表对象</param>
//		/// <param name="storedProcedurePrefix">存储过程前缀.</param>
//		/// <param name="streamWriter">写入文件流</param>
//		private static void CreateSelectByMethods(Table table, string storedProcedurePrefix, StreamWriter streamWriter) 
//		{
//			// Create a stored procedure for each foreign key
//			foreach (ArrayList compositeKeyList in table.ForeignKeys.Values) {
//				// Insert a gap between methods
//				streamWriter.WriteLine();
//				streamWriter.WriteLine();
//
//				// Create the stored procedure name
//				StringBuilder stringBuilder = new StringBuilder(255);
//				stringBuilder.Append(storedProcedurePrefix + table.Name + "SelectAllBy");
//				for (int i = 0; i < compositeKeyList.Count; i++) {
//					Column column = (Column) compositeKeyList[i];
//					
//					if (i > 0) {
//						stringBuilder.Append("_" + Utility.FormatPascal(column.Name));
//					} else {
//						stringBuilder.Append(Utility.FormatPascal(column.Name));
//					}
//				}
//				string procedureName = stringBuilder.ToString();
//
//				// Create the method name
//				stringBuilder = new StringBuilder(255);
//				stringBuilder.Append("SelectAllBy");
//				for (int i = 0; i < compositeKeyList.Count; i++) {
//					Column column = (Column) compositeKeyList[i];
//					
//					if (i > 0) {
//						stringBuilder.Append("_" + Utility.FormatPascal(column.ProgrammaticAlias));
//					} else {
//						stringBuilder.Append(Utility.FormatPascal(column.ProgrammaticAlias));
//					}
//				}
//				string methodName = stringBuilder.ToString();
//
//				// Create the select function based on keys
//				// Append the method header
//				streamWriter.WriteLine("\t\t/// <summary>");
//				streamWriter.WriteLine("\t\t/// Selects all records from the " + table.Name + " table by a foreign key.");
//				streamWriter.WriteLine("\t\t/// </summary>");
//				
//				streamWriter.Write("\t\tpublic static IDataReader " + methodName + "(");
//				for (int i = 0; i < compositeKeyList.Count; i++) {
//					Column column = (Column) compositeKeyList[i];
//					streamWriter.Write(Utility.CreateMethodParameter(column) + ", ");
//				}
//				streamWriter.WriteLine("string connectionString) {");
//				
//				// Append the variable declarations
//				streamWriter.WriteLine("\t\t\tSqlConnection connection = new SqlConnection(connectionString);");
//				streamWriter.WriteLine();
//
//				// Append the try block
//				streamWriter.WriteLine("\t\t\ttry {");
//
//				// Append the command object creation
//				streamWriter.WriteLine("\t\t\t\tusing (SqlCommand command = new SqlCommand()) {");
//				streamWriter.WriteLine("\t\t\t\t\t// Initialize the command");
//				streamWriter.WriteLine("\t\t\t\t\tcommand.Connection = connection;");
//				streamWriter.WriteLine("\t\t\t\t\tcommand.CommandText = \"" + procedureName + "\";");
//				streamWriter.WriteLine("\t\t\t\t\tcommand.CommandType = CommandType.StoredProcedure;");
//				streamWriter.WriteLine("");
//
//				// Append the parameter appends ;)
//				streamWriter.WriteLine("\t\t\t\t\t// Create and append the parameters");
//				foreach (Column column in compositeKeyList) {
//					streamWriter.WriteLine("\t\t\t\t\t" + Utility.CreateSqlParameter(column, false));
//				}
//				streamWriter.WriteLine("");
//
//				// Append the execute statement
//				streamWriter.WriteLine("\t\t\t\t\t// Open the database connection and execute the query");
//				streamWriter.WriteLine("\t\t\t\t\tconnection.Open();");
//				streamWriter.WriteLine("\t\t\t\t\treturn command.ExecuteReader(CommandBehavior.CloseConnection);");
//				streamWriter.WriteLine("\t\t\t\t}");
//				
//				// Append the catch and finally blocks
//				streamWriter.WriteLine("\t\t\t} catch {");
//				streamWriter.WriteLine("\t\t\t\t// Close the connection object and rethrow the exception");
//				streamWriter.WriteLine("\t\t\t\tconnection.Close();");
//				streamWriter.WriteLine("\t\t\t\tthrow;");
//				streamWriter.WriteLine("\t\t\t}");
//
//				// Append the method footer
//				streamWriter.WriteLine("\t\t}");
//			}
//		}
	}
}

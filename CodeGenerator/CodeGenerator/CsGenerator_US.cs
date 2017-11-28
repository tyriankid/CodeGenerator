using System;
using System.Collections;
using System.IO;
using System.Text;

namespace CodeGenerator
{
	internal class CsGenerator_US {
		private CsGenerator_US() {}

		/// <summary>
		/// ����Module��
		/// </summary>
		/// <param name="table">�����</param>
		/// <param name="storedProcedurePrefix">�洢����ǰ׺</param>
		/// <param name="path">���ļ�·��</param>
		internal static void CreateComponentClass(Table table, string targetNamespace, string storedProcedurePrefix, string path) {
//			string className = Utility.FormatPascal(table.Name);
			string className = Utility.FormatPascal(table.ProgrammaticAlias);
			
			//�������ļ�
			StreamWriter streamWriter = new StreamWriter(path + className + ".cs");
			// ���� classͷ
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
			streamWriter.WriteLine("\t/// " + className + "��");
			streamWriter.WriteLine("\t/// </summary>");
			streamWriter.WriteLine("\t[Serializable]");
			streamWriter.WriteLine("\tpublic  class " + className + " {");
			streamWriter.WriteLine();

			//����ÿ�����ݿ��ֶ���
			streamWriter.WriteLine("\t\t#region �ֶ���");
			for (int i = 0; i < table.Columns.Count; i++) 
			{
				Column column = (Column) table.Columns[i];
				streamWriter.WriteLine("\t\tpublic static string Field" + Utility.FormatPascal(column.ProgrammaticAlias) + " = \"" + Utility.FormatPascal(column.ProgrammaticAlias) + "\";");
			}
			streamWriter.WriteLine("\t\t#endregion");
			streamWriter.WriteLine();
			
			//Ϊÿ�д�������
			streamWriter.WriteLine("\t\t#region ����");
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

			//��ӹ��캯��
			streamWriter.WriteLine("\t\t#region ���캯��");
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

			//����������
			CreateCollectionClass(table, streamWriter);

			streamWriter.WriteLine("}");

			// Flush and close the stream
			streamWriter.Flush();
			streamWriter.Close();
		}

		/// <summary>
		/// ����Business��
		/// </summary>
		/// <param name="table">�����</param>
		/// <param name="storedProcedurePrefix">�洢����ǰ׺</param>
		/// <param name="path">���ļ�·��</param>
		internal static void CreateBusinessClass(Table table, string targetNamespace, string storedProcedurePrefix, string path) 
		{
			string className = Utility.FormatPascal(table.ProgrammaticAlias) + "Manager";
			
			// ���� classͷ
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
			streamWriter.WriteLine("\t/// " + className + "��");
			streamWriter.WriteLine("\t/// </summary>");
			streamWriter.WriteLine("\t[Serializable]");
			streamWriter.WriteLine("\tpublic  class " + className + " {");
			streamWriter.WriteLine();

			// ������������
			streamWriter.WriteLine("\t\t#region ��������");

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
		/// ������������
		/// </summary>
		/// <param name="table">�����</param>
		/// <param name="storedProcedurePrefix">�洢����ǰ׺</param>
		/// <param name="streamWriter">д���ļ���</param>
		private static void CreateInsertMethod(Table table, string storedProcedurePrefix, StreamWriter streamWriter) {
			// Append the method header
			streamWriter.WriteLine("\t\t/// <summary>");
			streamWriter.WriteLine("\t\t/// ������¼");
			streamWriter.WriteLine("\t\t/// </summary>");
		

			streamWriter.Write("\t\tpublic static void Insert(" + Utility.FormatPascal(table.ProgrammaticAlias) +  " " + table.ProgrammaticAlias.ToLower() + ")");
			streamWriter.WriteLine("\t\t{");
			streamWriter.WriteLine("\t\t\tString strSpName = \"" + storedProcedurePrefix + table.Name + "Insert\";");
			streamWriter.WriteLine("\t\t\tSqlParameter[] sqlSpParaArray = new SqlParameter[" + table.Columns.Count.ToString() +  "];");
			
			// ��������
			streamWriter.WriteLine("\t\t\t// ��������");
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
		/// �������·���
		/// </summary>
		/// <param name="table">�����</param>
		/// <param name="storedProcedurePrefix">�洢����ǰ׺</param>
		/// <param name="streamWriter">д���ļ���</param>
		private static void CreateUpdateMethod(Table table, string storedProcedurePrefix, StreamWriter streamWriter) {
			if (table.PrimaryKeys.Count > 0 && table.Columns.Count != table.PrimaryKeys.Count && table.Columns.Count != table.ForeignKeys.Count) {
				// �������
				streamWriter.WriteLine();
				streamWriter.WriteLine();

				// ͷ
				streamWriter.WriteLine("\t\t/// <summary>");
				streamWriter.WriteLine("\t\t/// ���¼�¼");
				streamWriter.WriteLine("\t\t/// </summary>");
				
				streamWriter.Write("\t\tpublic static void Update(" + Utility.FormatPascal(table.ProgrammaticAlias) +  " " + table.ProgrammaticAlias.ToLower() + ")");
				streamWriter.WriteLine("\t\t{");
				streamWriter.WriteLine("\t\t\tString strSpName = \"" + storedProcedurePrefix + table.Name + "Update\";");
				streamWriter.WriteLine("\t\t\tSqlParameter[] sqlSpParaArray = new SqlParameter[" + table.Columns.Count.ToString() +  "];");
			
				// ��������
				streamWriter.WriteLine("\t\t\t// ��������");
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
		/// ����������
		/// </summary>
		/// <param name="table">�����</param>
		/// <param name="streamWriter">д���ļ���</param>
		private static void CreateCollectionClass(Table table, StreamWriter streamWriter)
		{
			string className = Utility.FormatPascal(table.ProgrammaticAlias);

			streamWriter.WriteLine("\t/// <summary>");
			streamWriter.WriteLine("\t/// " + className + "�����ࡣ");
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
		/// ��������Keyɾ������
		/// </summary>
		/// <param name="table">�����</param>
		/// <param name="storedProcedurePrefix">�洢����ǰ׺</param>
		/// <param name="streamWriter">д���ļ���</param>
		private static void CreateDeleteMethod(Table table, string storedProcedurePrefix, StreamWriter streamWriter) {
			if (table.PrimaryKeys.Count > 0) {
				// �������
				streamWriter.WriteLine();
				streamWriter.WriteLine();

				// ��������Keyɾ������
				// ����ͷ
				streamWriter.WriteLine("\t\t/// <summary>");
				streamWriter.WriteLine("\t\t/// ͨ������ɾ�� " + table.Name + "��¼");
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
		/// �����������ɾ���ķ���
		/// </summary>
		/// <param name="table">�����</param>
		/// <param name="storedProcedurePrefix">�洢����ǰ׺.</param>
		/// <param name="streamWriter">д���ļ���</param>
		private static void CreateDeleteByMethods(Table table, string storedProcedurePrefix, StreamWriter streamWriter) {
			// ����ÿ���������ɾ������
			foreach (ArrayList compositeKeyList in table.ForeignKeys.Values) {
				// �������
				streamWriter.WriteLine();
				streamWriter.WriteLine();

				// ���ô洢������
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

				// ���÷�����
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

				// �����������ɾ������
				// ����ͷ
				streamWriter.WriteLine("\t\t/// <summary>");
				streamWriter.WriteLine("\t\t/// �������ɾ�� " + table.Name + " ���м�¼");
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
		/// ��������������ȡ���󷽷�
		/// </summary>
		/// <param name="table">�����</param>
		/// <param name="storedProcedurePrefix">�洢����ǰ׺.</param>
		/// <param name="streamWriter">д���ļ���</param>
		private static void CreateSelectMethod(Table table, string storedProcedurePrefix, StreamWriter streamWriter) 
		{
			if (table.PrimaryKeys.Count > 0 && table.Columns.Count != table.ForeignKeys.Count) {
				#region ��Ӹ���������������󷽷�
				// �������
				streamWriter.WriteLine();
				streamWriter.WriteLine();

				// ����ͷ
				streamWriter.WriteLine("\t\t/// <summary>");
				streamWriter.WriteLine("\t\t/// ���������� " + table.Name + " ���л�ȡ������¼");
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

				#region ��Ӹ�����������DataSet����
				// �������
				streamWriter.WriteLine();
				streamWriter.WriteLine();

				// ����ͷ
				streamWriter.WriteLine("\t\t/// <summary>");
				streamWriter.WriteLine("\t\t/// ���������� " + table.Name + " ���л�ȡ������¼������DataSet");
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
		/// ���������������ؼ�¼�ķ���
		/// </summary>
		/// <param name="table">�����</param>
		/// <param name="storedProcedurePrefix">�洢����ǰ׺.</param>
		/// <param name="streamWriter">д���ļ���</param>
		private static void CreateSelectByWhereMethod(Table table, string storedProcedurePrefix, StreamWriter streamWriter) 
		{
			if (table.Columns.Count != table.PrimaryKeys.Count && table.Columns.Count != table.ForeignKeys.Count) {
				
				#region ��Ӹ���Where��������󷽷�
				// �������
				streamWriter.WriteLine();
				streamWriter.WriteLine();

				// ����ͷ
				streamWriter.WriteLine("\t\t/// <summary>");
				streamWriter.WriteLine("\t\t/// �� " + table.Name + " ���л�ȡ���м�¼");
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

				#region ��Ӹ���Where����DataSet����
				// �������
				streamWriter.WriteLine();
				streamWriter.WriteLine();

				// ����ͷ
				streamWriter.WriteLine("\t\t/// <summary>");
				streamWriter.WriteLine("\t\t/// �� " + table.Name + " ���л�ȡ���м�¼,����DataSet");
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
//		/// ���������������ط�ҳ��¼�ķ���
//		/// </summary>
//		/// <param name="table">�����</param>
//		/// <param name="storedProcedurePrefix">�洢����ǰ׺.</param>
//		/// <param name="streamWriter">д���ļ���</param>
//		private static void CreateSelectByPaging(Table table, string storedProcedurePrefix, StreamWriter streamWriter)
//		{
//			if (table.Columns.Count != table.PrimaryKeys.Count && table.Columns.Count != table.ForeignKeys.Count) 
//			{
//
//				#region ��Ӱ�������ҳ��������󼯺Ϸ���
//				// �������
//				streamWriter.WriteLine();
//				streamWriter.WriteLine();
//
//				// ����ͷ
//				streamWriter.WriteLine("\t\t/// <summary>");
//				streamWriter.WriteLine("\t\t/// �� " + table.Name + " ���и���������ȡ��ҳ��¼");
//				streamWriter.WriteLine("\t\t/// </summary>");
//
//				string className = Utility.FormatPascal(table.ProgrammaticAlias);
//
//				string classCollectionName = className + "Collection";
//				string classCollection = className.ToLower() + "s";
//
//				//		create procedure pagination3
//				//		@tblname   varchar(255),       -- ����
//				//		@strgetfields varchar(1000) = ''*'',  -- ��Ҫ���ص��� 
//				//		@fldname varchar(255)='''',      -- ������ֶ���
//				//		@pagesize   int = 10,          -- ҳ�ߴ�
//				//		@pageindex  int = 1,           -- ҳ��
//				//		@docount  int = 0,   -- ���ؼ�¼����, �� 0 ֵ�򷵻�
//				//		@ordertype int = 0,  -- ������������, �� 0 ֵ����
//				//		@strwhere  varchar(1500) = ''''  -- ��ѯ���� (ע��: ��Ҫ�� where)
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
//				streamWriter.WriteLine("\t\t\t//���ؼ�¼����");
//				streamWriter.WriteLine("\t\t\tcount = (int)DataProvider.Instance.GetScalarBySp(strSpName, sqlSpParaArray);");
//
//
//				streamWriter.WriteLine("\t\t\tsqlSpParaArray[5] = new SqlParameter(\"@docount\", 0);");
//				streamWriter.WriteLine("\t\t\tsqlSpParaArray[6] = new SqlParameter(\"@ordertype\", orderType?1:0);");
//
//				streamWriter.WriteLine("\t\t\t//���ؼ�¼");
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
//				#region ��Ӱ�������ҳ����DataSet����
//				// �������
//				streamWriter.WriteLine();
//				streamWriter.WriteLine();
//
//				// ����ͷ
//				streamWriter.WriteLine("\t\t/// <summary>");
//				streamWriter.WriteLine("\t\t/// �� " + table.Name + " ���и���������ȡ��ҳ��¼������DataSet");
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
//				streamWriter.WriteLine("\t\t\t//���ؼ�¼����");
//				streamWriter.WriteLine("\t\t\tcount = (int)DataProvider.Instance.GetScalarBySp(strSpName, sqlSpParaArray);");
//
//
//				streamWriter.WriteLine("\t\t\tsqlSpParaArray[5] = new SqlParameter(\"@docount\", 0);");
//				streamWriter.WriteLine("\t\t\tsqlSpParaArray[6] = new SqlParameter(\"@ordertype\", orderType?1:0);");
//
//				streamWriter.WriteLine("\t\t\t//���ؼ�¼");
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
		/// ���������������ط�ҳ��¼�ķ���
		/// </summary>
		/// <param name="table">�����</param>
		/// <param name="storedProcedurePrefix">�洢����ǰ׺.</param>
		/// <param name="streamWriter">д���ļ���</param>
		private static void CreateSelectByPaging(Table table, string storedProcedurePrefix, StreamWriter streamWriter)
		{
			if (table.Columns.Count != table.PrimaryKeys.Count && table.Columns.Count != table.ForeignKeys.Count) 
			{

				#region ��Ӱ�������ҳ��������󼯺Ϸ���
				// �������
				streamWriter.WriteLine();
				streamWriter.WriteLine();

				// ����ͷ
				streamWriter.WriteLine("\t\t/// <summary>");
				streamWriter.WriteLine("\t\t/// �� " + table.Name + " ���и���������ȡ��ҳ��¼");
				streamWriter.WriteLine("\t\t/// </summary>");

				string className = Utility.FormatPascal(table.ProgrammaticAlias);

				string classCollectionName = className + "Collection";
				string classCollection = className.ToLower() + "s";

				//		create procedure pagination3
				//		@tblname   varchar(255),       -- ����
				//		@strgetfields varchar(1000) = ''*'',  -- ��Ҫ���ص��� 
				//		@fldname varchar(255)='''',      -- ������ֶ���
				//		@pagesize   int = 10,          -- ҳ�ߴ�
				//		@pageindex  int = 1,           -- ҳ��
				//		@docount  int = 0,   -- ���ؼ�¼����, �� 0 ֵ�򷵻�
				//		@ordertype int = 0,  -- ������������, �� 0 ֵ����
				//		@strwhere  varchar(1500) = ''''  -- ��ѯ���� (ע��: ��Ҫ�� where)

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

				streamWriter.WriteLine("\t\t\t//���ؼ�¼");
				streamWriter.WriteLine("\t\t\tDataSet ds = DataProvider.Instance.GetDatasetBySp(strSpName, sqlSpParaArray);");
				streamWriter.WriteLine("\t\t\t//���ؼ�¼����");
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

				#region ��Ӱ�������ҳ����DataSet����
				// �������
				streamWriter.WriteLine();
				streamWriter.WriteLine();

				// ����ͷ
				streamWriter.WriteLine("\t\t/// <summary>");
				streamWriter.WriteLine("\t\t/// �� " + table.Name + " ���и���������ȡ��ҳ��¼������DataSet");
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

				streamWriter.WriteLine("\t\t\t//���ؼ�¼");
				streamWriter.WriteLine("\t\t\tDataSet ds = DataProvider.Instance.GetDatasetBySp(strSpName, sqlSpParaArray);");
				streamWriter.WriteLine("\t\t\t//���ؼ�¼����");
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
//		/// <param name="table">�����</param>
//		/// <param name="storedProcedurePrefix">�洢����ǰ׺.</param>
//		/// <param name="streamWriter">д���ļ���</param>
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

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeGenerator
{
    internal class CsGenerator
    {
        /// <summary>
        /// 创建Model类
        /// </summary>
        /// <param name="dataTable">表对象</param>
        /// <param name="path">类文件路径</param>
        internal static void CreateModelClass(DataTable dataTable, string tablemark, string path, string targetNamespace = "Bigeer")
        {
            string className = dataTable.TableName.ToString() + "Entity";
            targetNamespace += ".Model";

            //创建类文件
            StreamWriter streamWriter = new StreamWriter(path + className + ".cs");
            // 创建 class头
            streamWriter.WriteLine("using System;");
            streamWriter.WriteLine("using System.Data;");
            streamWriter.WriteLine("using System.Collections;");
            streamWriter.WriteLine();
            streamWriter.WriteLine("namespace " + targetNamespace + " {");

            streamWriter.WriteLine("\t/// <summary>");
            streamWriter.WriteLine("\t/// " + tablemark + "-实体类");//管理员-实体类
            streamWriter.WriteLine("\t/// </summary>");
            streamWriter.WriteLine("\t[Serializable]");
            streamWriter.WriteLine("\tpublic  class " + className + " {");
            streamWriter.WriteLine();

            //定义每列数据库字段名
            streamWriter.WriteLine("\t\t#region 字段名");
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                DataColumn column = dataTable.Columns[i];
                streamWriter.WriteLine("\t\tpublic static string Field" + Utility.FormatPascal(column.ColumnName) + " = \"" + Utility.FormatPascal(column.ColumnName) + "\";");
            }
            streamWriter.WriteLine("\t\t#endregion");
            streamWriter.WriteLine();

            //为每列创建属性
            streamWriter.WriteLine("\t\t#region 属性");
            string privateName, publicName;
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                DataColumn column = dataTable.Columns[i];
                privateName = "_" + Utility.FormatCamel(column.ColumnName);
                publicName = Utility.FormatPascal(column.ColumnName);

                streamWriter.WriteLine("\t\tprivate " + Utility.GetClassType(column.DataType.Name) + " " + privateName + ";");
                streamWriter.WriteLine("\t\tpublic " + Utility.GetClassType(column.DataType.Name) + " " + publicName);
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
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                DataColumn column = dataTable.Columns[i];
                streamWriter.WriteLine("\t\t\tif (dr[Field" + Utility.FormatPascal(column.ColumnName) + "] != DBNull.Value)");
                streamWriter.WriteLine("\t\t\t{");
                streamWriter.WriteLine("\t\t\t_" + Utility.FormatCamel(column.ColumnName) + " = (" + Utility.GetClassType(column.DataType.Name) + ")dr[Field" + Utility.FormatPascal(column.ColumnName) + "];");
                streamWriter.WriteLine("\t\t\t}");
            }
            streamWriter.WriteLine("\t\t}");

            streamWriter.WriteLine("\t\t#endregion");

            streamWriter.WriteLine();

            streamWriter.WriteLine("\t}");

            //创建集合类
            //CreateCollectionClass(table, streamWriter);

            streamWriter.WriteLine("}");

            streamWriter.Flush();
            streamWriter.Close();
        }

        /// <summary>
        /// 创建Dal类
        /// </summary>
        /// <param name="dataTable">表对象</param>
        /// <param name="path">类文件路径</param>
        internal static void CreateDalClass(DataTable dataTable, string tablemark, string keyname, string path, DataTable dtField, string targetNamespace = "Bigeer")
        {
            string tableName = dataTable.TableName.ToString();
            string className = dataTable.TableName.ToString() + "Manager";

            //创建类文件
            StreamWriter streamWriter = new StreamWriter(path + className + ".cs");
            // 创建 class头
            streamWriter.WriteLine("using System;");
            streamWriter.WriteLine("using System.Collections.Generic;");
            streamWriter.WriteLine("using System.Data;");
            streamWriter.WriteLine("using System.Data.SqlClient;");
            streamWriter.WriteLine("using System.Linq;");
            streamWriter.WriteLine("using System.Text;");
            streamWriter.WriteLine("using " + targetNamespace + ".Model;");
            streamWriter.WriteLine("using YH.DataAccess;");
            streamWriter.WriteLine("using YH.Utility;");
            streamWriter.WriteLine();
            streamWriter.WriteLine("namespace " + targetNamespace + ".Dal");
            streamWriter.WriteLine("{");

            streamWriter.WriteLine("\t/// <summary>");
            streamWriter.WriteLine("\t/// " + tablemark + "-数据库操作类");
            streamWriter.WriteLine("\t/// </summary>");
            streamWriter.WriteLine("\tpublic class " + className + "");
            streamWriter.WriteLine("\t{");

            //streamWriter.WriteLine("\t\tprivate readonly static string dbServerName = null; //数据库服务名，为空时调用主服务器");
            streamWriter.WriteLine();

            //根据主键查询数据集
            streamWriter.WriteLine("\t\t/// <summary>");
            streamWriter.WriteLine("\t\t/// 根据主键查询数据集");
            streamWriter.WriteLine("\t\t/// </summary>");
            streamWriter.WriteLine("\t\tpublic static DataTable LoadData(Guid ID, DbServers.DbServerName currDbName = DbServers.DbServerName.LatestDB)");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine("\t\t\tstring selectSql = string.Format(@\"Select * From " + tableName + " Where " + keyname + "='{0}'\", ID);");
            streamWriter.WriteLine("\t\t\tDataSet ds = DataAccessFactory.GetDataProvider(DbServers.GetCurrentDB(currDbName)).GetDataset(selectSql);");
            streamWriter.WriteLine("\t\t\tds.Tables[0].TableName = \"" + tableName + "\";");
            streamWriter.WriteLine("\t\t\tds.Tables[0].PrimaryKey = new DataColumn[] { ds.Tables[0].Columns[\"" + keyname + "\"] };");
            streamWriter.WriteLine("\t\t\treturn ds.Tables[0];");
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine();

            //根据主键查询数据实体
            streamWriter.WriteLine("\t\t/// <summary>");
            streamWriter.WriteLine("\t\t/// 根据主键查询数据实体");
            streamWriter.WriteLine("\t\t/// </summary>");
            streamWriter.WriteLine("\t\tpublic static " + tableName + "Entity LoadEntity(Guid ID, DbServers.DbServerName currDbName = DbServers.DbServerName.LatestDB)");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine("\t\t\tstring selectSql = string.Format(@\"Select * From " + tableName + " Where " + keyname + "='{0}'\", ID);");
            streamWriter.WriteLine("\t\t\tusing (IDataReader reader = DataAccessFactory.GetDataProvider(DbServers.GetCurrentDB(currDbName)).GetReader(selectSql))");
            streamWriter.WriteLine("\t\t\t{");
            streamWriter.WriteLine("\t\t\t\treturn ReaderConvert.ReaderToModel<" + tableName + "Entity>(reader);");
            streamWriter.WriteLine("\t\t\t}");
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine();

            //根据条件查询数据集
            streamWriter.WriteLine("\t\t/// <summary>");
            streamWriter.WriteLine("\t\t/// 根据条件查询数据集");
            streamWriter.WriteLine("\t\t/// </summary>");
            streamWriter.WriteLine("\t\tpublic static DataTable SelectListData(string where = null, string selectFields =\"*\", string orderby = null, int top = 0, DbServers.DbServerName currDbName = DbServers.DbServerName.LatestDB)");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine("\t\t\tif (!string.IsNullOrEmpty(where)) where = \" Where \" + where;");
            streamWriter.WriteLine("\t\t\tstring selectSql = string.Format(@\"Select {2} {1} From " + tableName + " {0}\", where, selectFields, top == 0 ? \"\" : \"top \" + top);");
            streamWriter.WriteLine("\t\t\tif (!string.IsNullOrEmpty(orderby)) selectSql += \" Order By \" + orderby;");
            streamWriter.WriteLine("\t\t\tDataSet ds = DataAccessFactory.GetDataProvider(DbServers.GetCurrentDB(currDbName)).GetDataset(selectSql);");
            streamWriter.WriteLine("\t\t\tds.Tables[0].TableName = \"" + tableName + "\";");
            streamWriter.WriteLine("\t\t\tds.Tables[0].PrimaryKey = new DataColumn[] { ds.Tables[0].Columns[\"" + keyname + "\"] };");
            streamWriter.WriteLine("\t\t\treturn ds.Tables[0];");
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine();

            //根据条件查询首行首列
            streamWriter.WriteLine("\t\t/// <summary>");
            streamWriter.WriteLine("\t\t/// 根据条件查询首行首列");
            streamWriter.WriteLine("\t\t/// </summary>");
            streamWriter.WriteLine("\t\tpublic static object SelectScalar(string where = null, string selectFields =\"*\", string orderby = null, DbServers.DbServerName currDbName = DbServers.DbServerName.LatestDB)");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine("\t\t\tif (!string.IsNullOrEmpty(where)) where = \" Where \" + where;");
            streamWriter.WriteLine("\t\t\tstring selectSql = string.Format(@\"Select {1} From " + tableName + " {0}\", where, selectFields);");
            streamWriter.WriteLine("\t\t\tif (!string.IsNullOrEmpty(orderby)) selectSql += \" Order By \" + orderby;");
            streamWriter.WriteLine("\t\t\treturn DataAccessFactory.GetDataProvider(DbServers.GetCurrentDB(currDbName)).GetScalar(selectSql);");
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine();

            //根据条件查询数据实体
            streamWriter.WriteLine("\t\t/// <summary>");
            streamWriter.WriteLine("\t\t/// 根据条件查询数据实体");
            streamWriter.WriteLine("\t\t/// </summary>");
            streamWriter.WriteLine("\t\tpublic static IList<" + tableName + "Entity> SelectListEntity(string where = null, string selectFields =\"*\", string orderby = null, int top = 0, DbServers.DbServerName currDbName = DbServers.DbServerName.LatestDB)");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine("\t\t\tif (!string.IsNullOrEmpty(where)) where = \" Where \" + where;");
            streamWriter.WriteLine("\t\t\tstring selectSql = string.Format(@\"Select {2} {1} From " + tableName + " {0}\", where, selectFields, top == 0 ? \"\" : \"top \" + top);");
            streamWriter.WriteLine("\t\t\tif (!string.IsNullOrEmpty(orderby)) selectSql += \" Order By \" + orderby;");
            streamWriter.WriteLine("\t\t\tusing (IDataReader reader = DataAccessFactory.GetDataProvider(DbServers.GetCurrentDB(currDbName)).GetReader(selectSql))");
            streamWriter.WriteLine("\t\t\t{");
            streamWriter.WriteLine("\t\t\t\treturn ReaderConvert.ReaderToList<" + tableName + "Entity>(reader);");
            streamWriter.WriteLine("\t\t\t}");
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine();

            //根据主键删除
            streamWriter.WriteLine("\t\t/// <summary>");
            streamWriter.WriteLine("\t\t/// 根据主键删除");
            streamWriter.WriteLine("\t\t/// </summary>");
            streamWriter.WriteLine("\t\tpublic static void Del(Guid ID, DbServers.DbServerName currDbName = DbServers.DbServerName.LatestDB)");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine("\t\t\tstring deleteSql = string.Format(@\"Delete From " + tableName + " Where " + keyname + "='{0}'\", ID);");
            streamWriter.WriteLine("\t\t\tDataAccessFactory.GetDataProvider(DbServers.GetCurrentDB(currDbName)).Execute(deleteSql);");
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine();

            //根据条件删除
            streamWriter.WriteLine("\t\t/// <summary>");
            streamWriter.WriteLine("\t\t/// 根据条件删除");
            streamWriter.WriteLine("\t\t/// </summary>");
            streamWriter.WriteLine("\t\tpublic static void DelListData(string where = null, DbServers.DbServerName currDbName = DbServers.DbServerName.LatestDB)");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine("\t\t\tif (!string.IsNullOrEmpty(where)) where = \" Where \" + where;");
            streamWriter.WriteLine("\t\t\tstring deleteSql = string.Format(@\"Delete From " + tableName + " {0}\", where);");
            streamWriter.WriteLine("\t\t\tDataAccessFactory.GetDataProvider(DbServers.GetCurrentDB(currDbName)).Execute(deleteSql);");
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine();

            //保存数据
            string insertFields1 = "";
            string insertFields2 = "";
            string updateFields1 = "";
            //string sqlParameterFields = "";
            foreach (DataRow dr in dtField.Rows)
            {
                //if (dr["name"].ToString().ToLower() == keyname.ToLower()) continue;
                insertFields1 += string.Format("{0},", dr["name"].ToString());
                insertFields2 += string.Format("@{0},", dr["name"].ToString());
                updateFields1 += string.Format("{0}=@{0},", dr["name"].ToString());
                //sqlParameterFields += string.Format("\t\t\t\tnew SqlParameter(\"@{0}\",entity.{1}),", dr["name"].ToString(), Utility.FormatPascal(dr["name"].ToString()));
            }
            insertFields1 = insertFields1.TrimEnd(',');
            insertFields2 = insertFields2.TrimEnd(',');
            updateFields1 = updateFields1.TrimEnd(',');
            //sqlParameterFields = sqlParameterFields.TrimEnd(',');
            streamWriter.WriteLine("\t\t/// <summary>");
            streamWriter.WriteLine("\t\t/// 保存数据");
            streamWriter.WriteLine("\t\t/// </summary>");
            streamWriter.WriteLine("\t\tpublic static bool SaveEntity(" + tableName + "Entity entity, bool isAdd, DbServers.DbServerName currDbName = DbServers.DbServerName.LatestDB)");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine("\t\t\ttry");
            streamWriter.WriteLine("\t\t\t{");
            streamWriter.WriteLine("\t\t\t\tstring execSql = (isAdd) ?");
            streamWriter.WriteLine("\t\t\t\t\"Insert Into " + tableName + "(" + insertFields1 + ")values(" + insertFields2 + ")\" :");
            streamWriter.WriteLine("\t\t\t\t\"Update " + tableName + " Set " + updateFields1 + " Where " + keyname + "=@" + keyname + "\";");
            streamWriter.WriteLine("\t\t\t\tSqlParameter[] para = new SqlParameter[]");
            streamWriter.WriteLine("\t\t\t\t{");
            for (int i = 0; i < dtField.Rows.Count;i++ )
            {
                //if (dtField.Rows[i]["name"].ToString().ToLower() == keyname.ToLower()) continue;
                string fname = dtField.Rows[i]["name"].ToString();
                string fnameD=Utility.FormatPascal(fname);
                string hz=(i == dtField.Rows.Count - 1) ? "" : ",";
                switch (dtField.Select(string.Format("name='{0}'", fname))[0]["name2"].ToString().ToLower())
                {
                    case "tinyint":
                    case "int":
                    case "numeric":
                    case "money":
                        streamWriter.WriteLine("\t\t\t\t\tnew SqlParameter(\"@" + fname + "\",entity." + fnameD + "),");
                        break;
                    case "datetime":
                        streamWriter.WriteLine("\t\t\t\t\t(entity." + fnameD + "==null || entity." + fnameD + "==DateTime.MinValue)?new SqlParameter(\"@" + fname + "\",DBNull.Value):new SqlParameter(\"@" + fname + "\",entity." + fnameD + "),");
                        break;
                    default: 
                        streamWriter.WriteLine("\t\t\t\t\t(entity." + fnameD + "==null)?new SqlParameter(\"@" + fname + "\",DBNull.Value):new SqlParameter(\"@" + fname + "\",entity." + fnameD + "),");
                        break;
                }
                
            }
            streamWriter.WriteLine("\t\t\t\t};");
            streamWriter.WriteLine("\t\t\t\tDataAccessFactory.GetDataProvider(DbServers.GetCurrentDB(currDbName)).Execute(execSql, para);");
            streamWriter.WriteLine("\t\t\t\treturn true;");
            streamWriter.WriteLine("\t\t\t}");
            streamWriter.WriteLine("\t\t\tcatch");
            streamWriter.WriteLine("\t\t\t{");
            streamWriter.WriteLine("\t\t\t\treturn false;");
            streamWriter.WriteLine("\t\t\t}");
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine();


            streamWriter.WriteLine("\t}");
            streamWriter.WriteLine("}");

            streamWriter.Flush();
            streamWriter.Close();
        }
    
        /// <summary>
        /// 创建Bll类
        /// </summary>
        /// <param name="dataTable">表对象</param>
        /// <param name="path">类文件路径</param>
        internal static void CreateBllClass(DataTable dataTable, string tablemark, string path, string targetNamespace = "Bigeer")
        {
            string tableName = dataTable.TableName.ToString();
            string className = dataTable.TableName.ToString() + "Business";

            //创建类文件
            StreamWriter streamWriter = new StreamWriter(path + className + ".cs");
            // 创建 class头
            streamWriter.WriteLine("using System;");
            streamWriter.WriteLine("using System.Collections.Generic;");
            streamWriter.WriteLine("using System.Data;");
            streamWriter.WriteLine("using System.Linq;");
            streamWriter.WriteLine("using System.Text;");
            streamWriter.WriteLine("using " + targetNamespace + ".Common;");
            streamWriter.WriteLine("using " + targetNamespace + ".Dal;");
            streamWriter.WriteLine("using " + targetNamespace + ".Model;");
            streamWriter.WriteLine("using YH.Utility;");
            //streamWriter.WriteLine("using Bigeer.Plugin;");
            streamWriter.WriteLine();
            streamWriter.WriteLine("namespace " + targetNamespace + ".Bll");
            streamWriter.WriteLine("{");

            streamWriter.WriteLine("\t/// <summary>");
            streamWriter.WriteLine("\t/// " + tablemark + "-业务操作类");
            streamWriter.WriteLine("\t/// </summary>");
            streamWriter.WriteLine("\tpublic class " + className + "");
            streamWriter.WriteLine("\t{");

            //根据主键加载数据集
            streamWriter.WriteLine("\t\t/// <summary>");
            streamWriter.WriteLine("\t\t/// 根据主键加载" + tablemark + "数据集");
            streamWriter.WriteLine("\t\t/// </summary>");
            streamWriter.WriteLine("\t\tpublic static DataTable LoadData(Guid ID, DbServers.DbServerName currDbName = DbServers.DbServerName.LatestDB)");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine("\t\t\tif (Globals.GetMasterSettings().OpenCacheServer)");
            streamWriter.WriteLine("\t\t\t{");
            streamWriter.WriteLine("\t\t\t\treturn null;    //后续扩冲： 开启缓存服务器后，从缓存服务器拿取数据");
            streamWriter.WriteLine("\t\t\t}");
            streamWriter.WriteLine("\t\t\telse");
            streamWriter.WriteLine("\t\t\t{");
            streamWriter.WriteLine("\t\t\t\treturn " + tableName + "Manager.LoadData(ID, currDbName);");
            streamWriter.WriteLine("\t\t\t}");
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine();

            //根据主键加载实体
            streamWriter.WriteLine("\t\t/// <summary>");
            streamWriter.WriteLine("\t\t/// 根据主键加载" + tablemark + "实体");
            streamWriter.WriteLine("\t\t/// </summary>");
            streamWriter.WriteLine("\t\tpublic static " + tableName + "Entity LoadEntity(Guid ID, DbServers.DbServerName currDbName = DbServers.DbServerName.LatestDB)");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine("\t\t\t\treturn " + tableName + "Manager.LoadEntity(ID, currDbName);");
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine();

            //根据条件查询数据集
            streamWriter.WriteLine("\t\t/// <summary>");
            streamWriter.WriteLine("\t\t/// 根据条件查询" + tablemark + "数据集");
            streamWriter.WriteLine("\t\t/// </summary>");
            streamWriter.WriteLine("\t\tpublic static DataTable GetListData(string where = null, string selectFields =\"*\", string orderby = null, int top = 0, DbServers.DbServerName currDbName = DbServers.DbServerName.LatestDB)");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine("\t\t\treturn " + tableName + "Manager.SelectListData(where,selectFields,orderby,top, currDbName);");
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine();

            //根据条件查询首行首列
            streamWriter.WriteLine("\t\t/// <summary>");
            streamWriter.WriteLine("\t\t/// 根据条件查询" + tablemark + "首行首列");
            streamWriter.WriteLine("\t\t/// </summary>");
            streamWriter.WriteLine("\t\tpublic static object GetScalar(string where = null, string selectFields =\"*\", string orderby = null, DbServers.DbServerName currDbName = DbServers.DbServerName.LatestDB)");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine("\t\t\treturn " + tableName + "Manager.SelectScalar(where,selectFields,orderby, currDbName);");
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine();

            //根据条件查询实体
            streamWriter.WriteLine("\t\t/// <summary>");
            streamWriter.WriteLine("\t\t/// 根据条件查询" + tablemark + "数据实体");
            streamWriter.WriteLine("\t\t/// </summary>");
            streamWriter.WriteLine("\t\tpublic static IList<" + tableName + "Entity> GetListEntity(string where = null, string selectFields =\"*\", string orderby = null, int top = 0, DbServers.DbServerName currDbName = DbServers.DbServerName.LatestDB)");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine("\t\t\t\treturn " + tableName + "Manager.SelectListEntity(where,selectFields,orderby,top, currDbName);");
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine();

            //根据主键删除
            streamWriter.WriteLine("\t\t/// <summary>");
            streamWriter.WriteLine("\t\t/// 根据主键删除" + tablemark);
            streamWriter.WriteLine("\t\t/// </summary>");
            streamWriter.WriteLine("\t\tpublic static void Del(Guid ID, DbServers.DbServerName currDbName = DbServers.DbServerName.LatestDB)");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine("\t\t\t" + tableName + "Manager.Del(ID, currDbName);");
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine();

            //根据条件删除
            streamWriter.WriteLine("\t\t/// <summary>");
            streamWriter.WriteLine("\t\t/// 根据条件删除" + tablemark);
            streamWriter.WriteLine("\t\t/// </summary>");
            streamWriter.WriteLine("\t\tpublic static void DelListData(string where = null, DbServers.DbServerName currDbName = DbServers.DbServerName.LatestDB)");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine("\t\t\t" + tableName + "Manager.DelListData(where, currDbName);");
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine();

            //保存
            streamWriter.WriteLine("\t\t/// <summary>");
            streamWriter.WriteLine("\t\t/// 保存" + tablemark);
            streamWriter.WriteLine("\t\t/// </summary>");
            streamWriter.WriteLine("\t\tpublic static bool SaveEntity(" + tableName + "Entity entity, bool isAdd, DbServers.DbServerName currDbName = DbServers.DbServerName.LatestDB)");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine("\t\t\treturn " + tableName + "Manager.SaveEntity(entity, isAdd, currDbName);");
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine();


            streamWriter.WriteLine("\t}");
            streamWriter.WriteLine("}");

            streamWriter.Flush();
            streamWriter.Close();
        }
    
    }
}

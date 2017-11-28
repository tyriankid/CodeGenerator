using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CodeGenerator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private string databasename = "";

        private void btnConnection_Click(object sender, EventArgs e)
        {
            DataTable dtData = new DataTable();
            dtData.Columns.Add("s",typeof(bool));
            dtData.Columns.Add("tablename", typeof(string));
            dtData.Columns.Add("tablekey", typeof(string));
            dtData.Columns.Add("tablemark", typeof(string));

            databasename = this.tboxDatabaseName.Text.Trim();
            string selectSql = Utility.GetTableQuery(databasename);
            DataTable dataTable = getDatable(selectSql);
            foreach (DataRow dr in dataTable.Rows)
            {
                selectSql = string.Format("exec sp_pkeys '{0}'", dr["TABLE_NAME"].ToString());
                DataTable dataKey = getDatable(selectSql);
                string strKey = dataKey.Rows.Count > 0 ? dataKey.Rows[0]["COLUMN_NAME"].ToString() : "";
                dtData.Rows.Add(new object[] { false, dr["TABLE_NAME"].ToString(), strKey,"" });
            }
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.DataSource = dtData;
        }

        private DataTable getDatable(string selectSql)
        {
            DataTable dataTable = new DataTable();
            string connectionString = "Server={0}; Database={1}; User ID={2}; Password={3};";
            connectionString = string.Format(connectionString, this.tboxServerName.Text.Trim(), this.tboxDatabaseName.Text.Trim()
                , this.tboxUserName.Text.Trim(), this.tboxPassword.Text.Trim());
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlDataAdapter dataAdapter = new SqlDataAdapter(selectSql, connection);
                dataAdapter.Fill(dataTable);
            }
            return dataTable;
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.DataSource == null) return;
            DataTable dtData = this.dataGridView1.DataSource as DataTable;
            bool b = dtData.Select("s=false").Length > 0 ? true : false;
            foreach (DataRow dr in dtData.Rows)
            {
                dr["s"] = b;
            }
        }

        private void btnGenerator_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.DataSource == null) return;
            DataTable dtData = this.dataGridView1.DataSource as DataTable;
            if (dtData.Select("s=true").Length==0)
            {
                MessageBox.Show("没有选择待生成的数据表");
                return;
            }

            DataRow[] rows = dtData.Select("s=true");
            foreach (DataRow dr in rows)
            {
                string selectSql = string.Format("Select * From {0} Where 1=2", dr["tablename"].ToString());
                DataTable dataTable = getDatable(selectSql);
                dataTable.TableName = dr["tablename"].ToString();
                CreateModel(dr,dataTable);
                CreateDal(dr, dataTable);
                CreateBll(dr, dataTable);
            }
            MessageBox.Show("生成代码类成功。");
        }

        private void CreateModel(DataRow dr,DataTable dataTable)
        {
            string modelPath = Application.StartupPath + "\\" + databasename + "\\" + tboxNamepase.Text.Trim() + ".Model";
            if (!Directory.Exists(modelPath))
            {
                Directory.CreateDirectory(modelPath);
            }
            CsGenerator.CreateModelClass(dataTable, dr["tablemark"].ToString(), modelPath + "\\", tboxNamepase.Text.Trim());
        }

        private void CreateDal(DataRow dr, DataTable dataTable)
        {
            string modelPath = Application.StartupPath + "\\" + databasename + "\\" + tboxNamepase.Text.Trim() + ".Dal";
            if (!Directory.Exists(modelPath))
            {
                Directory.CreateDirectory(modelPath);
            }

            string selectSqlFiels = "Select a.[name],b.name as name2 From syscolumns a,systypes b Where a.xtype=b.xtype AND b.[name]<>'sysname' AND a.id=object_id('{0}')";
            DataTable dtField = getDatable(string.Format(selectSqlFiels, dataTable.TableName));
            CsGenerator.CreateDalClass(dataTable, dr["tablemark"].ToString(), dr["tablekey"].ToString(), modelPath + "\\", dtField, tboxNamepase.Text.Trim());
        }

        private void CreateBll(DataRow dr, DataTable dataTable)
        {
            string modelPath = Application.StartupPath + "\\" + databasename + "\\" + tboxNamepase.Text.Trim() + ".Bll";
            if (!Directory.Exists(modelPath))
            {
                Directory.CreateDirectory(modelPath);
            }
            CsGenerator.CreateBllClass(dataTable, dr["tablemark"].ToString(), modelPath + "\\", tboxNamepase.Text.Trim());
        }

    }
}

namespace CodeGenerator
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.tboxServerName = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tboxNamepase = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tboxDatabaseName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tboxPassword = new System.Windows.Forms.TextBox();
            this.tboxUserName = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnGenerator = new System.Windows.Forms.Button();
            this.btnAll = new System.Windows.Forms.Button();
            this.btnConnection = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.s = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.tablename = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tablekey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tablemark = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(624, 10);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "服务器";
            // 
            // tboxServerName
            // 
            this.tboxServerName.Location = new System.Drawing.Point(70, 30);
            this.tboxServerName.Name = "tboxServerName";
            this.tboxServerName.Size = new System.Drawing.Size(195, 21);
            this.tboxServerName.TabIndex = 1;
            this.tboxServerName.Text = ".";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.tboxNamepase);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.tboxDatabaseName);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.tboxPassword);
            this.groupBox1.Controls.Add(this.tboxUserName);
            this.groupBox1.Controls.Add(this.tboxServerName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(624, 143);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "配置数据库";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(298, 118);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "命名空间名";
            // 
            // tboxNamepase
            // 
            this.tboxNamepase.Location = new System.Drawing.Point(379, 115);
            this.tboxNamepase.Name = "tboxNamepase";
            this.tboxNamepase.Size = new System.Drawing.Size(124, 21);
            this.tboxNamepase.TabIndex = 7;
            this.tboxNamepase.Text = "Kangaroo";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(298, 33);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 4;
            this.label4.Text = "数据库名";
            // 
            // tboxDatabaseName
            // 
            this.tboxDatabaseName.Location = new System.Drawing.Point(357, 30);
            this.tboxDatabaseName.Name = "tboxDatabaseName";
            this.tboxDatabaseName.Size = new System.Drawing.Size(249, 21);
            this.tboxDatabaseName.TabIndex = 0;
            this.tboxDatabaseName.Text = "Kangaroo";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(322, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "密码";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "用户名";
            // 
            // tboxPassword
            // 
            this.tboxPassword.Location = new System.Drawing.Point(357, 74);
            this.tboxPassword.Name = "tboxPassword";
            this.tboxPassword.Size = new System.Drawing.Size(249, 21);
            this.tboxPassword.TabIndex = 3;
            this.tboxPassword.Text = "kaifokid";
            // 
            // tboxUserName
            // 
            this.tboxUserName.Location = new System.Drawing.Point(70, 74);
            this.tboxUserName.Name = "tboxUserName";
            this.tboxUserName.Size = new System.Drawing.Size(195, 21);
            this.tboxUserName.TabIndex = 2;
            this.tboxUserName.Text = "sa";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnGenerator);
            this.groupBox2.Controls.Add(this.btnAll);
            this.groupBox2.Controls.Add(this.btnConnection);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 153);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(624, 57);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            // 
            // btnGenerator
            // 
            this.btnGenerator.Location = new System.Drawing.Point(522, 20);
            this.btnGenerator.Name = "btnGenerator";
            this.btnGenerator.Size = new System.Drawing.Size(75, 23);
            this.btnGenerator.TabIndex = 6;
            this.btnGenerator.Text = "生成";
            this.btnGenerator.UseVisualStyleBackColor = true;
            this.btnGenerator.Click += new System.EventHandler(this.btnGenerator_Click);
            // 
            // btnAll
            // 
            this.btnAll.Location = new System.Drawing.Point(428, 20);
            this.btnAll.Name = "btnAll";
            this.btnAll.Size = new System.Drawing.Size(75, 23);
            this.btnAll.TabIndex = 5;
            this.btnAll.Text = "全选/反选";
            this.btnAll.UseVisualStyleBackColor = true;
            this.btnAll.Click += new System.EventHandler(this.btnAll_Click);
            // 
            // btnConnection
            // 
            this.btnConnection.Location = new System.Drawing.Point(70, 21);
            this.btnConnection.Name = "btnConnection";
            this.btnConnection.Size = new System.Drawing.Size(75, 23);
            this.btnConnection.TabIndex = 4;
            this.btnConnection.Text = "连接数据库";
            this.btnConnection.UseVisualStyleBackColor = true;
            this.btnConnection.Click += new System.EventHandler(this.btnConnection_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.s,
            this.tablename,
            this.tablekey,
            this.tablemark});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 210);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView1.Size = new System.Drawing.Size(624, 232);
            this.dataGridView1.TabIndex = 2;
            // 
            // s
            // 
            this.s.DataPropertyName = "s";
            this.s.HeaderText = "选择";
            this.s.Name = "s";
            this.s.Width = 50;
            // 
            // tablename
            // 
            this.tablename.DataPropertyName = "tablename";
            this.tablename.HeaderText = "表名";
            this.tablename.Name = "tablename";
            this.tablename.ReadOnly = true;
            this.tablename.Width = 150;
            // 
            // tablekey
            // 
            this.tablekey.DataPropertyName = "tablekey";
            this.tablekey.HeaderText = "主键";
            this.tablekey.Name = "tablekey";
            this.tablekey.ReadOnly = true;
            // 
            // tablemark
            // 
            this.tablemark.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.tablemark.DataPropertyName = "tablemark";
            this.tablemark.HeaderText = "描述";
            this.tablemark.Name = "tablemark";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 442);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "贝格-代码生成器V1.0(内部版 By Jinhb 2016.07.27)";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tboxServerName;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tboxUserName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tboxPassword;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tboxDatabaseName;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnConnection;
        private System.Windows.Forms.Button btnGenerator;
        private System.Windows.Forms.Button btnAll;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn s;
        private System.Windows.Forms.DataGridViewTextBoxColumn tablename;
        private System.Windows.Forms.DataGridViewTextBoxColumn tablekey;
        private System.Windows.Forms.DataGridViewTextBoxColumn tablemark;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tboxNamepase;
    }
}


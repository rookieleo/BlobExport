using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using Momo.Forms;
using System.Xml;
using System.IO;

namespace BlobExport
{
    public partial class FormMain : MForm
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            this.Waiting(delegate
            {
                this.Invoke((EventHandler)delegate
                {
                    mTextBox1.Text = Config.GetInstance().IPAddress;
                    mTextBox2.Text = Config.GetInstance().UserName;
                    mTextBox3.Text = Config.GetInstance().Password;
                });
            }, ERollingBarStyle.PointsRolling, "配置初始化，请稍候");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(this.mTextBox1.Text) && !String.IsNullOrEmpty(this.mTextBox2.Text) && !String.IsNullOrEmpty(this.mTextBox3.Text))
            {
                this.Waiting(delegate
                {
                    ArrayList dbList = DBHelper.GetAllDataBase(this.mTextBox1.Text, this.mTextBox2.Text, this.mTextBox3.Text);

                    this.Invoke((EventHandler)delegate
                    {
                        this.mComboBox1.Items.AddRange(dbList.ToArray());
                        Config.GetInstance().IPAddress = mTextBox1.Text;
                        Config.GetInstance().UserName = mTextBox2.Text;
                        Config.GetInstance().Password = mTextBox3.Text;
                        Config.GetInstance().Save();
                    });
                }, ERollingBarStyle.PointsRolling, "正在连接，请稍候");
            }
            else
            {
                MessageBoxEx.Show("请输入连接地址");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            sfd.FileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".dat";
            sfd.Filter = "二进制数据文件（*.dat）|*.dat";
            sfd.FilterIndex = 1;
            sfd.DefaultExt = ".dat";
            sfd.DereferenceLinks = false;
            sfd.Title = "二进制数据文件导出";
            sfd.RestoreDirectory = true;

            //点了保存按钮进入 
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                String dbName = this.mComboBox1.Items[this.mComboBox1.SelectedIndex].ToString();
                String tableName = this.mComboBox2.Items[this.mComboBox2.SelectedIndex].ToString();
                String columnName = this.mComboBox3.Items[this.mComboBox3.SelectedIndex].ToString();

                this.Waiting(delegate
                {
                    DBHelper.SaveData(this.mTextBox1.Text, this.mTextBox2.Text, this.mTextBox3.Text, dbName, tableName, columnName, this.mTextBox4.Text, sfd.FileName.ToString());
                }, ERollingBarStyle.PointsRolling, "正在导出，请稍候");
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            String dbName = this.mComboBox1.Items[this.mComboBox1.SelectedIndex].ToString();

            if (!String.IsNullOrEmpty(dbName))
            {
                ArrayList dbList = DBHelper.GetAllTableName(this.mTextBox1.Text, this.mTextBox2.Text, this.mTextBox3.Text, dbName);

                this.mComboBox2.Items.AddRange(dbList.ToArray());
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            String dbName = this.mComboBox1.Items[this.mComboBox1.SelectedIndex].ToString();
            String tableName = this.mComboBox2.Items[this.mComboBox2.SelectedIndex].ToString();

            if (!String.IsNullOrEmpty(dbName))
            {
                ArrayList dbList = DBHelper.GetAllColumnName(this.mTextBox1.Text, this.mTextBox2.Text, this.mTextBox3.Text, dbName, tableName);

                this.mComboBox3.Items.AddRange(dbList.ToArray());
            }
        }
    }
}

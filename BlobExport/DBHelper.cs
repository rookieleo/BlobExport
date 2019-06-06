using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data.SqlClient;
using System.Data;
using System.IO;

public class DBHelper
{
    /// <summary>
    /// 获取指定IP地址的数据库所有数据库实例名。
    /// </summary>
    /// <param name="ip">指定的 IP 地址。</param>
    /// <param name="username">登录数据库的用户名。</param>
    /// <param name="password">登陆数据库的密码。</param>
    /// <returns>返回包含数据实例名的列表。</returns>
    public static ArrayList GetAllDataBase(string ip, string username, string password)
    {
        ArrayList DBNameList = new ArrayList();

        SqlConnection Connection = new SqlConnection(String.Format("Data Source={0};Initial Catalog = master;User ID = {1};PWD = {2}", ip, username, password));

        DataTable DBNameTable = new DataTable();

        SqlDataAdapter Adapter = new SqlDataAdapter("select name from master..sysdatabases", Connection);

        lock (Adapter)
        {
            Adapter.Fill(DBNameTable);
        }

        foreach (DataRow row in DBNameTable.Rows)
        {
            DBNameList.Add(row["name"]);
        }

        DBNameList.Sort();

        Connection.Close();

        return DBNameList;
    }

    /// <summary>
    /// 获取指定IP地址的数据库所有数据库实例名。
    /// </summary>
    /// <param name="ip">指定的 IP 地址。</param>
    /// <param name="username">登录数据库的用户名。</param>
    /// <param name="password">登陆数据库的密码。</param>
    /// <returns>返回包含数据实例名的列表。</returns>
    public static ArrayList GetAllTableName(string ip, string username, string password, string dbName)
    {
        ArrayList DBNameList = new ArrayList();

        using (SqlConnection Connection = new SqlConnection(String.Format("Data Source = {0};Initial Catalog = {3};User ID = {1};PWD = {2}", ip, username, password, dbName)))
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }

            using (SqlCommand command = new SqlCommand("Select * From Information_Schema.Tables", Connection))
            {
                using (SqlDataReader dr = command.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (dr.Read())
                    {
                        DBNameList.Add(dr["Table_Name"]);
                    }
                }
            }

            DBNameList.Sort();

            if (Connection.State == ConnectionState.Open)
            {
                Connection.Close();
            }
        }

        return DBNameList;
    }

    /// <summary>
    /// 获取指定IP地址的数据库所有数据库实例名。
    /// </summary>
    /// <param name="ip">指定的 IP 地址。</param>
    /// <param name="username">登录数据库的用户名。</param>
    /// <param name="password">登陆数据库的密码。</param>
    /// <returns>返回包含数据实例名的列表。</returns>
    public static ArrayList GetAllColumnName(string ip, string username, string password, string dbName, string tableName)
    {
        ArrayList DBNameList = new ArrayList();

        using (SqlConnection Connection = new SqlConnection(String.Format("Data Source = {0};Initial Catalog = {3};User ID = {1};PWD = {2}", ip, username, password, dbName)))
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }

            using (SqlCommand command = new SqlCommand("Select * From Information_Schema.Columns t Where t.Table_Name = '" + tableName + "'", Connection))
            {
                using (SqlDataReader dr = command.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (dr.Read())
                    {
                        DBNameList.Add(dr["Column_Name"]);
                    }
                }
            }

            DBNameList.Sort();

            if (Connection.State == ConnectionState.Open)
            {
                Connection.Close();
            }
        }

        return DBNameList;
    }

    public static void SaveData(string ip, string username, string password, string dbName, string tableName, string columnName, string strWhere, string path)
    {
        using (SqlConnection Connection = new SqlConnection(String.Format("Data Source = {0};Initial Catalog = {3};User ID = {1};PWD = {2}", ip, username, password, dbName)))
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }

            using (SqlCommand command = new SqlCommand("Select " + columnName + " From " + tableName + " " + strWhere, Connection))
            {
                using (SqlDataReader dr = command.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    int count = 0;

                    while (dr.Read())
                    {
                        path = Path.GetDirectoryName(path) + "\\" + DateTime.Now.ToString("yyyyMMddHHmmss") + Convert.ToString(count) + ".dat";

                        using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                        {
                            using (BinaryWriter binWriter = new BinaryWriter(fs))
                            {
                                byte[] data = (byte[])dr["data"];

                                binWriter.Write(data, 0, data.Length);

                                binWriter.Close();
                            }
                            fs.Close();
                        }

                        count++;
                    }
                }
            }

            if (Connection.State == ConnectionState.Open)
            {
                Connection.Close();
            }
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace RecoderServerApplication.SQLite
{
    public class SQLite_RW
    {
        public static List<SQLite_DataStruct> SqlData = new List<SQLite_DataStruct>();
        static string database_name;
        public static void Initial_SQLite(string filename)
        {
            database_name = filename;
            try
            {
                if (System.IO.File.Exists(filename + ".sqlite"))
                {
                    //存在文件
                    //SQLiteConnection cn = new SQLiteConnection("data source=device_info.sqlite");
                    //if (cn.State != System.Data.ConnectionState.Open)
                    //{
                    //    cn.Open();
                    //    SQLiteCommand cmd = new SQLiteCommand();
                    //    cmd.Connection = cn;
                    //    cmd.CommandText = "SELECT * FROM device";
                    //    SQLiteDataReader sr = cmd.ExecuteReader();
                    //    sql_data.Items.Clear();
                    //    int i = 0;
                    //    while (sr.Read())
                    //    {
                    //        //Console.WriteLine($"{sr.GetString(0)} {sr.GetString(1)}");
                    //        UI_Trans data2 = new UI_Trans
                    //        {
                    //            Index = i,
                    //            ID = sr.GetString(0),
                    //            Bind = sr.GetString(1),
                    //        };
                    //        sql_data.Items.Add(data2);
                    //        i++;
                    //    }
                    //}
                    //cn.Close();
                }
                else
                {
                    //不存在文件
                    SQLiteConnection cn = new SQLiteConnection("data source=" + database_name + ".sqlite");
                    if (cn.State != System.Data.ConnectionState.Open)
                    {
                        cn.Open();
                        SQLiteCommand cmd = new SQLiteCommand();
                        cmd.Connection = cn;
                        cmd.CommandText = "CREATE TABLE " + "Device" + "(ID varchar PRIMARY KEY,UserID varchar,NickName varchar)";
                        //cmd.CommandText = "CREATE TABLE IF NOT EXISTS t1(id varchar(4),score int)";
                        cmd.ExecuteNonQuery();
                    }
                    cn.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        public static SQLite_InitData GetInitData()
        {
            SQLite_InitData recv = new SQLite_InitData();
            SQLiteConnection cn = new SQLiteConnection("data source=" + database_name + ".sqlite");
            if (cn.State != System.Data.ConnectionState.Open)
            {
                cn.Open();
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.Connection = cn;
                cmd.CommandText = "SELECT * FROM system";
                SQLiteDataReader sr = cmd.ExecuteReader();
                //sql_data.Items.Clear();
                if (sr.Read())
                {
                    recv.init_Group = sr.GetValue(1).ToString();
                    recv.init_WIFI = sr.GetValue(2).ToString();
                    recv.init_PASS = sr.GetValue(3).ToString();
                    recv.init_IP = sr.GetValue(4).ToString();
                    recv.init_port = sr.GetValue(5).ToString();
                    recv.init_Server_IP = sr.GetValue(6).ToString();
                    recv.init_Server_Port = sr.GetValue(7).ToString();
                    //Console.WriteLine($"{sr.GetString(0)} {sr.GetString(1)}");
                }


            }
            cn.Close();
            return recv;
        }
        public static List<SQLite_DataStruct> GetData()
        {
            SqlData.Clear();
            SQLiteConnection cn = new SQLiteConnection("data source=" + database_name + ".sqlite");
            if (cn.State != System.Data.ConnectionState.Open)
            {
                cn.Open();
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.Connection = cn;
                cmd.CommandText = "SELECT * FROM device";
                SQLiteDataReader sr = cmd.ExecuteReader();
                //sql_data.Items.Clear();
                int i = 0;
                while (sr.Read())
                {
                    //Console.WriteLine($"{sr.GetString(0)} {sr.GetString(1)}");
                    SQLite_DataStruct data2 = new SQLite_DataStruct
                    {
                        Index = i,
                        DeviceID = sr.GetString(0),
                        UserID = sr.GetString(1),
                        NickName = sr.GetString(2)
                    };
                    SqlData.Add(data2);
                    i++;
                }
            }
            cn.Close();
            return SqlData;
        }
        public static bool SetData(SQLite_InitData data)
        {
            SQLiteConnection cn = new SQLiteConnection("data source=" + database_name + ".sqlite");
            if (cn.State != System.Data.ConnectionState.Open)
            {
                cn.Open();
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.Connection = cn;

                try
                {//'ID' int,'group' string,'wifi' string,'pass' string,'ip' string,'port' string,'serverip' string,'serverport' string
                    cmd.CommandText = "UPDATE system SET 'group'='"+ (data.init_Group == null ? "" : data.init_Group) + 
                                                        "',wifi='"+ (data.init_WIFI == null ? "" : data.init_WIFI) + 
                                                        "',pass='"+ (data.init_PASS == null ? "" : data.init_PASS) + 
                                                        "',ip='"+ (data.init_IP == null ? "" : data.init_IP) + 
                                                        "',port='"+ (data.init_port == null ? "" : data.init_port) + 
                                                        "',serverip='"+ (data.init_Server_IP == null ? "" : data.init_Server_IP) + 
                                                        "',serverport='"+ (data.init_Server_Port == null ? "" : data.init_Server_Port) + "' WHERE ID=0";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    cn.Close();
                    MessageBox.Show(ex.Message);
                    return false;
                }
                

            }
            cn.Close();
            return true;
        }
        public static bool SetData(SQLite_DataStruct data)
        {
            SQLiteConnection cn = new SQLiteConnection("data source=" + database_name + ".sqlite");
            if (cn.State != System.Data.ConnectionState.Open)
            {
                cn.Open();
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.Connection = cn;
                cmd.CommandText = "INSERT INTO device VALUES('" + data.DeviceID + "','" + data.UserID + "','" + data.NickName + "')";
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception es)
                {
                    try
                    {
                        cmd.CommandText = "UPDATE device SET UserID=@userid,NickName=@nickname WHERE ID='" + data.DeviceID + "'";
                        cmd.Parameters.Add("userid", DbType.String).Value = data.UserID == null ? "": data.UserID;
                        cmd.Parameters.Add("nickname", DbType.String).Value = data.NickName == null ? "" : data.NickName;
                        cmd.ExecuteNonQuery();
                    }
                    catch(Exception ex)
                    {
                        cn.Close();
                        MessageBox.Show(ex.Message);
                        return false;
                    }
                }

            }
            cn.Close();
            return true;
        }

        public static bool DeleteData(SQLite_DataStruct data)
        {
            SQLiteConnection cn = new SQLiteConnection("data source=" + database_name + ".sqlite");
            if (cn.State != System.Data.ConnectionState.Open)
            {
                try
                {
                    cn.Open();
                    SQLiteCommand cmd = new SQLiteCommand();
                    cmd.Connection = cn;
                    cmd.CommandText = "DELETE FROM device WHERE ID='" + data.DeviceID + "'";
                    cmd.ExecuteNonQuery();
                }
                catch(Exception e)
                {
                    cn.Close();
                    MessageBox.Show(e.Message);
                    return false;

                }
            }
            cn.Close();
            return true;
        }

    }
}

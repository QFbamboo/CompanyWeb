using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using MySql.Data.MySqlClient;

namespace CompanyWeb
{
    public static class MySqlHelper
    {
        //public static readonly string connstr =
        //    ConfigurationManager.ConnectionStrings["connstr"].ConnectionString;

        static string connstr = "";

        public static MySqlConnection OpenConnection()
        {
            MySqlConnection conn = new MySqlConnection(connstr);
            conn.Open();
            return conn;
        }

        public static int ExecuteNonQuery(string cmdText,
            params MySqlParameter[] parameters)
        {
            using (MySqlConnection conn = new MySqlConnection(connstr))
            {
                conn.Open();
                return ExecuteNonQuery(conn, cmdText, parameters);
            }
        }

        public static object ExecuteScalar(string cmdText,
            params MySqlParameter[] parameters)
        {
            using (MySqlConnection conn = new MySqlConnection(connstr))
            {

                conn.Open();
                return ExecuteScalar(conn, cmdText, parameters);
            }
        }

        public static DataTable ExecuteDataTable(string cmdText,
            params MySqlParameter[] parameters)
        {
            using (MySqlConnection conn = new MySqlConnection(connstr))
            {
                conn.Open();
                return ExecuteDataTable(conn, cmdText, parameters);
            }
        }

        public static int ExecuteNonQuery(MySqlConnection conn, string cmdText,
           params MySqlParameter[] parameters)
        {
            using (MySqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = cmdText;
                cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteNonQuery();
            }
        }

        public static object ExecuteScalar(MySqlConnection conn, string cmdText,
            params MySqlParameter[] parameters)
        {
            using (MySqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = cmdText;
                cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteScalar();
            }
        }

        public static DataTable ExecuteDataTable(MySqlConnection conn, string cmdText,
            params MySqlParameter[] parameters)
        {
            using (MySqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = cmdText;
                cmd.Parameters.AddRange(parameters);
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        public static object ToDBValue(this object value)
        {
            return value == null ? DBNull.Value : value;
        }

        public static object FromDBValue(this object dbValue)
        {
            return dbValue == DBNull.Value ? null : dbValue;
        }
    }
}

using Foundation.Dapper.Enum;
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Foundation.Dapper.Extensions
{
    public static class DapperExtensions
    {
        /// <summary>
        /// 获取对应数据库连接对象
        /// </summary>
        /// <param name="dataBaseType">数据库类型</param>
        /// <param name="connectionString">连接字符串</param>
        /// <returns></returns>
        public static IDbConnection GetDbConnection(this DataBaseType dataBaseType, string connectionString)
        {
            IDbConnection dbConnection;
            switch (dataBaseType)
            {
                case DataBaseType.MySql: dbConnection = new MySqlConnection(connectionString); break;
                case DataBaseType.Oracle: dbConnection = new OracleConnection(connectionString); break;
                default: dbConnection = new SqlConnection(connectionString); break;
            }
            dbConnection?.Open();
            return dbConnection;
        }
    }
}

using Foundation.Dapper.Enum;
using System;
using System.Collections.Generic;
using System.Data;

namespace Foundation.Dapper.Interface
{
    public interface IDapperExecute : IDisposable
    {
        /// <summary>
        /// 获取数据库连接对象
        /// </summary>
        IDbConnection DbConnection { get; }
        /// <summary>
        /// 设置数据库连接
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        IDapperExecute SetConnection(string connectionString, DataBaseType dataBaseType = DataBaseType.SQLServer);
        /// <summary>
        /// 启用新事务
        /// </summary>
        void StartNewTransaction();
        /// <summary>
        /// 提交事务
        /// </summary>
        void CommitTransaction();
        /// <summary>
        /// 回滚事务
        /// </summary>
        void RollBackTransaction();
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="sql">Sql语句</param>
        /// <param name="param"></param>
        /// <returns></returns>
        IEnumerable<dynamic> Query(string sql, object param = null);
        /// <summary>
        /// 查询记录数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        int Count(string sql, object param = null);
        /// <summary>
        /// 执行修改、删除语句
        /// </summary>
        /// <param name="sql">Sql语句</param>
        /// <param name="param"></param>
        /// <returns></returns>
        int Execute(string sql, object param = null);
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

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
        /// <param name="beginTransaction">是否启用事务</param>
        /// <returns></returns>
        IDapperExecute SetConnection(string connectionString, bool beginTransaction = false);
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

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="sql">Sql语句</param>
        /// <param name="param"></param>
        /// <param name="dtName">DataTable名称</param>
        /// <returns></returns>
        DataTable Query(string sql, object param = null, string dtName = null);

        bool IsOracle(string connectionString = null);
    }
}

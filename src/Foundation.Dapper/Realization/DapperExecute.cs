using Foundation.Dapper.Enum;
using Foundation.Dapper.Interface;
using Foundation.Dapper.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Dapper;
using System.Linq;

namespace Foundation.Dapper.Realization
{
    /// <summary>
    /// Dapper执行器
    /// </summary>
    public class DapperExecute : IDapperExecute
    {
        #region private

        /// <summary>
        /// 连接对象
        /// </summary>
        private IDbConnection _dbConnection;
        /// <summary>
        /// 事务对象
        /// </summary>
        private IDbTransaction _dbTransaction;

        #endregion private

        /// <summary>
        /// 连接信息
        /// </summary>
        public IDbConnection DbConnection { get => _dbConnection; }

        /// <summary>
        /// 开启事务
        /// </summary>
        public void StartNewTransaction() => _dbTransaction = _dbConnection?.BeginTransaction();

        /// <summary>
        /// 提交事务
        /// </summary>
        public void CommitTransaction() => _dbTransaction?.Commit();

        /// <summary>
        /// 归滚事务
        /// </summary>
        public void RollBackTransaction() => _dbTransaction?.Rollback();

        /// <summary>
        /// 初始化连接对象
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="dataBaseType">数据库类型</param>
        /// <returns></returns>
        public IDapperExecute SetConnection(string connectionString, DataBaseType dataBaseType = DataBaseType.SQLServer)
        {
            _dbConnection = dataBaseType.GetDbConnection(connectionString);
            return this;
        }

        public void Dispose() { }

        public int Execute(string sql, object pairs = null) => _dbConnection.Execute(sql, pairs);

        public IEnumerable<dynamic> Query(string sql, object pairs = null) => _dbConnection.Query(sql, pairs);

        public int Count(string sql, object pairs = null) => _dbConnection.Query(sql, pairs)?.Count() ?? 0;
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Foundation.Server.Domain.Entity
{
    /// <summary>
    ///     表示数据库类型
    /// </summary>
    public enum DatabaseType
    {
        /// <summary>
        ///     SqlServer数据库类型
        /// </summary>
        SqlServer,

        /// <summary>
        ///     Sqlite数据库类型
        /// </summary>
        Sqlite,

        /// <summary>
        ///     MySql数据库类型
        /// </summary>
        MySql,

        /// <summary>
        ///     Oracle数据库类型
        /// </summary>
        Oracle
    }
}
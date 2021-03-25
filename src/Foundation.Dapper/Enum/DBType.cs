using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Foundation.Dapper.Enum
{
    public enum DataBaseType
    {
        [Description("SQL Server 数据库")]
        SQLServer,
        [Description("Oracle 数据库")]
        Oracle,
        [Description("MySql 数据库")]
        MySql
    }
}

using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace Diomin.Foundation.Core.Extensions
{
    /// <summary>
    /// 列表字段排序条件
    /// </summary>
    public class SortCondition
    {
        /// <summary>
        /// 根据排序字段名称、升序排序初始化实例
        /// </summary>
        /// <param name="sortField"></param>
        public SortCondition(string sortField) : this(sortField, ListSortDirection.Ascending)
        { }

        /// <summary>
        /// 根据排序字段名称和排序方式初始化实例
        /// </summary>
        /// <param name="sortField"></param>
        /// <param name="listSortDirection"></param>
        public SortCondition(string sortField, ListSortDirection listSortDirection)
        {
            SortField = sortField;
            ListSortDirection = listSortDirection;
        }

        /// <summary>
        /// 获取或设置 排序字段名称
        /// </summary>
        public string SortField { get; set; }

        /// <summary>
        /// 获取或设置 排序方向
        /// </summary>
        public ListSortDirection ListSortDirection { get; set; }
    }
    /// <summary>
    /// 泛型的列表字段排序条件
    /// </summary>
    /// <typeparam name="T">列表类型</typeparam>
    public class SortCondition<T> : SortCondition
    {
        /// <summary>
        /// 使用排序字段初始化实例
        /// </summary>
        /// <param name="keySelector"></param>
        public SortCondition(Expression<Func<T, object>> keySelector)
            : this(keySelector, ListSortDirection.Ascending)
        { }
        /// <summary>
        /// 使用排序字段与排序方式初始化实例
        /// </summary>
        /// <param name="keySelector"></param>
        /// <param name="listSortDirection"></param>
        public SortCondition(Expression<Func<T, object>> keySelector, ListSortDirection listSortDirection)
            : base(GetPropertyName(keySelector), listSortDirection)
        { }


        /// <summary>
        /// 从泛型委托获取属性名
        /// </summary>
        private static string GetPropertyName(Expression<Func<T, object>> keySelector)
        {
            string param = keySelector.Parameters.First().Name;
            string operand = ((dynamic)keySelector.Body).Operand.ToString();
            operand = operand.Substring(param.Length + 1, operand.Length - param.Length - 1);
            return operand;
        }
    }
}
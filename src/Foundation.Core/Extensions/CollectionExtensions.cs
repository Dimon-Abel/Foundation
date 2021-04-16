using Foundation.Core.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Foundation.Core.Extensions
{
    /// <summary>
    ///     集合扩展方法类
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// 比较两个集合内是否有重叠值
        /// </summary>
        /// <param name="lst1">集合A</param>
        /// <param name="lst2">集合B</param>
        /// <param name="func">对比委托</param>
        /// <returns></returns>
        public static bool CompareList<T, V>(this ICollection<T> lst1, ICollection<V> lst2, Func<T, V, bool> func)
        {
            foreach (var item in lst1)
                foreach (var item2 in lst2)
                    if (func.Invoke(item, item2))
                        return true;
            return false;
        }

        /// <summary>
        /// 如果不为空，添加项
        /// </summary>
        public static void AddIfNotNull<T>(this ICollection<T> collection, T value) where T : class
        {
            Check.NotNull(collection, nameof(collection));
            if (value != null)
            {
                collection.Add(value);
            }
        }
        /// <summary>
        ///     如果不存在，添加项
        /// </summary>
        public static void AddIfNotExist<T>(this ICollection<T> collection, T value)
        {
            Check.NotNull(collection, nameof(collection));
            if (!collection.Contains(value))
                collection.Add(value);
        }

        /// <summary>
        ///     将集合中的某个字段转换为string后循环生成合并后的字符串
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="collections">集合对象</param>
        /// <param name="separator">分隔符，默认为逗号</param>
        /// <returns>返回合并后的字符串</returns>
        public static string ExpandAndToString<T>(this IEnumerable<T> collections, string separator = ",")
        {
            return collections.ExpandAndToString(t => t.ToString(), separator);
        }

        /// <summary>
        ///     将集合中的某个字段循环调用委托生成合并后的字符串
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="collection">集合对象</param>
        /// <param name="itemFormatFunc">单个集合项的转换委托</param>
        /// <param name="separetor">分隔符，默认为逗号</param>
        /// <returns>返回合并后的字符串</returns>
        public static string ExpandAndToString<T>(this IEnumerable<T> collection, Func<T, string> itemFormatFunc,
            string separetor = ",")
        {
            collection = collection as IList<T> ?? collection.ToList();
            itemFormatFunc.CheckNotNull("itemFormatFunc");
            if (!collection.Any())
                return null;
            var sb = new StringBuilder();
            var i = 0;
            var count = collection.Count();
            foreach (var t in collection)
            {
                if (i == count - 1)
                    sb.Append(itemFormatFunc(t));
                else
                    sb.Append(itemFormatFunc(t) + separetor);
                i++;
            }
            return sb.ToString();
        }

        /// <summary>
        ///     判断集合是否为空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static bool IsEmpty<T>(this IEnumerable<T> collection)
        {
            if (collection == null) return true;
            collection = collection as IList<T> ?? collection.ToList();
            return !collection.Any();
        }

        /// <summary>
        ///     根据第三方条件是否为真来决定是否执行指定条件的查询
        /// </summary>
        /// <typeparam name="T">动态类型</typeparam>
        /// <param name="source">要查询的数据源</param>
        /// <param name="predicate">查询条件</param>
        /// <param name="condition">第三方条件(bool 表达式)</param>
        /// <returns>返回查询的结果</returns>
        public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, Func<T, bool> predicate, bool condition)
        {
            predicate.CheckNotNull("predicate");
            source = source as IList<T> ?? source.ToList();
            return condition ? source.Where(predicate) : source;
        }

        /// <summary>
        ///     根据指定条件返回集合中不重复的元素（按条件去重，如帐号相同的数据只取第一条）
        /// </summary>
        /// <typeparam name="T">动态类型</typeparam>
        /// <typeparam name="TKey">动态筛选条件类型</typeparam>
        /// <param name="source">要操作的源</param>
        /// <param name="keySelector">重复数据筛选条件</param>
        /// <returns>返回去重后的集合</returns>
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        {
            keySelector.CheckNotNull("keySelector");
            source = source as IList<T> ?? source.ToList();
            return source.GroupBy(keySelector).Select(group => group.First());
        }

        /// <summary>
        ///     将<see cref="IOrderedEnumerable{T}" />集合按指定字段与排序方式进行排序
        /// </summary>
        /// <typeparam name="T">集合项类型</typeparam>
        /// <param name="source">要排序的数据集合</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="sortDirection">排序类型</param>
        /// <returns>返回排序后的集合</returns>
        public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> source, string propertyName,
            ListSortDirection sortDirection = ListSortDirection.Ascending)
        {
            propertyName.CheckNotNullOrEmpty("propertyName");
            return CollectionPropertySorter<T>.OrderBy(source, propertyName, sortDirection);
        }

        /// <summary>
        ///     将<see cref="IOrderedEnumerable{T}" />集合按指定字段排序条件进行排序
        /// </summary>
        /// <typeparam name="T">动态类型</typeparam>
        /// <param name="source">要排序的数据集合</param>
        /// <param name="sortCondition">列表字段排序条件</param>
        /// <returns>返回排序后的集合</returns>
        public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> source, SortCondition sortCondition)
        {
            sortCondition.CheckNotNull("sortCondition");
            return source.OrderBy(sortCondition.SortField, sortCondition.ListSortDirection);
        }

        /// <summary>
        ///     将<see cref="IOrderedEnumerable{T}" />集合按指定字段排序条件进行排序
        /// </summary>
        /// <typeparam name="T">动态类型</typeparam>
        /// <param name="source">要排序的数据集合</param>
        /// <param name="sortCondition">泛型列表字段排序条件</param>
        /// <returns>返回排序后的集合</returns>
        public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> source, SortCondition<T> sortCondition)
        {
            sortCondition.CheckNotNull("sortCondition");
            return source.OrderBy(sortCondition.SortField, sortCondition.ListSortDirection);
        }

        /// <summary>
        ///     把<see cref="IOrderedEnumerable{T}" />集合继续按指定字段排序方式进行排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="propertyName"></param>
        /// <param name="sortDirection"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<T> ThenBy<T>(this IOrderedEnumerable<T> source,
            string propertyName,
            ListSortDirection sortDirection = ListSortDirection.Ascending)
        {
            source.CheckNotNull("source");
            propertyName.CheckNotNullOrEmpty("propertyName");

            return CollectionPropertySorter<T>.ThenBy(source, propertyName, sortDirection);
        }

        /// <summary>
        ///     把<see cref="IOrderedEnumerable{T}" />集合继续指定字段排序方式进行排序
        /// </summary>
        /// <typeparam name="T">动态类型</typeparam>
        /// <param name="source">要排序的数据集合</param>
        /// <param name="sortCondition">列表字段排序条件</param>
        /// <returns>返回排序后的集合</returns>
        public static IOrderedEnumerable<T> ThenBy<T>(this IOrderedEnumerable<T> source, SortCondition sortCondition)
        {
            source.CheckNotNull("source");
            sortCondition.CheckNotNull("sortCondition");

            return source.ThenBy(sortCondition.SortField, sortCondition.ListSortDirection);
        }

        /// <summary>
        ///     把<see cref="IOrderedEnumerable{T}" />集合继续指定字段排序方式进行排序
        /// </summary>
        /// <typeparam name="T">动态类型</typeparam>
        /// <param name="source">要排序的数据集合</param>
        /// <param name="sortCondition">泛型列表字段排序条件</param>
        /// <returns>返回排序后的集合</returns>
        public static IOrderedEnumerable<T> ThenBy<T>(this IOrderedEnumerable<T> source, SortCondition<T> sortCondition)
        {
            source.CheckNotNull("source");
            sortCondition.CheckNotNull("sortCondition");

            return source.ThenBy(sortCondition.SortField, sortCondition.ListSortDirection);
        }

        #region DataTable

        /// <summary>
        ///     将DataTable转为List T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataTable dt)
        {
            return dt.ToJson().FromJson<List<T>>();
        }

        #endregion

        #region IQueryable的扩展方法

        /// <summary>
        ///     根据第三方条件是否为真来决定是否执行指定条件的查询
        /// </summary>
        /// <typeparam name="T">动态类型</typeparam>
        /// <param name="source">要查询的数据源</param>
        /// <param name="predicate">查询条件</param>
        /// <param name="condition">第三方条件(bool 表达式)</param>
        /// <returns>返回查询的结果</returns>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate,
            bool condition)
        {
            source.CheckNotNull("source");
            predicate.CheckNotNull("predicate");
            return condition ? source.Where(predicate) : source;
        }

        /// <summary>
        ///     把<see cref="IQueryable{T}" />集合按指定字段与排序方式进行排序
        /// </summary>
        /// <typeparam name="T">动态类型</typeparam>
        /// <param name="source">要排序的数据集</param>
        /// <param name="propertyName">排序属性名</param>
        /// <param name="sortDirection">排序类型</param>
        /// <returns>返回排序后的集合</returns>
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source,
            string propertyName,
            ListSortDirection sortDirection = ListSortDirection.Ascending)
        {
            source.CheckNotNull("source");
            propertyName.CheckNotNullOrEmpty("propertyName");

            return CollectionPropertySorter<T>.OrderBy(source, propertyName, sortDirection);
        }

        /// <summary>
        ///     把<see cref="IQueryable{T}" />集合按指定字段排序条件进行排序
        /// </summary>
        /// <typeparam name="T">动态类型</typeparam>
        /// <param name="source">要排序的数据集</param>
        /// <param name="sortCondition">列表字段排序条件</param>
        /// <returns>返回排序后的集合</returns>
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, SortCondition sortCondition)
        {
            source.CheckNotNull("source");
            sortCondition.CheckNotNull("sortCondition");

            return source.OrderBy(sortCondition.SortField, sortCondition.ListSortDirection);
        }

        /// <summary>
        ///     把<see cref="IQueryable{T}" />集合按指定字段排序条件进行排序
        /// </summary>
        /// <typeparam name="T">动态类型</typeparam>
        /// <param name="source">要排序的数据集</param>
        /// <param name="sortCondition">泛型列表字段排序条件</param>
        /// <returns>返回排序后的集合</returns>
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, SortCondition<T> sortCondition)
        {
            source.CheckNotNull("source");
            sortCondition.CheckNotNull("sortCondition");
            return source.OrderBy(sortCondition.SortField, sortCondition.ListSortDirection);
        }

        /// <summary>
        ///     把<see cref="IOrderedQueryable{T}" />集合继续按指定字段排序方式进行排序
        /// </summary>
        /// <typeparam name="T">动态类型</typeparam>
        /// <param name="source">要排序的数据集</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="sortDirection">排序类型</param>
        /// <returns>返回排序后的集合</returns>
        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source,
            string propertyName,
            ListSortDirection sortDirection = ListSortDirection.Ascending)
        {
            source.CheckNotNull("source");
            propertyName.CheckNotNullOrEmpty("propertyName");

            return CollectionPropertySorter<T>.ThenBy(source, propertyName, sortDirection);
        }

        /// <summary>
        ///     根据<see cref="DataSearch" />排序及分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="search">数据查询对象</param>
        /// <returns></returns>
        public static IQueryable<T> OrderAndPager<T>(this IQueryable<T> source, DataSearch search)
        {
            source.CheckNotNull("source");
            if (search != null)
            {
                if (!search.SortField.IsNullOrEmpty())
                    source = source.OrderBy(search.SortField,
                        search.SortType == "descending" ? ListSortDirection.Descending : ListSortDirection.Ascending);
                else
                    source = source.OrderBy("Id"); //排序字段为空但又要分页，这种情况下必须指定一个排序字段，否则报错

                if (search.PageIndex > 0)
                    source = source.Skip((search.PageIndex - 1) * search.PageSize).Take(search.PageSize);
            }
            return source;
        }

        /// <summary>
        ///     把<see cref="IOrderedQueryable{T}" />集合继续指定字段排序方式进行排序
        /// </summary>
        /// <typeparam name="T">动态类型</typeparam>
        /// <param name="source">要排序的数据集合</param>
        /// <param name="sortCondition">列表字段排序条件</param>
        /// <returns>返回排序后的集合</returns>
        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, SortCondition sortCondition)
        {
            source.CheckNotNull("source");
            sortCondition.CheckNotNull("sortCondition");

            return source.ThenBy(sortCondition.SortField, sortCondition.ListSortDirection);
        }

        #endregion
    }
}

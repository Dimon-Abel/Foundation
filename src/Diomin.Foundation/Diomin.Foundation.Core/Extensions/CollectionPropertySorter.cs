﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Diomin.Foundation.Core.Extensions
{
    /// <summary>
    /// 集合排序辅助类
    /// </summary>
    public static class CollectionPropertySorter<T>
    {
        private static readonly ConcurrentDictionary<string, LambdaExpression> Cache = new ConcurrentDictionary<string, LambdaExpression>();

        /// <summary>
        /// 按指定的属性名称对<see cref="IEnumerable{T}"/>序列进行排序
        /// </summary>
        /// <param name="source"><see cref="IEnumerable{T}"/>序列</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="sortDirection">排序方向</param>
        /// <returns></returns>
        public static IOrderedEnumerable<T> OrderBy(IEnumerable<T> source, string propertyName, ListSortDirection sortDirection)
        {
            propertyName.CheckNotNullOrEmpty("propertyName");
            dynamic expression = GetKeySelector(propertyName);
            dynamic keySelector = expression.Compile();
            return sortDirection == ListSortDirection.Ascending
                ? Enumerable.OrderBy(source, keySelector)
                : Enumerable.OrderByDescending(source, keySelector);
        }
        /// <summary>
        /// 按指定的属性名称对<see cref="IOrderedEnumerable{T}"/>进行继续排序
        /// </summary>
        /// <param name="source"><see cref="IOrderedEnumerable{T}"/>序列</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="sortDirection">排序方向</param>
        /// <returns>返回再次排序后的排序结果</returns>
        public static IOrderedEnumerable<T> ThenBy(IOrderedEnumerable<T> source, string propertyName, ListSortDirection sortDirection)
        {
            propertyName.CheckNotNullOrEmpty("propertyName");
            dynamic expression = GetKeySelector(propertyName);
            dynamic keySelector = expression.Compile();
            return sortDirection == ListSortDirection.Ascending
                ? Enumerable.ThenBy(source, keySelector)
                : Enumerable.ThenByDescending(source, keySelector);
        }
        /// <summary>
        /// 按指定的属性名称对<see cref="IQueryable{T}"/>序列进行排序
        /// </summary>
        /// <param name="source"><see cref="IQueryable{T}"/>序列</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="sortDirection">排序方向</param>
        /// <returns>返回排序后的排序结果</returns>
        public static IOrderedQueryable<T> OrderBy(IQueryable<T> source, string propertyName, ListSortDirection sortDirection)
        {
            propertyName.CheckNotNullOrEmpty("propertyName");
            dynamic keySelector = GetKeySelector(propertyName);
            return sortDirection == ListSortDirection.Ascending
                ? Queryable.OrderBy(source, keySelector)
                : Queryable.OrderByDescending(source, keySelector);
        }
        /// <summary>
        /// 按指定的属性名称对<see cref="IOrderedQueryable{T}"/>序列进行排序
        /// </summary>
        /// <param name="source"><see cref="IOrderedQueryable{T}"/>序列</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="sortDirection">排序方向</param>
        /// <returns>返回再次排序后的排序结果</returns>
        public static IOrderedQueryable<T> ThenBy(IOrderedQueryable<T> source, string propertyName, ListSortDirection sortDirection)
        {
            propertyName.CheckNotNullOrEmpty("propertyName");
            dynamic keySelector = GetKeySelector(propertyName);
            return sortDirection == ListSortDirection.Ascending
                ? Queryable.ThenBy(source, keySelector)
                : Queryable.ThenByDescending(source, keySelector);
        }
        private static LambdaExpression GetKeySelector(string keyName)
        {
            Type type = typeof(T);
            string key = type.FullName + "." + keyName;
            if (Cache.ContainsKey(key))
                return Cache[key];

            ParameterExpression param = Expression.Parameter(type);
            string[] propertyNames = keyName.Split('.');
            Expression propertyAccess = param;
            foreach (string propertyName in propertyNames)
            {
                PropertyInfo property = type.GetProperty(propertyName);
                if (property == null)
                {
                    throw new Exception(string.Format("指定对象中不存在名称为“{0}”的属性。", propertyName));
                }
                type = property.PropertyType;
                propertyAccess = Expression.MakeMemberAccess(propertyAccess, property);
            }
            LambdaExpression keySelector = Expression.Lambda(propertyAccess, param);
            Cache[key] = keySelector;
            return keySelector;
        }
    }
}

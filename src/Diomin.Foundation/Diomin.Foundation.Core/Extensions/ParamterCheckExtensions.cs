using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Diomin.Foundation.Core.Extensions
{
    /// <summary>
    /// 用于参数检查的扩展方法
    /// </summary>
    public static class ParamterCheckExtensions
    {
        /// <summary>
        /// 验证指定值的断言是否为真；如果不为真则抛出指定消息message的指定类型Texception的异常
        /// </summary>
        /// <typeparam name="TException">异常类型</typeparam>
        /// <param name="assertion">要验证的断言</param>
        /// <param name="message">异常消息</param>
        private static void Require<TException>(bool assertion, string message) where TException : Exception
        {
            if (assertion)
                return;
            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException("message");
            //创建指定类型（Texception）的对象实例
            TException exception = (TException)Activator.CreateInstance(typeof(TException), message);
            throw exception;
        }

        /// <summary>
        /// 验证指定值的断言表达式是否为真；如果不为真则抛出Exception异常
        /// </summary>
        /// <typeparam name="T">要判断的值的类型</typeparam>
        /// <param name="value"></param>
        /// <param name="assertionFunc">要验证的断言表达式</param>
        /// <param name="message">异常消息</param>
        public static void Required<T>(this T value, Func<T, bool> assertionFunc, string message)
        {
            if (assertionFunc == null)
            {
                throw new ArgumentException("assertionFunc");
            }
            Require<Exception>(assertionFunc(value), message);
        }

        /// <summary>
        /// 验证指定值的断言表达式是否为真，如果不为真则抛出<see cref="TException"/>异常
        /// </summary>
        /// <typeparam name="T">要判断的值的类型</typeparam>
        /// <typeparam name="TException">抛出的异常类型</typeparam>
        /// <param name="value">要判断的值</param>
        /// <param name="assertionFunc">要验证的断言表达式</param>
        /// <param name="message">异常消息</param>
        public static void Required<T, TException>(this T value, Func<T, bool> assertionFunc, string message) where TException : Exception
        {
            if (assertionFunc == null)
            {
                throw new ArgumentNullException("assertionFunc");
            }
            Require<TException>(assertionFunc(value), message);
        }

        /// <summary>
        /// 检查参数不能为空引用；否则抛出ArgumentNullException异常
        /// </summary>
        /// <typeparam name="T">要检查的对象类型</typeparam>
        /// <param name="value">要检查的对象值</param>
        /// <param name="paramName">参数名称</param>
        public static void CheckNotNull<T>(this T value, string paramName) where T : class
        {
            Require<ArgumentNullException>(value != null, string.Format("参数“{0}”不能为空引用！", paramName));
        }

        /// <summary>
        /// 检查参数不能为空引用、空字符串、空格；否则抛出ArgumentNullException异常或ArgumentException异常
        /// </summary>
        /// <param name="value">字符串的值</param>
        /// <param name="paramName">参数名称</param>
        public static void CheckNotNullOrEmpty(this string value, string paramName)
        {
            value.CheckNotNull(paramName);
            Require<ArgumentException>(value.Trim().Length > 0, string.Format("参数“{0}”不能为空引用、空字符串、空格！", paramName));
        }

        /// <summary>
        /// 检查Guid的值不能为Guid.Empty；否则抛出ArgumentException异常
        /// </summary>
        /// <param name="value">Guid类型的值</param>
        /// <param name="paramName">参数名称</param>
        public static void CheckNotEmpty(this Guid value, string paramName)
        {
            Require<ArgumentException>(value != Guid.Empty, string.Format("参数“{0}”的值不能为Guid.Empty ！", paramName));
        }

        /// <summary>
        /// 检查集合不能为空引用或空集合；否则抛出ArgumentNullException异常或ArgumentException异常
        /// </summary>
        /// <typeparam name="T">集合类型</typeparam>
        /// <param name="collection">检查的集合</param>
        /// <param name="paramName">参数名称</param>
        public static void CheckNotNullOrEmpty<T>(this IEnumerable<T> collection, string paramName)
        {
            collection.CheckNotNull(paramName);
            Require<ArgumentException>(collection.Any(), string.Format("参数“{0}”不能为空引用或空集合！", paramName));
        }

        /// <summary>
        /// 检查参数是否小于或等于指定值；否则抛出ArgumentOutOfRangeException异常
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="value">当前值</param>
        /// <param name="paramName">参数名称</param>
        /// <param name="target">要比较的值</param>
        /// <param name="canEqual">是否可以等于</param>
        public static void CheckLessThan<T>(this T value, string paramName, T target, bool canEqual = false) where T : IComparable<T>
        {
            bool flag = canEqual ? value.CompareTo(target) <= 0 : value.CompareTo(target) < 0;
            string format = canEqual ? "参数“{0}”的值必须小于或等于“{1}”！" : "参数“{0}”的值必须小于“{1}”！";
            Require<ArgumentOutOfRangeException>(flag, string.Format(format, paramName, target));
        }

        /// <summary>
        /// 检查参数是否大于或等于指定值；否则抛出ArgumentOutOfRangeException异常
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="value">当前值</param>
        /// <param name="paramName">参数名称</param>
        /// <param name="target">要比较的值</param>
        /// <param name="canEqual">是否可以等于</param>
        public static void CheckGreaterThan<T>(this T value, string paramName, T target, bool canEqual = false) where T : IComparable<T>
        {
            bool flag = canEqual ? value.CompareTo(target) >= 0 : value.CompareTo(target) > 0;
            string format = canEqual ? "参数“{0}”的值必须大于或等于“{1}”！" : "参数“{0}”的值必须大于“{1}”！";
            Require<ArgumentOutOfRangeException>(flag, string.Format(format, paramName, target));
        }

        /// <summary>
        /// 检查参数是否在指定的范围之间；否则抛出ArgumentOutOfRangeException异常
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="value">当前值</param>
        /// <param name="paramName">参数名称</param>
        /// <param name="start">起始值</param>
        /// <param name="end">结束值</param>
        /// <param name="startEqual">是否可以等于起始值</param>
        /// <param name="endEqual">是否可以等于结束值</param>
        public static void CheckBetweenAnd<T>(this T value, string paramName, T start, T end, bool startEqual = false, bool endEqual = false) where T : IComparable<T>
        {
            bool flag = startEqual ? value.CompareTo(start) >= 0 : value.CompareTo(start) > 0;
            string message = startEqual
                ? string.Format("参数“{0}”的值必须在“{1}”与“{2}”之间，且不能等于“{3}”！", paramName, start, end, start)
                : string.Format("参数“{0}”的值必须在“{1}”与“{2}”之间！", paramName, start, end);
            Require<ArgumentOutOfRangeException>(flag, message);

            flag = endEqual ? value.CompareTo(end) <= 0 : value.CompareTo(end) < 0;
            message = endEqual
                ? string.Format("参数“{0}”的值必须在“{1}”与“{2}”之间，且不能等于“{3}”！", paramName, start, end, end)
                : string.Format("参数“{0}”的值必须在“{1}”与“{2}”之间。", paramName, start, end);
            Require<ArgumentOutOfRangeException>(flag, message);
        }

        /// <summary>
        /// 检查指定文件夹目录路径是否存在；否则抛出ArgumentNullException异常或DirectoryNotFoundException异常
        /// </summary>
        /// <param name="directory">文本夹目录路径</param>
        /// <param name="paramName">参数名称</param>
        public static void CheckDirectoryExists(this string directory, string paramName = null)
        {
            CheckNotNull(directory, paramName);
            Require<DirectoryNotFoundException>(Directory.Exists(directory), string.Format("指定的目录路径“{0}”不存在！", directory));
        }

        /// <summary>
        /// 检查指定文件目录路径是否存在；否则抛出ArgumentNullException异常或FileNotFoundException异常
        /// </summary>
        /// <param name="filename">文件目录路径</param>
        /// <param name="paramName"></param>
        public static void CheckFileExists(this string filename, string paramName = null)
        {
            CheckNotNull(filename, paramName);
            Require<FileNotFoundException>(File.Exists(filename), string.Format("指定的文件路径“{0}”不存在。", filename));
        }
    }
}

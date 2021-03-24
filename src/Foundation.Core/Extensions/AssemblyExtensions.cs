using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Foundation.Core.Extensions
{
    /// <summary>
    /// 程序集扩展操作类
    /// </summary>
    public static class AssemblyExtensions
    {
        /// <summary>
        /// 获取程序集的文件版本
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static Version GetFileVersion(this Assembly assembly)
        {
            assembly.CheckNotNull("assembly");
            FileVersionInfo info = FileVersionInfo.GetVersionInfo(assembly.Location);
            return new Version(info.FileVersion);
        }
        /// <summary>
        /// 获取程序集的产品版本
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static Version GetProductVersion(this Assembly assembly)
        {
            assembly.CheckNotNull("assembly");
            FileVersionInfo info = FileVersionInfo.GetVersionInfo(assembly.Location);
            return new Version(info.ProductVersion);
        }
    }
}

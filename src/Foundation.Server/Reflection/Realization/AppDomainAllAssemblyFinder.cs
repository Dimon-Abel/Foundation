using Foundation.Core.Extensions;
using Foundation.Server.Reflection.Base;
using Foundation.Server.Reflection.Interface;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Foundation.Server.Reflection.Realization
{
    /// <summary>
    ///     应用程序目录程序集查找器
    /// </summary>
    public class AppDomainAllAssemblyFinder : FinderBase<Assembly>, IAllAssemblyFinder
    {
        private readonly bool _filterNetAssembly;

        /// <summary>
        ///     初始化一个<see cref="AppDomainAllAssemblyFinder" />类型的新实例
        /// </summary>
        public AppDomainAllAssemblyFinder(bool filterNetAssembly = true)
        {
            _filterNetAssembly = filterNetAssembly;
        }

        /// <summary>
        ///     重写以实现程序集的查找
        /// </summary>
        /// <returns></returns>
        protected override Assembly[] FindAllItems()
        {
            List<Assembly> assemblies = new List<Assembly>();

            //排除的程序集
            string[] filters =
            {
                "System",
                "Microsoft",
                "netstandard",
                "dotnet",
                "Window",
                "mscorlib"
            };
            // 查找目标项
            string[] findTargets =
            {
                "subscribe.dll",
            };

            //遍历文件夹的方式，用于传统.netfx
            var path = AppDomain.CurrentDomain.BaseDirectory;

            var files = Directory.GetFiles(path, "*.dll", SearchOption.TopDirectoryOnly)
                .Concat(Directory.GetFiles(path, "*.exe", SearchOption.TopDirectoryOnly))
                .ToArray();

            files = files.Where(x => findTargets.Any(t => x.ToLower().EndsWith(t))).ToArray();

            if (_filterNetAssembly)
            {
                var files1 = files;
                files = files.WhereIf(m => files1.Any(n => m.StartsWith(n, StringComparison.OrdinalIgnoreCase)),
                    _filterNetAssembly).ToArray();
            }

            Assembly[] pathAssemblies = files.Select(Assembly.LoadFrom).ToArray();

            var context = DependencyContext.Default;

            var names = new List<string>();
            var dllNames = context.CompileLibraries.SelectMany(m => m.Assemblies).Distinct()
                .Select(m => m.Replace(".dll", "")).ToArray();
            if (dllNames.Length > 0)
                names = (from name in dllNames
                         let i = name.LastIndexOf('/') + 1
                         select name.Substring(i, name.Length - i)).Distinct()
                    .WhereIf(name => !filters.Any(name.StartsWith), _filterNetAssembly)
                    .ToList();
            else
                foreach (var library in context.CompileLibraries)
                {
                    var name = library.Name;
                    if (_filterNetAssembly && filters.Any(name.StartsWith))
                        continue;
                    if (!names.Contains(name))
                        names.Add(name);
                }

            if (pathAssemblies.Any())
            {
                foreach (var assembly in pathAssemblies)
                {
                    var name = assembly.FullName.Split(",")[0];
                    if (!names.Contains(name))
                        names.Add(name);
                }
            }

            Assembly[] contextAssemblies = LoadFiles(names);
            assemblies.AddRange(contextAssemblies);

            return contextAssemblies;
        }

        private static Assembly[] LoadFiles(IEnumerable<string> files)
        {
            var assemblies = new List<Assembly>();
            foreach (var file in files)
            {
                var name = new AssemblyName(file);
                try
                {
                    assemblies.Add(Assembly.Load(name));
                }
                catch (FileNotFoundException)
                {
                }
            }
            return assemblies.ToArray();
        }
    }
}
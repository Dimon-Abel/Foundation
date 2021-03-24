using Foundation.Core.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Core.Extensions
{
    /// <summary>
    /// 类型辅助扩展类
    /// </summary>
    public static class TypeExtensions
    {
        public static Type GetElementTypeEx(this Type type) => GetElementType(type);
        /// <summary>获取一个类型的元素类型</summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static Type GetElementType(Type type)
        {
            if (type.HasElementType) return type.GetElementType();

            if (type.As<IEnumerable>())
            {
                // 如果实现了IEnumerable<>接口，那么取泛型参数
                foreach (var item in type.GetInterfaces())
                {
                    if (item.IsGenericType && item.GetGenericTypeDefinition() == typeof(IEnumerable<>)) return item.GetGenericArguments()[0];
                }
                //// 通过索引器猜测元素类型
                //var pi = type.GetProperty("Item", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                //if (pi != null) return pi.PropertyType;
            }

            return null;
        }
        /// <summary>反射调用指定对象的方法</summary>
        /// <param name="target">要调用其方法的对象，如果要调用静态方法，则target是类型</param>
        /// <param name="method">方法</param>
        /// <param name="parameters">方法参数</param>
        /// <returns></returns>
        public static object Invoke(this object target, MethodBase method, params object[] parameters)
        {
            //if (target == null) throw new ArgumentNullException("target");
            if (method == null) throw new ArgumentNullException("method");
            if (!method.IsStatic && target == null) throw new ArgumentNullException("target");

            return method.Invoke(target, parameters);
        }

        /// <summary>反射创建指定类型的实例</summary>
        /// <param name="type">类型</param>
        /// <param name="parameters">参数数组</param>
        /// <returns></returns>
        public static object CreateInstance(this Type type, params object[] parameters)
        {
            try
            {
                if (parameters == null || parameters.Length == 0)
                    return Activator.CreateInstance(type, true);
                else
                    return Activator.CreateInstance(type, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);//("创建对象失败 type={0} parameters={1} {2}".F(type.FullName, parameters.GroupJoin(), ex.GetTrue()?.Message), ex);
            }
        }
        public static TypeCode GetTypeCode(this Type type) => Type.GetTypeCode(type);

        /// <summary>
        /// 判断当前类型是否可由指定类型派生
        /// </summary>
        public static bool IsDeriveClassFrom<TBaseType>(this Type type, bool canAbstract = false)
        {
            return IsDeriveClassFrom(type, typeof(TBaseType), canAbstract);
        }

        /// <summary>
        /// 判断当前类型是否可由指定类型派生
        /// </summary>
        public static bool IsDeriveClassFrom(this Type type, Type baseType, bool canAbstract = false)
        {
            Check.NotNull(type, nameof(type));
            Check.NotNull(baseType, nameof(baseType));

            return type.IsClass && (!canAbstract && !type.IsAbstract) && type.IsBaseOn(baseType);
        }

        #region 布尔类型

        /// <summary>
        /// 把布尔值转换为小写字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToLower(this bool value)
        {
            return value.ToString().ToLower();
        }
        #endregion

        #region 日期类型

        /// <summary>
        /// 当前时间是否周末
        /// </summary>
        /// <param name="dateTime">时间点</param>
        /// <returns></returns>
        public static bool IsWeekend(this DateTime dateTime)
        {
            DayOfWeek[] weeks = { DayOfWeek.Saturday, DayOfWeek.Sunday };
            return weeks.Contains(dateTime.DayOfWeek);
        }

        /// <summary>
        /// 当前时间是否工作日（周一到周五）
        /// </summary>
        /// <param name="dateTime">时间点</param>
        /// <returns></returns>
        public static bool IsWeekday(this DateTime dateTime)
        {
            DayOfWeek[] weeks = { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };
            return weeks.Contains(dateTime.DayOfWeek);
        }

        /// <summary>
        /// 获取时间相对唯一的字符串
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="milsec">是否使用毫秒</param>
        /// <returns></returns>
        public static string ToUniqueString(this DateTime dateTime, bool milsec = false)
        {
            int sedonds = dateTime.Hour * 3600 + dateTime.Minute * 60 + dateTime.Second;
            string value = string.Format("{0}{1}{2}", dateTime.ToString("yy"), dateTime.DayOfYear, sedonds);
            return milsec ? value + dateTime.ToString("fff") : value;
        }
        #endregion

        #region 枚举类型

        /// <summary>
        /// 获取枚举项上的<see cref="DescriptionAttribute"/>特性的文字描述
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToDescription(this Enum value)
        {
            Type type = value.GetType();
            MemberInfo member = type.GetMember(value.ToString()).FirstOrDefault();
            return member != null ? member.ToDescription() : value.ToString();
        }
        #endregion

        #region 对象类型
        /// <summary>
        /// 判断类型是否为Nullable类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullableType(this Type type)
        {
            return (type != null && type.IsGenericType) && (type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
        /// <summary>
        /// 由类型的Nullabled类型返回实际类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetNonNummableType(this Type type)
        {
            if (IsNullableType(type))
                return type.GetGenericArguments()[0];
            return type;
        }
        /// <summary>
        /// 通过类型转换器获取Nullable类型的基础类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetUnNullableType(this Type type)
        {
            if (IsNullableType(type))
                return new NullableConverter(type).UnderlyingType;
            return type;
        }
        /// <summary>
        /// 检查指定类型成员中是否存在指定的Attribute特性
        /// </summary>
        /// <typeparam name="T">特性类型</typeparam>
        /// <param name="memberInfo">要检查的类型成员</param>
        /// <param name="inherit">是否从继承中查找</param>
        /// <returns>是否存在特性</returns>
        public static bool HasAttribute<T>(this MemberInfo memberInfo, bool inherit = false) where T : Attribute
        {
            return memberInfo.IsDefined(typeof(T), inherit);
        }
        /// <summary>
        /// 从类型成员中获取指定的Attribute特性
        /// </summary>
        /// <typeparam name="T">特性类型</typeparam>
        /// <param name="memberInfo">要检查的类型成员</param>
        /// <param name="inherit">是否从继承中查找</param>
        /// <returns>存在返回第一个，不存在返回null</returns>
        public static T GetAttribute<T>(this MemberInfo memberInfo, bool inherit = false) where T : Attribute
        {
            var descripts = memberInfo.GetCustomAttributes(typeof(T), inherit);
            return descripts.FirstOrDefault() as T;
        }
        /// <summary>
        /// 从类型成员中获取指定的Attribute特性
        /// </summary>
        /// <typeparam name="T">特性类型</typeparam>
        /// <param name="memberInfo">类型成员</param>
        /// <param name="inherit">是否从继承中查找</param>
        /// <returns>返回所有指定Attribute特性的数组</returns>
        public static T[] GetAttributes<T>(this MemberInfo memberInfo, bool inherit = false) where T : Attribute
        {
            return memberInfo.GetCustomAttributes(typeof(T), inherit).Cast<T>().ToArray();
        }
        /// <summary>
        /// 获取对象类型的Description特性描述信息
        /// </summary>
        /// <param name="type">类型对象</param>
        /// <param name="inherit">是否从继承中查找</param>
        /// <returns>返回Description特性描述信息，如不存在则返回类型的全名</returns>
        public static string ToDescription(this Type type, bool inherit = false)
        {
            DescriptionAttribute desc = type.GetAttribute<DescriptionAttribute>(inherit);
            return desc == null ? type.FullName : desc.Description;
        }
        /// <summary>
        /// 获取类型中成员元数据的Description特性描述信息
        /// </summary>
        /// <param name="memberInfo">类型成员</param>
        /// <param name="inherit">是否从继承中查找</param>
        /// <returns>返回Description特性描述信息，如不存在则返回成员的全名</returns>
        public static string ToDescription(this MemberInfo memberInfo, bool inherit = false)
        {
            DescriptionAttribute desc = memberInfo.GetAttribute<DescriptionAttribute>(inherit);
            if (desc != null)
                return desc.Description;
            DisplayAttribute display = memberInfo.GetAttribute<DisplayAttribute>(inherit);
            if (display != null)
                return display.Name;
            return memberInfo.Name;
        }
        /// <summary>
        /// 判断类型是否为集合类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsEnumerable(this Type type)
        {
            if (type == typeof(string))
                return false;
            return typeof(IEnumerable).IsAssignableFrom(type);
        }
        /// <summary>
        /// 判断当前泛型类型是否可由指定类型的实例填充
        /// </summary>
        /// <param name="genericType">泛型类型</param>
        /// <param name="type">指定类型</param>
        /// <returns></returns>
        public static bool IsGenericAssignableFrom(this Type genericType, Type type)
        {
            genericType.CheckNotNull("genericType");
            type.CheckNotNull("type");
            if (!genericType.IsGenericType)
                throw new ArgumentException("该功能只支持泛型类型的调用，非泛型类型可使用 IsAssignableFrom 方法！");
            List<Type> allOthers = new List<Type> { type };
            if (genericType.IsInterface)
                allOthers.AddRange(type.GetInterfaces());
            foreach (var other in allOthers)
            {
                Type cur = other;
                while (cur != null)
                {
                    if (cur.IsGenericType)
                        cur = cur.GetGenericTypeDefinition();
                    if (cur.IsSubclassOf(genericType) || cur == genericType)
                        return true;
                    cur = cur.BaseType;
                }
            }
            return false;
        }
        /// <summary>
        /// 判断当前方法是否为异步方法
        /// </summary>
        /// <param name="methodInfo">类型的方法</param>
        /// <returns></returns>
        public static bool IsAsync(this MethodInfo methodInfo)
        {
            return methodInfo.ReturnType == typeof(Task) || methodInfo.ReturnType.IsGenericType && methodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Task<>);
        }
        /// <summary>
        /// 判断当前类型是指定基类的派生类
        /// </summary>
        /// <param name="type">当前类型</param>
        /// <param name="baseType">要判断的基类型</param>
        /// <returns></returns>
        //public static bool IsBaseOn(this Type type, Type baseType) 
        //{
        //    if (type.IsGenericTypeDefinition)
        //        return baseType.IsGenericAssignableFrom(type);
        //    return baseType.IsAssignableFrom(type);
        //}
        public static bool IsBaseOn(this Type type, Type baseType)
        {
            if (baseType.IsGenericTypeDefinition)
            {
                return baseType.IsGenericAssignableFrom(type);
            }
            return baseType.IsAssignableFrom(type);
        }
        /// <summary>
        /// 判断当前类型是指定基类的派生类
        /// </summary>
        /// <typeparam name="TBaseType">要判断的基类型</typeparam>
        /// <param name="type">当前类型</param>
        /// <returns></returns>
        public static bool IsBaseOn<TBaseType>(this Type type)
        {
            Type baseType = typeof(TBaseType);
            return type.IsBaseOn(baseType);
        }
        /// <summary>
        /// 获取类型的全名，附带所在类库
        /// </summary>
        public static string GetFullNameWithModule(this Type type)
        {
            return $"{type.FullName},{type.Module.Name.Replace(".dll", "").Replace(".exe", "")}";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetName(this Type type)
        {
            return type.Name;
        }
        /// <summary>
        /// 获取类型的Description特性描述信息
        /// </summary>
        /// <param name="type">类型对象</param>
        /// <param name="inherit">是否搜索类型的继承链以查找描述特性</param>
        /// <returns>返回Description特性描述信息，如不存在则返回类型的全名</returns>
        public static string GetDescription(this Type type, bool inherit = false)
        {
            DescriptionAttribute desc = type.GetAttribute<DescriptionAttribute>(inherit);
            return desc == null ? type.FullName : desc.Description;
        }
        /// <summary>
        /// 获取成员元数据的Description特性描述信息
        /// </summary>
        /// <param name="member">成员元数据对象</param>
        /// <param name="inherit">是否搜索成员的继承链以查找描述特性</param>
        /// <returns>返回Description特性描述信息，如不存在则返回成员的名称</returns>
        public static string GetDescription(this MemberInfo member, bool inherit = false)
        {
            DescriptionAttribute desc = member.GetAttribute<DescriptionAttribute>(inherit);
            if (desc != null)
            {
                return desc.Description;
            }
            DisplayNameAttribute displayName = member.GetAttribute<DisplayNameAttribute>(inherit);
            if (displayName != null)
            {
                return displayName.DisplayName;
            }
            DisplayAttribute display = member.GetAttribute<DisplayAttribute>(inherit);
            if (display != null)
            {
                return display.Name;
            }
            return member.Name;
        }
        #endregion

        #region 类型转换

        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <param name="destinationType">转换的目标类型</param>
        /// <returns></returns>
        public static object To(this object value, Type destinationType)
        {
            return To(value, destinationType, CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <param name="destinationType">转换的目标类型</param>
        /// <param name="culture">区域信息</param>
        /// <returns></returns>
        public static object To(object value, Type destinationType, CultureInfo culture)
        {
            if (value != null)
            {
                var sourceType = value.GetType();

                var destinationConverter = TypeDescriptor.GetConverter(destinationType);
                if (destinationConverter.CanConvertFrom(value.GetType()))
                    return destinationConverter.ConvertFrom(null, culture, value);

                var sourceConverter = TypeDescriptor.GetConverter(sourceType);
                if (sourceConverter.CanConvertTo(destinationType))
                    return sourceConverter.ConvertTo(null, culture, value, destinationType);

                if (destinationType.IsEnum && value is int i)
                    return Enum.ToObject(destinationType, i);

                if (!destinationType.IsInstanceOfType(value))
                    return Convert.ChangeType(value, destinationType, culture);
            }
            return value;
        }
        /// <summary>
        /// 类型转换
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="value">要转换的值</param>
        /// <returns></returns>
        public static T To<T>(this object value)
        {
            return (T)To(value, typeof(T));
        }

        #endregion
    }
}

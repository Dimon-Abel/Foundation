using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace Foundation.Core.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// 利用表达式树赋值对象
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <returns></returns>
        private static Func<TIn, TOut> GetFunc<TIn, TOut>()
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TIn), "p");
            List<MemberBinding> memberBindingList = new List<MemberBinding>();

            foreach (var item in typeof(TOut).GetProperties())
            {
                if (!item.CanWrite)
                    continue;

                MemberExpression property = Expression.Property(parameterExpression, typeof(TIn).GetProperty(item.Name));
                MemberBinding memberBinding = Expression.Bind(item, property);
                memberBindingList.Add(memberBinding);
            }

            MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(TOut)), memberBindingList.ToArray());
            Expression<Func<TIn, TOut>> lambda = Expression.Lambda<Func<TIn, TOut>>(memberInitExpression, new ParameterExpression[] { parameterExpression });

            return lambda.Compile();
        }
        /// <summary>
        /// 克隆
        /// </summary>
        /// <typeparam name="TIn">克隆对象</typeparam>
        /// <typeparam name="TOut">克隆目标对象</typeparam>
        /// <param name="inT">克隆实体对象</param>
        /// <returns></returns>
        public static TOut Clone<TIn, TOut>(this TIn inT)
        {
            return GetFunc<TIn, TOut>()(inT);
        }

        /// <summary>
        /// 将对象类型转换为指定类型
        /// </summary>
        /// <param name="value">当前Object对象</param>
        /// <param name="conversionType">需要转换的类型</param>
        /// <returns>返回转换后的Object（目标类型）</returns>
        public static object CastTo(this object value, Type conversionType)
        {
            if (value == null)
                return null;
            if (conversionType.IsNullableType())
                conversionType = conversionType.GetUnNullableType();
            if (conversionType.IsEnum)
                return Enum.Parse(conversionType, value.ToString());
            if (conversionType == typeof(Guid))
                return Guid.Parse(value.ToString());
            return Convert.ChangeType(value, conversionType);
        }
        /// <summary>
        /// 将对象转换为指定类型
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="value">要转换的对象</param>
        /// <returns>返回转换后的目标类型</returns>
        public static T CastTo<T>(this object value)
        {
            if (value == null && default(T) == null)
            {
                return default(T);
            }
            if (value.GetType() == typeof(T))
            {
                return (T)value;
            }
            object result = CastTo(value, typeof(T));
            return (T)result;
        }
        /// <summary>
        /// 将对象类型转换为指定类型，如转换失败返回指定的默认值
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="value">要转换的对象</param>
        /// <param name="defaultValue">转换失败返回指定的默认值</param>
        /// <returns></returns>
        public static T CastTo<T>(this object value, T defaultValue)
        {
            try
            {
                return CastTo<T>(value);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }
        /// <summary>
        /// 判断当前值是否介于指定范围内
        /// </summary>
        /// <typeparam name="T">动态类型</typeparam>
        /// <param name="value">要判断的类型值</param>
        /// <param name="start">范围起点</param>
        /// <param name="end">范围终点</param>
        /// <param name="leftEqual">是否可等于上限（默认等于）</param>
        /// <param name="rightEqual">是否可等于下限（默认等于）</param>
        /// <returns></returns>
        public static bool IsBetween<T>(this IComparable<T> value, T start, T end, bool leftEqual = false, bool rightEqual = false) where T : IComparable
        {
            bool flag = leftEqual ? value.CompareTo(start) >= 0 : value.CompareTo(start) > 0;
            bool flag2 = rightEqual ? value.CompareTo(end) <= 0 : value.CompareTo(end) < 0;
            return flag && flag2;
        }
        /// <summary>
        /// 将对象（主要是匿名对象）转换为dynamic
        /// </summary>
        /// <param name="value">对象值</param>
        /// <returns>返回dynamic动态类型</returns>
        public static dynamic ToDynamic(this object value)
        {
            IDictionary<string, object> expendo = new ExpandoObject();
            Type type = value.GetType();
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(type);
            foreach (PropertyDescriptor property in properties)
            {
                var val = property.GetValue(value);
                if (property.PropertyType.FullName.StartsWith("<>f__AnonymousType"))
                {
                    dynamic dval = val.ToDynamic();
                    expendo.Add(property.Name, dval);
                }
                else
                {
                    expendo.Add(property.Name, val);
                }
            }
            return expendo as ExpandoObject;
        }

        /// <summary>
        /// 【高效】将当前对象转为byte[]字节
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this object value)
        {
            if (value == null) return null;
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, value);
            byte[] buffer = ms.GetBuffer();
            ms.Dispose();
            return buffer;
            //下面方法对字典会报错
            //byte[] buff = new byte[Marshal.SizeOf(value)];
            //IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(buff, 0);
            //Marshal.StructureToPtr(value, ptr, true);
            //return buff;
        }
        /// <summary>
        /// 【高效】将当前字节转换为<typeparamref name="T"/>对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ToObject<T>(this byte[] value)
        {
            try
            {
                if (value == null || value.Length == 0)
                    return default(T);

                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream m2 = new MemoryStream(value);
                T t = (T)bf.Deserialize(m2);
                m2.Dispose();
                return t;
                //IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(value, 0);
                //return Marshal.PtrToStructure<T>(ptr);
            }
            catch (Exception)
            {
                return default(T);
            }
        }
        /// <summary>
        /// 对比两个byte数据是否相等
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool EqualByte(this byte[] source, byte[] target)
        {
            return StructuralComparisons.StructuralEqualityComparer.Equals(source, target);   //一样的数据但返回的还是false 和Enumerable.SequenceEqual() 结果一样

            ////这种方法性能较差
            //if (source.Length != target.Length)
            //    return false;
            //for (int i = 0; i < source.Length; i++)
            //    if (source[i] != target[i])
            //        return false;
            //return true;
        }
        /// <summary>获取成员绑定的显示名，优先DisplayName，然后Description</summary>
        /// <param name="member"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static String GetDisplayName(this MemberInfo member, bool inherit = true)
        {
            var att = member.GetCustomAttribute<DisplayNameAttribute>(inherit);
            if (att != null && !att.DisplayName.IsNullOrWhiteSpace()) return att.DisplayName;

            return null;
        }
        /// <summary>获取自定义属性的值。可用于ReflectionOnly加载的程序集</summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="target">目标对象</param>
        /// <param name="inherit">是否递归</param>
        /// <returns></returns>
        public static TResult GetCustomAttributeValue<TAttribute, TResult>(this MemberInfo target, bool inherit = true) where TAttribute : Attribute
        {
            if (target == null) return default(TResult);

            try
            {
                var list = CustomAttributeData.GetCustomAttributes(target);
                if (list != null && list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        if (typeof(TAttribute).FullName != item.Constructor.DeclaringType.FullName) continue;

                        var args = item.ConstructorArguments;
                        if (args != null && args.Count > 0) return (TResult)args[0].Value;
                    }
                }
                if (inherit && target is Type)
                {
                    target = (target as Type).BaseType;
                    if (target != null && target != typeof(Object))
                        return GetCustomAttributeValue<TAttribute, TResult>(target, inherit);
                }
            }
            catch
            {
                // 出错以后，如果不是仅反射加载，可以考虑正面来一次
                if (!target.Module.Assembly.ReflectionOnly)
                {
                    //var att = GetCustomAttribute<TAttribute>(target, inherit);
                    var att = target.GetCustomAttribute<TAttribute>(inherit);
                    if (att != null)
                    {
                        var pi = typeof(TAttribute).GetProperties().FirstOrDefault(p => p.PropertyType == typeof(TResult));
                        if (pi != null) return (TResult)att.GetValue(pi);
                    }
                }
            }

            return default(TResult);
        }

        /// <summary>获取目标对象的成员值</summary>
        /// <param name="target">目标对象</param>
        /// <param name="member">成员</param>
        /// <returns></returns>
        public static object GetValue(this object target, MemberInfo member)
        {
            // 有可能跟普通的 PropertyInfo.GetValue(Object target) 搞混了
            if (member == null)
            {
                member = target as MemberInfo;
                target = null;
            }

            if (member is PropertyInfo)
                return target.GetValue(member as PropertyInfo);
            else if (member is FieldInfo)
                return target.GetValue(member as FieldInfo);
            else
                throw new ArgumentOutOfRangeException("member");
        }
        public static void SetValue(this Object target, MemberInfo member, Object value)
        {
            if (member is PropertyInfo)
                target.SetValue(member as PropertyInfo, value);
            else if (member is FieldInfo)
                target.SetValue(member as FieldInfo, value);
            else
                throw new ArgumentOutOfRangeException("member");
        }
        public static object GetValue(this object target, PropertyInfo property) => property.GetValue(target, null);
        public static object GetValue(this object target, FieldInfo field) => field.GetValue(target);
        /// <summary>设置目标对象的属性值</summary>
        /// <param name="target">目标对象</param>
        /// <param name="property">属性</param>
        /// <param name="value">数值</param>
        public static void SetValue(this object target, PropertyInfo property, Object value) => property.SetValue(target, value.ChangeType(property.PropertyType), null);

        /// <summary>设置目标对象的字段值</summary>
        /// <param name="target">目标对象</param>
        /// <param name="field">字段</param>
        /// <param name="value">数值</param>
        public static void SetValue(this object target, FieldInfo field, Object value) => field.SetValue(target, value.ChangeType(field.FieldType));
        public static bool As<T>(this Type type) => type.As(typeof(T));

        public static bool As(this Type type, Type baseType)
        {
            if (type == null) return false;
            if (type == baseType) return true;

            // 如果基类是泛型定义，补充完整，例如IList<>
            if (baseType.IsGenericTypeDefinition
                && type.IsGenericType && !type.IsGenericTypeDefinition
                && baseType is TypeInfo inf && inf.GenericTypeParameters.Length == type.GenericTypeArguments.Length)
                baseType = baseType.MakeGenericType(type.GenericTypeArguments);

            if (type == baseType) return true;

            if (baseType.IsAssignableFrom(type)) return true;
            var rs = false;

            return rs;
        }
        private static DateTime _dt1970 = new DateTime(1970, 1, 1);

        /// <summary>转为布尔型。支持大小写True/False、0和非零</summary>
        /// <param name="value">待转换对象</param>
        /// <param name="defaultValue">默认值。待转换对象无效时使用</param>
        /// <returns></returns>
        public static bool ToBoolean(this object value, bool defaultValue = false)
        {
            if (value == null || value == DBNull.Value) return defaultValue;

            // 特殊处理字符串，也是最常见的
            if (value is string str)
            {
                str = str.Trim();
                if (str.IsNullOrEmpty()) return defaultValue;

                if (bool.TryParse(str, out var b)) return b;

                if (string.Equals(str, bool.TrueString, StringComparison.OrdinalIgnoreCase)) return true;
                if (string.Equals(str, bool.FalseString, StringComparison.OrdinalIgnoreCase)) return false;

                // 特殊处理用数字0和1表示布尔型
                str = ToDBC(str);
                if (int.TryParse(str, out var n)) return n > 0;

                return defaultValue;
            }

            try
            {
                return Convert.ToBoolean(value);
            }
            catch { return defaultValue; }
        }
        public static double ToDouble(this object value, double defaultValue = 0)
        {
            if (value == null || value == DBNull.Value) return defaultValue;

            // 特殊处理字符串，也是最常见的
            if (value is string str)
            {
                str = ToDBC(str).Trim();
                if (str.IsNullOrEmpty()) return defaultValue;

                if (double.TryParse(str, out var n)) return n;
                return defaultValue;
            }
            else if (value is byte[] buf)
            {
                if (buf == null || buf.Length < 1) return defaultValue;

                switch (buf.Length)
                {
                    case 1:
                        return buf[0];
                    case 2:
                        return BitConverter.ToInt16(buf, 0);
                    case 3:
                        return BitConverter.ToInt32(new byte[] { buf[0], buf[1], buf[2], 0 }, 0);
                    case 4:
                        return BitConverter.ToInt32(buf, 0);
                    default:
                        // 凑够8字节
                        if (buf.Length < 8)
                        {
                            var bts = new Byte[8];
                            Buffer.BlockCopy(buf, 0, bts, 0, buf.Length);
                            buf = bts;
                        }
                        return BitConverter.ToDouble(buf, 0);
                }
            }

            try
            {
                return Convert.ToDouble(value);
            }
            catch { return defaultValue; }
        }
        /// <summary>转为整数，转换失败时返回默认值。支持字符串、全角、字节数组（小端）、时间（Unix秒）</summary>
        /// <param name="value">待转换对象</param>
        /// <param name="defaultValue">默认值。待转换对象无效时使用</param>
        /// <returns></returns>
        public static int ToInt(this object value, int defaultValue = 0)
        {
            if (value == null || value == DBNull.Value) return defaultValue;

            // 特殊处理字符串，也是最常见的
            if (value is string str)
            {
                // 拷贝而来的逗号分隔整数
                str = str.Replace(",", null);
                str = ToDBC(str).Trim();
                if (str.IsNullOrEmpty()) return defaultValue;

                if (Int32.TryParse(str, out var n)) return n;
                return defaultValue;
            }

            // 特殊处理时间，转Unix秒
            if (value is DateTime dt)
            {
                if (dt == DateTime.MinValue) return 0;

                //// 先转UTC时间再相减，以得到绝对时间差
                //return (Int32)(dt.ToUniversalTime() - _dt1970).TotalSeconds;
                return (Int32)(dt - _dt1970).TotalSeconds;
            }

            if (value is Byte[] buf)
            {
                if (buf == null || buf.Length < 1) return defaultValue;

                switch (buf.Length)
                {
                    case 1:
                        return buf[0];
                    case 2:
                        return BitConverter.ToInt16(buf, 0);
                    case 3:
                        return BitConverter.ToInt32(new Byte[] { buf[0], buf[1], buf[2], 0 }, 0);
                    case 4:
                        return BitConverter.ToInt32(buf, 0);
                    default:
                        break;
                }
            }

            try
            {
                return Convert.ToInt32(value);
            }
            catch { return defaultValue; }
        }

        /// <summary>转为长整数。支持字符串、全角、字节数组（小端）、时间（Unix毫秒）</summary>
        /// <param name="value">待转换对象</param>
        /// <param name="defaultValue">默认值。待转换对象无效时使用</param>
        /// <returns></returns>
        public static long ToLong(this object value, long defaultValue = 0)
        {
            if (value == null || value == DBNull.Value) return defaultValue;

            // 特殊处理字符串，也是最常见的
            if (value is string str)
            {
                // 拷贝而来的逗号分隔整数
                str = str.Replace(",", null);
                str = ToDBC(str).Trim();
                if (str.IsNullOrEmpty()) return defaultValue;

                if (int.TryParse(str, out var n)) return n;
                return defaultValue;
            }

            // 特殊处理时间，转Unix毫秒
            if (value is DateTime dt)
            {
                if (dt == DateTime.MinValue) return 0;

                //// 先转UTC时间再相减，以得到绝对时间差
                //return (Int32)(dt.ToUniversalTime() - _dt1970).TotalSeconds;
                return (int)(dt - _dt1970).TotalMilliseconds;
            }

            if (value is byte[] buf)
            {
                if (buf == null || buf.Length < 1) return defaultValue;

                switch (buf.Length)
                {
                    case 1:
                        return buf[0];
                    case 2:
                        return BitConverter.ToInt16(buf, 0);
                    case 3:
                        return BitConverter.ToInt32(new Byte[] { buf[0], buf[1], buf[2], 0 }, 0);
                    case 4:
                        return BitConverter.ToInt32(buf, 0);
                    case 8:
                        return BitConverter.ToInt64(buf, 0);
                    default:
                        break;
                }
            }

            //暂时不做处理  先处理异常转换
            try
            {
                return Convert.ToInt64(value);
            }
            catch { return defaultValue; }
        }
        public static string ToDBC(this string str)
        {
            var ch = str.ToCharArray();
            for (var i = 0; i < ch.Length; i++)
            {
                // 全角空格
                if (ch[i] == 0x3000)
                    ch[i] = (char)0x20;
                else if (ch[i] > 0xFF00 && ch[i] < 0xFF5F)
                    ch[i] = (char)(ch[i] - 0xFEE0);
            }
            return new string(ch);
        }
        public static MethodInfo GetMethodEx(this Type type, String name, params Type[] paramTypes)
        {
            if (name.IsNullOrEmpty()) return null;

            // 如果其中一个类型参数为空，得用别的办法
            if (paramTypes.Length > 0 && paramTypes.Any(e => e == null))
                return GetMethods(type, name, paramTypes.Length).FirstOrDefault();

            return GetMethod(type, name, paramTypes);
        }
        /// <summary>获取方法</summary>
        /// <remarks>用于具有多个签名的同名方法的场合，不确定是否存在性能问题，不建议普通场合使用</remarks>
        /// <param name="type">类型</param>
        /// <param name="name">名称</param>
        /// <param name="paramTypes">参数类型数组</param>
        /// <returns></returns>
        public static MethodInfo GetMethod(Type type, String name, params Type[] paramTypes)
        {
            MethodInfo mi = null;
            while (true)
            {
                if (paramTypes == null || paramTypes.Length == 0)
                    mi = type.GetMethod(name, bf);
                else
                    mi = type.GetMethod(name, bf, null, paramTypes, null);
                if (mi != null) return mi;

                type = type.BaseType;
                if (type == null || type == typeof(Object)) break;
            }
            return null;
        }
        static readonly BindingFlags bf = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

        //public static BindingFlags Bfic { get; } = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.IgnoreCase;

        /// <summary>获取指定名称的方法集合，支持指定参数个数来匹配过滤</summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="paramCount">参数个数，-1表示不过滤参数个数</param>
        /// <returns></returns>
        public static MethodInfo[] GetMethods(Type type, String name, Int32 paramCount = -1)
        {
            var ms = type.GetMethods(bf);
            if (ms == null || ms.Length == 0) return ms;

            var list = new List<MethodInfo>();
            foreach (var item in ms)
            {
                if (item.Name == name)
                {
                    if (paramCount >= 0 && item.GetParameters().Length == paramCount) list.Add(item);
                }
            }
            return list.ToArray();
        }
    }
}

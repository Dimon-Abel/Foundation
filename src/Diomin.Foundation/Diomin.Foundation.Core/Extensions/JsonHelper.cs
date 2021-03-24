using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;

namespace Diomin.Foundation.Core.Extensions
{
    /// <summary>
    ///     Json 辅助类
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        ///     将对象转换为JSON字符串
        /// </summary>
        /// <param name="obj">要转换的对象</param>
        /// <param name="camelCase">是否小写名称</param>
        /// <param name="indented"></param>
        /// <returns></returns>
        public static string ToJsonString(this object obj, bool camelCase = false, bool indented = false)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            if (camelCase)
            {
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }

            if (indented)
            {
                settings.Formatting = Newtonsoft.Json.Formatting.Indented;
            }

            return JsonConvert.SerializeObject(obj, settings);
        }

        /// <summary>
        /// 根据键值对信息赋值实例化过的对象
        /// </summary>
        /// <param name="obj">需要赋值的对象</param>
        public static void SetValueByTarget(this object obj, Dictionary<string, object> attributes)
        {
            foreach (KeyValuePair<string, object> attribute in attributes)
            {
                PropertyInfo property = obj.GetType().GetProperty(attribute.Key, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                if (property != null && property.CanWrite && !attribute.Key.Contains("FlowElement") && !attribute.Key.Contains("incomingFlows") && !attribute.Key.Contains("outgoingFlows"))
                {
                    Type propertyType = property.PropertyType;
                    if (attribute.Value != null && attribute.Value.ToString() != "{}" && attribute.Value.ToString() != "undefined")
                    {
                        if (attribute.Key != "attributes")
                        {
                            object obj2 = attribute.Value.ConvertObject(propertyType);
                            if (propertyType.IsClass && !propertyType.FullName.Contains("System"))
                            {
                                Dictionary<string, object> attributes2 = attribute.Value.ToDictionary();
                                obj2.SetValueByTarget(attributes2);
                            }
                            if (obj2 != null)
                                property.SetValue(obj, obj2);
                        }
                    }
                    else if (attribute.Value == null)
                    {
                        property.SetValue(obj, null);
                    }
                }
            }
        }
        public static object ConvertObject(this object obj, Type type)
        {
            if (type == null)
            {
                return obj;
            }
            if (obj != null)
            {
                switch (obj.GetType().FullName)
                {
                    case "Newtonsoft.Json.Linq.JArray":
                        if (type.Name == typeof(Dictionary<string, object>).Name)
                            return GetObjecByType(obj, type);// obj.ToDictionary();
                        else
                            return JArray.FromObject(obj)?.ToObject(type);
                    case "Newtonsoft.Json.Linq.JObject":
                        if (type.Name == typeof(Dictionary<string, object>).Name)
                            return obj.ToDictionary();
                        else
                            return JObject.FromObject(obj)?.ToObject(type);
                    default:
                        {
                            return GetObjecByType(obj, type);
                        }
                }
            }
            if (!type.IsValueType)
            {
                return null;
            }
            return Activator.CreateInstance(type);
        }

        private static object GetObjecByType(object obj, Type type)
        {
            if (type == typeof(Dictionary<string, object>))
            {
                return obj.ToDictionary();
            }
            Type underlyingType = Nullable.GetUnderlyingType(type);
            if (type.IsAssignableFrom(obj.GetType()))
            {
                return obj;
            }
            if ((underlyingType ?? type).IsEnum)
            {
                if (underlyingType != null && string.IsNullOrEmpty(obj.ToString()))
                {
                    return null;
                }
                return Enum.Parse(underlyingType ?? type, obj.ToString());
            }
            if (typeof(IConvertible).IsAssignableFrom(underlyingType ?? type))
            {
                try
                {
                    return Convert.ChangeType(obj, underlyingType ?? type, null);
                }
                catch
                {
                    return (underlyingType == null) ? Activator.CreateInstance(type) : null;
                }
            }
            TypeConverter converter = TypeDescriptor.GetConverter(type);
            if (converter.CanConvertFrom(obj.GetType()))
            {
                return converter.ConvertFrom(obj);
            }
            ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
            if (constructor != null)
            {
                object obj3 = constructor.Invoke(null);
                PropertyInfo[] properties = type.GetProperties();
                Type type2 = obj.GetType();
                PropertyInfo[] array = properties;
                foreach (PropertyInfo propertyInfo in array)
                {
                    if (propertyInfo.Name == "Item" && type2.FullName == "Newtonsoft.Json.Linq.JArray")
                    {
                        JArray.Parse(obj.ToString());
                    }
                    PropertyInfo property = type2.GetProperty(propertyInfo.Name, propertyInfo.PropertyType);
                    if (propertyInfo.CanWrite && property != null && property.CanRead)
                    {
                        object value = property.GetValue(obj, null).ConvertObject(propertyInfo.PropertyType);
                        propertyInfo.SetValue(obj3, value, null);
                    }
                }
                return obj3;
            }
            return obj;
        }

        public static object CreateObjectNoCache(string AssemblyPath, string ClassNamespace)
        {
            try
            {
                Assembly assm = Assembly.Load(AssemblyPath);//第一步：通过程序集名称加载程序集
                object objType = assm.CreateInstance(ClassNamespace);// 第二步：通过命名空间+类名创建类的实例。
                return objType;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     处理Json的时间格式为正常格式
        /// </summary>
        public static string JsonDateTimeFormat(this string json)
        {
            json.CheckNotNullOrEmpty("json");
            json = Regex.Replace(json,
                @"\\/Date\((\d+)\)\\/",
                match =>
                {
                    DateTime dt = new DateTime(1970, 1, 1);
                    dt = dt.AddMilliseconds(long.Parse(match.Groups[1].Value));
                    dt = dt.ToLocalTime();
                    return dt.ToString("yyyy-MM-dd HH:mm:ss.fff");
                });
            return json;
        }

        /// <summary>
        ///     把对象序列化成Json字符串格式
        /// </summary>
        /// <param name="object">Json 对象</param>
        /// <param name="useCamelCaseResolver">是否使用驼峰样式(首字母小写)输出对象</param>
        /// <returns>Json 字符串</returns>
        public static string ToJson(this object @object, bool useCamelCaseResolver = false)
        {
            if (@object == null)
                return null;
            JsonSerializerSettings jsonSetting = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore, // 忽略循环引用配置
                DateFormatString = "yyyy-MM-dd HH:mm:ss.FFFF"
            };
            if (useCamelCaseResolver)
            {
                jsonSetting.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }
            string json = JsonConvert.SerializeObject(@object, jsonSetting);
            return JsonDateTimeFormat(json);
        }

        /// <summary>
        /// 添加字符串转义字符
        /// 如  "财务部"  转义为 "\"财务部\""
        /// </summary>
        /// <returns></returns>
        public static string ToEscape(this string @str)
        {
            return $"\"{@str}\"";
        }

        /// <summary>
        ///     把Json字符串转换为强类型对象
        /// </summary>
        public static T FromJson<T>(this string json)
        {
            try
            {
                json.CheckNotNullOrEmpty("json");
                json = JsonDateTimeFormat(json);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        /// <summary>
        ///     把XmlDocument对象序列化为json字符串
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static string ToJsonForXml(this XmlDocument xml)
        {
            xml.CheckNotNull("xml");
            return JsonConvert.SerializeXmlNode(xml);
        }

        /// <summary>
        ///     实体转键值对
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="t">需要转换的实体值</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetProperties<T>(this T t)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            if (t == null)
                return null;
            var properties = t.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            if (properties.Length <= 0)
                return null;
            foreach (var item in properties)
            {
                string name = item.Name; //实体类字段名称
                string value = item.GetValue(t, null) == null ? "" : item.GetValue(t, null).ToString(); //该字段的值

                if (item.PropertyType.IsValueType || item.PropertyType.Name.StartsWith("String"))
                {
                    ret.Add(name, value); //在此可转换value的类型
                }
            }

            return ret;
        }

        /// <summary>
        /// 对象转换为字典
        /// </summary>
        /// <param name="obj">待转化的对象</param>
        /// <param name="isIgnoreNull">是否忽略NULL 这里我不需要转化NULL的值，正常使用可以不穿参数 默认全转换</param>
        /// <returns></returns>
        public static Dictionary<string, object> ObjectToMap(this object obj, bool isIgnoreNull = false)
        {
            Dictionary<string, object> map = new Dictionary<string, object>();

            Type t = obj.GetType(); // 获取对象对应的类， 对应的类型

            PropertyInfo[] pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance); // 获取当前type公共属性

            foreach (PropertyInfo p in pi)
            {
                MethodInfo m = p.GetGetMethod();

                if (m != null && m.IsPublic)
                {
                    // 进行判NULL处理 
                    if (m.Invoke(obj, new object[] { }) != null || !isIgnoreNull)
                    {
                        map.Add(p.Name, m.Invoke(obj, new object[] { })); // 向字典添加元素
                    }
                }
            }
            return map;
        }

        public static Dictionary<string, object> ToDictionary(this object source)
        {
            return source.ToDictionary<object>();
        }
        /// <summary>
        /// 将json字符串反序列化为字典
        /// </summary>
        /// <param name="JsonStr">需要反序列化的json字符串</param>
        /// <returns></returns>
        public static Dictionary<string, object> ToDic(this string JsonStr)
        {
            var dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonStr);
            return dic;
        }
        public static Dictionary<string, string> ToDics(this string JsonStr)
        {
            var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonStr);
            return dic;
        }
        public static Dictionary<string, T> GetDicByKey<T>(this Dictionary<string, T> dic, string key)
        {
            return dic[key].ToDictionary<T>();
        }
        public static Dictionary<string, T> ToDictionary<T>(this object source)
        {
            if (source == null)
            {
                ThrowExceptionWhenSourceArgumentIsNull();
            }

            Dictionary<string, T> dictionary = new Dictionary<string, T>();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(source))
            {
                AddPropertyToDictionary<T>(property, source, dictionary);
            }

            return dictionary;
        }

        private static void AddPropertyToDictionary<T>(PropertyDescriptor property, object source, Dictionary<string, T> dictionary)
        {
            object value = property.GetValue(source);
            if (IsOfType<T>(value))
            {
                dictionary.Add(property.Name, (T)value);
            }
        }

        private static bool IsOfType<T>(object value)
        {
            return value is T;
        }

        private static void ThrowExceptionWhenSourceArgumentIsNull()
        {
            throw new ArgumentNullException("source", "无法将对象转换为字典。源对象为空。");
        }

        /// <summary>
        /// 修改对象为指定类型的值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object ChangeType(this object value, string type)
        {
            switch (type.ToLower())
            {
                case "bool":
                case "boolean":
                    return value == null ? false : (bool)Convert.ChangeType(value, typeof(bool));
                case "decimal":
                    return value == null ? 0 : (decimal)Convert.ChangeType(value, typeof(decimal));
                case "double":
                    return value == null ? 0 : (double)Convert.ChangeType(value, typeof(double));
                case "float":
                    return value == null ? 0 : (float)Convert.ChangeType(value, typeof(float));
                case "int":
                    return value == null ? 0 : (int)Convert.ChangeType(value, typeof(int));
                case "long":
                    return value == null ? 0 : (long)Convert.ChangeType(value, typeof(long));
                case "short":
                    return value == null ? 0 : (short)Convert.ChangeType(value, typeof(short));
                case "string":
                    return value == null ? "" : (string)Convert.ChangeType(value, typeof(string));
                case "date":
                case "datetime":
                    return value == null ? DateTime.Now : (DateTime)Convert.ChangeType(value, typeof(DateTime));
                case "array":
                    return value == null ? null : (value as string[]) ?? (value as JArray)?.ToObject<string[]>() ?? new string[] { value.ToString() };
                default:
                    return value.ToString().Split(",");
            }
        }
        public static Type GetTypeByString(this string strType)
        {
            try
            {
                switch (strType.ToLower())
                {
                    case "bool":
                        return Type.GetType("System.bool", true, true);
                    case "byte":
                        return Type.GetType("System.Byte", true, true);
                    case "sbyte":
                        return Type.GetType("System.SByte", true, true);
                    case "char":
                        return Type.GetType("System.Char", true, true);
                    case "decimal":
                        return Type.GetType("System.Decimal", true, true);
                    case "double":
                        return Type.GetType("System.Double", true, true);
                    case "float":
                        return Type.GetType("System.Single", true, true);
                    case "int":
                        return Type.GetType("System.Int32", true, true);
                    case "uint":
                        return Type.GetType("System.UInt32", true, true);
                    case "long":
                        return Type.GetType("System.Int64", true, true);
                    case "ulong":
                        return Type.GetType("System.UInt64", true, true);
                    case "object":
                        return Type.GetType("System.object", true, true);
                    case "short":
                        return Type.GetType("System.Int16", true, true);
                    case "ushort":
                        return Type.GetType("System.UInt16", true, true);
                    case "string":
                        return Type.GetType("System.String", true, true);
                    case "date":
                    case "datetime":
                        return Type.GetType("System.DateTime", true, true);
                    case "guid":
                        return Type.GetType("System.Guid", true, true);
                    case "array":
                        return Type.GetType("System.Array", true, true); // 补充array
                    default:
                        return Type.GetType(strType, true, true);
                }
            }
            catch
            {
                return Type.GetType(strType, true, true);
            }
        }

        /// <summary>
        /// 修改对象为指定类型的值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object ChangeType(this object value, Type type)
        {
            if (value == null && type.IsGenericType) return Activator.CreateInstance(type);
            if (value == null) return null;
            if (type == value.GetType()) return value;
            if (type.IsEnum)
            {
                if (value is string)
                    return Enum.Parse(type, value as string);
                else
                    return Enum.ToObject(type, value);
            }
            if (!type.IsInterface && type.IsGenericType)
            {
                Type innerType = type.GetGenericArguments()[0];
                object innerValue = ChangeType(value, innerType);
                return Activator.CreateInstance(type, new object[] { innerValue });
            }
            if (value is string && type == typeof(Guid)) return new Guid(value as string);
            if (value is string && type == typeof(Version)) return new Version(value as string);
            if (!(value is IConvertible)) return value;
            return Convert.ChangeType(value, type);
        }
    }
}
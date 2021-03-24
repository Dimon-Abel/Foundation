using System;
using System.Collections.Generic;
using System.Text;

namespace Foundation.Core.Data
{
    /// <summary>
    ///     由Guid与DateTime混合构成的可排序字符串操作类
    /// </summary>
    public static class XGuidHelper
    {
        /// <summary>
        ///     生成用于数据库操作的Guid字符串，可提高检索效率
        /// </summary>
        /// <returns>返回包含了时间信息的唯一码</returns>
        public static Guid NewXGuid()
        {
            var guidArray = Guid.NewGuid().ToByteArray();
            var dtBase = new DateTime(1900, 1, 1);
            var dtNow = DateTime.Now;
            //获取用于生成byte字符串的天数与毫秒数
            var days = new TimeSpan(dtNow.Ticks - dtBase.Ticks);
            var msecs = new TimeSpan(dtNow.Ticks - new DateTime(dtNow.Year, dtNow.Month, dtNow.Day).Ticks);
            //转换成byte数组
            //注意SqlServer的时间计数只能精确到1/300秒
            var daysArray = BitConverter.GetBytes(days.Days);
            var msecsArray = BitConverter.GetBytes((long)(msecs.TotalMilliseconds / 3.333333));

            //反转字节以符合SqlServer的排序
            Array.Reverse(daysArray);
            Array.Reverse(msecsArray);

            //把字节复制到Guid中
            Array.Copy(daysArray, daysArray.Length - 2, guidArray, guidArray.Length - 6, 2);
            Array.Copy(msecsArray, msecsArray.Length - 4, guidArray, guidArray.Length - 4, 4);
            return new Guid(guidArray);
        }

        /// <summary>
        ///     获取XGuid中生成的时间信息
        /// </summary>
        /// <param name="id">XGuid</param>
        /// <returns>返回时间</returns>
        public static DateTime GetDateFromXGuid(Guid id)
        {
            var baseDate = new DateTime(1900, 1, 1);
            var daysArray = new byte[4];
            var msecsArray = new byte[4];
            var guidArray = id.ToByteArray();

            // Copy the date parts of the guid to the respective byte arrays. 
            Array.Copy(guidArray, guidArray.Length - 6, daysArray, 2, 2);
            Array.Copy(guidArray, guidArray.Length - 4, msecsArray, 0, 4);

            // Reverse the arrays to put them into the appropriate order 
            Array.Reverse(daysArray);
            Array.Reverse(msecsArray);

            // Convert the bytes to ints 
            var days = BitConverter.ToInt32(daysArray, 0);
            var msecs = BitConverter.ToInt32(msecsArray, 0);

            var date = baseDate.AddDays(days);
            date = date.AddMilliseconds(msecs * 3.333333);

            return date;
        }

        /// <summary>
        ///     根据Guid生成一个16位长度的唯一字符串
        /// </summary>
        /// <returns></returns>
        public static string GetXGuid()
        {
            long i = 1;
            foreach (var bt in Guid.NewGuid().ToByteArray())
                i *= bt + 1;
            return string.Format("{0:x}", i - DateTime.Now.Ticks).ToUpper();
        }
        /// <summary>
        /// 返回日时分+16位长度的唯一字符串
        /// </summary>
        /// <param name="format">默认的日期格式化</param>
        /// <returns></returns>
        public static string GetDateGuid(string format = "ddHHmm")
        {
            return string.Format("{0}{1}", DateTime.Now.ToString(format), GetXGuid());
        }
    }
}

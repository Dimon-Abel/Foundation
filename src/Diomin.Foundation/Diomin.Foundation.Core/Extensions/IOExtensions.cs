using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Diomin.Foundation.Core.Extensions
{
    public static class IOExtensions
    {
        public static object GetValue(this PropertyInfo property)
        {
            return property.GetValue(property, null);
        }

        #region 数据流转换
        /// <summary>数据流转为字节数组</summary>
        /// <remarks>
        /// 针对MemoryStream进行优化。内存流的Read实现是一个个字节复制，而ToArray是调用内部内存复制方法
        /// 如果要读完数据，又不支持定位，则采用内存流搬运
        /// 如果指定长度超过数据流长度，就让其报错，因为那是调用者所期望的值
        /// </remarks>
        /// <param name="stream">数据流</param>
        /// <param name="length">长度，0表示读到结束</param>
        /// <returns></returns>
        public static byte[] ReadBytes(this Stream stream, long length = -1)
        {
            if (stream == null) return null;
            if (length == 0) return new byte[0];

            if (length > 0 && stream.CanSeek && stream.Length - stream.Position < length)
                throw new Exception($"无法从长度只有{stream.Length - stream.Position}的数据流里面读取{length}字节的数据");

            if (length > 0)
            {
                var buf = new byte[length];
                var n = stream.Read(buf, 0, buf.Length);
                //if (n != buf.Length) buf = buf.ReadBytes(0, n);
                return buf;
            }

            // 如果要读完数据，又不支持定位，则采用内存流搬运
            if (!stream.CanSeek)
            {
                //var ms = new MemoryStream(); //Pool.MemoryStream.Get();
                //while (true)
                //{
                //    var buf = new byte[1024];
                //    var count = stream.Read(buf, 0, buf.Length);
                //    if (count <= 0) break;

                //    ms.Write(buf, 0, count);
                //    if (count < buf.Length) break;
                //}
                return null;
                //return ms.Put(true);
            }
            else
            {
                // 如果指定长度超过数据流长度，就让其报错，因为那是调用者所期望的值
                length = (int)(stream.Length - stream.Position);

                var buf = new byte[length];
                stream.Read(buf, 0, buf.Length);
                return buf;
            }
        }

        /// <summary>数据流转为字节数组，从0开始，无视数据流的当前位置</summary>
        /// <param name="stream">数据流</param>
        /// <returns></returns>
        public static byte[] ToArray(this Stream stream)
        {
            if (stream is MemoryStream) return (stream as MemoryStream).ToArray();

            stream.Position = 0;
            return stream.ReadBytes();
        }

        /// <summary>从数据流中读取字节数组，直到遇到指定字节数组</summary>
        /// <param name="stream">数据流</param>
        /// <param name="buffer">字节数组</param>
        /// <param name="offset">字节数组中的偏移</param>
        /// <param name="length">字节数组中的查找长度</param>
        /// <returns>未找到时返回空，0位置范围大小为0的字节数组</returns>
        public static byte[] ReadTo(this Stream stream, byte[] buffer, long offset = 0, long length = -1)
        {
            //if (!stream.CanSeek) throw new Exception("流不支持查找！");

            if (length == 0) return new byte[0];
            if (length < 0) length = buffer.Length - offset;

            var ori = stream.Position;
            var p = stream.IndexOf(buffer, offset, length);
            stream.Position = ori;
            if (p < 0) return null;
            if (p == 0) return new byte[0];

            return stream.ReadBytes(p);
        }
        public static long IndexOf(this Stream stream, byte[] buffer, long offset = 0, long length = -1)
        {
            if (length <= 0) length = buffer.Length - offset;

            // 位置
            long p = -1;

            for (long i = 0; i < length;)
            {
                var c = stream.ReadByte();
                if (c == -1) return -1;

                p++;
                if (c == buffer[offset + i])
                {
                    i++;

                    // 全部匹配，退出
                    if (i >= length) return p - length + 1;
                }
                else
                {
                    //i = 0; // 只要有一个不匹配，马上清零
                    // 不能直接清零，那样会导致数据丢失，需要逐位探测，窗口一个个字节滑动
                    // 上一次匹配的其实就是j=0那个，所以这里从j=1开始
                    var n = i;
                    i = 0;
                    for (var j = 1; j < n; j++)
                    {
                        // 在字节数组前(j,n)里面找自己(0,n-j)
                        if (CompareTo(buffer, j, n, buffer, 0, n - j) == 0)
                        {
                            // 前面(0,n-j)相等，窗口退回到这里
                            i = n - j;
                            break;
                        }
                    }
                }
            }

            return -1;
        }
        /// <summary>比较两个字节数组大小。相等返回0，不等则返回不等的位置，如果位置为0，则返回1。</summary>
        /// <param name="source"></param>
        /// <param name="start"></param>
        /// <param name="count">数量</param>
        /// <param name="buffer">缓冲区</param>
        /// <param name="offset">偏移</param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static int CompareTo(this byte[] source, long start, long count, byte[] buffer, long offset = 0, long length = -1)
        {
            if (source == buffer) return 0;

            if (start < 0) start = 0;
            if (count <= 0 || count > source.Length - start) count = source.Length - start;
            if (length <= 0 || length > buffer.Length - offset) length = buffer.Length - offset;

            // 逐字节比较
            for (var i = 0; i < count && i < length; i++)
            {
                var rs = source[start + i].CompareTo(buffer[offset + i]);
                if (rs != 0) return i > 0 ? i : 1;
            }

            // 比较完成。如果长度不相等，则较长者较大
            if (count != length) return count > length ? 1 : -1;

            return 0;
        }
        /// <summary>从数据流中读取字节数组，直到遇到指定字节数组</summary>
        /// <param name="stream">数据流</param>
        /// <param name="str"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] ReadTo(this Stream stream, string str, Encoding encoding = null)
        {
            if (encoding == null) encoding = Encoding.UTF8;
            return stream.ReadTo(encoding.GetBytes(str));
        }

        /// <summary>从数据流中读取一行，直到遇到换行</summary>
        /// <param name="stream">数据流</param>
        /// <param name="encoding"></param>
        /// <returns>未找到返回null，0位置返回string.Empty</returns>
        public static string ReadLine(this Stream stream, Encoding encoding = null)
        {
            var bts = stream.ReadTo(Environment.NewLine, encoding);
            //if (bts == null || bts.Length < 1) return null;
            if (bts == null) return null;

            stream.Seek(encoding.GetByteCount(Environment.NewLine), SeekOrigin.Current);
            if (bts.Length == 0) return string.Empty;

            return encoding.GetString(bts);
        }

        /// <summary>流转换为字符串</summary>
        /// <param name="stream">目标流</param>
        /// <param name="encoding">编码格式</param>
        /// <returns></returns>
        public static string ToStr(this Stream stream, Encoding encoding = null)
        {
            if (stream == null) return null;
            if (encoding == null) encoding = Encoding.UTF8;

            var buf = stream.ReadBytes();
            if (buf == null || buf.Length < 1) return null;

            // 可能数据流前面有编码字节序列，需要先去掉
            var idx = 0;
            var preamble = encoding.GetPreamble();
            if (preamble != null && preamble.Length > 0)
            {
                if (buf.StartsWith(preamble)) idx = preamble.Length;
            }

            return encoding.GetString(buf, idx, buf.Length - idx);
        }
        /// <summary>一个数据流是否以另一个数组开头。如果成功，指针移到目标之后，否则保持指针位置不变。</summary>
        /// <param name="source"></param>
        /// <param name="buffer">缓冲区</param>
        /// <returns></returns>
        public static bool StartsWith(this Stream source, byte[] buffer)
        {
            var p = 0;
            for (var i = 0; i < buffer.Length; i++)
            {
                var b = source.ReadByte();
                if (b == -1) { source.Seek(-p, SeekOrigin.Current); return false; }
                p++;

                if (b != buffer[i]) { source.Seek(-p, SeekOrigin.Current); return false; }
            }
            return true;
        }

        /// <summary>一个数据流是否以另一个数组结尾。如果成功，指针移到目标之后，否则保持指针位置不变。</summary>
        /// <param name="source"></param>
        /// <param name="buffer">缓冲区</param>
        /// <returns></returns>
        public static bool EndsWith(this Stream source, byte[] buffer)
        {
            if (source.Length < buffer.Length) return false;

            var p = source.Length - buffer.Length;
            source.Seek(p, SeekOrigin.Current);
            if (source.StartsWith(buffer)) return true;

            source.Seek(-p, SeekOrigin.Current);
            return false;
        }

        /// <summary>一个数组是否以另一个数组开头</summary>
        /// <param name="source"></param>
        /// <param name="buffer">缓冲区</param>
        /// <returns></returns>
        public static bool StartsWith(this byte[] source, byte[] buffer)
        {
            if (source.Length < buffer.Length) return false;

            for (var i = 0; i < buffer.Length; i++)
            {
                if (source[i] != buffer[i]) return false;
            }
            return true;
        }

        /// <summary>一个数组是否以另一个数组结尾</summary>
        /// <param name="source"></param>
        /// <param name="buffer">缓冲区</param>
        /// <returns></returns>
        public static bool EndsWith(this byte[] source, byte[] buffer)
        {
            if (source.Length < buffer.Length) return false;

            var p = source.Length - buffer.Length;
            for (var i = 0; i < buffer.Length; i++)
            {
                if (source[p + i] != buffer[i]) return false;
            }
            return true;
        }
        /// <summary>字节数组转换为字符串</summary>
        /// <param name="buf">字节数组</param>
        /// <param name="encoding">编码格式</param>
        /// <param name="offset">字节数组中的偏移</param>
        /// <param name="count">字节数组中的查找长度</param>
        /// <returns></returns>
        public static string ToStr(this byte[] buf, Encoding encoding = null, int offset = 0, int count = -1)
        {
            if (buf == null || buf.Length < 1 || offset >= buf.Length) return null;
            if (encoding == null) encoding = Encoding.UTF8;

            var size = buf.Length - offset;
            if (count < 0 || count > size) count = size;

            // 可能数据流前面有编码字节序列，需要先去掉
            var idx = 0;
            var preamble = encoding?.GetPreamble();
            if (preamble != null && preamble.Length > 0 && buf.Length >= offset + preamble.Length)
            {
                if (buf.ReadBytes(offset, preamble.Length).StartsWith(preamble)) idx = preamble.Length;
            }

            return encoding.GetString(buf, offset + idx, count - idx);
        }
        /// <summary>复制数组</summary>
        /// <param name="src">源数组</param>
        /// <param name="offset">起始位置</param>
        /// <param name="count">复制字节数</param>
        /// <returns>返回复制的总字节数</returns>
        public static byte[] ReadBytes(this byte[] src, int offset = 0, int count = -1)
        {
            if (count == 0) return new byte[0];

            // 即使是全部，也要复制一份，而不只是返回原数组，因为可能就是为了复制数组
            if (count < 0) count = src.Length - offset;

            var bts = new byte[count];
            Buffer.BlockCopy(src, offset, bts, 0, bts.Length);
            return bts;
        }
        #endregion
    }
}


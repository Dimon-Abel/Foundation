using System;
using System.Collections.Generic;
using System.Text;

namespace Foundation.Core.Entity
{
    /// <summary>
    /// 返回码
    /// </summary>
    public enum ReturnResultCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success = 1,
        /// <summary>
        /// 失败
        /// </summary>
        Failed = 0,
        /// <summary>
        /// 无效
        /// </summary>
        Invalid = 2,
        /// <summary>
        /// 到期
        /// </summary>
        Expired = 3,
        /// <summary>
        /// 异常
        /// </summary>
        Error = 9,
    }

    /// <summary>
    /// 统一结果返回类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReturnResult<T>
    {
        public ReturnResult() : this(ReturnResultCode.Success, default(T)) { }
        public ReturnResult(T data) : this(ReturnResultCode.Success, data) { }
        public ReturnResult(ReturnResultCode code, T data) : this(code, data, null) { }
        public ReturnResult(ReturnResultCode code, T data, string message = null)
        {
            this.Code = code;
            this.Data = data;
            this.Message = message;

        }
        /// <summary>
        /// 获取或设置 结果状态码
        /// </summary>
        public ReturnResultCode Code { get; set; }
        /// <summary>
        /// 获取或设置 结果数据
        /// </summary>
        public T Data { get; set; }
        /// <summary>
        /// 获取或设置 结果消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 获取或设置 数量（可用于分页查询）
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 成功
        /// </summary>
        public static ReturnResult Successed => new ReturnResult(ReturnResultCode.Success, null, "操作成功");
        /// <summary>
        /// 失败
        /// </summary>
        public static ReturnResult Failed => new ReturnResult(ReturnResultCode.Failed, null, "操作失败");
    }

    /// <summary>
    /// 统一结果返回类
    /// </summary>
    public class ReturnResult : ReturnResult<string>
    {
        public ReturnResult() : this(ReturnResultCode.Success, null) { }
        public ReturnResult(string data) : this(ReturnResultCode.Success, data) { }
        public ReturnResult(ReturnResultCode code, string data) : this(code, data, null) { }

        public bool Succeeded() => Code == ReturnResultCode.Success;

        public ReturnResult(ReturnResultCode code, string data, string message = null)
        {
            this.Code = code;
            this.Data = data;
            this.Message = message;
        }
    }
}

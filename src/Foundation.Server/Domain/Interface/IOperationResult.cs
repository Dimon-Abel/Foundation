namespace Foundation.Server.Domain.Interface
{
    /// <summary>
    ///     操作结果接口
    /// </summary>
    /// <typeparam name="TResultType">结果类型</typeparam>
    public interface IOperationResult<TResultType>
        : IOperationResult<TResultType, object>
    {
    }

    /// <summary>
    ///     操作结果接口
    /// </summary>
    /// <typeparam name="TResultType">结果类型</typeparam>
    /// <typeparam name="TData">结果数据</typeparam>
    public interface IOperationResult<TResultType, TData>
    {
        /// <summary>
        ///     获取或设置 结果类型
        /// </summary>
        TResultType ResultType { get; set; }

        /// <summary>
        ///     获取或设置 返回消息
        /// </summary>
        string Message { get; set; }

        /// <summary>
        ///     获取或设置 结果数据
        /// </summary>
        TData Data { get; set; }
    }
}
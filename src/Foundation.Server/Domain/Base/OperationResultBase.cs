using Foundation.Server.Domain.Interface;

namespace Foundation.Server.Domain.Base
{
    /// <summary>
    ///     操作结果
    /// </summary>
    /// <typeparam name="TResultType">结果类型</typeparam>
    public abstract class OperationResultBase<TResultType> : OperationResultBase<TResultType, object>,
        IOperationResult<TResultType>
    {
        protected OperationResultBase()
            : this(default(TResultType))
        {
        }

        protected OperationResultBase(TResultType type)
            : base(type, null, null)
        {
        }

        protected OperationResultBase(TResultType type, string message)
            : base(type, message, null)
        {
        }

        protected OperationResultBase(TResultType type, string message, object data)
            : base(type, message, data)
        {
        }
    }

    /// <summary>
    ///     操作结果
    /// </summary>
    /// <typeparam name="TResultType">结果类型</typeparam>
    /// <typeparam name="TData">结果数据</typeparam>
    public abstract class OperationResultBase<TResultType, TData> : IOperationResult<TResultType, TData>
    {
        /// <summary>
        ///     返回消息
        /// </summary>
        protected string _message;

        protected OperationResultBase()
            : this(default(TResultType))
        {
        }

        protected OperationResultBase(TResultType type)
            : this(type, null, default(TData))
        {
        }

        protected OperationResultBase(TResultType type, string message)
            : this(type, message, default(TData))
        {
        }

        protected OperationResultBase(TResultType type, string message, TData data)
        {
            ResultType = type;
            _message = message;
            Data = data;
        }

        /// <summary>
        ///     获取或设置 结果类型
        /// </summary>
        public TResultType ResultType { get; set; }

        /// <summary>
        ///     获取或设置 返回消息
        /// </summary>
        public virtual string Message
        {
            get => _message;
            set => _message = value;
        }

        /// <summary>
        ///     获取或设置 结果数据
        /// </summary>
        public TData Data { get; set; }
    }
}

using Foundation.Core.Extensions;
using Foundation.Server.Domain.Base;
using Foundation.Server.Domain.Enums;

namespace Foundation.Server.Domain.Entity
{
    public class OperationResult : OperationResult<object>
    {
        static OperationResult()
        {
            Success = new OperationResult(OperationResultType.Success, "操作成功");
            NoChanged = new OperationResult(OperationResultType.NoChanged, "无任何变化");
        }

        public OperationResult()
            : this(OperationResultType.NoChanged)
        {
        }

        public OperationResult(OperationResultType resultType)
            : this(resultType, null, null)
        {
        }

        public OperationResult(OperationResultType resultType, string message)
            : this(resultType, message, null)
        {
        }

        public OperationResult(OperationResultType resultType, string message, object data)
            : base(resultType, message, data)
        {
        }

        /// <summary>
        ///     获取 成功的操作结果
        /// </summary>
        public static OperationResult Success { get; }

        /// <summary>
        ///     获取 未变更的操作结果（将覆盖父类中的NoChanged属性）
        /// </summary>
        public new static OperationResult NoChanged { get; }
    }

    /// <summary>
    ///     业务单元操作结果信息类（泛型）
    /// </summary>
    /// <typeparam name="TData">返回数据的类型</typeparam>
    public class OperationResult<TData> : OperationResultBase<OperationResultType, TData>
    {
        static OperationResult()
        {
            NoChanged = new OperationResult<TData>(OperationResultType.NoChanged);
        }

        public OperationResult()
            : this(OperationResultType.NoChanged)
        {
        }

        public OperationResult(OperationResultType type)
            : this(type, null, default(TData))
        {
        }

        public OperationResult(OperationResultType type, string message)
            : base(type, message, default(TData))
        {
        }

        public OperationResult(OperationResultType type, string message, TData data)
            : base(type, message, data)
        {
        }

        /// <summary>
        ///     获取或设置 返回消息
        /// </summary>
        public override string Message
        {
            get => _message ?? ResultType.ToDescription();
            set => _message = value;
        }

        /// <summary>
        ///     获取 未变更的操作结果
        /// </summary>
        public static OperationResult<TData> NoChanged { get; }

        /// <summary>
        ///     获取 是否成功
        /// </summary>
        public bool Successed => ResultType == OperationResultType.Success;
    }
}
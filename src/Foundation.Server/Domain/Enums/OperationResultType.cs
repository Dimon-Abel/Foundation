using System.ComponentModel;

namespace Foundation.Server.Domain.Enums
{
    /// <summary>
    ///     表示业务单元操作结果的类别
    /// </summary>
    public enum OperationResultType
    {
        /// <summary>
        ///     输入信息验证失败
        /// </summary>
        [Description("输入信息验证失败！")] ValidError,

        /// <summary>
        ///     指定参数的数据不存在
        /// </summary>
        [Description("指定参数的数据不存在！")] QueryNull,

        /// <summary>
        ///     操作没有引发任何变化，提交将取消
        /// </summary>
        [Description("操作没有引发任何变化，提交将取消！")] NoChanged,

        /// <summary>
        ///     操作成功
        /// </summary>
        [Description("操作成功！")] Success,

        /// <summary>
        ///     操作引发错误
        /// </summary>
        [Description("操作引发错误！")] Error,

        /// <summary>
        /// 未授权
        /// </summary>

        [Description("未授权！")] UnAuth
    }
}

namespace Foundation.Server.Domain.Interface
{
    /// <summary>
    /// 数据模型接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEntity<out T>
    {
        T Id { get; }
    }
}

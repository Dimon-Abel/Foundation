namespace Foundation.Server.Domain.Interface
{
    /// <summary>
    /// 输入Dto
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IInputDto<T>
    {
        T Id { get; set; }
    }
}

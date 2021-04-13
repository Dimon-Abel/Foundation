namespace Foundation.Server.Domain.Interface
{
    /// <summary>
    /// 输出Dto
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IOutputDto<T>
    {
        T Id { get; set; }
    }
}

using System;

namespace Foundation.Server.Domain.Interface
{
    /// <summary>
    ///     工作单元管理器，统一处理各个工作单元的事务
    /// </summary>
    public interface IUnitOfWorkManager : IDisposable
    {
        /// <summary>
        ///     获取 事务是否已提交
        /// </summary>
        bool HasCommited { get; }

        /// <summary>
        ///     获取指定实体所在的工作单元对象
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TKey">实体主键类型</typeparam>
        /// <returns>工作单元对象</returns>
        IUnitOfWork GetUnitOfWork<TEntity, TKey>() where TEntity : IEntity<TKey>;

        /// <summary>
        ///     获取指定实体所在的工作单元对象
        /// </summary>
        /// <param name="entityType">实体类型</param>
        /// <returns>工作单元对象</returns>
        IUnitOfWork GetUnitOfWork(Type entityType);
        /// <summary>
        ///     获取指定实体类所属的上下文类型
        /// </summary>
        /// <param name="entityType">实体类型</param>
        /// <returns>上下文类型</returns>
        Type GetDbContextType(Type entityType);
        /// <summary>
        /// 根据数据库上下文获取工作单元
        /// </summary>
        /// <param name="dbContextType"></param>
        /// <returns></returns>
        IUnitOfWork GetUnitOfWorkByDbContextType(Type dbContextType);

        /// <summary>
        ///     Engine
        ///     提交所有工作单元的事务更改
        /// </summary>
        void Commit();
    }
}

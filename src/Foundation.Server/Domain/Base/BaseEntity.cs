using Foundation.Core.Data;
using Foundation.Core.Extensions;
using Foundation.Server.Domain.Interface;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Foundation.Server.Domain.Base
{
    /// <summary>
    ///     实体类基类
    /// </summary>
    public abstract class EntityBase<TKey> : IEntity<TKey> where TKey : IEquatable<TKey>
    {
        /// <summary>
        ///     初始化一个<see cref="EntityBase{TKey}" />类型的新实例
        /// </summary>
        protected EntityBase()
        {
            if (typeof(TKey) == typeof(string))
                Id = XGuidHelper.GetDateGuid().CastTo<TKey>();
            else if (typeof(TKey) == typeof(Guid))
                Id = XGuidHelper.NewXGuid().CastTo<TKey>();
        }

        /// <summary>
        ///     获取或设置 主键
        /// </summary>
        [DisplayName("主键")]
        [StringLength(50)]
        public TKey Id { get; set; }

        /// <summary>
        ///     判断两个实体是否是同一数据记录的实体
        /// </summary>
        /// <param name="obj">要比较的实体信息</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is EntityBase<TKey> entity))
                return false;
            return IsKeyEqual(entity.Id, Id);
        }

        /// <summary>
        ///     实体ID是否相等
        /// </summary>
        public static bool IsKeyEqual(TKey id1, TKey id2)
        {
            if (id1 == null && id2 == null)
                return true;
            if (id1 == null || id2 == null)
                return false;

            var type = typeof(TKey);
            return type.IsDeriveClassFrom(typeof(IEquatable<TKey>)) ? id1.Equals(id2) : Equals(id1, id2);
        }

        /// <summary>
        ///     用作特定类型的哈希函数。
        /// </summary>
        /// <returns>
        ///     当前 <see cref="T:System.Object" /> 的哈希代码。<br />
        ///     如果<c>Id</c>为<c>null</c>则返回0，
        ///     如果不为<c>null</c>则返回<c>Id</c>对应的哈希值
        /// </returns>
        public override int GetHashCode()
        {
            return Id == null ? 0 : Id.ToString().GetHashCode();
        }
    }
}
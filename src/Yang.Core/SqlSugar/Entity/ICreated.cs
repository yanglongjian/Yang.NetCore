using System;

namespace Yang.Core
{
    /// <summary>
    /// 创建时间
    /// </summary>
    public interface ICreatedTime
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime CreatedTime { get; set; }
    }

    /// <summary>
    /// 创建审计
    /// </summary>
    /// <typeparam name="TUserKey"></typeparam>
    public interface ICreatedAudit<TUserKey> : ICreatedTime where TUserKey : struct
    {
        /// <summary>
        /// 创建者
        /// </summary>
        TUserKey? CreatedId { get; set; }
    }
}




using System;

namespace Yang.Core
{
    /// <summary>
    /// 更新审计
    /// </summary>
    /// <typeparam name="TUserKey"></typeparam>
    public interface IUpdated<TUserKey> where TUserKey : struct
    {
        /// <summary>
        /// 更新者
        /// </summary>
        TUserKey? UpdatedId { get; set; }

        /// <summary>
        ///最后更新时间
        /// </summary>
        DateTime? UpdatedTime { get; set; }
    }
}




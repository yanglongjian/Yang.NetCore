using System;

namespace Yang.Core
{
    /// <summary>
    /// 删除审计
    /// </summary>
    /// <typeparam name="TUserKey"></typeparam>
    public interface IDeleted
    {
        /// <summary>
        /// 是否删除
        /// </summary>
        bool IsDeleted { get; set; }
    }
}




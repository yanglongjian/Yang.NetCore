using System.Collections.Generic;

namespace Yang.Admin.Application.Dtos
{
    /// <summary>
    /// 实体信息数据审计输入Dto
    /// </summary>
    public class EntityAuditInputDto
    {
        /// <summary>
        /// 编号集合
        /// </summary>
        public List<int> Ids { get; set; }

        /// <summary>
        /// 数据审计
        /// </summary>
        public bool IsAudit { get; set; }
    }
}




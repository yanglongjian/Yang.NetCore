using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yang.Admin.Domain;

namespace Yang.Admin.Application.Dtos
{
    /// <summary>
    /// 实体数据输出
    /// </summary>
    public class EntityOutputDto
    {
        /// <summary>
        /// 实体编号
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 实体名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 子节点
        /// </summary>
        public List<Operate> Children { get; set; }
    }

    /// <summary>
    /// 实体操作
    /// </summary>
    public class Operate
    {
        /// <summary>
        /// 操作类型
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
    }
}


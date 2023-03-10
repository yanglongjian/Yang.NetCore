using System.Collections.Generic;
using Yang.Admin.Domain;

namespace Yang.Admin.Application.Dtos
{
    /// <summary>
    /// 模块数输出Dto
    /// </summary>
    public class ModuleTreeOutputDto
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 模块名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsChecked { get; set; }
        /// <summary>
        /// 是否子节点
        /// </summary>
        public bool HasChildren { get; set; }

        /// <summary>
        /// 访问类型
        /// </summary>
        public AccessType AccessType { get; set; }

        /// <summary>
        /// 子节点集合
        /// </summary>
        public List<ModuleTreeOutputDto> Children { get; set; }
    }
}




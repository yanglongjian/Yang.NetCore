using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yang.Admin.Application.Dtos
{
    /// <summary>
    /// 动态路由输出信息
    /// </summary>
    public class RouteOutputDto
    {
        /// <summary>
        /// 组件名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 扩展信息
        /// </summary>
        public Meta Meta { get; set; }
        /// <summary>
        /// 子菜单
        /// </summary>
        public List<RouteOutputDto> Children { get; set; }
        /// <summary>
        /// 组件
        /// </summary>
        public string Component { get; set; }
    }

    /// <summary>
    /// 扩展信息
    /// </summary>
    public class Meta
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 类型（menu/iframe/link）
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 标签
        /// </summary>
        public string Tag { get; set; }
        /// <summary>
        /// 是否隐藏
        /// </summary>
        public bool Hidden { get; set; }
        /// <summary>
        /// 是否整页
        /// </summary>
        public bool Fullpage { get; set; }
    }
}


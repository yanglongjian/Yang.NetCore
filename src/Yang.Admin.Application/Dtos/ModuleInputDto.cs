using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yang.Admin.Domain;

namespace Yang.Admin.Application.Dtos
{
    /// <summary>
    /// 菜单输入
    /// </summary>
    public class ModuleInputDto
    {

        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 父节点
        /// </summary>
        public int ParentId { get; set; }


        /// <summary>
        /// 菜单类型
        /// </summary>
        public ModuleType Type { get; set; }


        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 菜单别名
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 路由地址
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 重定向
        /// </summary>
        public string Redirect { get; set; }

        /// <summary>
        /// 组件
        /// </summary>
        public string Component { get; set; }


        /// <summary>
        /// 排序
        /// </summary>
        public int OrderCode { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public StatusType Status { get; set; }

        /// <summary>
        /// 是否隐藏
        /// </summary>
        public bool Hidden { get; set; }

        /// <summary>
        /// 面包屑
        /// </summary>
        public bool Bread { get; set; }

        /// <summary>
        /// 整页路由
        /// </summary>
        public bool FullPage { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public string Tag { get; set; }

    }
}


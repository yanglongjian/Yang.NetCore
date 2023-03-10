using System.ComponentModel;

namespace Yang.Admin.Domain
{
    /// <summary>
    /// 状态类型
    /// </summary>
    public enum StatusType
    {
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")] Normal = 0,

        /// <summary>
        /// 停用
        /// </summary>
        [Description("停用")] Stop = 1,
    }


    /// <summary>
    /// 菜单类型
    /// </summary>
    public enum ModuleType
    {
        /// <summary>
        /// 菜单
        /// </summary>
        [Description("菜单")] Menu = 0,

        /// <summary>
        /// Iframe
        /// </summary>
        [Description("Iframe")] Iframe = 1,

        /// <summary>
        /// 外链
        /// </summary>
        [Description("外链")] Link = 2,

        /// <summary>
        /// 按钮
        /// </summary>
        [Description("按钮")] Button = 3,
    }


    /// <summary>
    /// 权限类型
    /// </summary>
    public enum PermissionType
    {
        /// <summary>
        /// 全部可见
        /// </summary>
        [Description("全部可见")] 全部可见 = 1,

        /// <summary>
        /// 本人可见
        /// </summary>
        [Description("本人可见")] 本人可见 = 2,

        /// <summary>
        /// 所在部门可见
        /// </summary>
        [Description("所在部门可见")] 所在部门可见 = 3,

        /// <summary>
        /// 所在部门及子级可见
        /// </summary>
        [Description("所在部门及子级可见")] 所在部门及子级可见 = 4,

        /// <summary>
        /// 选择的部门可见
        /// </summary>
        [Description("选择的部门可见")] 选择的部门可见 = 5,

        /// <summary>
        /// 自定义
        /// </summary>
        [Description("自定义")] 自定义 = 6,
    }
}


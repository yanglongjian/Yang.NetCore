using SqlSugar;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Yang.Core;

namespace Yang.Admin.Domain
{
    /// <summary>
    /// 模块信息
    /// </summary>
    [SugarTable("Sys_Module", "模块信息")]
    public class Module : IEntity<string>
    {
        /// <summary>
        /// 编号
        /// </summary>
        [Required]
        [SugarColumn(IsPrimaryKey = true, ColumnDescription = "编号")]
        public string Id { get; set; }

        /// <summary>
        /// 父节点
        /// </summary>
        [SugarColumn(ColumnDescription = "父节点")]
        public string ParentId { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        [SugarColumn(ColumnDescription = "功能位置")]
        public string Postion { get; set; }

        /// <summary>
        /// 区域
        /// </summary>
        [SugarColumn(ColumnDescription = "区域")]
        public string Area { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        [SugarColumn(ColumnDescription = "排序码")]
        public int OrderCode { get; set; }


        /// <summary>
        /// 功能名称
        /// </summary>
        [SugarColumn(ColumnDescription = "功能名称")]
        public string Name { get; set; }


        /// <summary>
        /// 代码
        /// </summary>
        [Required]
        [SugarColumn(ColumnDescription = "代码", UniqueGroupNameList = new[] { "index_module_code" })]
        public string Code { get; set; }

        /// <summary>
        /// 控制器
        /// </summary>
        [SugarColumn(ColumnDescription = "控制器")]
        public string Controller { get; set; }

        /// <summary>
        /// 功能
        /// </summary>
        [SugarColumn(ColumnDescription = "功能")]
        public string Action { get; set; }

        /// <summary>
        /// 是否是控制器
        /// </summary>
        [SugarColumn(ColumnDescription = "是否是控制器")]
        public bool IsController { get; set; }

        /// <summary>
        /// 访问类型
        /// </summary>
        [SugarColumn(ColumnDescription = "访问类型")]
        public AccessType AccessType { get; set; }



        /// <summary>
        /// 树结构
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<Module> Children { get; set; }

        /// <summary>
        /// 主键值
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string Key
        {
            get
            {
                return Id;
            }
        }

        /// <summary>
        /// 标题
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string Title
        {
            get
            {
                return Name;
            }
        }
    }

    /// <summary>
    /// 功能访问类型
    /// </summary>
    public enum AccessType
    {
        /// <summary>
        /// 匿名用户可访问
        /// </summary>
        [Description("匿名访问")] Anonymous = 0,

        /// <summary>
        /// 登录用户可访问
        /// </summary>
        [Description("登录访问")] LoggedIn = 1,

        /// <summary>
        /// 指定角色可访问
        /// </summary>
        [Description("角色访问")] RoleLimit = 2
    }
}




using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Yang.Core;

namespace Yang.Admin.Domain
{
    /// <summary>
    /// 角色信息
    /// </summary>
    [SugarTable("Sys_Role", "角色信息")]
    public class Role : IEntity<int>, ICreatedTime
    {
        /// <summary>
        /// 编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnDescription = "编号")]
        public int Id { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        [Required]
        [SugarColumn(ColumnDescription = "角色名称")]
        public string Name { get; set; }

        /// <summary>
        /// 是否系统用户
        /// </summary>
        [SugarColumn(ColumnDescription = "是否系统角色")]
        public bool IsSystem { get; set; }

        /// <summary>
        /// 是否管理角色
        /// </summary>
        [SugarColumn(ColumnDescription = "是否管理角色")]
        public bool IsAdmin { get; set; }

        /// <summary>
        /// 是否管理角色
        /// </summary>
        [SugarColumn(ColumnDescription = "是否默认角色")]
        public bool IsDefault { get; set; }


        /// <summary>
        /// 状态
        /// </summary>
        [SugarColumn(ColumnDescription = "状态")]
        public int Status { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(ColumnDescription = "描述")]
        public string Remark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnDescription = "创建时间")]
        public DateTime CreatedTime { get; set; }

        #region 扩展信息

        /// <summary>
        /// 模块
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        [Navigate(typeof(ModuleRole), nameof(ModuleRole.RoleId), nameof(ModuleRole.ModuleId))] //注意顺序
        public List<Module> Modules { get; set; } //只能是null不能赋默认值

        #endregion
    }
}




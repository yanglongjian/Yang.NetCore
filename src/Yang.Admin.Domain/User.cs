using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Yang.Core;

namespace Yang.Admin.Domain
{
    /// <summary>
    /// 用户信息
    /// </summary>
    [SugarTable("Sys_User", "用户信息")]
    public class User : IEntity<int>, ICreatedTime
    {
        /// <summary>
        /// 编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnDescription = "编号")]
        public int Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        [SugarColumn(ColumnDescription = "账号")]
        public string Account { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        [Required]
        [SugarColumn(ColumnDescription = "用户昵称")]
        public string NickName { get; set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        [SugarColumn(ColumnDescription = "用户头像", DefaultValue = "")]
        public string Avatar { get; set; }


        /// <summary>
        /// 手机
        /// </summary>
        [SugarColumn(ColumnDescription = "手机")]
        public string Mobile { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [SugarColumn(ColumnDescription = "邮箱")]
        public string Email { get; set; }

        /// <summary>
        /// 是否系统用户
        /// </summary>
        [SugarColumn(ColumnDescription = "是否系统用户")]
        public bool IsSystem { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [SugarColumn(ColumnDescription = "状态")]
        public int Status { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(ColumnDescription = "备注")]
        public string Remark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnDescription = "创建时间")]
        public DateTime CreatedTime { get; set; }




        #region  级联信息

        /// <summary>
        /// 用户明细
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(UserDetail.UserId))]//一对一
        public UserDetail Detail { get; set; }

        /// <summary>
        /// 用户角色
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(UserRole.UserId))]//一对多
        public List<UserRole> UserRoles { get; set; }


        /// <summary>
        /// 角色
        /// </summary>
        [Navigate(typeof(UserRole), nameof(UserRole.UserId), nameof(UserRole.RoleId))] //注意顺序
        public List<Role> Roles { get; set; } //只能是null不能赋默认值
        #endregion


        /// <summary>
        /// 权限代码列表
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<string> PermissionCodeList { get; set; }
    }
}




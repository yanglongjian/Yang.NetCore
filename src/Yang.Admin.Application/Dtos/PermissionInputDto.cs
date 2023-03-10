using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yang.Admin.Domain;

namespace Yang.Admin.Application.Dtos
{
    /// <summary>
    /// 权限输入
    /// </summary>
    public class PermissionInputDto
    {
        /// <summary>
        /// 角色编号
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 权限类型
        /// </summary>
        public PermissionType PermissionType { get; set; }

        /// <summary>
        /// 自定义值
        /// </summary>
        public string Custom { get; set; }

        /// <summary>
        /// 部门编号
        /// </summary>
        public int[] DeptId { get; set; }

        /// <summary>
        /// 模块编号
        /// </summary>
        public int[] PermissionId { get; set; }
    }
}


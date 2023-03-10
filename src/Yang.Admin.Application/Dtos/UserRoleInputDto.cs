using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yang.Admin.Application.Dtos
{
    /// <summary>
    /// 角色分配输入
    /// </summary>
    public class UserRoleInputDto
    {
        /// <summary>
        /// 角色编号
        /// </summary>
        public int RoleId { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public int[] UserId { get; set; }
    }
}


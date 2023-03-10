using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yang.Admin.Application.Dtos
{
    /// <summary>
    /// 设置用户角色Dto
    /// </summary>
    public class UserSetRoleInputDto
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 角色编号
        /// </summary>
        public int[] RoleIds { get; set; }
    }
}




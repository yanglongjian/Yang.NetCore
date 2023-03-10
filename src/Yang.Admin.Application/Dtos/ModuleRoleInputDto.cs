using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yang.Admin.Application.Dtos
{
    /// <summary>
    /// 角色模块权限设置
    /// </summary>
    public class ModuleRoleInputDto
    {
        /// <summary>
        /// 角色编号
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 模块编号集合
        /// </summary>
        public string[] ModuleIds { get; set; }
    }
}




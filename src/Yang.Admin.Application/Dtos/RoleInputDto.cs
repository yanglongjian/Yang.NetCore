namespace Yang.Admin.Application.Dtos
{
    /// <summary>
    /// 角色输入Dto
    /// </summary>
    public class RoleInputDto
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 是否管理角色
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// 是否管理角色
        /// </summary>
        public bool IsDefault { get; set; }


        /// <summary>
        /// 是否锁定
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Remark { get; set; }
    }
}




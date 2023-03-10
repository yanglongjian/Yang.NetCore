namespace Yang.Admin.Application.Dtos
{
    /// <summary>
    /// 用户模块权限设置
    /// </summary>
    public class ModuleUserInputDto
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 模块编号集合
        /// </summary>
        public string[] ModuleIds { get; set; }
    }
}




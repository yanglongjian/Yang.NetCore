namespace Yang.Admin.Application
{
    /// <summary>
    /// 后台管理数据种子
    /// </summary>
    public class AdminSeedData : ISingleton
    {
        /// <summary>
        /// 
        /// </summary>
        public static void InitData()
        {
            if (DbContext.Instance.Queryable<Role>().Any()) return;
            var role = new Role
            {
                Name = "系统管理员",
                IsSystem = true,
                Remark = "系统最高权限管理角色",
            };
            int roleId = DbContext.Instance.Insertable(role).ExecuteReturnIdentity();
            if (DbContext.Instance.Queryable<User>().Any()) return;
            var user = new User
            {
                Account = "admin",
                NickName = "管理员",
                Avatar = "https://avatars.githubusercontent.com/u/32290372",
                Mobile = "18150370180",
                Email = "admin@qq.com",
                IsSystem = true,
                Status = 1,
                Detail = new UserDetail
                {
                    Password = MD5Encryption.Encrypt("aa123456")
                },
                UserRoles = new List<UserRole>
                    {
                        new UserRole{  RoleId=roleId }
                    }
            };
            DbContext.Instance.Insertable(user)
                .AddSubList(it => it.Detail.UserId)
                .AddSubList(it => it.UserRoles.First().UserId)
                .ExecuteCommand();

            "[Admin] 数据种子初始化完成".LogInformation();
        }

    }
}

using Mapster;
using System.Linq;
using Yang.Admin.Domain;
namespace Yang.Admin.Application.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    public class Mapper : IRegister
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<User, UserOutputDto>()
                .Map(dest => dest.Roles, src => src.Roles.Select(r => r.Name).ToArray())
            .Map(dest => dest.DeptName, src => src.Dept.Name)
            .Map(dest => dest.PostName, src => src.Post.Name)
            ;
        }
    }
}




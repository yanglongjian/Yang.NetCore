using Furion;
using Furion.DependencyInjection;
using Furion.LinqBuilder;
using Mapster;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Yang.Core
{
    /// <summary>
    /// 仓储操作类
    /// </summary>
    public class Repository : IRepository, IScoped
    {
        private readonly SqlSugarClient _db;

        /// <summary>
        /// 初始化一个<see cref="Repository{TEntity, TKey}"/>类型的新实例
        /// </summary>
        public Repository()
        {
            _db = App.GetService<DbContext>().Db;
        }

        /// <summary>
        /// Sql客户端
        /// </summary>
        public SqlSugarClient Db => _db;



        /// <summary>
        /// 查询数据源
        /// </summary>
        public virtual ISugarQueryable<T> Queryable<T>() where T : class, new()
        {
            return Db.Queryable<T>();
        }


        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="defaultColumn"></param>
        /// <param name="orderByType"></param>
        /// <returns></returns>
        public virtual async Task<(List<T> rows, int total)> GetPageList<T>(PageRequest request, string defaultColumn = "", OrderByType orderByType = OrderByType.Asc) where T : class, new()
        {
            var predicate = FilterHelper.GetExpression<T>(request.FilterRules);
            var orderFields = FilterHelper.GetSortCondition<T>(request.SortConditions, defaultColumn, orderByType);

            RefAsync<int> total = 0;

            var rows = await Queryable<T>().Where(predicate)
                 .OrderBy(orderFields)
                 .ToPageListAsync(request.PageIndex, request.PageSize, total);


            return (rows, total.Value);
        }




        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task<bool> IsExist<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            return await Db.Queryable<T>().AnyAsync(predicate);
        }


        /// <summary>
        /// 插入返回主键
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dto"></param>
        /// <returns></returns>
        public virtual async Task<int> InsertDto<T, TDto>(TDto dto) where T : class, new()
        {
            var entity = dto.Adapt<T>();
            return await Db.Insertable(entity).EnableDiffLogEvent().ExecuteReturnIdentityAsync();
        }


        /// <summary>
        /// 插入返回主键
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dto"></param>
        /// <returns></returns>
        public virtual async Task<int> Insert<T>(T entity) where T : class, new()
        {
            return await Db.Insertable(entity).EnableDiffLogEvent().ExecuteReturnIdentityAsync();
        }


        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual async Task<int> InsertBatch<T>(List<T> entities) where T : class, new()
        {
            return await Db.Insertable(entities).EnableDiffLogEvent().ExecuteCommandAsync();
        }


        /// <summary>
        /// 更新Dto字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="dto"></param>
        /// <returns></returns>
        public virtual async Task<int> UpdateDto<T, TDto>(TDto dto) where T : class, new()
        {
            var entity = dto.Adapt<T>();

            //取交集否则更新Sql异常
            var entityColumns = typeof(T).GetPropertyName().ToList();
            var dtoColumns = typeof(TDto).GetPropertyName().Where(r => r.ToUpper() != "ID");
            var updateColumns = entityColumns.Intersect(dtoColumns).ToArray();

            return await Db.Updateable(entity).UpdateColumns(updateColumns).EnableDiffLogEvent().ExecuteCommandAsync();
        }


        /// <summary>
        /// 按条件更新字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columns"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual async Task<int> Update<T>(List<Expression<Func<T, bool>>> columns, Expression<Func<T, bool>> expression) where T : class, new()
        {
            var updateT = Db.Updateable<T>();
            foreach (var column in columns)
            {

                updateT.SetColumns(column);
            }
            return await updateT.Where(expression).ExecuteCommandAsync();
        }



        /// <summary>
        /// 按需更新(字段以逗号分隔)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public virtual async Task<int> Update<T>(T entity, string columns = "") where T : class, new()
        {
            if (columns.IsEmpty())
            {
                return await Db.Updateable(entity).EnableDiffLogEvent().ExecuteCommandAsync();
            }
            var updateColumns = columns.Split(",");
            return await Db.Updateable(entity).UpdateColumns(updateColumns).EnableDiffLogEvent().ExecuteCommandAsync();
        }



        /// <summary>
        /// 逻辑删除
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        public virtual async Task<int> Delete<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            var type = typeof(T);

            if (type.GetProperty("IsDeleted").IsNotNull())
            {
                // 逻辑删除
                return await Db.Updateable<T>()
                        .SetColumns("IsDeleted", true)
                        .Where(predicate).EnableDiffLogEvent("软删除").ExecuteCommandAsync();
            }
            else
            {
                return await Db.Deleteable<T>().Where(predicate).EnableDiffLogEvent(type.Name).ExecuteCommandAsync();
            }
        }
    }




    /// <summary>
    /// 仓储操作接口
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// Sql客户端
        /// </summary>
        SqlSugarClient Db { get; }


        /// <summary>
        /// 查询数据源
        /// </summary>
        ISugarQueryable<T> Queryable<T>() where T : class, new();


        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="defaultColumn"></param>
        /// <param name="orderByType"></param>
        /// <returns></returns>
        Task<(List<T> rows, int total)> GetPageList<T>(PageRequest request, string defaultColumn = "", OrderByType orderByType = OrderByType.Asc) where T : class, new();


        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<bool> IsExist<T>(Expression<Func<T, bool>> predicate) where T : class, new();


        /// <summary>
        /// 插入返回主键
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<int> InsertDto<T, TDto>(TDto dto) where T : class, new();


        /// <summary>
        /// 插入返回主键
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<int> Insert<T>(T entity) where T : class, new();

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task<int> InsertBatch<T>(List<T> entities) where T : class, new();


        /// <summary>
        /// 更新Dto字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<int> UpdateDto<T, TDto>(TDto dto) where T : class, new();

        /// <summary>
        /// 按条件更新字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columns"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<int> Update<T>(List<Expression<Func<T, bool>>> columns, Expression<Func<T, bool>> expression) where T : class, new();


        /// <summary>
        /// 按需更新(字段以逗号分隔)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        Task<int> Update<T>(T entity,string columns = "") where T : class, new();


        /// <summary>
        /// 逻辑删除
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        Task<int> Delete<T>(Expression<Func<T, bool>> predicate) where T : class, new();
    }
}



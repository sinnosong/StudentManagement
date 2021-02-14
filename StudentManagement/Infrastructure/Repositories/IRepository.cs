using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StudentManagement.Infrastructure.Repositories
{
    /// <summary>
    /// 此接口是所有仓储的约定，仅用作标识
    /// </summary>
    /// <typeparam name="TEntity">传入仓储的实体</typeparam>
    /// <typeparam name="TPrimaryKey">传入仓储的主键</typeparam>
    public interface IRepository<TEntity, TPrimaryKey> where TEntity : class
    {
        #region 查询

        /// <summary>
        /// 获取用于从整个表中检索实体的IQueryable
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> GetAll();

        /// <summary>
        /// 用户获取所有实体
        /// </summary>
        /// <returns>所有实体列表</returns>
        List<TEntity> GetAllList();

        /// <summary>
        /// 用户获取所有提示的异步实现
        /// </summary>
        /// <returns>所有实体列表</returns>
        Task<List<TEntity>> GetAllListAsync();

        /// <summary>
        /// 用于获取传入本方法的所有实体 <paramref name="predicate"/>
        /// </summary>
        /// <param name="predicate">筛选实体的条件</param>
        /// <returns>所有实体列表</returns>
        List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 用于获取传入本方法的所有实体 <paramref name="predicate"/>
        /// </summary>
        /// <param name="predicate">筛选实体的条件</param>
        /// <returns>所有实体列表</returns>
        Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 通过传入的筛选条件获取实体信息，如果查询不到返回值，会引发异常
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        TEntity Single(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 通过传入的筛选条件获取实体信息，如果查询不到返回值，会引发异常
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 通过传入的筛选条件查询实体信息，如果没有找到返回null
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 通过传入的筛选条件查询实体信息，如果没有找到返回null
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

        #endregion 查询

        #region Insert

        /// <summary>
        /// 添加一个新实体信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        TEntity Insert(TEntity entity);

        /// <summary>
        /// 添加一个新实体信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<TEntity> InsertAsync(TEntity entity);

        #endregion Insert

        #region Update

        /// <summary>
        /// 更新现有实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        TEntity Update(TEntity entity);

        /// <summary>
        /// 更新现有实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<TEntity> UpdateAsync(TEntity entity);

        #endregion Update

        #region Delete

        /// <summary>
        /// 删除一个实体
        /// </summary>
        /// <param name="entity"></param>
        void Delete(TEntity entity);

        /// <summary>
        /// 删除一个实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task DeleteAsync(TEntity entity);

        /// <summary>
        /// 根据传入的条件删除多个实体
        /// </summary>
        /// <param name="predicate"></param>
        void Delete(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 根据传入的条件删除多个实体
        /// </summary>
        /// <param name="predicate"></param>
        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);

        #endregion Delete

        #region 总和计算

        /// <summary>
        /// 获取此仓储中所有实体的总和
        /// </summary>
        /// <returns>实体的总数</returns>
        int Count();

        /// <summary>
        /// 获取此仓储的所有实体总和
        /// </summary>
        /// <returns>实体的总数</returns>
        Task<int> CountAsync();

        /// <summary>
        /// 条件筛选统计实体总数
        /// </summary>
        /// <param name="direcate"></param>
        /// <returns></returns>
        int Count(Expression<Func<TEntity, bool>> predirecate);

        /// <summary>
        /// 条件筛选统计实体总数
        /// </summary>
        /// <param name="direcate"></param>
        /// <returns></returns>
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predirecate);

        /// <summary>
        /// 获取此仓储中所有实体的总和，返回值大于int最大值时使用
        /// </summary>
        /// <returns>实体的总数</returns>
        long LongCount();

        /// <summary>
        /// 获取此仓储的所有实体总和，返回值大于int最大值时使用
        /// </summary>
        /// <returns>实体的总数</returns>
        Task<long> LongCountAsync();

        /// <summary>
        /// 条件筛选统计实体总数，返回值大于int最大值时使用
        /// </summary>
        /// <param name="direcate"></param>
        /// <returns></returns>
        long LongCount(Expression<Func<TEntity, bool>> predirecate);

        /// <summary>
        /// 条件筛选统计实体总数，返回值大于int最大值时使用
        /// </summary>
        /// <param name="direcate"></param>
        /// <returns></returns>
        Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predirecate);

        #endregion 总和计算
    }
}
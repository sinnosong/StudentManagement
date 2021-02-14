using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StudentManagement.Infrastructure.Repositories
{
    /// <summary>
    /// 默认仓储的通用功能实现，用于所有领域模型
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    public class RepositoryBase<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey> where TEntity : class
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        private readonly AppDbContext _dbContext;

        public virtual DbSet<TEntity> Table => _dbContext.Set<TEntity>();

        public RepositoryBase(AppDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public int Count()
        {
            return GetAll().Count();
        }

        public int Count(Expression<Func<TEntity, bool>> predirecate)
        {
            return GetAll().Where(predirecate).Count();
        }

        public async Task<int> CountAsync()
        {
            return await GetAll().CountAsync();
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predirecate)
        {
            return await GetAll().Where(predirecate).CountAsync();
        }

        public void Delete(TEntity entity)
        {
            AttachIfNot(entity);
            Table.Remove(entity);
            Save();
        }

        public void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            foreach (var entity in GetAll().Where(predicate).ToList())
            {
                Delete(entity);
            }
        }

        public async Task DeleteAsync(TEntity entity)
        {
            AttachIfNot(entity);
            Table.Remove(entity);
            await SaveAsync();
        }

        public async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            foreach (var entity in GetAll().Where(predicate).ToList())
            {
                await DeleteAsync(entity);
            }
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }

        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().FirstOrDefaultAsync(predicate);
        }

        public IQueryable<TEntity> GetAll()
        {
            return Table.AsQueryable();
        }

        public List<TEntity> GetAllList()
        {
            return GetAll().ToList();
        }

        public List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).ToList();
        }

        public async Task<List<TEntity>> GetAllListAsync()
        {
            return await GetAll().ToListAsync();
        }

        public async Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().Where(predicate).ToListAsync();
        }

        public TEntity Insert(TEntity entity)
        {
            var newEntity = Table.Add(entity).Entity;
            Save();
            return newEntity;
        }

        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            var entityEntry = await Table.AddAsync(entity);
            await SaveAsync();
            return entityEntry.Entity;
        }

        public long LongCount()
        {
            return GetAll().LongCount();
        }

        public long LongCount(Expression<Func<TEntity, bool>> predirecate)
        {
            return GetAll().Where(predirecate).LongCount();
        }

        public async Task<long> LongCountAsync()
        {
            return await GetAll().LongCountAsync();
        }

        public async Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predirecate)
        {
            return await GetAll().Where(predirecate).LongCountAsync();
        }

        public TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Single(predicate);
        }

        public async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().SingleAsync(predicate);
        }

        public TEntity Update(TEntity entity)
        {
            AttachIfNot(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
            Save();
            return entity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            AttachIfNot(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
            await SaveAsync();
            return entity;
        }

        /// <summary>
        /// 检查实体是否处于跟踪状态，如果是则直接返回，如果不是，则添加跟踪状态
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void AttachIfNot(TEntity entity)
        {
            var entry = _dbContext.ChangeTracker.Entries().FirstOrDefault(ent => ent.Entity == entity);
            if (entry != null)
            {
                return;
            }
            Table.Attach(entity);
        }

        protected void Save()
        {
            _dbContext.SaveChanges();
        }

        protected async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
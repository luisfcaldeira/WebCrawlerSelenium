﻿using Crawler.Domain.Interfaces.Services.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Crawler.Services.Databases.DAL.Repositories
{
    internal class BaseRepository<T> : IBaseRepository<T>, IDisposable where T : class
    {
        private bool disposed = false;
        public DbContext DbContext { get; }
        public DbSet<T> DbSet { get; }

        public BaseRepository(DbContext dbContext)
        {
            DbContext = dbContext;

            DbSet = dbContext.Set<T>();

        }
        public void Add(T entity)
        {
            DbContext.Add(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            Task.Run(() =>
            {
                foreach (T entity in entities)
                {
                    Add(entity);
                }
            });
        }

        public IEnumerable<T> GetAll()
        {
            return DbSet.ToListAsync().Result;
        }

        public T? GetById(object id)
        {
            return DbSet.FindAsync(id).Result;
        }

        public void Delete(object id)
        {
            var entityToDelete = DbSet.FindAsync(id).Result;
            if (entityToDelete != null)
                Delete(entityToDelete);
        }

        public void Delete(T entityToDelete)
        {
            DbSet.Remove(entityToDelete);
        }

        public virtual void Update(T entity)
        {
            try
            {
                DbSet.Attach(entity);
            }
            catch (InvalidOperationException ex)
            {
                if (!ex.Message.Contains("it is already being tracked"))
                    throw;
            }
            DbContext.Entry(entity).State = EntityState.Modified;
        }

        public void Detach(T entity)
        {
            DbContext.Entry(entity).State = EntityState.Detached;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    DbContext.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

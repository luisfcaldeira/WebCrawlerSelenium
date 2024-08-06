using Crawler.Domain.Entities;
using Crawler.Domain.Interfaces.Services.DAL;
using Crawler.Domain.Interfaces.Services.DAL.Repositories;
using Crawler.Services.Databases.Contexts;
using Crawler.Services.Databases.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Crawler.Services.Databases.DAL
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private bool disposed = false;

        private IArticleRepository _articleRepository;
        public IArticleRepository ArticleRepository
        {
            get
            {
                if (_articleRepository == null)
                    _articleRepository = new ArticleRepository(DbContext);

                return _articleRepository;
            }
        }

        private IUrlRepository _urlRepository;
        public IUrlRepository UrlRepository
        {
            get
            {
                if(_urlRepository == null)
                    _urlRepository = new UrlRepository(DbContext);
                return _urlRepository;
            }
        }

        public CrawlerDbContext DbContext { get; }

        public UnitOfWork(CrawlerDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public void Save()
        {
            try
            {
                WaitTransaction();

                using (var trans = DbContext.Database.BeginTransaction())
                {
                    DbContext.SaveChanges();
                    trans.Commit();
                }
            }
            catch (DbUpdateConcurrencyException e)
            {
                foreach (var entry in e.Entries)
                {
                    if (entry.Entity is Url)
                    {
                        var databaseValues = entry.GetDatabaseValues();

                        entry.CurrentValues.SetValues(databaseValues);
                    }
                }
            }
        }

        private void WaitTransaction()
        {
            while (DbContext.Database.CurrentTransaction != null) ;
        }

        public void DisableTracking()
        {
            this.DbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public void EnableTracking()
        {
            this.DbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
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

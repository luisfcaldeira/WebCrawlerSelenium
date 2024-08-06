using Crawler.Domain.Entities;
using Crawler.Domain.Interfaces.Services.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Crawler.Services.Databases.DAL.Repositories
{
    internal class ArticleRepository : BaseRepository<Article>, IArticleRepository
    {
        public ArticleRepository(DbContext dbContext) : base(dbContext)
        {
        }

    }
}

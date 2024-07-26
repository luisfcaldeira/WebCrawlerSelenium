using Crawler.Domain.Entities;
using Crawler.Domain.Interfaces.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Crawler.Services.Databases.DAL.Repositories
{
    internal class UrlRepository : BaseRepository<Url>, IUrlRepository
    {
        public UrlRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public bool Exists(Url url)
        {
            return GetAll().Where(u => u.Equals(url)).Any();
        }
    }
}

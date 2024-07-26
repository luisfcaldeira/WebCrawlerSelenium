using Crawler.Domain.Entities;

namespace Crawler.Domain.Interfaces.DAL.Repositories
{
    public interface IUrlRepository : IBaseRepository<Url>
    {
        bool Exists(Url url);
    }
}

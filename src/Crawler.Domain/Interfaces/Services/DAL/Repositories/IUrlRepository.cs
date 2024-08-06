using Crawler.Domain.Entities;

namespace Crawler.Domain.Interfaces.Services.DAL.Repositories
{
    public interface IUrlRepository : IBaseRepository<Url>
    {
        bool Exists(Url url);
        Url GetUrl(Url url);
        void ClearAllDeadUrl();
    }
}

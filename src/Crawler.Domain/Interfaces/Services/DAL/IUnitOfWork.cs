using Crawler.Domain.Interfaces.Services.DAL.Repositories;

namespace Crawler.Domain.Interfaces.Services.DAL
{
    public interface IUnitOfWork
    {
        IArticleRepository ArticleRepository { get; }
        void Save();
        public void DisableTracking();
        public void EnableTracking();
    }
}

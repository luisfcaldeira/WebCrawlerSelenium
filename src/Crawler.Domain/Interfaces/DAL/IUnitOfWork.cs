using Crawler.Domain.Interfaces.DAL.Repositories;

namespace Crawler.Domain.Interfaces.DAL
{
    public interface IUnitOfWork
    {
        IArticleRepository ArticleRepository { get; }
        void Save();
        public void DisableTracking();
        public void EnableTracking();
    }
}

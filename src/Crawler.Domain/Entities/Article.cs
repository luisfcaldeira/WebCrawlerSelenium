namespace Crawler.Domain.Entities
{
    public class Article
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; private set; }
        public string Content { get; private set; }
        public DateTime Created { get; private set; } = DateTime.Now;
        public Url Url { get; private set; }

        public Article(string title, string content, Url url)
        {
            Title = title;
            Content = content;
            Url = url;
        }

        protected Article()
        {
            
        }
    }
}



namespace Crawler.Domain.Entities
{
    public class Url
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Uri Uri { get; private set; }
        public DateTime Created { get; private set; } = DateTime.Now;
        public bool PrimeiraPagina { get; set; }
        public DateTime? Visited {  get; set; }

        public Url(string value)
        {
            Uri = new Uri(value);
        }

        protected Url()
        {
            
        }

        public bool IsVisited()
        {
            return Visited != null;
        }

        public override bool Equals(object? obj)
        {
            return obj is Url url &&
                   EqualityComparer<Uri>.Default.Equals(Uri, url.Uri);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Uri);
        }

        public override string? ToString()
        {
            return Uri.ToString();
        }
    }
}

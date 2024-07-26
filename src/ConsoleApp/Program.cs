using Crawler.Domain.Entities;
using Crawler.Services.Databases.Contexts;
using Crawler.Services.Databases.DAL;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace Selenium
{
    internal class Program
    {
        static void Main()
        {
            var options = new DbContextOptionsBuilder<CrawlerDbContext>();
            options.UseSqlServer(@"Server=DESKTOP-L0UN16O;Database=WebCrawlerSelenium;User Id=acatc2;Password=acatc2;MultipleActiveResultSets=True;TrustServerCertificate=true");
            var unitOfWork = new UnitOfWork(new CrawlerDbContext(options.Options));

            var driver = new EdgeDriver();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            driver.Navigate().GoToUrl("https://br.investing.com/news/latest-news");

            driver.FindElement(By.ClassName("onetrust-close-btn-handler")).Click();

            var newsList = driver.FindElement(By.XPath("//ul[@data-test='news-list']"));

            var anchors = newsList.FindElements(By.XPath("//a[@data-test='article-title-link']"));

            foreach(var a in anchors)
            {
                try
                {
                    var address = a.GetAttribute("href");

                    var url = new Url(address);
                    url.PrimeiraPagina = true;

                    if(!unitOfWork.UrlRepository.Exists(url))
                    {
                        unitOfWork.UrlRepository.Add(url);
                    }
                }
                catch (StaleElementReferenceException) { }
            }
            unitOfWork.Save();


            var urls = unitOfWork.UrlRepository.GetAll().Where(u => !u.IsVisited());

            foreach (var url in urls)
            {
                url.Visited = DateTime.Now;
                driver.Navigate().GoToUrl(url.ToString());

                var title = driver.FindElement(By.Id("articleTitle")).Text;
                var content = driver.FindElement(By.Id("article")).Text;

                var article = new Article(title, content, url);

                unitOfWork.ArticleRepository.Add(article);
            }

            unitOfWork.Save();
            driver.Quit();
        }
    }
}

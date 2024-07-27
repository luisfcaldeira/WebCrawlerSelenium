using Crawler.Domain.Entities;
using Crawler.Services.Databases.Contexts;
using Crawler.Services.Databases.DAL;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace ConsoleApp
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

            var urls = new List<string>()
            { 
                "https://br.investing.com/news/latest-news",
                "https://br.investing.com/news/economy",
                "https://br.investing.com/news/politics",
            };

            foreach(var url in urls)
            {
                NavigateToUrl(driver, url);

                GetUrls(unitOfWork, driver);

                VisitUrls(unitOfWork, driver);
            }

            driver.Quit();
        }

        private static void NavigateToUrl(EdgeDriver driver, string url)
        {
            driver.Navigate().GoToUrl(url);

            try
            {
                driver.FindElement(By.ClassName("onetrust-close-btn-handler")).Click();

            } 
            catch (Exception) { }

            try
            {
                driver.FindElement(By.XPath("//a[@class='signup_close__usH77']")).Click();
            }
            catch (Exception) { }
        }

        private static void GetUrls(UnitOfWork unitOfWork, EdgeDriver driver)
        {
            var newsList = driver.FindElement(By.XPath("//ul[@data-test='news-list']"));
            var anchors = newsList.FindElements(By.XPath("//a[@data-test='article-title-link']"));

            foreach (var a in anchors)
            {
                try
                {
                    var address = a.GetAttribute("href");

                    var url = new Url(address);
                    url.PrimeiraPagina = true;

                    if (!unitOfWork.UrlRepository.Exists(url))
                    {
                        unitOfWork.UrlRepository.Add(url);
                    }
                }
                catch (StaleElementReferenceException) { }
            }
            unitOfWork.Save();
        }

        private static void VisitUrls(UnitOfWork unitOfWork, EdgeDriver driver)
        {
            var urls = unitOfWork.UrlRepository.GetAll().Where(u => !u.IsVisited());

            foreach (var url in urls)
            {
                url.Visited = DateTime.Now;
                driver.Navigate().GoToUrl(url.ToString());

                var title = driver.FindElement(By.Id("articleTitle")).Text;
                var content = driver.FindElement(By.Id("article")).Text;

                var article = new Article(title, content, url);

                unitOfWork.ArticleRepository.Add(article);

                Task.Delay(300).Wait();
            }

            unitOfWork.Save();
            
        }
    }
}

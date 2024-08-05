using Crawler.Domain.Entities;
using Crawler.Services.Databases.Contexts;
using Crawler.Services.Databases.DAL;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace ConsoleApp
{
    internal class Program
    {
        static void Main()
        {
            var options = new DbContextOptionsBuilder<CrawlerDbContext>();
            //options.UseSqlite(@"Data Source=../../../volume/Scraper.db");
            options.UseNpgsql("server=192.168.68.113;Port=5432;user id=postgres;password = root; database = ScraperSelenium");
            //options.UseSqlServer(@"Server=DESKTOP-L0UN16O;Database=WebCrawlerSelenium;User Id=acatc2;Password=acatc2;MultipleActiveResultSets=True;TrustServerCertificate=true");
            
            var unitOfWork = new UnitOfWork(new CrawlerDbContext(options.Options));

            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--disable-extensions");
            chromeOptions.AddArgument("--headless");
            chromeOptions.AddArgument("--disable-gpu");
            chromeOptions.AddArgument("--no-sandbox");

            var driver = new ChromeDriver(chromeOptions);
            //driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

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

        private static void NavigateToUrl(ChromeDriver driver, string url)
        {
            Console.WriteLine($"navigating to {url}...");
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

        private static void GetUrls(UnitOfWork unitOfWork, ChromeDriver driver)
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

        private static void VisitUrls(UnitOfWork unitOfWork, ChromeDriver driver)
        {
            var urls = unitOfWork.UrlRepository.GetAll().Where(u => !u.IsVisited());

            foreach (var url in urls)
            {
                try
                {
                    Console.WriteLine($"Visiting: {url}");
                    driver.Navigate().GoToUrl(url.ToString());
                    var title = driver.FindElement(By.Id("articleTitle")).Text;
                    var content = driver.FindElement(By.Id("article")).Text;

                    var article = new Article(title, content, url);

                    url.Visited = DateTime.Now;
                    unitOfWork.ArticleRepository.Add(article);
                } catch (Exception)
                {
                    Console.WriteLine($"Ignoring {url}...");
                }

                Task.Delay(300).Wait();
            }

            unitOfWork.Save();
            
        }
    }
}

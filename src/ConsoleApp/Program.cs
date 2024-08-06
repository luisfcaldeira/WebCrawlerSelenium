using ConsoleApp.Services;
using Crawler.Domain.Entities;
using Crawler.Domain.Interfaces.Services.Infra;
using Crawler.Services.Databases.Contexts;
using Crawler.Services.Databases.DAL;
using Crawler.Services.Infra;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace ConsoleApp
{
    internal class Program
    {
        static void Main()
        {
            var appConfig = new AppConfig();

            var options = new DbContextOptionsBuilder<CrawlerDbContext>();
            options.UseSqlite(appConfig.GetValue<string>("SqlLiteConnectionString"));
            
            var unitOfWork = new UnitOfWork(new CrawlerDbContext(options.Options));

            ChromeOptions chromeOptions = new ChromeOptions();

            var browserConfigs = appConfig.GetArray("BrowserDriver");

            foreach(var config in browserConfigs)
            {
                chromeOptions.AddArgument(config);
            }

            var driver = new ChromeDriver(chromeOptions);
            //driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            var urls = appConfig.GetArray("Sites");

            SetAllUrlToDead(unitOfWork);
            
            foreach(var url in urls)
            {

                NavigateToUrl(driver, url);

                GetUrls(unitOfWork, driver);

                VisitUrls(unitOfWork, driver);
            }

            ClearDb(unitOfWork);

            driver.Quit();
        }

        private static void SetAllUrlToDead(UnitOfWork unitOfWork)
        {
            var urls = unitOfWork.UrlRepository.GetAll();

            foreach(var url in urls)
            {
                url.KeepMeAlive = false;
            }

            unitOfWork.Save();
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
                    } else
                    {
                        var dbUrl = unitOfWork.UrlRepository.GetUrl(url);
                        dbUrl.KeepMeAlive = true;
                    }
                }
                catch (StaleElementReferenceException) { }
            }
            unitOfWork.Save();
        }

        private static void VisitUrls(UnitOfWork unitOfWork, ChromeDriver driver)
        {
            var appConfig = new AppConfig();
            var urls = unitOfWork.UrlRepository.GetAll().Where(u => !u.IsVisited());
            IEmailSender emailSender = new EmailSender(appConfig.GetValue<string>("AzureCommunicationConnectionString"));

            foreach (var url in urls)
            {
                try
                {
                    Console.WriteLine($"Visiting: {url}");
                    driver.Navigate().GoToUrl(url.ToString());
                    var title = driver.FindElement(By.Id("articleTitle")).Text;
                    var content = driver.FindElement(By.Id("article")).Text;
                    var article = new Article(title, content, url);

                    unitOfWork.ArticleRepository.Add(article);
                    emailSender.SendEmail(
                        toEmail: appConfig.GetValue<string>("EmailAdmin"), 
                        sender: appConfig.GetValue<string>("EmailSender"), 
                        subject: article.Title, 
                        message: "<h1>" + article.Title + "</h1>" + 
                                url + 
                                "<br>" + 
                                article.Content
                        , planText: article.Content);

                    url.Visited = DateTime.Now;

                } catch (Exception)
                {
                    Console.WriteLine($"Ignoring {url}...");
                }

                Task.Delay(300).Wait();
            }

            unitOfWork.Save();
            
        }

        private static void ClearDb(UnitOfWork unitOfWork)
        {
            unitOfWork.UrlRepository.ClearAllDeadUrl();
            unitOfWork.Save();
        }

    }
}

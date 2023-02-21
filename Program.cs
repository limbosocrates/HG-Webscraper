using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Net;

namespace HG_WebScraper
{
    internal class Program
    {
        private static bool debugMode = false; //for testing; won't save to file if true
        static void Main(string[] args)
        {
            Console.WriteLine("Starting");
            try
            {
                IConfiguration Configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddCommandLine(args)
                    .Build();

                var appConfig = Configuration.GetSection(AppConfiguration.Name).Get<AppConfiguration>();

                Console.WriteLine($"Processing file {appConfig.File}");
                bool running = true;
                while (running)
                {
                    var html = File.ReadAllText(appConfig.File);
                    StreamWriter output;
                    if(debugMode)
                        output = new StreamWriter(new MemoryStream());
                    else
                        output =new StreamWriter( File.Create(appConfig.OutputFile));
                    
                   // var options = new ChromeOptions()
                   // {
                   //     BinaryLocation = "C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe"
                   // };

                   // options.AddArguments(new List<string>() { "headless", "disable-gpu" });
                   // var browser = new ChromeDriver(options);
                   // browser.Navigate().GoToUrl(appConfig.SiteUrl);
                   // //WebDriverWait wait = new WebDriverWait(browser, TimeSpan.FromSeconds(10)); 
                   // //IWebElement firstResult = wait.Until(ExpectedConditions.ElementExists(By.Id("product-search-table")));
                   // Thread.Sleep(5000);

                   //var elms= browser.FindElements(By.XPath("//*[@class='row product-listing']"));
                   // var html = browser.PageSource;

                   // File.WriteAllText(@"c:\temp\test.html", html);

                    //HtmlWeb web = new HtmlWeb();

                    //HtmlDocument doc = web.Load(appConfig.SiteUrl);

                    HtmlDocument doc = new HtmlDocument(); 
                    doc.LoadHtml(html);

                    var listing = doc.DocumentNode.SelectNodes("//*[@class='row product-listing']");
                    foreach (HtmlNode item in listing)
                    {
                        var subCategory = item.SelectSingleNode("(.//preceding::div[@class='product-catalog-second-category'])[last()]")?.InnerText;
                        var category= item.SelectSingleNode("(.//preceding::div[@class='product-catalog-category product-category-divider'])[last()]")?.InnerText; 
                        var detailsNode = item.SelectSingleNode(".//*[@class='product-details']");

                        var rows = item.SelectNodes(".//tbody/tr");
                        foreach (var row in rows)
                        {
                            var descNode = row.SelectSingleNode("./th/div/a");
                            if (descNode == null) continue;
                            var priceNode = row.SelectSingleNode(".//span");
                            string line = $"{category};{subCategory};{detailsNode.FirstChild.InnerText};{descNode.InnerText};{priceNode.InnerText}";
                            line=WebUtility.HtmlDecode(line);
                            //Console.WriteLine(line);
                            output.WriteLine(line);
                        }
                    }

                    if(!debugMode)
                        Console.WriteLine($"Output written to {appConfig.OutputFile}");

                    output.Dispose();
                    running = false;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }

    //TODO: Customize configuration class
    public class AppConfiguration
    {
        public const string Name = "App";

        public string SiteUrl { get; set; }
        public string File { get; set; }
        public string OutputFile { get; set; }
    }
}

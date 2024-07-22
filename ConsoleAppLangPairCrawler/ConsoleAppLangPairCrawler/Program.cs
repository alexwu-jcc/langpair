using ClsLibWebContentScraper;
using ConsoleAppLangPairCrawler;
using log4net;
using ClsLibWebContentScraper.utils;


namespace LangPairCrawlerApp
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        static async Task Main(string[] args)
        {
           //await uncoverRelatUrls();
            await runCrawler();

        }

        static async Task uncoverRelatedUrls()
        {
            log.Info("Application started");

            var links = ConfigManager.GetLinks();
            foreach (var link in links)
            {
                IUrlUncoverer urlUncoverer = new UrlUncoverer();
                var sinkFile = ConfigManager.GetPageLinkSink();
                await urlUncoverer.getEmbeddedPageLinks(link, sinkFile);
            }

            log.Info("Application ended");
        }

        static async Task runCrawler()
        {
            log.Info("Application started");

            JobPayload payload = new JobPayload(ConfigManager.GetLanguagePairsSupported(), ConfigManager.GetLinks());
            List<string> pageLinks;
            if (CommonUtils.isPageLinkPrepolutated(out pageLinks))
            {
                payload.pageLinks = pageLinks;
            }

            
            var crawler = new LangPairCrawler();
            await crawler.CrawlAndSave(payload);

            log.Info("Application ended");
        }
    }
  
             
}

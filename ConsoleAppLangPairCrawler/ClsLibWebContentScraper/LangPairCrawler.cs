using HtmlAgilityPack;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ClsLibWebContentScraper
{ 
    public class LangPairCrawler
    {
        

        private static readonly ILog log = LogManager.GetLogger(typeof(LangPairCrawler));


        private string outputFolder = "output";
        private static string mainSelector = "//body//div[contains(@class, 'dialog-off-canvas-main-canvas')]//main";
        private static string urlSelector = "//body//div[contains(@class, 'dialog-off-canvas-main-canvas')]//main//div[contains(@class, 'page__content')]//article//div[contains(@class, 'jcc-text-section')]//div[contains(@class, 'jcc-text-section__column-left')]";

        public LangPairCrawler()
        {
            outputFolder = Path.Combine(outputFolder, DateTime.Now.ToString("MM-dd"));

            string currentDirectory = Directory.GetCurrentDirectory();
            int backWardCount = 3;
            while(backWardCount > 0)
            {
                currentDirectory = currentDirectory.Substring(0, currentDirectory.LastIndexOf("\\"));
                backWardCount--;
            }
            outputFolder = Path.Combine(currentDirectory, outputFolder);
            Directory.CreateDirectory(outputFolder);
            log.Info($"Output directory created: {outputFolder}");
        }

        public async Task CrawlAndSaveHelper(string url)
        {

            HttpClient client = new HttpClient();
                       

            var response = await client.GetStringAsync(url);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(response);

            var part = doc.DocumentNode.SelectSingleNode(urlSelector);
            if(part == null)
            {
                return;
            }

            var anchorTags = part.SelectNodes(".//a[@href]");
            var urlSet = new HashSet<string>();
            foreach (var anchor in anchorTags)
            {
                string href = anchor.GetAttributeValue("href", string.Empty);
                if (!href.StartsWith("http"))
                {
                    urlSet.Add("https://selfhelp.courts.ca.gov" + href);
                    Console.WriteLine("url added:" + "https://selfhelp.courts.ca.gov" + href);
                }
            }


            var alldata = new HashSet<(string, string)>();
            int urlCount = urlSet.Count;
            int currentCount = 1;
            foreach (var u in urlSet)
            {
                log.Info($"Processing URL {currentCount} / {urlCount}: {u}");
                currentCount++;
                var pageResponse = await client.GetStringAsync(u);
                var pageDoc = new HtmlDocument();
                pageDoc.LoadHtml(pageResponse);

                var dataNode = pageDoc.DocumentNode.SelectSingleNode(mainSelector);
                string data = dataNode.InnerText;

                var findEsNode = pageDoc.DocumentNode.SelectSingleNode("//link[@hreflang='es']");
                if (findEsNode != null)
                {
                    string esUrl = findEsNode.GetAttributeValue("href", string.Empty);
                    var responseEs = await client.GetStringAsync(esUrl);
                    var esDoc = new HtmlDocument();
                    esDoc.LoadHtml(responseEs);

                    var dataEsNode = esDoc.DocumentNode.SelectSingleNode(mainSelector);
                    string dataEs = dataEsNode.InnerText;

                    var linesEn = data.Split('\n');
                    var linesEs = dataEs.Split('\n');
                    int numLines = Math.Min(linesEn.Length, linesEs.Length);

                    for (int i = 0; i < numLines; i++)
                    {
                        string stren = LangPairCrawlerHelpers.Treat(linesEn[i], 1);
                        string stres = LangPairCrawlerHelpers.Treat(linesEs[i], 2);
                        if (stren != stres && stren.Split(' ').Length > 1)
                        {
                            alldata.Add((stren, stres));
                        }
                    }
                }

                if(alldata.Count <= 0)
                {
                    return;
                }

                string timenow = DateTime.Now.ToString("MM-dd-HH-mm-ss");
                string filename = Path.Combine(outputFolder, $"parsed_{timenow}");
                string[] filenames = { filename + "_e.txt", filename + "_s.txt" };
                log.Info($"Saving data to: {filename}");
                using (StreamWriter fe = new StreamWriter(filenames[0], false, System.Text.Encoding.UTF8))
                using (StreamWriter fs = new StreamWriter(filenames[1], false, System.Text.Encoding.UTF8))
                {
                    foreach (var dataPair in alldata)
                    {
                        fe.WriteLine(dataPair.Item1);
                        fs.WriteLine(dataPair.Item2);
                    }
                }
                log.Info("Data saved successfully");
            }
        }

        public async Task CrawlAndSave(JobPayload payload)
        {
            int mode = 2;

            switch (mode)
            {
                case 1:
                    Console.WriteLine("case 1");
                    foreach (var url in payload.sourceUrlList)
                    {
                        await CrawlAndSaveHelper(url);
                    }
                    break;
                case 2:
                    Console.WriteLine("case 2");
                    foreach (var url in payload.PageLinks)
                    {
                        await CrawlAndSaveHelper(url);
                    }
                    break;

                default:
                    break;

            }
        }
    }
}

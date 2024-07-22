using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System;
using System.Linq;
using System.Security.Policy;
using ClsLibWebContentScraper;
using System.ComponentModel.Design;
using System.Diagnostics;

namespace ClsLibWebContentScraper
{
    public class UrlUncoverer : IUrlUncoverer
    {
        public async Task getEmbeddedPageLinks(string url, string resultingUrlSinkFile)
        {
            url = "https://selfhelp.courts.ca.gov/small-claims-california";

            var resultingSet = new HashSet<string>();
         

            HttpClient client = new HttpClient();
            var response = await client.GetStringAsync(url);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(response);
            var nodes1 = doc.DocumentNode.SelectNodes("//*//a[starts-with(@href, 'http')]");
            var nodes2 = doc.DocumentNode.SelectNodes("//*//a[starts-with(@href, 'https')]");
            var nodes = doc.DocumentNode.SelectNodes("//a[@href]");
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    string tempUrl = node.GetAttributeValue("href", "");
                    if (tempUrl != null)
                    {
                        if (!tempUrl.StartsWith("http://") && !tempUrl.StartsWith("https://"))
                        {
                            tempUrl = url + tempUrl;
                            resultingSet.Add(tempUrl);
                        }
                        else
                        {
                            resultingSet.Add(tempUrl);
                        }
                    }
                }
                
                if(resultingSet.Count > 0)
                {
                    var lastIndex = resultingUrlSinkFile.LastIndexOf("\\");
                    var dir = resultingUrlSinkFile.Substring(0, lastIndex);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }


                    using (var sw = new StreamWriter(resultingUrlSinkFile))
                    {
                       
                        foreach (var r in resultingSet.OrderBy(x => x).ToList())
                        {
                            sw.WriteLine(r);
                        }
                    }
                }
            }
            else
            {
                Debug.WriteLine("No matching nodes found.");
            }

            return;
        }
    }


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClsLibWebContentScraper
{
    public struct JobPayload
    {
        public List<String> langPairList;
        public List<String> sourceUrlList;
        public List<String> pageLinks;

        public JobPayload(List<String> langPairList, List<String> urlList, List<string>pageLinks)
        {
            this.langPairList = new List<string>(langPairList);
            this.sourceUrlList = new List<String>(urlList);
            this.pageLinks = pageLinks;
        }
        public JobPayload(List<String> langPairList, List<String> urlList)
        {
            this.langPairList = new List<string>(langPairList);
            this.sourceUrlList = new List<String>(urlList);
            this.pageLinks = new List<String>();
        }

        public List<String> PageLinks
        {
            get
            {
                return this.pageLinks;
            }
            set
            {
                this.pageLinks = new List<string>();
                this.pageLinks.AddRange(value);
            }
        }
    }
}

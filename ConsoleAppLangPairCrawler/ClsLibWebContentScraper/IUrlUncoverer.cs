using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClsLibWebContentScraper
{
    public interface IUrlUncoverer
    {
        Task getEmbeddedPageLinks(string url, string resultingUrlSinkFile);
    }
}

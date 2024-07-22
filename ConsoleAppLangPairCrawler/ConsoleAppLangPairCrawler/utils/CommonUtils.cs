using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppLangPairCrawler.utils
{
    internal class CommonUtils
    {

        public static bool isPageLinkPrepolutated(out List<string>content)
        {
            List<string> cleansedLines = new List<string>();
            string filePath = ConfigManager.GetPageLinkSink();
            if (filePath != null && filePath.Length > 0 && File.Exists(filePath))
            {
                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Length > 0)
                {  
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            line = line.Trim();
                            if (!string.IsNullOrEmpty(line))
                            {
                                cleansedLines.Add(line);
                            }
                        }
                    }
                }  
            }
            content = cleansedLines;
            return content.Count > 0? true: false;

        }

    }
}

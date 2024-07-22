using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.IO;



namespace ClsLibWebContentScraper.utils
{
    public class CommonUtils
    {
        public static bool cleanUpFolder(string pathToFolder)
        {
            bool res = false;
            try
            {
                string[] files = Directory.GetFiles(pathToFolder);
                foreach (string file in files)
                {
                    File.Delete(file);
                    Console.WriteLine($"Deleted: {file}");
                }
                Console.WriteLine("All files deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            return res;
        }
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

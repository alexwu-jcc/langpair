using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Configuration;

using log4net;

namespace ClsLibWebContentScraper.utils
{
    
    class ConfigManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ConfigManager));


        public static string GetPageLinkSink()
        {
            return GetAppSettingStringValue("PageLinkSink");
        }

        public static List<string> GetLanguagePairsSupported()
        {
            return GetAppSettingValues("LanguagePairsSupported");
        }


        public static List<string> GetLinks()
        {
            return GetAppSettingValues("PageLinksToExplore");
        }

        static string GetAppSettingStringValue(string key)
        {
            try
            {
                return ConfigurationManager.AppSettings[key];
            }
            catch (ConfigurationErrorsException ex)
            {
                throw new Exception("Error reading app settings: " + ex.Message);
            }
        }
        static List<string> GetAppSettingValues(string key)
        {
            try
            {
                string value = ConfigurationManager.AppSettings[key];
                if (value != null)
                {
                    return new List<string>(value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                }
            }
            catch (ConfigurationErrorsException ex)
            {
                throw new Exception("Error reading app settings: " + ex.Message);
            }

            return new List<string>();

        }
    }


}

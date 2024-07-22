using System.Runtime.Serialization;


namespace ConsoleAppLangPairCrawler
{
    [Serializable]
    internal class LangPairCrawlerException : Exception
    {

        public LangPairCrawlerException() : base() { }


        public LangPairCrawlerException(string message) : base(message) { }


        public LangPairCrawlerException(string message, Exception innerException) : base(message, innerException) { }


        protected LangPairCrawlerException(SerializationInfo info, StreamingContext context) : base(info, context) { }


        public int ErrorCode
        {
            get; set;

        }

    }
}



    
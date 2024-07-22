internal static class LangPairCrawlerHelpers
{

    public static string Treat(string s, int langId)
    {
        string res = s.Trim().Replace("\xa0", " ");
        string sa = s.Trim();
        if (sa.Length >= 2 && sa[1] == '.' && char.IsDigit(sa[0]))
        {
            if(langId == 1)
            {
                return sa.Substring(2).Replace("e&#39;", ","); // filter out "'" as it appears in "I'll"
            }

            return sa.Substring(2);

        }

        return res;
    }
}
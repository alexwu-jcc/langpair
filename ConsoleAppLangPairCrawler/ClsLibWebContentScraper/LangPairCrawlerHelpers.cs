internal static class LangPairCrawlerHelpers
{

    public static string Treat(string s)
    {
        s = s.Trim().Replace("\xa0", " ");
        string sa = s.Trim();
        if (sa.Length >= 2 && sa[1] == '.' && char.IsDigit(sa[0]))
        {
            return sa.Substring(2);
        }
        return s;
    }
}
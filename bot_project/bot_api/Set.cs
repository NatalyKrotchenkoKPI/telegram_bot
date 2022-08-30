using System.Net;

namespace bot_api
{
    public class Set
    {
        public static string SetStr(string A)
        {
            return "{"+'"' + "BookList" + '"' + ":[" + A.Replace("}{", "},{") + "]}";
        }
    }
}

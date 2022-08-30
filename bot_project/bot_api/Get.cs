using System.Net;

namespace bot_api
{
    public class Get
    {
        public static string GetStr(string A)
        {

            System.Net.HttpWebRequest request = (HttpWebRequest)WebRequest.Create(A);

            request.Method = "GET";

            WebResponse response = request.GetResponse();

            Stream s = response.GetResponseStream();

            StreamReader reader = new StreamReader(s);

            string answer = reader.ReadToEnd();

            response.Close();

            return answer;
        }
    }
}

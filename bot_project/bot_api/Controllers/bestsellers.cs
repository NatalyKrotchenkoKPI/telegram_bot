using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace bot_api.Controllers
{   //https://localhost:7296/bestsellers/genre=history
    [Route("[controller]")]
    [ApiController]
    public class bestsellersGenre : ControllerBase
    {
        [HttpGet("/bestsellers/genre={genre}")]
        public string GetInfo(string genre)
        {
            string jsonString = $"https://api.nytimes.com//svc/books/v3/lists/best-sellers/{genre}.json?api-key=DyPanM1ZfgALaAEzxYZpYXgh2FJrrxu5";
            JObject googleSearch = JObject.Parse(Get.GetStr(jsonString));
            IList<JToken> results = googleSearch["results"].Children().ToList();
            var options = new JsonSerializerOptions { WriteIndented = true };
            string response = "";
            foreach (var result in results)
            {
                Bestsellers bestseller = result.ToObject<Bestsellers>();
                ResponseBestsellers serializeresponse = new ResponseBestsellers
                {
                    title = bestseller.title.Replace("\u0027", ""),
                    author = bestseller.author
                };
                response += JsonSerializer.Serialize(serializeresponse, options);
            }            
            return Set.SetStr(response);
        }
    }

    public class Bestsellers
    {
        public string title { get; set; }
        public string author { get; set; }
    }
    public class ResponseBestsellers
    {
        public string title { get; set; }
        public string author { get; set; }
    }
}
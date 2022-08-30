using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace bot_api.Controllers
{   //https://localhost:7296/books/genre=love
    [Route("[controller]")]
    [ApiController]
    public class byGenre : ControllerBase
    {
        [HttpGet("/books/genre={genre}")]
        public string GetInfo(string genre)
        {
            string jsonString = $"https://openlibrary.org/subjects/{genre}.json";
            JObject googleSearch = JObject.Parse(Get.GetStr(jsonString));
            IList<JToken> results = googleSearch["works"].Children().ToList();
            var options = new JsonSerializerOptions { WriteIndented = true };
            string response = "";
            foreach (var result in results)
            {
                Books_ book = result.ToObject<Books_>();
                ResponseBook serializeresponse = new ResponseBook
                {
                    title = book.title.Replace("\u00E9", ""),
                    author = book.authors[0].name
                };
                    response += JsonSerializer.Serialize(serializeresponse, options);
            }
            return Set.SetStr(response);
        }
    }



    public class Books_
    {
        public string title { get; set; }
        public List <authors> authors { get; set; }
    }
    public class authors
    {
        public string name { get; set; }
    }

    public class ResponseBook
    {
        public string title { get; set; }
        public string author { get; set; }
    }


}
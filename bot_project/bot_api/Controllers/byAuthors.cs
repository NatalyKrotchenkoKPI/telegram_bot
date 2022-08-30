using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace bot_api.Controllers
{   //https://localhost:7296/books/author=Tolstoy OL26783A
    [Route("[controller]")]
    [ApiController]
    public class byAuthors : ControllerBase
    {
        [HttpGet("/books/author={surname}")]
        public string GetInfo(string surname)
        {
            string jsonString = $"https://openlibrary.org/search/authors.json?q={surname}";
            JObject googleSearch = JObject.Parse(Get.GetStr(jsonString));
            IList<JToken> results = googleSearch["docs"].Children().ToList();
            byAuthor searchResult = results[0].ToObject<byAuthor>();
            string authorKey = searchResult.key;

            string jsonString1 = $"https://openlibrary.org/authors/{authorKey}/works.json";
            JObject googleSearch1 = JObject.Parse(Get.GetStr(jsonString1));
            IList<JToken> results1 = googleSearch1["entries"].Children().ToList();

            var options = new JsonSerializerOptions { WriteIndented = true };
            string response = "";
            foreach (var result in results1)
            {
                Entires entires = result.ToObject<Entires>();
                ResponseBookAuthor serializeresponse = new ResponseBookAuthor
                {
                    title = entires.title
                };
                    response += JsonSerializer.Serialize(serializeresponse, options);
            }
            return Set.SetStr(response);
        }
    }

    public class byAuthor
    {
        public string key { get; set; }
    }

    public class Entires
    {
        public string title { get; set; }
    }

    public class ResponseBookAuthor
    {
        public string title { get; set; }
    }


}
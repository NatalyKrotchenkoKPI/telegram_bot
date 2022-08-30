using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace bot_api.Controllers
{   //https://localhost:7296/recommendBook/id=exampleId3
    [Route("[controller]")]
    [ApiController]
    public class recommendBookController : ControllerBase
    {
        [HttpGet("/recommendBook/id={id}")]
        public string GetInfoGenre(string id)
        {
            var jSon = Set.SetStr(System.IO.File.ReadAllText(@"res/loc.txt"));
            var books = JObject.Parse(jSon)["BookList"].ToObject<List<MyBooks>>();
            var options = new JsonSerializerOptions { WriteIndented = true };
            string response = "";
            foreach (var book in books)
            {
                if (book.Id == id)
                {
                    if (book.Genre != "")
                    {
                        
                        string jsonString = $"https://openlibrary.org/subjects/{book.Genre}.json";
                        JObject googleSearch = JObject.Parse(Get.GetStr(jsonString));
                        IList<JToken> results = googleSearch["works"].Children().ToList();

                        Random sluchainaya_kniga = new Random();
                        int kng = sluchainaya_kniga.Next(1, results.Count - 1);
                        Books_ book_ = results[kng].ToObject<Books_>();
                        ResponseRecommendBook serializeresponse = new ResponseRecommendBook
                        {
                            title = book_.title,
                            author = book_.authors[0].name
                        };
                        response += JsonSerializer.Serialize(serializeresponse, options);
                        break;
                    }
                    else
                    if (book.Author != "")
                    {
                        string jsonString = $"https://openlibrary.org/search/authors.json?q={book.Author}";
                        JObject googleSearch = JObject.Parse(Get.GetStr(jsonString));
                        IList<JToken> results = googleSearch["docs"].Children().ToList();
                        byAuthor searchResult = results[0].ToObject<byAuthor>();
                        string authorKey = searchResult.key;

                        string jsonString1 = $"https://openlibrary.org/authors/{authorKey}/works.json";
                        JObject googleSearch1 = JObject.Parse(Get.GetStr(jsonString1));
                        IList<JToken> results1 = googleSearch1["entries"].Children().ToList();

                        Entires entires = results1[0].ToObject<Entires>();
                        ResponseRecommendBook serializeresponse = new ResponseRecommendBook
                        {
                            title = entires.title,
                            author = book.Author
                        };
                        response += JsonSerializer.Serialize(serializeresponse, options);
                        break;
                    }
                }
            }

            return response;

        }
    }



    public class Books_1
    {
        public List<checkedBooks> BookList { get; set; }
    }
    public class checkedBooks
    {
        public string Title { get; set; }
        public string Id { get; set; }
        public string Author { get; set; }
        public string Rate { get; set; }
        public string Genre { get; set; }
    }

    public class ResponseRecommendBook
    {
        public string title { get; set; }
        public string author { get; set; }
    }


}
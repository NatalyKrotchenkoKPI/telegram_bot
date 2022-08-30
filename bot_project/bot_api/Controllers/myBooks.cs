using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace bot_api.Controllers
{   //https://localhost:7296/myBoks/id=ExampleId
    [Route("[controller]")]
    [ApiController]
    public class myBooksController : ControllerBase
    {
        [HttpGet("/myBoks/id={id}")]
        public string GetInfoGenre(string id)
        {
            var jSon = Set.SetStr(System.IO.File.ReadAllText(@"res/loc.txt"));
            var books = JObject.Parse(jSon)["BookList"].ToObject<List<MyBooks>>();
            var options = new JsonSerializerOptions { WriteIndented = true };
            string response = "";
            foreach(var book in books)
            {
                if (book.Id == id)
                {
                    myBooksResponse serializeresponse = new myBooksResponse
                    {
                        Title = book.Title,
                        Author = book.Author,
                        Rate = book.Rate,
                        Genre = book.Genre
                    };
                    response += System.Text.Json.JsonSerializer.Serialize(serializeresponse, options);
                }
            }
            return Set.SetStr(response);
        }

    }

    public class myBooksResponse
    {
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public string Rate { get; set; } = "";
        public string Genre { get; set; } = "";
    }


}
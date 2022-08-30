using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System;
using System.IO;
using System.Text;

namespace bot_api.Controllers
{
    //https://localhost:7296/addToList/id=exampleId/author=ExampleAuthor/genre=ExampleGenre/title=ExampleTitle/rate=ExampleRate
    [Route("api/[controller]")]
    [ApiController]
    public class addToListController : Controller
    {
        [HttpGet("/addToList/id={id}/author={author}/genre={genre}/title={title}/rate={rate}")]//
        public void Get(string id, string author, string genre, string title, string rate)//
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var books = new MyBooks
            {Title = title,
                Genre = genre,
                Rate = rate,
                Author = author,
                Id = id};
            string response = JsonSerializer.Serialize(books, options);
           
            StreamWriter sw = new StreamWriter(@"res/loc.txt", true);
            sw.Write(response);
            sw.Close();
            
        }
    }
    public class bookList
    {
        public List <MyBooks> BookList { get; set; }
    }
    public class MyBooks
    {
        public string Title { get; set; }
        public string Id { get; set; }
        public string Author { get; set; }
        public string Rate { get; set; }
        public string Genre { get; set; }
    }
}
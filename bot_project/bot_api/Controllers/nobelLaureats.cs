using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace bot_api.Controllers
{   //https://localhost:7296/nobelLaureats/year=2018
    [Route("[controller]")]
    [ApiController]
    public class nobelLaureats : ControllerBase
    {
        [HttpGet("/nobelLaureats/year={year}")]
        public string GetInfo(string year)
        {
            string jsonString = $"https://api.nobelprize.org/v1/prize.json?year={year}&yearTo={year}&category=literature";
            JObject googleSearch = JObject.Parse(Get.GetStr(jsonString));
            IList<JToken> results = googleSearch["prizes"].Children().ToList();

            SearchPrizes searchResult = results[0].ToObject<SearchPrizes>();
            Response serializeresponse = new Response
            {
                name = searchResult.laureates[0].firstname,
                surname = searchResult.laureates[0].surname,
                motivation = searchResult.laureates[0].motivation
            };
            var options = new JsonSerializerOptions { WriteIndented = true };
            string response = JsonSerializer.Serialize(serializeresponse, options);
            return response;
        }
    }
    public class SearchPrizes
    {
        public List<laureates> laureates { get; set; }
    }
    public class laureates
    {
        public string firstname { get; set; }
        public string surname { get; set; }
        public string motivation { get; set; }
    }
    public class Response
    {
        public string name { get; set; }
        public string surname { get; set; }
        public string motivation { get; set; }

    }
}
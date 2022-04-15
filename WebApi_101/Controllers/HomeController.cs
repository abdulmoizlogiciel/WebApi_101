using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace WebApi_101.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;
        public HomeController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }


        [HttpGet("Test2")]
        public async Task<string> Test2Action()
        {
            var watch = new Stopwatch();

            watch.Start();
            //await Test2.Main2();
            await Task.Run(() => Task.Delay(5000));
            watch.Stop();

            return "response asdf: " + watch.ElapsedMilliseconds;
        }



        [HttpGet]
        public string Get()
        {
            return _memoryCache.Get("key1").ToString();
        }



        [HttpPost]
        public void Post([FromForm] Model2 model2)
        {
            var result = new StringBuilder();
            using (var reader = new StreamReader(model2.TheFile.OpenReadStream()))
            {
                using var ms = new MemoryStream();

                model2.TheFile.CopyTo(ms);

                while (reader.Peek() >= 0)
                    result.AppendLine(reader.ReadLine());
            }

            string fileContent = result.ToString();
        }
    }

    public class Model2
    {
        public IFormFile TheFile { get; set; }
    }
}

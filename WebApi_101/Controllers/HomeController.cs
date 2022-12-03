using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WebApi_101.Attributes;

namespace WebApi_101.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        List<IDictionary<string, object>> _list = new List<IDictionary<string, object>>() { new Dictionary <string, object>{
                { "Name1", "Value1" }
            } };

        List<Model1> _list2 = new List<Model1>()
        {
            new Model1{ Prop1= "val1" },
        };

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

        [HttpGet("datetimeresponse")]
        public ActionResult DateTimeResponse()
        {
            string strTime = "2022-12-15T23:48:56Z";

            return Ok(new[]
            {
                new DateTime(2022, 12, 15, 23, 23, 56, 0),
                new DateTime(2022, 12, 15, 23, 23, 56, 8),
                new DateTime(2022, 12, 15, 23, 23, 56, 45),
                new DateTime(2022, 12, 15, 23, 23, 56, 457),
                DateTime.Parse(strTime).ToUniversalTime(),
                DateTime.Parse(strTime),
            });
        }

        [HttpGet("testing1")]
        //[NoPropertyNamingPolicy]
        public object Testing1()
        {
            return _list2;
        }

        [HttpGet("testing2")]
        [PascalCasePropertyNamingPolicy]
        public object Testing2()
        {
            return _list2;
        }
    }

    public class Model2
    {
        public IFormFile TheFile { get; set; }
    }

    public class Model1
    {
        public string Prop1 { get; set; }
    }
}

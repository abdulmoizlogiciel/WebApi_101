using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;


namespace WebApi_101.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly ISomeService _someService;
        private readonly IMemoryCache _memoryCache;


        public WeatherForecastController(ILogger<WeatherForecastController> logger, ISomeService someService, IMemoryCache memoryCache)
        {
            _logger = logger;
            _someService = someService;
            _memoryCache = memoryCache;
        }

        [HttpGet("casting")]
        public string Casting()
        {
            object weatherForecast = new WeatherForecast();
            var limit = 99999999;

            Stopwatch stopwatch1 = new Stopwatch();
            stopwatch1.Start();
            for (int index = 0; index < limit; index++)
            {
                var a = (WeatherForecast)weatherForecast;
            }
            stopwatch1.Stop();


            Stopwatch stopwatch2 = new Stopwatch();
            stopwatch2.Start();
            for (int index = 0; index < limit; index++)
            {
                var a = weatherForecast as WeatherForecast;
            }
            stopwatch2.Stop();

            return $" 1: {stopwatch1.ElapsedMilliseconds}, \n 2: {stopwatch2.ElapsedMilliseconds}";
        }

        [HttpGet("save/{data}")]
        public string Save(string data)
        {
            _someService.SetPersisted(data);
            _memoryCache.Set("key1", data);
            return $"{data} success";
        }

        [HttpGet("set/{data}")]
        public string SET(string data)
        {
            _someService.SetNow(data);
            return $"{data} success";
        }

        [HttpGet("get")]
        public IActionResult GetPersisted()
        {
            return Ok(new RestResponse { ResponseMessage = $"{_someService.GetPersisted()}  .... {_memoryCache.Get("key1")}" });
            //return Ok(new RestResponse { ResponseMessage = _someService.GetStaticVariable() });
        }
        private class RestResponse
        {
            public string ResponseMessage { get; set; }
        }

        [HttpGet]
        //[Authorize]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("GetJwtToken")]
        public async Task<string> GetJwtTokenAsync()
        {
            TokenResponse tokenResponse;
            {
                using var client = new HttpClient();

                var discoveryDocument = await client.GetDiscoveryDocumentAsync(Config.IdentityServerUrl);

                var clientCredentialsTokenRequest = new ClientCredentialsTokenRequest
                {
                    Address = discoveryDocument.TokenEndpoint,

                    ClientId = "m2m.client",
                    ClientSecret = "SuperSecretPassword",
                    Scope = "weatherapi.read",
                };

                tokenResponse = await client.RequestClientCredentialsTokenAsync(clientCredentialsTokenRequest);
            }

            var token = tokenResponse.AccessToken;


            using var client2 = new HttpClient();

            client2.SetBearerToken(token);

            var a = await client2.GetAsync("https://localhost:44319/weatherforecast");

            return token;
        }
    }
}

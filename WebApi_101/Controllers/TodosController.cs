using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi_101.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TodosController : ControllerBase
    {
        [HttpPost]
        public ModelApiPost Post(ModelApiPost modelApiPost)
        {
            return modelApiPost;
        }

        [HttpGet]
        public string Get()
        {
            string customer_id = null;
            if (Request.Headers.TryGetValue("Authorization", out StringValues authorization))
            {
                var token = new JwtSecurityToken(authorization[0].Split(' ')[1]);
                customer_id = token.Claims.First(x => x.Type.Equals("customer_id")).Value;
            }

            return $"Hello :{customer_id} from: {HttpContext.Connection.LocalPort}";
        }

        [HttpGet("{id}")]
        public string GetWithId(string id)
        {
            return "Hello from dotnet." + id;
        }
    }

    public class ModelApiPost
    {
        public string Id { get; set; }
    }
}

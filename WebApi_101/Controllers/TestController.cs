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
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet("todos")]
        public string Todos()
        {
            string customer_id = null;
            if (Request.Headers.TryGetValue("Authorization", out StringValues authorization))
            {
                var token = new JwtSecurityToken(authorization.First().Split(' ')[1]);
                customer_id = token.Claims.First(x => x.Type.Equals("customer_id")).Value;
            }

            return $"Hello :{customer_id} from Todos: {HttpContext.Connection.LocalPort}";
        }

        [HttpGet("posts")]
        public string Posts()
        {
            string customer_id = null;
            if (Request.Headers.TryGetValue("Authorization", out StringValues authorization))
            {
                var token = new JwtSecurityToken(authorization.First().Split(' ')[1]);
                customer_id = token.Claims.First(x => x.Type.Equals("customer_id")).Value;
            }

            return $"Hello :{customer_id} from Posts: {HttpContext.Connection.LocalPort}";
        }
    }
}

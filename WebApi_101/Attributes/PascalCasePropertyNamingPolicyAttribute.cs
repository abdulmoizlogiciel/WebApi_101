using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Text.Json;

namespace WebApi_101.Attributes
{
    /// <summary>
    /// Pascal naming convention, acheived by combinig the following sources:
    /// https://stackoverflow.com/a/52623772/8075004
    /// https://stackoverflow.com/a/64210540/8075004
    /// https://stackoverflow.com/a/59701123/8075004
    /// </summary>
    public class PascalCasePropertyNamingPolicyAttribute : ActionFilterAttribute
    {
        private static readonly SystemTextJsonOutputFormatter SSystemTextJsonOutputFormatter = new(new JsonSerializerOptions
        {
            // for PascalCase policy:
            PropertyNamingPolicy = null,

            // for camelCase policy:
            //PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult objectResult)
            {
                objectResult.Formatters.Add(SSystemTextJsonOutputFormatter);
            }
        }
    }
}

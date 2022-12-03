using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IO;
using System.IO;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace WebApi_101.Middlewares
{
    public class RequestCultureMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly bool _logRequestBody, _logResponseBody;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager = new();

        private static readonly int s_readChunkBufferLength = 4;

        public RequestCultureMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _logRequestBody = bool.Parse(configuration["Logging:RequestBody"]);
            _logResponseBody = bool.Parse(configuration["Logging:ResponseBody"]);
        }

        public async Task Invoke(HttpContext httpContext, ILogger<RequestCultureMiddleware> logger)
        {
            if (_logRequestBody)
            {
                await ReadRequestBodyInChunksAsync(httpContext, logger);
            }

            if (_logResponseBody)
            {
                await CallNextAndLogResponseBodyAsync(httpContext, logger);
            }
            else
            {
                await _next(httpContext);
            }
        }

        private async Task ReadRequestBodyInChunksAsync(HttpContext context, ILogger logger)
        {
            // source: https://stackoverflow.com/a/52328142
            // source: https://stackoverflow.com/a/64927281

            context.Request.EnableBuffering();

            await using MemoryStream stream = _recyclableMemoryStreamManager.GetStream();

            await context.Request.Body.CopyToAsync(stream);

            stream.Seek(0, SeekOrigin.Begin);

            using var textWriter = new StringWriter();
            using var reader = new StreamReader(stream);
            var readChunk = new char[s_readChunkBufferLength];

            int readChunkLength;
            //do while: is useful for the last iteration in case readChunkLength < chunkLength
            do
            {
                readChunkLength = reader.ReadBlock(readChunk, 0, s_readChunkBufferLength);
                textWriter.Write(readChunk, 0, readChunkLength);
            } while (readChunkLength > 0);

            string result = textWriter.ToString();

            context.Request.Body.Seek(0, SeekOrigin.Begin);

            logger.LogInformation("Serialized request body: {0}", result);
        }

        private async Task CallNextAndLogResponseBodyAsync(HttpContext context, ILogger logger)
        {
            await using MemoryStream memoryStream = _recyclableMemoryStreamManager.GetStream();

            Stream originalBodyStream = context.Response.Body;
            context.Response.Body = memoryStream;

            await _next(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            string responseBodySerialized = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            logger.LogInformation("Serialized response body: {0}", responseBodySerialized);

            await memoryStream.CopyToAsync(originalBodyStream);
        }

        private static async Task<string> ReadResponseBodyAsync(HttpResponse httpResponse)
        {
            byte[] buffer = new byte[(int)httpResponse.ContentLength];
            await httpResponse.Body.ReadAsync(buffer, 0, buffer.Length);
            string result = System.Text.Encoding.ASCII.GetString(buffer);

            //httpResponse.EnableBuffering();
            //string result;
            //using (var streamReader = new StreamReader(httpResponse.Body))
            //{
            //    result = await streamReader.ReadToEndAsync();
            //}
            //httpResponse.Body.Position = 0;
            return result;
        }

        private static async Task ReadRequestBodyAsync(HttpRequest httpRequest, ILogger logger)
        {
            //byte[] buffer = new byte[(int)httpContext.Request.ContentLength];
            //await httpContext.Request.Body.ReadAsync(buffer, 0, buffer.Length);
            //requestBody = System.Text.Encoding.ASCII.GetString(buffer);

            // sources:
            // https://stackoverflow.com/a/40994711
            // https://stackoverflow.com/a/58089658
            httpRequest.EnableBuffering();

            //await using var requestStream = _recyclableMemoryStreamManager.GetStream();
            //await httpRequest.Body.CopyToAsync(requestStream);
            //await httpRequest.Body.CopyToAsync(requestStream);

            string result;
            // https://stackoverflow.com/a/60135776
            using (var streamReader = new StreamReader(httpRequest.Body, leaveOpen: true))
            {
                result = await streamReader.ReadToEndAsync();
            }
            httpRequest.Body.Position = 0;

            logger.LogInformation("Serialized request body: {0}", result);
        }
    }
}
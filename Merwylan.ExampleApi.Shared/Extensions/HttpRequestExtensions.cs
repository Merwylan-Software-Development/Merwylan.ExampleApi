using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Merwylan.ExampleApi.Shared.Extensions
{
    public static class HttpRequestExtensions
    {
        public static async Task CopyStreamTo(this HttpRequest incomingRequest, HttpWebRequest targetRequest)
        {
            targetRequest.CopyContentHeadersFrom(incomingRequest);
            targetRequest.ContentType = incomingRequest.ContentType;

            if (incomingRequest.ContentLength != null)
                targetRequest.ContentLength = (long)incomingRequest.ContentLength;

            targetRequest.Method = incomingRequest.Method;

            await using var targetStream = targetRequest.GetRequestStream();
            await incomingRequest.Body.CopyToAsync(targetStream);
        }

        public static void CopyContentHeadersFrom(this HttpWebRequest targetRequest, HttpRequest request)
        {
            var targetHeaders = targetRequest.Headers;

            var contentHeaders = request.Headers.Where(pair =>
                pair.Key.StartsWith("Content-", StringComparison.OrdinalIgnoreCase));

            targetHeaders.Clear();

            foreach (var (name, value) in contentHeaders)
            {
                targetHeaders.Add(name, value);
            }
        }

        public static bool TryGetContentFromFile(this HttpRequest request, out Stream? stream)
        {
            stream = null;
            try
            {
                stream = request.Form.Files[0].OpenReadStream();
                return true;
            }
            catch
            {
                return false;
            }

        }

        public static Stream GetContentFromFileOrBody(this HttpRequest request)
        {
            bool isContentAttachedAsFile;
            try
            {
                isContentAttachedAsFile = request.Form.Files.Count != 0;
            }
            catch
            {
                isContentAttachedAsFile = false;
            }

            return isContentAttachedAsFile
                ? request.Form.Files[0].OpenReadStream() // We do not support multiple files at once
                : request.Body;
        }

    }
}

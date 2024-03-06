using Netcorext.Extensions.AspNetCore.Handlers;

namespace Netcorext.Extensions.AspNetCore.Helpers;

public static class HttpContextExtension
{
    public static string GetRequestId(this HttpContext context, params string[] headerNames)
    {
        var requestId = context.TraceIdentifier;

        var headers = headerNames.Length == 0 ? new[] { RequestIdHttpMessageHandler.DEFAULT_HEADER_NAME } : headerNames;

        foreach (var name in headers)
        {
            if (!context.Request.Headers.ContainsKey(name) || string.IsNullOrWhiteSpace(context.Request.Headers[name]))
                continue;

            requestId = context.Request.Headers[name];

            break;
        }

        requestId ??= Guid.NewGuid().ToString();

        return requestId;
    }
}

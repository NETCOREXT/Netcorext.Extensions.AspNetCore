using Netcorext.Extensions.AspNetCore.Handlers;

namespace Microsoft.Extensions.DependencyInjection;

public static class HttpClientBuilderExtension
{
    public static IHttpClientBuilder AddRequestId(this IHttpClientBuilder builder)
    {
        return AddRequestId(builder, RequestIdHttpMessageHandler.DEFAULT_HEADER_NAME, RequestIdHttpMessageHandler.DEFAULT_HEADER_NAME);
    }

    public static IHttpClientBuilder AddRequestId(this IHttpClientBuilder builder, params string[] headerNames)
    {
        return AddRequestId(builder, RequestIdHttpMessageHandler.DEFAULT_HEADER_NAME, headerNames);
    }

    public static IHttpClientBuilder AddRequestId(this IHttpClientBuilder builder, string headerName, params string[] headerNames)
    {
        builder.AddHttpMessageHandler(provider =>
                                      {
                                          var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();

                                          return new RequestIdHttpMessageHandler(httpContextAccessor, headerName, headerNames);
                                      });

        return builder;
    }
}

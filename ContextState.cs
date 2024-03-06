using System.Security.Claims;
using Netcorext.Contracts;
using Netcorext.Extensions.AspNetCore.Handlers;
using Netcorext.Extensions.AspNetCore.Helpers;

namespace Netcorext.Extensions.AspNetCore;

public sealed class ContextState : IContextState
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private string _requestId = Guid.NewGuid().ToString();
    private ClaimsPrincipal _user = new();
    private IDictionary<object, object?> _items = new Dictionary<object, object?>();

    public ContextState(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? RequestId
    {
        get => _httpContextAccessor.HttpContext?.GetRequestId() ?? _requestId;
        set
        {
            if (_httpContextAccessor.HttpContext == null)
                _requestId = value ?? Guid.NewGuid().ToString();
            else
                _httpContextAccessor.HttpContext.Request.Headers[RequestIdHttpMessageHandler.DEFAULT_HEADER_NAME] = value ?? Guid.NewGuid().ToString();
        }
    }

    public ClaimsPrincipal? User
    {
        get => _httpContextAccessor.HttpContext?.User ?? _user;
        set
        {
            if (_httpContextAccessor.HttpContext == null)
                _user = value ?? new ClaimsPrincipal();
            else
                _httpContextAccessor.HttpContext.User = value ?? new ClaimsPrincipal();
        }
    }

    public IDictionary<object, object?> Items
    {
        get => _httpContextAccessor.HttpContext?.Items ?? _items;
        set
        {
            if (_httpContextAccessor.HttpContext != null)
                _httpContextAccessor.HttpContext.Items = value;
            else
                _items = value;
        }
    }
}

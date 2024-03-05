using System.Security.Claims;
using Netcorext.Contracts;

namespace Netcorext.Extensions.AspNetCore;

public sealed class ContextState : IContextState
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private ClaimsPrincipal _user = new();
    private IDictionary<object, object?> _items = new Dictionary<object, object?>();

    public ContextState(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
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

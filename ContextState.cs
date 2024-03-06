using System.Security.Claims;
using Netcorext.Contracts;
using Netcorext.Extensions.AspNetCore.Handlers;
using Netcorext.Extensions.AspNetCore.Helpers;

namespace Netcorext.Extensions.AspNetCore;

public sealed class ContextState : IContextState
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private string _requestId = Guid.NewGuid().ToString();
    private readonly AsyncLocal<ContextStateUserHolder> _userHolder = new AsyncLocal<ContextStateUserHolder>();
    private readonly AsyncLocal<ContextStateItemsHolder> _itemsHolder = new AsyncLocal<ContextStateItemsHolder>();

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
        get => _httpContextAccessor.HttpContext?.User ?? _userHolder.Value?.User;
        set
        {
            var holder = _userHolder.Value;
            if (holder != null)
                holder.User = null;

            if (value != null)
                _userHolder.Value = new ContextStateUserHolder { User = value };

            if (_httpContextAccessor.HttpContext != null)
                _httpContextAccessor.HttpContext.User = value ?? new ClaimsPrincipal();
        }
    }

    public IDictionary<object, object?> Items
    {
        get => _httpContextAccessor.HttpContext?.Items ?? _itemsHolder.Value?.Items ?? new Dictionary<object, object?>();
        set
        {
            var holder = _itemsHolder.Value;
            if (holder != null)
                holder.Items = null;

            _itemsHolder.Value = new ContextStateItemsHolder { Items = value };

            if (_httpContextAccessor.HttpContext != null)
                _httpContextAccessor.HttpContext.Items = value;
        }
    }

    private class ContextStateUserHolder
    {
        public ClaimsPrincipal? User;
    }
    private class ContextStateItemsHolder
    {
        public IDictionary<object, object?>? Items;
    }
}

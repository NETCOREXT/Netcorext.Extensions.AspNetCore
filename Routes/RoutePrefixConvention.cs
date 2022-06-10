using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Microsoft.Extensions.DependencyInjection;

public class RoutePrefixConvention : IApplicationModelConvention
{
    private readonly string _routePrefix;

    public RoutePrefixConvention(string routePrefix = "api/v{version:apiVersion}")
    {
        _routePrefix = routePrefix.TrimStart('/');
    }

    public void Apply(ApplicationModel application)
    {
        var routeModel = new AttributeRouteModel(new RouteAttribute(_routePrefix));

        foreach (var selector in application.Controllers.SelectMany(c => c.Selectors))
        {
            selector.AttributeRouteModel = selector.AttributeRouteModel == null ? routeModel : AttributeRouteModel.CombineAttributeRouteModel(routeModel, selector.AttributeRouteModel);
        }
    }
}
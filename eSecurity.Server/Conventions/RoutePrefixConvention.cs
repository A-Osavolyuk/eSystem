using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace eSecurity.Server.Conventions;

public class RoutePrefixConvention(string prefix) : IApplicationModelConvention
{
    private readonly AttributeRouteModel routePrefix = new AttributeRouteModel(new RouteAttribute(prefix));

    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            var matchedSelectors = controller.Selectors
                .Where(s => s.AttributeRouteModel != null)
                .ToList();

            if (matchedSelectors.Count != 0)
            {
                foreach (var selector in matchedSelectors)
                {
                    selector.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(
                        routePrefix, selector.AttributeRouteModel);
                }
            }
            else
            {
                controller.Selectors.Add(new SelectorModel
                {
                    AttributeRouteModel = routePrefix
                });
            }
        }
    }
}

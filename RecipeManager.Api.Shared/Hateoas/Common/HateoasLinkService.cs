using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using RecipeManager.Api.Shared.Hateoas.Models;
namespace RecipeManager.Api.Shared.Hateoas.Common;

public sealed class HateoasLinkService(IHttpContextAccessor httpAccessor, LinkGenerator linkGenerator)
{
    public Link GeneratePost(string endpoint, object? routeValues, string rel)
    {
        return Link.CreatePost(linkGenerator.GetUriByName(httpAccessor.HttpContext!, endpoint, routeValues)!, rel);
    }

    public Link GenerateGet(string endpoint, object? routeValues, string rel)
    {
        return Link.CreateGet(linkGenerator.GetUriByName(httpAccessor.HttpContext!, endpoint, routeValues)!, rel);
    }

    public Link GeneratePut(string endpoint, object? routeValues, string rel)
    {
        return Link.CreatePut(linkGenerator.GetUriByName(httpAccessor.HttpContext!, endpoint, routeValues)!, rel);
    }

    public Link GenerateDelete(string endpoint, object? routeValues, string rel)
    {
        return Link.CreateDelete(linkGenerator.GetUriByName(httpAccessor.HttpContext!, endpoint, routeValues)!, rel);
    }
}
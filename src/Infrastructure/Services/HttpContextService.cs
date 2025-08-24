using System.Security.Claims;

using Domain.Abstractions.Services;

using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

public class HttpContextService(IHttpContextAccessor httpContextAccessor) : IHttpContextService
{
    public string? GetCurrentUserIdentity()
    {
        return httpContextAccessor.HttpContext.User.Identity?.Name;
    }


    public List<string> GetCurrentUserRoles()
    {
        var user = httpContextAccessor.HttpContext?.User;
        if (user is null || user.Identity?.IsAuthenticated != true)
            return [];

        var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value)
                        .Concat(user.FindAll("role").Select(c => c.Value));

        return roles.ToList();
    }
}
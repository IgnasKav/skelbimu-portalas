using System;
using System.Threading.Tasks;
using Application.Advertisements;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace API.Services.Authentication;

public class AdvertisementAuthorizationHandler: AuthorizationHandler<OperationAuthorizationRequirement, Advertisement>
{
    private UserManager<User> _userManager;

    public AdvertisementAuthorizationHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
        Advertisement resource)
    {
        var user = await _userManager.GetUserAsync(context.User);
        var userRoles = await _userManager.GetRolesAsync(user);
        var isAdmin = userRoles.Contains(Constants.AdminRole);
        var isSupport = userRoles.Contains(Constants.SupportRole);
        var currentUserId = new Guid(_userManager.GetUserId(context.User));

        if (requirement.Name == Constants.Update || requirement.Name == Constants.Delete)
        {
            if (resource.OwnerId == currentUserId || isAdmin || isSupport)
            {
                context.Succeed(requirement);
            }
        }
        else if (requirement.Name == Constants.Read)
        {
            if (resource.OwnerId == currentUserId || isAdmin || isSupport || resource.State == AdvertisementState.Approved)
            {
                context.Succeed(requirement);
            }
        }
    }
}
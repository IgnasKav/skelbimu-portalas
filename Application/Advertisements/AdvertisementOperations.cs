using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Application.Advertisements;

public class AdvertisementOperations
{
    public static OperationAuthorizationRequirement Update = new() {Name=Constants.Update};
    public static OperationAuthorizationRequirement Read = new() {Name=Constants.Read}; 
    public static OperationAuthorizationRequirement Delete = new() {Name=Constants.Delete};
    public static OperationAuthorizationRequirement ChangeStatus = new() {Name=Constants.ChangeStatus};
}

public class Constants
{
    public static readonly string Read = "Read";
    public static readonly string Update = "Update";
    public static readonly string Delete = "Delete";
    public static readonly string ChangeStatus = "ChangeStatus";

    public static readonly string AdminRole = 
        "Admin";
    public static readonly string SupportRole = "Support";
    public static readonly string UserRole = "User";
}
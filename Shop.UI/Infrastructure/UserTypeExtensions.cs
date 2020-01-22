using Shop.Domain.Enums;

namespace Shop.UI.Infrastructure
{
    public static class UserTypeExtensions
    {
        public static string GetUser(this UserType @this)
        {
            if (@this == UserType.Authorizer)
                return "Authorizer";
            else if (@this == UserType.SuperUser)
                return "SuperUser";
            else
                return "User";
        }

        public static string GetClaim(this UserType @this)
        {
            if (@this == UserType.Authorizer)
                return "authorizer";
            else if (@this == UserType.SuperUser)
                return "superuser";
            else if (@this == UserType.BusinessUser)
                return "businessuser";
            else
                return "user";
        }
    }
}

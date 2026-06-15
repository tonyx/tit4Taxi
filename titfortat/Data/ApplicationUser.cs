using Microsoft.AspNetCore.Identity;
using TitForTat.Shared;

namespace titfortat.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
}

public static class ApplicationUserExtensions
{
    public static Commons.AppUserInfo ToAppUserInfo(this ApplicationUser applicationUser)
    {
        return new Commons.AppUserInfo(
            applicationUser.UserName ?? "",
            applicationUser.Email ?? "",
            applicationUser.EmailConfirmed,
            applicationUser.PhoneNumber ?? "",
            applicationUser.PhoneNumberConfirmed,
            applicationUser.TwoFactorEnabled
        );
    }
}

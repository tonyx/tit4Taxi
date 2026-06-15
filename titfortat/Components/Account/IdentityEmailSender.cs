using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using titfortat.Data;
using TitForTat.Shared.Infrastructure.Services;

namespace titfortat.Components.Account;

internal sealed class IdentityEmailSender : IEmailSender<ApplicationUser>
{
    private readonly IMailNotificator _mailNotificator;
    private readonly string _emailFrom;
    private readonly string _nameFrom;

    public IdentityEmailSender(
        IConfiguration configuration,
        IMailNotificator mailNotificator
    ) 
    {
        _mailNotificator = mailNotificator;
        _emailFrom = configuration.GetSection("EmailSettings").GetValue<string>("EmailFrom") ?? "noreply@biblionet.eu";
        _nameFrom = configuration.GetSection("EmailSettings").GetValue<string>("NameFrom") ?? "TitForTat";
    }

    public async Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) =>
        await _mailNotificator.SendEmailAsync(_emailFrom, _nameFrom, email, "Confirm your email", confirmationLink);

    public async Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) =>
        await _mailNotificator.SendEmailAsync(_emailFrom, _nameFrom, email, "reset your password", "You requested to reset your password. Click on the link to reset your password: " + resetLink);

    public async Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) =>
        await _mailNotificator.SendEmailAsync(_emailFrom, _nameFrom, email, "Reset your password", "You requested to reset your password. Use the following code to reset your password: " + resetCode);
}

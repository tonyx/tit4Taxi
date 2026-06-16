using TitForTat.Infrastructure.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sharpino;
using TitForTat.Client.Pages;
using TitForTat.Components;
using TitForTat.Components.Account;
using TitForTat.Data;
using TitForTat.Services;
using TitForTat.Shared.Services;
using Mailjet.Client;
using TitForTat.Server.MailQueueNotification;
using TitForTat.Shared.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

var connection = builder.Configuration.GetConnectionString("EventStore") ?? throw new InvalidOperationException("Connection string 'EventStore' not found.");

builder.Services.AddSingleton<Storage.IEventStore<string>>(_ => new PgStorage.PgEventStore(connection));
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<ICoopService, CoopService>();
builder.Services.AddSingleton<ILedgerService, LedgerService>();

builder.Services.AddSingleton<IRegistrationCodeService, RegistrationCodeService>();
var mailjetApiKey = builder.Configuration["Mailjet:ApiKey"];
var mailjetSecretKey = builder.Configuration["Mailjet:SecretKey"];
builder.Services.AddSingleton(new MailjetClient(mailjetApiKey, mailjetSecretKey));
builder.Services.AddSingleton<IMailResenderService, MailResenderService>();
builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityEmailSender>();
builder.Services.AddSingleton<IMailNotificator, MailNotificator>();
// builder.Services.AddHostedService<MailResenderScheduler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(TitForTat.Client._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

var copyUsersAtLogon = builder.Configuration.GetValue<bool>("CopyUsersAtLogon", false);
if (copyUsersAtLogon)
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        var users = await dbContext.Users.ToListAsync();
        var sharpinoUsers = users.Select(u => 
            Tuple.Create(
                TitForTat.Shared.Commons.UserId.NewUserId(Guid.Parse(u.Id)), 
                u.ToAppUserInfo()
            )
        );
        await userService.CreateUsersUnsafe(sharpinoUsers, default);
    }
}

// create an initial snapshot of the mail queue if not exists
using (var scope = app.Services.CreateScope())
{
    var mailResenderService = scope.ServiceProvider.GetRequiredService<IMailResenderService>();
    await mailResenderService.CreateInitialMailQueueInstanceAsync(Microsoft.FSharp.Core.FSharpOption<System.Threading.CancellationToken>.None);
}

// seed roles and assign admin
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = { "ADMIN", "MANAGER", "CONTROLLER" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    var adminUserName = builder.Configuration["AdminUserName"];
    if (!string.IsNullOrEmpty(adminUserName))
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var adminUser = await userManager.FindByNameAsync(adminUserName) ?? await userManager.FindByEmailAsync(adminUserName);
        if (adminUser != null)
        {
            if (!await userManager.IsInRoleAsync(adminUser, "ADMIN"))
            {
                await userManager.AddToRoleAsync(adminUser, "ADMIN");
            }
        }
    }
}

app.Run();

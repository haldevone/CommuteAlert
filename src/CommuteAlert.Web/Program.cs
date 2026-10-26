using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CommuteAlert.Web.Data;
using CommuteAlert.Web.Sl;
using CommuteAlert.Web.Services;
using CommuteAlert.Web.Security;
using CommuteAlert.Web.Notifications;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

var adminEmail = builder.Configuration["Admin:Email"];

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Admin", policy => policy.RequireAssertion(context =>
    !string.IsNullOrEmpty(adminEmail) &&
    string.Equals(context.User.Identity?.Name, adminEmail, StringComparison.OrdinalIgnoreCase)));

builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient<ISlApiService, SlApiService>(client =>
    client.Timeout = TimeSpan.FromSeconds(10));

builder.Services.AddScoped<AlertService>();

builder.Services.AddSingleton<WebhookProtector>();

builder.Services.AddScoped<AlertSender>();

builder.Services.AddHttpClient<INotifier, DiscordNotifier>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddHostedService<AlertScheduler>();

builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<SlSiteService>(client =>
    client.Timeout = TimeSpan.FromSeconds(30));


var app = builder.Build();

if (!EF.IsDesignTime)
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.MigrateAsync();

    var email = app.Configuration["Admin:Email"];
    var password = app.Configuration["Admin:Password"];
    var users = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password)
        && await users.FindByEmailAsync(email) == null)
    {
        var admin = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
        var result = await users.CreateAsync(admin, password);
        if (!result.Succeeded)
            throw new InvalidOperationException("Could not create the admin user: " +
                string.Join(", ", result.Errors.Select(e => e.Description)));
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseRouting();

// Registration is closed: only the admin creates accounts
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/Identity/Account/Register", StringComparison.OrdinalIgnoreCase) ||
        context.Request.Path.StartsWithSegments("/Identity/Account/RegisterConfirmation", StringComparison.OrdinalIgnoreCase))
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        return;
    }
    await next();
});

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();

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

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();

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



var app = builder.Build();

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

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();

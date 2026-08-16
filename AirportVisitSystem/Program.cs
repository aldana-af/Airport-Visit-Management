using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using AirportVisitSystem.Data;
using AirportVisitSystem.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// database context setup (office computer)
builder.Services.AddDbContext<AirportVisitDb>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// database context setup (home computer)
builder.Services.AddDbContext<AirportVisitDatabase1>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// EmployeeForm API client (typed HttpClient) — one place that knows
// EmployeeForm's base URL and API key. AccountController and the
// EmployeeHost/Manager registration flows depend on IEmployeeFormApiClient,
// not on HttpClient directly, so they never touch this config themselves.
builder.Services.AddHttpClient<IEmployeeFormApiClient, EmployeeFormApiClient>((serviceProvider, client) =>
{
    var config = builder.Configuration;
    var baseUrl = config["EmployeeFormApi:BaseUrl"];
    var apiKey = config["EmployeeFormApi:ApiKey"];

    if (string.IsNullOrWhiteSpace(baseUrl))
    {
        throw new InvalidOperationException(
            "EmployeeFormApi:BaseUrl is not configured. Set it in appsettings.Development.json.");
    }

    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();

    // Both apps run on localhost with self-signed dev certs during
    // development — trust them here the same way `curl -k` does.
    // This must NOT ship this way to production; a real deployment
    // needs a properly trusted certificate on EmployeeForm's side instead.
    if (builder.Environment.IsDevelopment())
    {
        handler.ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
    }

    return handler;
});

// cookie authentication setup
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// continue cookie authentication setup
app.UseAuthentication();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

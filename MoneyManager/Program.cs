using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using MoneyManager.Application;
using MoneyManager.Components;
using MoneyManager.Infrastructure;
using MoneyManager.Infrastructure.Persistence;

// Railway injects PORT — set ASPNETCORE_URLS so Kestrel picks it up
// without overriding launchSettings.json for local development
var railwayPort = Environment.GetEnvironmentVariable("PORT");
if (railwayPort != null)
{
    Environment.SetEnvironmentVariable("ASPNETCORE_URLS", $"http://0.0.0.0:{railwayPort}");
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSignalR(options =>
{
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
    options.HandshakeTimeout = TimeSpan.FromSeconds(30);
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddDataProtection()
    .PersistKeysToDbContext<AppDbContext>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<MoneyManager.Services.UserSessionService>();

var app = builder.Build();

app.UseForwardedHeaders();
app.UseWebSockets(new WebSocketOptions { KeepAliveInterval = TimeSpan.FromSeconds(15) });

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseAntiforgery();

app.MapGet("/health", () => Results.Ok("healthy"));

app.MapStaticAssets();
app.UseStaticFiles();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await app.Services.MigrateAsync();
await DbSeeder.SeedAsync(app.Services);

app.Run();

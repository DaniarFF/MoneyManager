using Microsoft.AspNetCore.HttpOverrides;
using MoneyManager.Application;
using MoneyManager.Components;
using MoneyManager.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Railway / Render передают порт через переменную PORT
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Доверяем заголовкам от Railway-прокси (нужно для WebSocket / SignalR)
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Clean Architecture layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Session для хранения авторизации в серверном Blazor
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<MoneyManager.Services.UserSessionService>();

var app = builder.Build();

// Должен быть ПЕРВЫМ — до всего остального
app.UseForwardedHeaders();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
if (app.Environment.IsDevelopment()) app.UseHttpsRedirection();
app.UseSession();
app.UseAntiforgery();

// Healthcheck endpoint
app.MapGet("/health", () => Results.Ok("healthy"));

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Применяем миграции при старте
await app.Services.MigrateAsync();

app.Run();

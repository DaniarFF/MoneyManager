using MoneyManager.Application;
using MoneyManager.Components;
using MoneyManager.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Railway / Render передают порт через переменную PORT
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

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

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
// HTTPS редирект отключён в production-контейнере (терминируется на proxy Railway/Render)
if (app.Environment.IsDevelopment()) app.UseHttpsRedirection();
app.UseSession();
app.UseAntiforgery();

// Healthcheck endpoint — отвечает сразу, до старта Blazor
app.MapGet("/health", () => Results.Ok("healthy"));

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Применяем миграции при старте
await app.Services.MigrateAsync();

app.Run();

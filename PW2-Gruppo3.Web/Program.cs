
using PW2_Gruppo3.Web.Clients;
using PW2_Gruppo3.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
builder.AddRedisOutputCache("cache");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Registro gli ApiClient con i relativi prefissi

builder.Services.AddHttpClient<AssemblyLineApiClient>(client =>
{
    client.BaseAddress = new("http://localhost:5580");
});

builder.Services.AddHttpClient<BatchApiClient>(client =>
{
    client.BaseAddress = new("http://localhost:5580");
});

builder.Services.AddHttpClient<CustomerApiClient>(client =>
{
    client.BaseAddress = new("http://localhost:5580");
});

builder.Services.AddHttpClient<LatheApiClient>(client =>
{
    client.BaseAddress = new("http://localhost:5580");
});

builder.Services.AddHttpClient<MillingApiClient>(client =>
{
    client.BaseAddress = new("http://localhost:5580");
});

builder.Services.AddHttpClient<SitesApiClient>(client =>
{
    client.BaseAddress = new("http://localhost:5580");
});

builder.Services.AddHttpClient<TestLineApiClient>(client =>
{
    client.BaseAddress = new("http://localhost:5580");
});


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseOutputCache();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
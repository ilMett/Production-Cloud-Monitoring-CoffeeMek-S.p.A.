using Microsoft.EntityFrameworkCore;
using OpenTelemetry;
using PW2_Gruppo3.ApiService;
using PW2_Gruppo3.ApiService.Crud;
using PW2_Gruppo3.ApiService.Data;
using PW2_Gruppo3.ApiService.Endpoints.Crud;
using PW2_Gruppo3.ApiService.Services;
using PW2_Gruppo3.Models;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register API services
builder.Services.AddScoped<IGenericService<AssemblyLine>, GenericService<AssemblyLine>>();
builder.Services.AddScoped<IGenericService<Batch>, GenericService<Batch>>();
builder.Services.AddScoped<IGenericService<Customer>, GenericService<Customer>>();
builder.Services.AddScoped<IGenericService<Lathe>, GenericService<Lathe>>();
builder.Services.AddScoped<IGenericService<Milling>, GenericService<Milling>>();
builder.Services.AddScoped<IGenericService<Site>, GenericService<Site>>();
builder.Services.AddScoped<IGenericService<TestLine>, GenericService<TestLine>>();

// BatchQueue services, Init of Batch queue
builder.Services.AddScoped<IBatchQueueService, BatchQueueService>();
builder.Services.AddHostedService<QueueInitializerHostedService>();


builder.Services.AddDbContext<ProductionMonitoringContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("db")));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Map of all of the endpoints
app.MapAssemblyLineEndpoints();
app.MapBatchEndpoints();
app.MapCustomerEndpoint();
app.MapLatheEndpoints();
app.MapMillingEndpoints();
app.MapSiteEndpoints();
app.MapTestLineEndpoints();
app.MapDefaultEndpoints();
app.MapDataGeneratorEndpoints();

await app.RunAsync();


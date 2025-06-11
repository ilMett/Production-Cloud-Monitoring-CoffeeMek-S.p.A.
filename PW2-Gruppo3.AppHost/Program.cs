var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var azureSql = builder.AddConnectionString("db");

var apiService = builder.AddProject<Projects.PW2_Gruppo3_ApiService>("apiservice");

builder.AddProject<Projects.PW2_Gruppo3_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WithReference(azureSql)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();

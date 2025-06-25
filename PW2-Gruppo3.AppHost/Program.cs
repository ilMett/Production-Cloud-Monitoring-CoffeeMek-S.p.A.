var builder = DistributedApplication.CreateBuilder(args);
builder.AddProject<Projects.PW2_Gruppo3_DataGenerator>("datagenerator");

var cache = builder.AddRedis("cache");

var azureSql = builder.AddConnectionString("db");

var apiService = builder.AddProject<Projects.PW2_Gruppo3_ApiService>("apiservice")    
    .WithReference(azureSql)
    .WaitFor(azureSql);

builder.AddProject<Projects.PW2_Gruppo3_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();

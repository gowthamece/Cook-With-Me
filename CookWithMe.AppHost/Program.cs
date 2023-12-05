var builder = DistributedApplication.CreateBuilder(args);

var apiservice = builder.AddProject<Projects.CookWithMe_ApiService>("apiservice");

builder.AddProject<Projects.CookWithMe_Web>("webfrontend")
    .WithReference(apiservice);

builder.Build().Run();

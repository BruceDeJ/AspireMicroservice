var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.TimeSheet_ApiService>("timesheet-api");
var authService = builder.AddProject<Projects.TimeSheet_AuthAPI>("auth-api");

builder.AddProject<Projects.TimeSheet_Web>("frontend-app")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WithReference(authService);

builder.AddProject<Projects.TimeSheet_ApiGateway>("gateway-api");

builder.Build().Run();

var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.TimeSheet_ApiService>("timesheet-api");
var authService = builder.AddProject<Projects.TimeSheet_AuthAPI>("auth-api");

var apiGateway = builder.AddProject<Projects.TimeSheet_ApiGateway>("gateway-api")
    .WithReference(apiService);

builder.AddProject<Projects.TimeSheet_Web>("frontend-app")
    .WithExternalHttpEndpoints()
    .WithReference(apiGateway)
    .WithReference(authService);

builder.Build().Run();

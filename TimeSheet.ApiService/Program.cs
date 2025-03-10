using AuthAPI;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using TimeSheet.ApiService;
using TimeSheet.ApiService.Domain;
using TimeSheet.ApiService.Model;
using TimeSheet.AuthAPI;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddDbContext<TimeSheetContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};
app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});

app.MapPost("/TimeSheetEntry", async (HttpRequest request, TimeSheetEntryInput input, TimeSheetContext context) =>
{
    var accessToken = request.Headers.SingleOrDefault(x => x.Key.ToLower() == HeaderNames.Authorization.ToLower()).Value.FirstOrDefault();
    var accessTokenBearerRemoved = accessToken?.Substring(7);

    var tokenHandler = new JwtSecurityTokenHandler();

    var validatedToken = tokenHandler.ReadJwtToken(accessTokenBearerRemoved);
    var userId = validatedToken.Claims.SingleOrDefault(x => x.Type == "UserID").Value;

    var newTimeSheetEntry = new TimeSheetEntry(input.DurationInMinutes, input.Description, userId);
    
    var createdTimeSheetEntry = await context.AddAsync(newTimeSheetEntry);
    await context.SaveChangesAsync();

    return Results.Created("/TimeSheetEntry", createdTimeSheetEntry.Entity);

});

app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

using AuthAPI;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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

app.MapGet("/TimeSheetEntry", async (HttpRequest request, TimeSheetContext context) =>
{
    var userId = GetUserIdFromJWT(request);

    var userTimeSheetEntries = await context.TimeSheetEntries.Where(x => x.LoggedBy == userId).ToListAsync();

    return Results.Ok(userTimeSheetEntries);
});

app.MapDelete("/TimeSheetEntry/{id}", async (int id, HttpRequest request, TimeSheetContext context) =>
{
    var timesheetEntry = await context.TimeSheetEntries.SingleOrDefaultAsync(x => x.Id == id);

    if (timesheetEntry == null)
    {
        return Results.NotFound();
    }

    context.Remove(timesheetEntry);
    await context.SaveChangesAsync();

    return Results.NoContent();
});

app.MapPost("/TimeSheetEntry", async (HttpRequest request, TimeSheetEntryInput input, TimeSheetContext context) =>
{
    var userId = GetUserIdFromJWT(request);

    var newTimeSheetEntry = new TimeSheetEntry(input.DurationInMinutes, input.Description, userId);
    
    var createdTimeSheetEntry = await context.AddAsync(newTimeSheetEntry);
    await context.SaveChangesAsync();

    return Results.Created("/TimeSheetEntry", createdTimeSheetEntry.Entity);
});

app.MapDefaultEndpoints();

app.Run();

string GetUserIdFromJWT(HttpRequest request)
{
    var bearerAccessToken = request.Headers.SingleOrDefault(x => x.Key.ToLower() == HeaderNames.Authorization.ToLower()).Value.FirstOrDefault();
    var accessToken = bearerAccessToken?.Substring(7);

    var tokenHandler = new JwtSecurityTokenHandler();

    var validatedToken = tokenHandler.ReadJwtToken(accessToken);
    var userId = validatedToken?.Claims?.SingleOrDefault(x => x.Type == "UserID")?.Value ?? string.Empty;

    return userId;
}

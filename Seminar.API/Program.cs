using dotenv.net;
using Microsoft.EntityFrameworkCore;
using Seminar.API.Extensions;
using Seminar.API.Middleware;
using Seminar.APPLICATION.Extensions;
using Seminar.INFRASTRUCTURE.Database;

//Load env
DotEnv.Load();
var builder = WebApplication.CreateBuilder(args);
// Add Controllers
builder.Services.AddControllers();
// Add Services
builder.Services.AddApplication(builder.Configuration);
// Add cors, swagger, authentication, and authorization, database
builder.Services.AddInfrastructure(builder.Configuration);

builder.Configuration.AddEnvironmentVariables();

var app = builder.Build();

await app.UseInitialiseDatabaseAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseCors("AllowLocalhost");
app.UseAuthorization();
app.UseMiddleware<CustomExceptionHandlerMiddleware>();
app.MapControllers();

try
{
    app.Logger.LogInformation("Ứng dụng đang khởi động...");
    app.Run();
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Đã xảy ra lỗi khi khởi động ứng dụng.");
}

using dotenv.net;
using Seminar.API.Extensions;
using Seminar.APPLICATION.Extensions;

//DotEnv.Load();
DotEnv.Load();
var builder = WebApplication.CreateBuilder(args);
Console.WriteLine($"DB_CONNECTION_STRING: {Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")}");
Console.WriteLine($"JWT_KEY: {Environment.GetEnvironmentVariable("JWT_KEY")}");
Console.WriteLine($"JWT_ISSUER: {Environment.GetEnvironmentVariable("JWT_ISSUER")}");
Console.WriteLine($"JWT_AUDIENCE: {Environment.GetEnvironmentVariable("JWT_AUDIENCE")}");

// Add Controllers
builder.Services.AddControllers();

// Add Services
builder.Services.AddApplication(builder.Configuration);

// Add cors, swagger, authentication, and authorization
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();
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

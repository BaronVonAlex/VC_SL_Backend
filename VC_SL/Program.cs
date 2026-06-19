using VC_SL.Data;
using VC_SL.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddSingleton<JsonDataStore>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IWinrateService, WinrateService>();
builder.Services.AddScoped<ILeaderboardService, LeaderboardService>();
builder.Services.AddScoped<IHmacService, HmacService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowStaticWebApp", policy =>
    {
        policy
            .WithOrigins("https://vcsl.online")
            .WithOrigins("http://localhost:5174")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowStaticWebApp");

app.UseMiddleware<HmacValidationMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
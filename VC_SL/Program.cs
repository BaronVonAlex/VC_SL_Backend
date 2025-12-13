using Microsoft.EntityFrameworkCore;
using VC_SL.Data;
using VC_SL.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("VcSlAzureSqlConnection"),
        sql =>
        {
            sql.EnableRetryOnFailure();
        }
    ));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IWinrateService, WinrateService>();
builder.Services.AddScoped<ILeaderboardService, LeaderboardService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowStaticWebApp", policy =>
    {
        policy.WithOrigins("https://vcsl.online")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowStaticWebApp");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
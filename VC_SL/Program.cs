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
    options.UseMySql(
        builder.Configuration.GetConnectionString("VcSlDbConnectionString"),
        new MySqlServerVersion(new Version(8, 0, 36))
    ));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IWinrateService, WinrateService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowStaticWebApp", policy =>
    {
        policy.WithOrigins("https://purple-plant-051730d03.2.azurestaticapps.net")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.Use(async (context, next) =>
{
    if (app.Environment.IsDevelopment())
    {
        await next();
        return;
    }

    var apiKey = context.Request.Headers["X-API-Key"].FirstOrDefault();
    var expectedKey = app.Configuration["ApiKey"];

    if (string.IsNullOrEmpty(apiKey) || apiKey != expectedKey)
    {
        context.Response.StatusCode = 403;
        await context.Response.WriteAsync("Forbidden: Invalid API Key");
        return;
    }

    await next();
});

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
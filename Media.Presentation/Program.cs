using Media.Persistence;
using Media.Infrastructure;
using Media.Presentation.SwaggerGen;
using Media.Presentation.Middleware;
using Media.Core.Options;
using Media.Presentation.Controllers;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGeneration();

// Create a logger. It ignores microsoft and system logs, but focusses on intentional logs.
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Fatal)
    .MinimumLevel.Override("System", LogEventLevel.Fatal)
    .Enrich.FromLogContext()
    .WriteTo.File(
        path: "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        shared: true)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddMemoryCache();
builder.Services.Configure<RateLimitingOptions>(builder.Configuration.GetSection("RateLimiting"));
builder.Services.Configure<UploadPolicyOptions>(builder.Configuration.GetSection("UploadPolicy"));

// Set the kestrel limit to the max file size + 1. Then the middleware can catch it and give 
// an appropriate message back.
var maxFileSize = builder.Configuration["UploadPolicy:MaxFileSize"];
builder.WebHost.ConfigureKestrel(options =>
    options.Limits.MaxRequestBodySize = long.Parse(maxFileSize + 1 ?? "-1"));

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithExposedHeaders("Content-Disposition");
    });
});

builder.Services.ConfigurePersistence(builder.Configuration);
builder.Services.ConfigureInfrastructure(builder.Configuration);

// Start the Runtime right away instead of when /health is first called.
HealthController.AppRunningTime = DateTime.Now;

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<LoggingMiddleware>();
app.UseMiddleware<RateLimitingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

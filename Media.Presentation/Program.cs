using Media.Persistence;
using Media.Infrastructure;
using Media.Presentation.SwaggerGen;
using Media.Presentation.Middleware;
using Media.Presentation.Controllers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGeneration();

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

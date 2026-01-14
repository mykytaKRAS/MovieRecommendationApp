using Microsoft.EntityFrameworkCore;
using MovieRecommendationApp.API.Endpoints;
using MovieRecommendationApp.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() 
    { 
        Title = "Movie Recommendation API", 
        Version = "v1",
        Description = "API for movie recommendations with user ratings and lists"
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
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

app.UseCors("AllowAll");

app.MapGet("/", () => "Movie Recommendation API is running! 🎬")
    .WithName("Root");
    
app.MapGet("/health", () => Results.Ok(new 
{ 
    status = "healthy", 
    timestamp = DateTime.UtcNow,
    environment = app.Environment.EnvironmentName 
}))
    .WithName("HealthCheck");

app.MapUserEndpoints();
app.MapMovieEndpoints();
app.MapGenreEndpoints();

app.Run();
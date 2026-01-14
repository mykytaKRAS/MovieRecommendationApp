using Microsoft.EntityFrameworkCore;
using MovieRecommendationApp.Application.DTOs.Auth;
using MovieRecommendationApp.Domain.Entities;
using MovieRecommendationApp.Infrastructure.Data;

namespace MovieRecommendationApp.API.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users").WithTags("Users");
        
        // GET /api/users
        group.MapGet("/", async (AppDbContext db) =>
        {
            var users = await db.Users
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Email,
                    u.Role,
                    u.CreatedAt
                })
                .ToListAsync();
                
            return Results.Ok(users);
        })
        .WithName("GetAllUsers");
        
        // GET /api/users/{id}
        group.MapGet("/{id:int}", async (int id, AppDbContext db) =>
        {
            var user = await db.Users
                .Where(u => u.Id == id)
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Email,
                    u.Role,
                    u.CreatedAt,
                    MoviesWatched = u.UserMovies.Count(um => um.Status == "watched"),
                    MoviesInWatchlist = u.UserMovies.Count(um => um.Status == "watchlist"),
                    FavoriteMovies = u.UserMovies.Count(um => um.Status == "favorite"),
                    AverageRating = u.UserMovies
                        .Where(um => um.UserRating != null)
                        .Average(um => (double?)um.UserRating)
                })
                .FirstOrDefaultAsync();
                
            return user is not null 
                ? Results.Ok(user) 
                : Results.NotFound(new { error = "User not found" });
        })
        .WithName("GetUserById");
        
        // POST /api/users
        group.MapPost("/", async (CreateUserRequest request, AppDbContext db) =>
        {
            if (string.IsNullOrWhiteSpace(request.Username))
            {
                return Results.BadRequest(new { error = "Username is required" });
            }
            
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return Results.BadRequest(new { error = "Email is required" });
            }
            
            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return Results.BadRequest(new { error = "Password is required" });
            }
            
            if (await db.Users.AnyAsync(u => u.Email == request.Email))
            {
                return Results.BadRequest(new { error = "Email already exists" });
            }
            
            if (await db.Users.AnyAsync(u => u.Username == request.Username))
            {
                return Results.BadRequest(new { error = "Username already exists" });
            }
            
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = request.Password,
                Role = request.Role,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            db.Users.Add(user);
            await db.SaveChangesAsync();
            
            return Results.Created($"/api/users/{user.Id}", new
            {
                user.Id,
                user.Username,
                user.Email,
                user.Role,
                user.CreatedAt
            });
        })
        .WithName("CreateUser");
        
        // PUT /api/users/{id}
        group.MapPut("/{id:int}", async (int id, CreateUserRequest request, AppDbContext db) =>
        {
            var user = await db.Users.FindAsync(id);
            if (user is null)
            {
                return Results.NotFound(new { error = "User not found" });
            }
            
            if (user.Email != request.Email && await db.Users.AnyAsync(u => u.Email == request.Email))
            {
                return Results.BadRequest(new { error = "Email already exists" });
            }
            
            if (user.Username != request.Username && await db.Users.AnyAsync(u => u.Username == request.Username))
            {
                return Results.BadRequest(new { error = "Username already exists" });
            }
            
            user.Username = request.Username;
            user.Email = request.Email;
            user.Role = request.Role;
            user.UpdatedAt = DateTime.UtcNow;
            
            await db.SaveChangesAsync();
            
            return Results.Ok(new
            {
                user.Id,
                user.Username,
                user.Email,
                user.Role,
                user.UpdatedAt
            });
        })
        .WithName("UpdateUser");
        
        // DELETE /api/users/{id}
        group.MapDelete("/{id:int}", async (int id, AppDbContext db) =>
        {
            var user = await db.Users.FindAsync(id);
            if (user is null)
            {
                return Results.NotFound(new { error = "User not found" });
            }
            
            db.Users.Remove(user);
            await db.SaveChangesAsync();
            
            return Results.NoContent();
        })
        .WithName("DeleteUser");
    }
}
using Microsoft.EntityFrameworkCore;
using MovieRecommendationApp.Infrastructure.Data;

namespace MovieRecommendationApp.API.Endpoints;

public static class GenreEndpoints
{
    public static void MapGenreEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/genres").WithTags("Genres");
        
        // GET /api/genres
        group.MapGet("/", async (AppDbContext db) =>
        {
            var genres = await db.Genres
                .OrderBy(g => g.Name)
                .Select(g => new { g.Id, g.Name })
                .ToListAsync();
                
            return Results.Ok(genres);
        })
        .WithName("GetAllGenres");
        
        // GET /api/genres/{id}
        group.MapGet("/{id:int}", async (int id, AppDbContext db) =>
        {
            var genre = await db.Genres
                .Where(g => g.Id == id)
                .Select(g => new
                {
                    g.Id,
                    g.Name,
                    MoviesCount = g.MovieGenres.Count
                })
                .FirstOrDefaultAsync();
                
            return genre is not null 
                ? Results.Ok(genre) 
                : Results.NotFound(new { error = "Genre not found" });
        })
        .WithName("GetGenreById");
        
        // GET /api/genres/{id}/movies
        group.MapGet("/{id:int}/movies", async (int id, AppDbContext db, int page = 1, int pageSize = 20) =>
        {
            var genre = await db.Genres.FindAsync(id);
            if (genre is null)
            {
                return Results.NotFound(new { error = "Genre not found" });
            }
            
            var query = db.Movies
                .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                .Where(m => m.MovieGenres.Any(mg => mg.GenreId == id))
                .OrderByDescending(m => m.VoteAverage);
            
            var total = await query.CountAsync();
            
            var movies = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new
                {
                    m.Id,
                    m.TmdbId,
                    m.Title,
                    m.ReleaseYear,
                    m.VoteAverage,
                    m.PosterPath
                })
                .ToListAsync();
            
            return Results.Ok(new
            {
                genre = new { genre.Id, genre.Name },
                data = movies,
                page,
                pageSize,
                total,
                totalPages = (int)Math.Ceiling(total / (double)pageSize)
            });
        })
        .WithName("GetMoviesByGenre");
    }
}
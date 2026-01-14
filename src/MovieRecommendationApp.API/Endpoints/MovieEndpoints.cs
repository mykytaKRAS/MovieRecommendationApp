using Microsoft.EntityFrameworkCore;
using MovieRecommendationApp.Application.DTOs.Movies;
using MovieRecommendationApp.Domain.Entities;
using MovieRecommendationApp.Infrastructure.Data;

namespace MovieRecommendationApp.API.Endpoints;

public static class MovieEndpoints
{
    public static void MapMovieEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/movies").WithTags("Movies");
        
        // GET /api/movies
        group.MapGet("/", async (AppDbContext db, int page = 1, int pageSize = 20) =>
        {
            var query = db.Movies
                .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                .OrderByDescending(m => m.CreatedAt);
            
            var total = await query.CountAsync();
            
            var movies = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new MovieDto
                {
                    Id = m.Id,
                    TmdbId = m.TmdbId,
                    Title = m.Title,
                    Overview = m.Overview,
                    ReleaseYear = m.ReleaseYear,
                    Runtime = m.Runtime,
                    PosterPath = m.PosterPath,
                    VoteAverage = m.VoteAverage,
                    VoteCount = m.VoteCount,
                    Genres = m.MovieGenres.Select(mg => mg.Genre.Name).ToList()
                })
                .ToListAsync();
            
            return Results.Ok(new
            {
                data = movies,
                page,
                pageSize,
                total,
                totalPages = (int)Math.Ceiling(total / (double)pageSize)
            });
        })
        .WithName("GetAllMovies");
        
        // GET /api/movies/{id}
        group.MapGet("/{id:int}", async (int id, AppDbContext db) =>
        {
            var movie = await db.Movies
                .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                .Where(m => m.Id == id)
                .Select(m => new MovieDetailDto
                {
                    Id = m.Id,
                    TmdbId = m.TmdbId,
                    Title = m.Title,
                    Overview = m.Overview,
                    ReleaseYear = m.ReleaseYear,
                    Runtime = m.Runtime,
                    PosterPath = m.PosterPath,
                    VoteAverage = m.VoteAverage,
                    VoteCount = m.VoteCount,
                    OriginalLanguage = m.OriginalLanguage,
                    Budget = m.Budget,
                    Revenue = m.Revenue,
                    Status = m.Status,
                    Genres = m.MovieGenres.Select(mg => mg.Genre.Name).ToList()
                })
                .FirstOrDefaultAsync();
                
            return movie is not null 
                ? Results.Ok(movie) 
                : Results.NotFound(new { error = "Movie not found" });
        })
        .WithName("GetMovieById");
        
        // GET /api/movies/tmdb/{tmdbId}
        group.MapGet("/tmdb/{tmdbId:int}", async (int tmdbId, AppDbContext db) =>
        {
            var movie = await db.Movies
                .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                .Where(m => m.TmdbId == tmdbId)
                .Select(m => new MovieDetailDto
                {
                    Id = m.Id,
                    TmdbId = m.TmdbId,
                    Title = m.Title,
                    Overview = m.Overview,
                    ReleaseYear = m.ReleaseYear,
                    Runtime = m.Runtime,
                    PosterPath = m.PosterPath,
                    VoteAverage = m.VoteAverage,
                    VoteCount = m.VoteCount,
                    OriginalLanguage = m.OriginalLanguage,
                    Budget = m.Budget,
                    Revenue = m.Revenue,
                    Status = m.Status,
                    Genres = m.MovieGenres.Select(mg => mg.Genre.Name).ToList()
                })
                .FirstOrDefaultAsync();
                
            return movie is not null 
                ? Results.Ok(movie) 
                : Results.NotFound(new { error = "Movie not found" });
        })
        .WithName("GetMovieByTmdbId");
        
        // GET /api/movies/search
        group.MapGet("/search", async (AppDbContext db, string query, int page = 1, int pageSize = 20) =>
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Results.BadRequest(new { error = "Search query is required" });
            }
            
            var moviesQuery = db.Movies
                .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                .Where(m => EF.Functions.ILike(m.Title, $"%{query}%"))
                .OrderByDescending(m => m.VoteAverage);
            
            var total = await moviesQuery.CountAsync();
            
            var movies = await moviesQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new MovieDto
                {
                    Id = m.Id,
                    TmdbId = m.TmdbId,
                    Title = m.Title,
                    Overview = m.Overview,
                    ReleaseYear = m.ReleaseYear,
                    Runtime = m.Runtime,
                    PosterPath = m.PosterPath,
                    VoteAverage = m.VoteAverage,
                    VoteCount = m.VoteCount,
                    Genres = m.MovieGenres.Select(mg => mg.Genre.Name).ToList()
                })
                .ToListAsync();
            
            return Results.Ok(new
            {
                data = movies,
                query,
                page,
                pageSize,
                total,
                totalPages = (int)Math.Ceiling(total / (double)pageSize)
            });
        })
        .WithName("SearchMovies");
        
        // POST /api/movies
        group.MapPost("/", async (CreateMovieRequest request, AppDbContext db) =>
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return Results.BadRequest(new { error = "Title is required" });
            }
            
            if (request.TmdbId <= 0)
            {
                return Results.BadRequest(new { error = "Valid TMDb ID is required" });
            }
            
            if (await db.Movies.AnyAsync(m => m.TmdbId == request.TmdbId))
            {
                return Results.BadRequest(new { error = "Movie with this TMDb ID already exists" });
            }
            
            var movie = new Movie
            {
                TmdbId = request.TmdbId,
                Title = request.Title,
                Overview = request.Overview,
                ReleaseYear = request.ReleaseYear,
                Runtime = request.Runtime,
                PosterPath = request.PosterPath,
                VoteAverage = request.VoteAverage,
                VoteCount = request.VoteCount,
                OriginalLanguage = request.OriginalLanguage,
                Budget = request.Budget,
                Revenue = request.Revenue,
                Status = request.Status,
                LastUpdated = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
            
            db.Movies.Add(movie);
            await db.SaveChangesAsync();
            
            if (request.GenreIds.Any())
            {
                foreach (var genreId in request.GenreIds)
                {
                    if (await db.Genres.AnyAsync(g => g.Id == genreId))
                    {
                        db.MovieGenres.Add(new MovieGenre
                        {
                            MovieId = movie.Id,
                            GenreId = genreId
                        });
                    }
                }
                await db.SaveChangesAsync();
            }
            
            return Results.Created($"/api/movies/{movie.Id}", new
            {
                movie.Id,
                movie.TmdbId,
                movie.Title,
                movie.ReleaseYear,
                movie.VoteAverage
            });
        })
        .WithName("CreateMovie");
        
        // PUT /api/movies/{id}
        group.MapPut("/{id:int}", async (int id, CreateMovieRequest request, AppDbContext db) =>
        {
            var movie = await db.Movies.Include(m => m.MovieGenres).FirstOrDefaultAsync(m => m.Id == id);
            if (movie is null)
            {
                return Results.NotFound(new { error = "Movie not found" });
            }
            
            movie.Title = request.Title;
            movie.Overview = request.Overview;
            movie.ReleaseYear = request.ReleaseYear;
            movie.Runtime = request.Runtime;
            movie.PosterPath = request.PosterPath;
            movie.VoteAverage = request.VoteAverage;
            movie.VoteCount = request.VoteCount;
            movie.OriginalLanguage = request.OriginalLanguage;
            movie.Budget = request.Budget;
            movie.Revenue = request.Revenue;
            movie.Status = request.Status;
            movie.LastUpdated = DateTime.UtcNow;
            
            db.MovieGenres.RemoveRange(movie.MovieGenres);
            
            if (request.GenreIds.Any())
            {
                foreach (var genreId in request.GenreIds)
                {
                    if (await db.Genres.AnyAsync(g => g.Id == genreId))
                    {
                        db.MovieGenres.Add(new MovieGenre
                        {
                            MovieId = movie.Id,
                            GenreId = genreId
                        });
                    }
                }
            }
            
            await db.SaveChangesAsync();
            
            return Results.Ok(new
            {
                movie.Id,
                movie.TmdbId,
                movie.Title,
                movie.ReleaseYear,
                movie.VoteAverage
            });
        })
        .WithName("UpdateMovie");
        
        // DELETE /api/movies/{id}
        group.MapDelete("/{id:int}", async (int id, AppDbContext db) =>
        {
            var movie = await db.Movies.FindAsync(id);
            if (movie is null)
            {
                return Results.NotFound(new { error = "Movie not found" });
            }
            
            db.Movies.Remove(movie);
            await db.SaveChangesAsync();
            
            return Results.NoContent();
        })
        .WithName("DeleteMovie");
    }
}
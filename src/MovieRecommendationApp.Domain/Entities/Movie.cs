namespace MovieRecommendationApp.Domain.Entities;

public class Movie
{
    public int Id { get; set; }
    public int TmdbId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Overview { get; set; }
    public int? ReleaseYear { get; set; }
    public int? Runtime { get; set; }
    public string? PosterPath { get; set; }
    public decimal? VoteAverage { get; set; }
    public int? VoteCount { get; set; }
    public string? OriginalLanguage { get; set; }
    public long? Budget { get; set; }
    public long? Revenue { get; set; }
    public string? Status { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
    public ICollection<UserMovie> UserMovies { get; set; } = new List<UserMovie>();
}
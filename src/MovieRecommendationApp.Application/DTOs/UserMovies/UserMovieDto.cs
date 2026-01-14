namespace MovieRecommendationApp.Application.DTOs.UserMovies;

public class UserMovieDto
{
    public int Id { get; set; }
    public int MovieId { get; set; }
    public int TmdbId { get; set; }
    public string MovieTitle { get; set; } = string.Empty;
    public string? PosterPath { get; set; }
    public int? ReleaseYear { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal? UserRating { get; set; }
    public string? Review { get; set; }
    public DateTime? WatchedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
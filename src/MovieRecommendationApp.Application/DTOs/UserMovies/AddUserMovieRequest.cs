namespace MovieRecommendationApp.Application.DTOs.UserMovies;

public class AddUserMovieRequest
{
    public int TmdbId { get; set; }
    public string Status { get; set; } = "watched";
    public decimal? UserRating { get; set; }
    public string? Review { get; set; }
    public DateTime? WatchedAt { get; set; }
}
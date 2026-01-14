namespace MovieRecommendationApp.Application.DTOs.UserMovies;

public class UpdateUserMovieRequest
{
    public string? Status { get; set; }
    public decimal? UserRating { get; set; }
    public string? Review { get; set; }
    public DateTime? WatchedAt { get; set; }
}
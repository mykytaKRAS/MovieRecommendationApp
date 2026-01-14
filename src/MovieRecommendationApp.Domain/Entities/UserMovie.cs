namespace MovieRecommendationApp.Domain.Entities;

public class UserMovie
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public int MovieId { get; set; }
    public Movie Movie { get; set; } = null!;
    
    public string Status { get; set; } = "watched";
    public decimal? UserRating { get; set; }
    public string? Review { get; set; }
    public DateTime? WatchedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
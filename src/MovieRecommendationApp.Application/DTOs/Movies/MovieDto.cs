namespace MovieRecommendationApp.Application.DTOs.Movies;

public class MovieDto
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
    public List<string> Genres { get; set; } = new();
}
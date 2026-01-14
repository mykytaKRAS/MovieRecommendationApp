namespace MovieRecommendationApp.Application.DTOs.Movies;

public class CreateMovieRequest
{
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
    public List<int> GenreIds { get; set; } = new();
}
using Microsoft.EntityFrameworkCore;
using MovieRecommendationApp.Domain.Entities;

namespace MovieRecommendationApp.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<MovieGenre> MovieGenres { get; set; }
    public DbSet<UserMovie> UserMovies { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Username).HasColumnName("username").HasMaxLength(50).IsRequired();
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
            entity.Property(e => e.Role).HasColumnName("role").HasMaxLength(20).IsRequired().HasDefaultValue("user");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Role);
        });
        
        // Movie configuration
        modelBuilder.Entity<Movie>(entity =>
        {
            entity.ToTable("movies");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TmdbId).HasColumnName("tmdb_id").IsRequired();
            entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(500).IsRequired();
            entity.Property(e => e.Overview).HasColumnName("overview");
            entity.Property(e => e.ReleaseYear).HasColumnName("release_year");
            entity.Property(e => e.Runtime).HasColumnName("runtime");
            entity.Property(e => e.PosterPath).HasColumnName("poster_path").HasMaxLength(255);
            entity.Property(e => e.VoteAverage).HasColumnName("vote_average").HasPrecision(3, 1);
            entity.Property(e => e.VoteCount).HasColumnName("vote_count");
            entity.Property(e => e.OriginalLanguage).HasColumnName("original_language").HasMaxLength(10);
            entity.Property(e => e.Budget).HasColumnName("budget");
            entity.Property(e => e.Revenue).HasColumnName("revenue");
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(50);
            entity.Property(e => e.LastUpdated).HasColumnName("last_updated").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            entity.HasIndex(e => e.TmdbId).IsUnique();
            entity.HasIndex(e => e.ReleaseYear);
            entity.HasIndex(e => e.VoteAverage);
        });
        
        // Genre configuration
        modelBuilder.Entity<Genre>(entity =>
        {
            entity.ToTable("genres");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever(); // Не auto-increment
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
        });
        
        // MovieGenre configuration (many-to-many)
        modelBuilder.Entity<MovieGenre>(entity =>
        {
            entity.ToTable("movie_genres");
            entity.HasKey(e => new { e.MovieId, e.GenreId });
            entity.Property(e => e.MovieId).HasColumnName("movie_id");
            entity.Property(e => e.GenreId).HasColumnName("genre_id");
            
            entity.HasOne(e => e.Movie)
                .WithMany(m => m.MovieGenres)
                .HasForeignKey(e => e.MovieId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Genre)
                .WithMany(g => g.MovieGenres)
                .HasForeignKey(e => e.GenreId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasIndex(e => e.MovieId);
            entity.HasIndex(e => e.GenreId);
        });
        
        // UserMovie configuration
        modelBuilder.Entity<UserMovie>(entity =>
        {
            entity.ToTable("user_movies");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(e => e.MovieId).HasColumnName("movie_id").IsRequired();
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValue("watched");
            entity.Property(e => e.UserRating).HasColumnName("user_rating").HasPrecision(2, 1);
            entity.Property(e => e.Review).HasColumnName("review");
            entity.Property(e => e.WatchedAt).HasColumnName("watched_at");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.UserMovies)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Movie)
                .WithMany(m => m.UserMovies)
                .HasForeignKey(e => e.MovieId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasIndex(e => new { e.UserId, e.MovieId }).IsUnique();
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.MovieId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.UserRating);
            entity.HasIndex(e => e.WatchedAt);
        });
        
        // Seed data - Genres
        modelBuilder.Entity<Genre>().HasData(
            new Genre { Id = 28, Name = "Action" },
            new Genre { Id = 12, Name = "Adventure" },
            new Genre { Id = 16, Name = "Animation" },
            new Genre { Id = 35, Name = "Comedy" },
            new Genre { Id = 80, Name = "Crime" },
            new Genre { Id = 99, Name = "Documentary" },
            new Genre { Id = 18, Name = "Drama" },
            new Genre { Id = 10751, Name = "Family" },
            new Genre { Id = 14, Name = "Fantasy" },
            new Genre { Id = 36, Name = "History" },
            new Genre { Id = 27, Name = "Horror" },
            new Genre { Id = 10402, Name = "Music" },
            new Genre { Id = 9648, Name = "Mystery" },
            new Genre { Id = 10749, Name = "Romance" },
            new Genre { Id = 878, Name = "Science Fiction" },
            new Genre { Id = 10770, Name = "TV Movie" },
            new Genre { Id = 53, Name = "Thriller" },
            new Genre { Id = 10752, Name = "War" },
            new Genre { Id = 37, Name = "Western" }
        );
    }
}
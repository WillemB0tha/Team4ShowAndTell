using Microsoft.EntityFrameworkCore;

namespace Consumer;

public class CornerDbContext: DbContext
{
    public CornerDbContext(DbContextOptions<CornerDbContext> options): base(options)
    {
        
    }
    public DbSet<ArticleMatrix>? ArticleMatrices { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost,1433;Database=CScrape;User Id=sa;Password=Password123;");
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
    
    
}
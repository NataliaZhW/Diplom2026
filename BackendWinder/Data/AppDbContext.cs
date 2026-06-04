using Microsoft.EntityFrameworkCore;
using BackendWinder.Models;

namespace BackendWinder.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options) { }
    
    public DbSet<BrandManuf> BrandManufs { get; set; }
    public DbSet<ColorThread> ColorThreads { get; set; }
}
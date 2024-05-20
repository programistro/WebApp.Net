using Microsoft.EntityFrameworkCore;
using WebApp.Net.Models;

namespace WebApp.Net.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();

    public DbSet<FilePath> Files => Set<FilePath>();
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=database.db");
    }
}
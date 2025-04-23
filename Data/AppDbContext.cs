namespace LoginApi.Data;

using Microsoft.EntityFrameworkCore;
using LoginApi.Models;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
}

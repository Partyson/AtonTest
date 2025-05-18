using AtonTest.Entities;
using Microsoft.EntityFrameworkCore;

namespace AtonTest.DataContext;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; }
}
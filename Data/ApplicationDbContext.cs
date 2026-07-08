using Microsoft.EntityFrameworkCore;
using BarInventoryAPI.Models;

namespace BarInventoryAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Producto> Productos { get; set; }
}
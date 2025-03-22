using Product.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Product.Infrastructure.Data;

public sealed class ProductsDbContext(DbContextOptions<ProductsDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Products table
    /// </summary>
    public DbSet<Commodity> Products => Set<Commodity>();

    /// <summary>
    /// Dual write problem solving table
    /// </summary>
    public DbSet<Outbox> Outbox => Set<Outbox>();
}

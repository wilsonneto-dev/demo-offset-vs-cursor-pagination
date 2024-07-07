using Microsoft.EntityFrameworkCore;

namespace Data;

public class ProductContext(DbContextOptions<ProductContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
}

namespace Data;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public Decimal Price { get; set; } = (decimal) Random.Shared.Next(0, 1_000_000) / 100m;
    public Guid Code { get; set; } = Guid.NewGuid();
}
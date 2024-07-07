using Bogus;
using Data;

namespace Seeder;

public class Worker(IServiceProvider serviceProvider, ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        double cycles = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ProductContext>();

                var faker = new Faker<Product>()
                    .RuleFor(p => p.Name, f => f.Commerce.ProductName());

                var products = faker.Generate(20_000);

                context.Products.AddRange(products);
                await context.SaveChangesAsync(stoppingToken);

                logger.LogInformation("+{Count} - {date} products generated and saved to the database", products.Count, DateTime.UtcNow.TimeOfDay);
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken); // Adjust the interval as needed
                
                if(++cycles % 20 == 0)
                    Console.WriteLine($"Total: {context.Products.Count()} products.");
                
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken); // Adjust the interval as needed
            }
        }
    }
}
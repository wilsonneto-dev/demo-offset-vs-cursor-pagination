using Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ProductContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        cfg => cfg.CommandTimeout(300)));

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// offset based pagination
app.MapGet("/products", (ProductContext db, int currentPage = 1, int perPage = 10) => 
    db.Products
        .OrderByDescending(x => x.Id)
        .Skip((currentPage -1) * perPage)
        .Take(perPage)
        .AsNoTracking()
        .ToList());


// cursor based pagination
app.MapGet("/v2/products", (ProductContext db, HttpContext ctx, int? lastSeen = null, int perPage = 10) =>
{
    IQueryable<Product> query = db.Products
        .OrderByDescending(x => x.Id);
    if(lastSeen is not null)
        query = query.Where(x => x.Id < lastSeen);
    var products = query.Take(perPage).AsNoTracking().ToList();
    
    ctx.Response.Headers.Append("last-seen", products[^1].Id.ToString());

    return products;
});

app.Run();
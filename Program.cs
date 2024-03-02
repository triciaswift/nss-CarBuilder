using CarBuilder.Models;
using CarBuilder.Models.DTOs;

List<PaintColor> paintColors = new List<PaintColor>
{
    new PaintColor {Id = 1, Price = 499.99M, Color = "Silver"},
    new PaintColor {Id = 2, Price = 599.99M, Color = "Midnight Blue"},
    new PaintColor {Id = 3, Price = 699.99M, Color = "Firebrick Red"},
    new PaintColor {Id = 4, Price = 799.99M, Color = "Spring Green"}
};

List<Interior> interiors = new List<Interior>
{
    new Interior {Id = 1, Price = 349.99M, Material = "Beige Fabric"},
    new Interior {Id = 2, Price = 449.99M, Material = "Charcoal Fabric"},
    new Interior {Id = 3, Price = 549.99M, Material = "White Leather"},
    new Interior {Id = 4, Price = 649.99M, Material = "Black Leather"}
};

List<Technology> technologies = new List<Technology>
{
    new Technology {Id = 1, Price = 579.99M, Package = "Basic Package"},
    new Technology {Id = 2, Price = 679.99M, Package = "Navigation Package"},
    new Technology {Id = 3, Price = 779.99M, Package = "Visibility Package"},
    new Technology {Id = 4, Price = 1279.99M, Package = "Ultra Package"}
};

List<Wheels> wheels = new List<Wheels>
{
    new Wheels {Id = 1, Price = 429.99M, Style = "17-inch Pair Radial"},
    new Wheels {Id = 2, Price = 529.99M, Style = "17-inch Pair Radial Black"},
    new Wheels {Id = 3, Price = 629.99M, Style = "17-inch Pair Spoke Silver"},
    new Wheels {Id = 4, Price = 729.99M, Style = "17-inch Pair Spoke Black"}
};

List<Order> orders = new List<Order>
{
    new Order {Id = 1, Timestamp = new DateTime(2023, 8, 24), WheelId = 4, TechnologyId = 4, PaintId = 4, InteriorId = 4}
};


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//! CORS - Cross-Origin Resource Sharing
// ports that the client and server are served from are different
builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    //! add CORS here as well
    app.UseCors(options =>
    {
        options.AllowAnyOrigin();
        options.AllowAnyMethod();
        options.AllowAnyHeader();
    });
}

app.UseHttpsRedirection();


// endpoints


// GET - retrieves all wheels
app.MapGet("/wheels", () =>
{
    return wheels.Select(w => new WheelsDTO
    {
        Id = w.Id,
        Price = w.Price,
        Style = w.Style
    });
});

// GET - retrieves all technologies
app.MapGet("/technologies", () =>
{
    return technologies.Select(t => new TechnologyDTO
    {
        Id = t.Id,
        Price = t.Price,
        Package = t.Package
    });
});

// GET - retrieves all interiors
app.MapGet("/interiors", () => 
{
    return interiors.Select(i => new InteriorDTO
    {
        Id = i.Id,
        Price = i.Price,
        Material = i.Material
    });
});

// GET - retrieves all paint colors
app.MapGet("/paintcolors", () => 
{
    return paintColors.Select(pc => new PaintColorDTO
    {
        Id = pc.Id,
        Price = pc.Price,
        Color = pc.Color
    });
});

// GET - retrieves all unfulfilled orders & their properties
app.MapGet("/orders", () =>
{
    // filter out orders marked as fulfilled
    orders = orders.Where(o => o.Fulfilled == false).ToList();
    
    return orders.Select(o => 
    {
        Wheels? wheel = wheels.FirstOrDefault(w => w.Id == o.WheelId);
        Technology? technology = technologies.FirstOrDefault(t => t.Id == o.TechnologyId);
        Interior? interior = interiors.FirstOrDefault(i => i.Id == o.InteriorId);
        PaintColor? paintColor = paintColors.FirstOrDefault(pc => pc.Id == o.PaintId);

        return new OrderDTO
        {
            Id = o.Id,
            Timestamp = o.Timestamp,
            WheelId = o.WheelId,
            Wheels = wheel == null ? null : new WheelsDTO
            {
                Id = wheel.Id,
                Price = wheel.Price,
                Style = wheel.Style
            },
            TechnologyId = o.TechnologyId,
            Technology = technology == null ? null : new TechnologyDTO
            {
                Id = technology.Id,
                Price = technology.Price,
                Package = technology.Package
            },
            InteriorId = o.InteriorId,
            Interior = interior == null ? null : new InteriorDTO
            {
                Id = interior.Id,
                Price = interior.Price,
                Material = interior.Material
            },
            PaintId = o.PaintId,
            PaintColor = paintColor == null ? null : new PaintColorDTO
            {
                Id = paintColor.Id,
                Price = paintColor.Price,
                Color = paintColor.Color
            },
        Fulfilled = o.Fulfilled
        };
    });
});

// GET - retrieve a single order
app.MapGet("/orders/{id}", (int id) =>
{
    Order order = orders.FirstOrDefault(o => o.Id == id);
    if (order == null)
    {
        return Results.NotFound();
    }

    Wheels? wheel = wheels.FirstOrDefault(w => w.Id == order.WheelId);
    Technology? technology = technologies.FirstOrDefault(t => t.Id == order.TechnologyId);
    Interior? interior = interiors.FirstOrDefault(i => i.Id == order.InteriorId);
    PaintColor? paintColor = paintColors.FirstOrDefault(pc => pc.Id == order.PaintId);

    return Results.Ok(new OrderDTO
    {
        Id = order.Id,
        Timestamp = order.Timestamp,
        WheelId = order.WheelId,
        Wheels = wheel == null ? null : new WheelsDTO
        {
            Id = wheel.Id,
            Price = wheel.Price,
            Style = wheel.Style
        },
        TechnologyId = order.TechnologyId,
        Technology = technology == null ? null : new TechnologyDTO
        {
            Id = technology.Id,
            Price = technology.Price,
            Package = technology.Package
        },
        InteriorId = order.InteriorId,
        Interior = interior == null ? null : new InteriorDTO
        {
            Id = interior.Id,
            Price = interior.Price,
            Material = interior.Material
        },
        PaintId = order.PaintId,
        PaintColor = paintColor == null ? null : new PaintColorDTO
        {
            Id = paintColor.Id,
            Price = paintColor.Price,
            Color = paintColor.Color
        },
        Fulfilled = order.Fulfilled
    });
});

// POST - creates a new order
app.MapPost("/orders", (Order order) =>
{
    Wheels? wheel = wheels.FirstOrDefault(w => w.Id == order.WheelId);
    Technology? technology = technologies.FirstOrDefault(t => t.Id == order.TechnologyId);
    Interior? interior = interiors.FirstOrDefault(i => i.Id == order.InteriorId);
    PaintColor? paintColor = paintColors.FirstOrDefault(pc => pc.Id == order.PaintId);

    if (wheel == null || technology == null || interior == null || paintColor == null)
    {
        return Results.BadRequest();
    }

    order.Timestamp = DateTime.Now;

    order.Id = orders.Max(o => o.Id) + 1;
    orders.Add(order);

    return Results.Created("/orders", new OrderDTO
    {
        Id = order.Id,
        Timestamp = order.Timestamp,
        WheelId = order.WheelId,
        Wheels = new WheelsDTO
        {
            Id = wheel.Id,
            Price = wheel.Price,
            Style = wheel.Style
        },
        TechnologyId = order.TechnologyId,
        Technology = new TechnologyDTO
        {
            Id = technology.Id,
            Price = technology.Price,
            Package = technology.Package
        },
        InteriorId = order.InteriorId,
        Interior = new InteriorDTO
        {
            Id = interior.Id,
            Price = interior.Price,
            Material = interior.Material
        },
        PaintId = order.PaintId,
        PaintColor = new PaintColorDTO
        {
            Id = paintColor.Id,
            Price = paintColor.Price,
            Color = paintColor.Color
        },
        Fulfilled = false
    });
});

// Fulfilled Orders
app.MapPost("/orders/{id}/fulfill", (int id) => 
{
    Order? orderToFulfill = orders.FirstOrDefault(o => o.Id == id);

    orderToFulfill.Fulfilled = true;
});

app.Run();

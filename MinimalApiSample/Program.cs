using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApiSample.Models;

var builder = WebApplication.CreateBuilder(args); //--initialize web application

//--inject ms sql db context
builder.Services.AddDbContext<SampleTestDBContext> (optional => optional.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//--inject repository to our application
//builder.Services.AddSingleton<ItemRepositiory>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build(); //--initialize web application

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//--
app.MapGet("/items", async (SampleTestDBContext db) =>
{
    return await db.Items.ToListAsync();
});

app.MapPost("/addItem", async (SampleTestDBContext db, Item item) =>
{
    if(await db.Items.FirstOrDefaultAsync(a => a.Id == item.Id) != null)
    {
        return Results.NotFound();
    }
    await db.AddAsync(item);
    await db.SaveChangesAsync();
    return Results.Created($"Items/{item.Id}",item);
});

app.MapGet("/items/{id}", async (SampleTestDBContext db, int id) =>
{
    var item = await db.Items.FirstOrDefaultAsync(a => a.Id == id);
    return item == null ? Results.NotFound() : Results.Ok(item);
});

app.MapPut("/update/{id}", async (SampleTestDBContext db, int id, Item item) =>
{
    var existItem = await db.Items.FirstOrDefaultAsync(a => a.Id == id);
    if (existItem == null)
    {
        return Results.NotFound();
    }

    existItem.Title = item.Title;
    existItem.IsCompleted = item.IsCompleted;

    db.Items.Update(existItem);
    await db.SaveChangesAsync();
    return Results.Ok(item);
});

app.MapDelete("/delate/{id}", async (SampleTestDBContext db, int id) =>
{
    var existingItem = await db.Items.FirstOrDefaultAsync(a => a.Id == id);
    if (existingItem == null)
    {
        return Results.NotFound();
    }

    db.Items.Remove(existingItem);
    await db.SaveChangesAsync();
    return Results.NoContent();
});


//--
//1st hrrp api
//() => {} -lamba function
app.MapGet("/", () =>
{
    return "Hello From Minimal API";
});

app.Run();

////--
////class
//class ItemRequest
//{
//    public int id { get; set; }
//    public string title { get; set; }
//    public string isCompleted { get; set; }
//}

////--
////record 
////can create above class in one line
//record Item(int id, string title, bool isCompleted);



////--
//app.MapGet("/items", ([FromServices] ItemRepositiory items) =>
//{
//    return items.GetAll();
//});

//app.MapPost("/addItem", ([FromServices] ItemRepositiory items, Item item) =>
//{
//    if (items.GetById(item.id) != null)
//    {
//        return Results.NotFound();
//    }
//    items.Add(item);
//    return Results.Created($"Items/{item.id}", item);
//});

//app.MapGet("/items/{id}", ([FromServices] ItemRepositiory items, int id) =>
//{
//    var item = items.GetById(id);
//    return item == null ? Results.NotFound() : Results.Ok(item);
//});

//app.MapPut("/update/{id}", ([FromServices] ItemRepositiory items, int id, Item item) =>
//{
//    if (items.GetById(id) == null)
//    {
//        return Results.NotFound();
//    }
//    items.Update(item);
//    return Results.Ok(item);
//});

//app.MapDelete("/delate/{id}", ([FromServices] ItemRepositiory items, int id) =>
//{
//    if (items.GetById(id) == null)
//    {
//        return Results.NotFound();
//    }

//    items.Delate(id);
//    return Results.NoContent();
//});



////--in memory db
//class ItemRepositiory
//{
//    private Dictionary<int, Item> items = new Dictionary<int, Item>();

//    public ItemRepositiory()
//    {
//        var item1 = new Item(1, "Go to Lecture", true);
//        var item2 = new Item(2, "Coock rice", false);
//        var item3 = new Item(3, "Drick cofee", true);

//        items.Add(item1.id, item1);
//        items.Add(item2.id, item2);
//        items.Add(item3.id, item3);

//    }

//    //get all data from db
//    public IEnumerable<Item> GetAll () => items.Values;

//    //get data by id
//    public Item GetById (int id)
//    { 
//        if(items.ContainsKey(id))
//        {
//            return items[id];
//        }
//        return null;
//    }


//    //add data to Item
//    public void Add (Item item) => items.Add(item.id, item);

//    //update data
//    public void Update(Item item) => items[item.id] = item;

//    //delate data
//    public void Delate(int id) => items.Remove(id);
//}






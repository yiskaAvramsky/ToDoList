
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TodoApi;
  // "ToDoDB": "server=localhost;user=root;password=aA1795aA;database=ToDoDB"
//   "ToDoDB": "server=bw7v1imp71cys7uyq3rm-mysql.services.clever-cloud.com;user=uetdqrwjwrgz243d;password=5anNmj9CQUi8RpmjUhMu;database=ToDoDB;port=3306"

var builder = WebApplication.CreateBuilder(args);

// הוספת שירותי Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ToDo API",
        Version = "v1",
        Description = "API for managing ToDo items",
        Contact = new OpenApiContact
        {
            Name = "Your Name",
            Email = "your_email@example.com",
        },
    });
});

builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("ToDoDB") ?? 
                     Environment.GetEnvironmentVariable("ToDoDB"),
                     new MySqlServerVersion(new Version(8, 0, 0))));

// הוספת שירותי CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("https://todolist-rfhi.onrender.com","http://localhost:3000/") // הוספת כתובת ה-Frontend ברנדר
              .AllowAnyHeader() // הרשאת כותרות
              .AllowAnyMethod(); // הרשאת מתודות (GET, POST, PUT, DELETE)
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("https://todolist-rfhi.onrender.com") // הוספת כתובת ה-Frontend ברנדר
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});



var app = builder.Build();

// שימוש במדיניות ה-CORS
app.UseCors("AllowSpecificOrigins");

// הפעלת Swagger רק בסביבת פיתוח
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDo API v1");
        options.RoutePrefix = ""; // מציג את Swagger בדף הראשי
    });
// }

app.MapGet("/", () => "Hello World!");


// שליפת כל המשימות
app.MapGet("/tasks", async (ToDoDbContext db) => await db.Items.ToListAsync());

// שליפת משימה 
app.MapGet("/tasks/{id}", async (ToDoDbContext db, int id) => {
  var item = await db.Items.FindAsync(id);
    return item;
});


// הוספת משימה חדשה
app.MapPost("/tasks", async (ToDoDbContext db, Item newItem) =>
{
    db.Items.Add(newItem);
    await db.SaveChangesAsync();
    return Results.Created($"/tasks/{newItem.Id}", newItem);
});

// עדכון משימה
app.MapPut("/tasks/{id}", async (ToDoDbContext db, int id, Item updatedItem) =>
{
    var item = await db.Items.FindAsync(id);
    if (item is null) return Results.NotFound();

    item.Name = updatedItem.Name;
    item.IsComplete = updatedItem.IsComplete;
    await db.SaveChangesAsync();

    return Results.NoContent();
});

// מחיקת משימה
app.MapDelete("/tasks/{id}", async (ToDoDbContext db, int id) =>
{
    var item = await db.Items.FindAsync(id);
    if (item is null) return Results.NotFound();

    db.Items.Remove(item);
    await db.SaveChangesAsync();

    return Results.Ok();
});

app.Run();

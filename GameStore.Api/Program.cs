using System.Text.Json.Serialization;
using GameStore.Api.Data;
using GameStore.Api.DTO;
using GameStore.Api.Entities;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<GameStoreContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultString"),
        new MySqlServerVersion(new Version(8, 0, 41)) 
    )
);
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        options.JsonSerializerOptions.MaxDepth = 64; // You can adjust max depth if needed.
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")  // Allow Angular frontend
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});
var app = builder.Build();
app.UseCors("AllowAngular");  
app.MapGet("/GetGames", async (GameStoreContext db) => await db.Games.Include(g => g.Genre).ToListAsync());

app.MapGet("/GetGameById/{Id}", async (GameStoreContext db, int Id) => {
    var game = await db.Games.Include(g => g.Genre).FirstOrDefaultAsync(g => g.Id == Id);

    if(game == null){
        return Results.NotFound(new { message = "No game with that id exists"});
    }
    
    return Results.Ok(game);
});

app.MapPost("/InsertGame", async (GameStoreContext db, GameDto newGame) => {

   var game = await db.Games.FirstOrDefaultAsync(g => g.Name == newGame.Name);

   if(game != null){
      return Results.Conflict(new {message = "Game with that name already exists"});
   }

   var newEntity = new Game
        {
            Name = newGame.Name,
            GenreId = newGame.GenreId,
            Price = newGame.Price,
            ReleaseDate = newGame.ReleaseDate
        };


   db.Add(newEntity);
   await db.SaveChangesAsync();

   return Results.Ok(new { message = "New Game added"});
});


app.MapDelete("/DeleteGame/{Name}", async (GameStoreContext db, string Name) => {
    var game = await db.Games.FirstOrDefaultAsync(game => game.Name == Name);

    if(game == null){
        return Results.NotFound(new { message = "No game found"});
    }

    db.Games.Remove(game);
    await db.SaveChangesAsync();

    return Results.Ok(new {messsage = "Game deleted"});
});

app.MapPut("UpdateGame", async (GameStoreContext db, GameDto updatedGame) => {
    var game = await db.Games.FirstOrDefaultAsync(game => game.Name == updatedGame.Name);

    if(game == null){
        return Results.NotFound(new {message = "No game found"});
    }

    if(updatedGame.Name != null) game.Name = updatedGame.Name;
    if(updatedGame.GenreId != 0) game.GenreId =  updatedGame.GenreId;
    if(updatedGame.Price != 0) game.Price = updatedGame.Price;
    if(updatedGame.ReleaseDate != default ) game.ReleaseDate = updatedGame.ReleaseDate;

    await db.SaveChangesAsync();
    return Results.Ok(new { message = "Game info updated", game});
});




app.Run();


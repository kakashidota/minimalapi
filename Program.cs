using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<Library>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/games", (Library games) => {
    return games.GetAll();
});

app.MapGet("/games/{id}", (Library game, int id) =>
{
    return game.GetById(id);
});

app.MapPost("/AddGame", (Library games, Game game) =>
{
    if(games.GetById(game.id) == null)
    {
        games.Add(game);
        return Results.Created($"/items/{game.id}", game);
    }

    return Results.BadRequest();
});


app.MapPut("/Updategames/{id}", (Library games, int id, Game game) =>
{
    if(games.GetById(id) == null)
    {
        return Results.BadRequest();
    }

    games.Update(game);
    return Results.Ok();
});

app.MapDelete("/Deletegames/{id}", (Library games, int id) =>
{
    if (games.GetById(id) == null)
    {
        return Results.BadRequest();
    }

    games.Delete(id);
    return Results.Ok();
});

app.UseHttpsRedirection();

app.Run();

record Game(int id, string titles, bool completed);

class Library
{
    private readonly Dictionary<int, Game> _games = new Dictionary<int, Game>();

    public Library()
    {
        var game1 = new Game(1, "Breathe of the wild", true);
        var game2 = new Game(2, "CS:GO", true);
        var game3 = new Game(3, "Crash Bandicot", false);

        _games.Add(game1.id, game1);
        _games.Add(game2.id, game2);
        _games.Add(game3.id, game3);

    }

    
    public List<Game> GetAll() => _games.Values.ToList();
    public Game GetById(int id) {

        if (_games.ContainsKey(id))
        {
            return _games[id];
        }

        return null;
    }
    public void Add(Game game) => _games.Add(game.id, game);
    public void Update(Game game) => _games[game.id] = game;
    public void Delete(int id) => _games.Remove(id);


}

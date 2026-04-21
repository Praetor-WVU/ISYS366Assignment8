using ISYS366Assignment3.Data;
using ISYS366Assignment3.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<MovieRepoEf>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/api/movies", async ([FromServices] MovieRepoEf repo) =>
{
    return Results.Ok(await repo.GetAllAsync());
});

app.MapGet("/api/movies/{id:int}", async ([FromServices] MovieRepoEf repo, int id) =>
{
    var movie = await repo.GetByIdAsync(id);
    return movie is not null ? Results.Ok(movie) : Results.NotFound();
});

app.MapPost("/api/movies", async ([FromServices] MovieRepoEf repo, [FromBody] Movie movie) =>
{
    await repo.AddAsync(movie);
    return Results.Created($"/api/movies/{movie.Id}", movie);
});

app.MapPut("/api/movies/{id:int}", async ([FromServices] MovieRepoEf repo, int id, [FromBody] Movie updatedMovie) =>
{
    var movie = await repo.GetByIdAsync(id);
    if (movie is null) return Results.NotFound();

    movie.Title = updatedMovie.Title;
    movie.ReleaseDate = updatedMovie.ReleaseDate;
    movie.Genre = updatedMovie.Genre;
    movie.Price = updatedMovie.Price;
    movie.Rank = updatedMovie.Rank;
    movie.ImageUri = updatedMovie.ImageUri;

    await repo.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/api/movies/{id:int}", async ([FromServices] MovieRepoEf repo, int id) =>
{
    var movie = await repo.GetByIdAsync(id);
    if (movie is null) return Results.NotFound();

    await repo.RemoveAsync(movie);
    return Results.NoContent();
});

app.Run();



using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TodoAPI.Data;
using TodoAPI.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DBContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

//testing
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();


app.MapGet("/todos", async (DBContext context) =>
{
    return await context.ToDos.ToListAsync();
});

app.MapPost("/todos", async (DBContext context, ToDo todo) =>
{
    todo.CreatedAt = DateTime.UtcNow;
    context.ToDos.Add(todo);
    await context.SaveChangesAsync();
    return Results.Created($"/todos/{todo.Id}", todo);

});


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.Run();

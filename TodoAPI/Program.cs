using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TodoAPI.Data;
using TodoAPI.Models;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DBContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
}); 

//testing
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();



//Create a ToDo item
app.MapPost("/todos", async (DBContext context, ToDo todo) =>
{
    context.ToDos.Add(todo);
    await context.SaveChangesAsync();
    return Results.Created($"/todos/{todo.Id}", todo);

});

// Update a ToDo item
app.MapPatch("/todos/{id}", async (int id, DBContext context, UpdateToDoRequest updateRequest) =>
{
    var todoToUpdate = await context.ToDos.FindAsync(id);
    
    if (todoToUpdate == null)
    {
        return Results.NotFound();
    }
    
    if (!string.IsNullOrEmpty(updateRequest.Title))
    {
        todoToUpdate.Title = updateRequest.Title;
    }

    if (updateRequest.CurrentStatus != null)
    {
        todoToUpdate.CurrentStatus = updateRequest.CurrentStatus.Value;
    }
    
    if (!string.IsNullOrEmpty(updateRequest.Description))
    {
        todoToUpdate.Description = updateRequest.Description;
    }
    
    if (updateRequest.Priority != null)
    {
        todoToUpdate.Priority = updateRequest.Priority.Value;
    }
    
    await context.SaveChangesAsync();
    return Results.Ok(todoToUpdate);
});

// Get a ToDo item by ID
app.MapGet("/todos/{id}", async (int id, DBContext context) =>
{
    var todo = await context.ToDos.FindAsync(id);
    if (todo == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(todo);
});

// Delete a ToDo item
app.MapDelete("/todos/{id}", async (int id, DBContext context) =>
{
    var todoToDelete = await context.ToDos.FindAsync(id);
    if (todoToDelete == null)
    {
        return Results.NotFound();
    }
    context.ToDos.Remove(todoToDelete);
    await context.SaveChangesAsync();
    return Results.NoContent();
});

// Get all ToDo items with optional filtering by status and search term
app.MapGet("/todos", async (DBContext context, Status? status, string? search, int? id) =>
{
    var query = context.ToDos.AsQueryable();
    if (id.HasValue)
    {
        query = query.Where(todo => todo.Id == id.Value);
    }
    if (status.HasValue)
    {
        query = query.Where(todo => todo.CurrentStatus == status.Value);
    }
    if (!string.IsNullOrEmpty(search))
    {
        query = query.Where(todo => todo.Title.Contains(search) || (todo.Description != null && todo.Description.Contains(search)));
    }
    return await query.ToListAsync();
});


app.UseHttpsRedirection();
app.UseCors("AllowReactApp");

app.Run();

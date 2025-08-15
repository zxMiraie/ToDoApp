using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Models;

public class ToDo
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public Status CurrentStatus { get; set; } = Status.NotStarted;
    public DateTime CreatedAt { get; set; }
}
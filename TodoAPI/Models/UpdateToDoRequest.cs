using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Models;

public class UpdateToDoRequest
{
    [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters")]
    [MinLength(1, ErrorMessage = "Title must be at least 1 character")]
    public string? Title { get; set; }
    
    public string? Description { get; set; }
    
    public Status? CurrentStatus { get; set; }
    
    public Priority? Priority { get; set; }
}


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entity;

[Table("application_users")]
public sealed class ApplicationUser : IEntity
{
    [Key]
    public int Id { get; set; }
    
    public Guid Uuid { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = default!;
    
    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = default!;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
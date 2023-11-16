namespace DataAccess.Entity;

public interface IEntity
{
    int Id { get; set; }
    Guid Uuid { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
}
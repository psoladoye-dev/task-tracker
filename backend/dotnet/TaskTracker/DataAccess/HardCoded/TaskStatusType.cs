using System.ComponentModel.DataAnnotations;
using Util;

namespace DataAccess.HardCoded;

public sealed class TaskStatusType : IHardCodedEntity
{
    private TaskStatusType(TaskStatusTypeEnum taskStatusTypeEnum)
    {
        Id = (int) taskStatusTypeEnum;
        Name = taskStatusTypeEnum.ToString();
        Description = taskStatusTypeEnum.GetEnumDescription();
    }

    public TaskStatusType() { } // Entity Framework
    
    [Key]
    public int Id { get; set; }
    
    [MaxLength(100)]
    [Required]
    public string Name { get; set; } = default!;
    
    [MaxLength(100)]
    [Required]
    public string Description { get; set; } = default!;

    public static implicit operator TaskStatusType(TaskStatusTypeEnum taskStatusTypeEnum) => new(taskStatusTypeEnum);
    public static implicit operator TaskStatusTypeEnum(TaskStatusType taskStatusType) => (TaskStatusTypeEnum)taskStatusType.Id;
}
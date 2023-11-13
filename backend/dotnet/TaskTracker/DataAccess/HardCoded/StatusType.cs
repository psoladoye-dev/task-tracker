using System.ComponentModel.DataAnnotations;
using Util;

namespace DataAccess.HardCoded;

public sealed class StatusType : IHardCodedEntity
{
    private StatusType(StatusTypeEnum statusTypeEnum)
    {
        Id = (int) statusTypeEnum;
        Name = statusTypeEnum.ToString();
        Description = statusTypeEnum.GetEnumDescription();
    }

    public StatusType() { } // Entity Framework
    
    [Key]
    public int Id { get; set; }
    
    [MaxLength(100)]
    [Required]
    public string Name { get; set; } = default!;
    
    [MaxLength(100)]
    [Required]
    public string Description { get; set; } = default!;

    public static implicit operator StatusType(StatusTypeEnum statusTypeEnum) => new(statusTypeEnum);
    public static implicit operator StatusTypeEnum(StatusType statusType) => (StatusTypeEnum)statusType.Id;
}
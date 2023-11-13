using System.ComponentModel;
using System.Text.Json.Serialization;

namespace DataAccess.HardCoded;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StatusTypeEnum
{
    [Description("To Do")]
    ToDo = 1,
    
    [Description("In Progress")]
    InProgress = 2,
    
    [Description("Done")]
    Done = 3
}
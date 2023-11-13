using System.ComponentModel;

namespace Util;

public static class EnumExtensions
{
    public static string GetEnumDescription<TEnum>(this TEnum @enum)
    {
        ArgumentNullException.ThrowIfNull(@enum);
        return @enum.GetType()
            .GetField(@enum.ToString()!)
            ?.GetCustomAttributes(typeof(DescriptionAttribute), false)
            .Cast<DescriptionAttribute>()
            .FirstOrDefault()?.Description ?? string.Empty;
    }
}
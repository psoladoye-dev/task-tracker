using Microsoft.EntityFrameworkCore;

namespace DataAccess.Extensions;

public static class ModelBuilderExtensions
{
    public static void SeedEnumItems<T, TEnum>(this ModelBuilder modelBuilder, Func<TEnum, T> converter) where T : class
    {
        Enum.GetValues(typeof(TEnum))
            .Cast<object>()
            .Select(value => converter((TEnum) value))
            .ToList()
            .ForEach(instance => modelBuilder.Entity<T>().HasData(instance));
    }
}
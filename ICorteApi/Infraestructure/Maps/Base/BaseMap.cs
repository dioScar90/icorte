using System.Text.RegularExpressions;
using ICorteApi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICorteApi.Infraestructure.Maps;

public abstract class BaseMap<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : class, IBaseTableEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        string? currentTableName = builder.Metadata.GetTableName();

        if (!string.IsNullOrEmpty(currentTableName))
        {
            string newTableName = CamelCaseToSnakeCase(currentTableName);
            builder.ToTable(newTableName);
        }

        foreach (var prop in typeof(TEntity).GetProperties())
        {
            if (!IsPrimitiveType(prop.PropertyType))
                continue;

            if (IsCompositeKeyName(prop.Name))
            {
                builder.Ignore(prop.Name);
                continue;
            }

            string column_name = CamelCaseToSnakeCase(prop.Name);
            builder.Property(prop.Name).HasColumnName(column_name);

            if (prop.PropertyType == typeof(decimal))
                builder.Property(prop.Name).HasPrecision(9, 4);

            if (prop.PropertyType.IsEnum && !IsUnableToBecomeString(prop.PropertyType))
                builder.Property(prop.Name).HasConversion<string>();
        }

        if (TEntityImplementsIPrimaryKeyEntity())
            builder.HasQueryFilter(x => !((IBaseEntity<TEntity>)x).IsDeleted); // same as 'x => !x.IsDeleted'
    }

    private static bool TEntityImplementsIPrimaryKeyEntity() =>
        typeof(IBaseEntity<TEntity>).IsAssignableFrom(typeof(TEntity));

    private static string CamelCaseToSnakeCase(string name) => Regex.Replace(name, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();

    private static bool IsCompositeKeyName(string name) => name.StartsWith("Id") && name.Length > 2;

    private static bool IsPrimitiveType(Type type)
    {
        // This line is necessary for allow nullable types.
        Type underlyingType = Nullable.GetUnderlyingType(type) ?? type;

        return underlyingType.IsPrimitive
            || underlyingType.IsEnum
            || underlyingType == typeof(string)
            || underlyingType == typeof(float)
            || underlyingType == typeof(decimal)
            || underlyingType == typeof(DateTime)
            || underlyingType == typeof(DateOnly)
            || underlyingType == typeof(TimeOnly)
            || underlyingType == typeof(DateTimeOffset)
            || underlyingType == typeof(TimeSpan)
            || underlyingType == typeof(Guid);
    }

    private static bool IsUnableToBecomeString(Type type)
    {
        // This line is necessary for allow nullable types.
        Type underlyingType = Nullable.GetUnderlyingType(type) ?? type;

        return underlyingType == typeof(DayOfWeek);
    }
}

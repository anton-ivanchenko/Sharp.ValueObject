using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq.Expressions;

namespace Sharp.ValueObject.EntityFrameworkCore
{
    public static class EntityTypeBuilderExtensions
    {
        public static ValueObjectBuilder<TEntity, TValueObject> ValueObject<TEntity, TValueObject>(this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, TValueObject?>> propertyExpression)
            where TEntity : class
            where TValueObject : ValueObject
        {
            return new ValueObjectBuilder<TEntity, TValueObject>(propertyExpression, builder, builder.OwnsOne(propertyExpression));
        }
    }
}

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq.Expressions;

namespace Sharp.ValueObject.EntityFrameworkCore
{
    public class ValueObjectBuilder<TEntity, TValueObject>
        where TEntity : class
        where TValueObject : ValueObject
    {
        private readonly EntityTypeBuilder<TEntity> _entityTypeBuilder;
        private readonly OwnedNavigationBuilder<TEntity, TValueObject> _ownedTypeBuilder;

        private readonly Expression<Func<TEntity, TValueObject?>> _valueObjectProperty;

        public ValueObjectBuilder(
            Expression<Func<TEntity, TValueObject?>> valueObject,
            EntityTypeBuilder<TEntity> entityTypeBuilder,
            OwnedNavigationBuilder<TEntity, TValueObject> ownedTypeBuilder)
        {
            _ownedTypeBuilder = ownedTypeBuilder;
            _valueObjectProperty = valueObject;
            _entityTypeBuilder = entityTypeBuilder;
        }

        public ValueObjectBuilder<TEntity, TValueObject> ValueProperty(Action<PropertyBuilder> action)
        {
            action.Invoke(_ownedTypeBuilder.Property("Value"));
            return this;
        }

        public ValueObjectBuilder<TEntity, TValueObject> IsRequired(bool required = true)
        {
            ValueProperty(v => v.IsRequired(required));
            _entityTypeBuilder.Navigation(_valueObjectProperty).IsRequired(required);
            return this;
        }
    }
}

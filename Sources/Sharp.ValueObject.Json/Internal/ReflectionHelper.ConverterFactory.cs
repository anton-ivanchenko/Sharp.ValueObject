using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Sharp.ValueObject.Json.Internal
{
    internal static partial class ReflectionHelper
    {
        public static Func<JsonConverter> GenerateJsonConverterFactory(Type targetType)
        {
            if (ValueObject.IsSingleValueObjectType(targetType))
            {
                return CreateSingleValueObjectFactory(targetType);
            }

            if (TryGetEnumerableGenericParameter(targetType, out Type? valueType))
            {
                return CreateEnumerableValueObjectFactory(targetType, valueType);
            }

            throw new InvalidOperationException(@$"This type of converter cannot be used to handle {targetType}");

            static Func<JsonConverter> CreateSingleValueObjectFactory(Type typeToConvert)
            {
                Type genericValueObjectType = ValueObject.GetGenericValueObjectType(typeToConvert);
                Type innerValueType = ValueObject.GetSingleValueObjectInnerValueType(genericValueObjectType);

                Type[] genericArguments = genericValueObjectType.GetGenericArguments();
                Type converterType = typeof(ValueObjectConverter<,>).MakeGenericType(genericArguments);

                Type converterConstructorParameter = typeof(IEqualityComparer<>).MakeGenericType(innerValueType);
                ConstructorInfo? converterConstructor = converterType
                    .GetConstructor(new[] { converterConstructorParameter });

                Debug.Assert(converterConstructor != null,
                    $"The type {converterType} does not have constructor with {converterConstructorParameter} parameter");

                var factoryMethod = Expression.Lambda<Func<JsonConverter>>(
                    Expression.New(converterConstructor, GetEqualityComparer(innerValueType)));

                return factoryMethod.Compile();
            }

            static Func<JsonConverter> CreateEnumerableValueObjectFactory(Type typeToConvert, Type valueType)
            {
                Type genericValueObjectType = ValueObject.GetGenericValueObjectType(valueType);
                Type[] genericArguments = genericValueObjectType.GetGenericArguments();

                Array.Resize(ref genericArguments, 3);
                genericArguments[2] = typeToConvert;

                Type converterType = typeof(EnumerableValueObjectConverter<,,>).MakeGenericType(genericArguments);
                ConstructorInfo? converterConstructor = converterType.GetConstructor(Array.Empty<Type>());

                Debug.Assert(converterConstructor != null,
                    $"Type {converterType} does not have parameterless constructor");

                var factoryMethod = Expression.Lambda<Func<JsonConverter>>(Expression.New(converterConstructor));

                return factoryMethod.Compile();
            }

            static bool TryGetEnumerableGenericParameter(Type typeToConvert, [NotNullWhen(true)] out Type? valueObjectType)
            {
                if (!ValueObject.IsValueObjectCollectionType(typeToConvert))
                {
                    valueObjectType = null;
                    return false;
                }

                valueObjectType = typeToConvert.GetInterfaces()
                    .Where(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    .Select(type => type.GetGenericArguments()[0])
                    .FirstOrDefault(type => ValueObject.IsSingleValueObjectType(type));

                return valueObjectType != null;
            }

            static Expression GetEqualityComparer(Type type)
            {
                BindingFlags propertyBindingMask = BindingFlags.Public | BindingFlags.Static;

                // TODO: For strings, case-insensitive comparisons are always used
                PropertyInfo equalityComparerProperty = (type == typeof(string))
                    ? typeof(StringComparer).GetProperty(nameof(StringComparer.OrdinalIgnoreCase), propertyBindingMask)!
                    : typeof(EqualityComparer<>).MakeGenericType(type).GetProperty(nameof(EqualityComparer<object>.Default), propertyBindingMask)!;

                return Expression.Property(expression: null, equalityComparerProperty);
            }
        }
    }
}

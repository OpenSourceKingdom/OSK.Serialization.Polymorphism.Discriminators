using OSK.DataStructures.Common;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace OSK.Serialization.Polymorphism.Discriminators.Internal.Services
{
    internal class PolymorphismEnumDiscriminatorStrategy : PolymorphismStrategy<DiscriminatorAttribute>, IPolymorphismEnumDiscriminatorStrategy
    {
        #region Variables

        private static ConcurrentDictionary<Type, Type> s_enumPropertyTypeLookup = new ConcurrentDictionary<Type, Type>();
        private static ConcurrentDictionary<Type, TwoWayMap<int, string>> s_enumMapLookup = new ConcurrentDictionary<Type, TwoWayMap<int, string>>();
        private static ConcurrentDictionary<string, Type> s_concreteTypeLookup = new ConcurrentDictionary<string, Type>();

        #endregion

        #region PolymorphismStrategy Overrides

        protected override Type GetConcreteType(DiscriminatorAttribute attribute, Type typeToConvert, object propertyValue)
        {
            var propertyEnumType = GetEnumPropertyType(typeToConvert, attribute.PolymorphicPropertyName);
            return propertyValue switch
            {
                int enumValue => GetConcreteType(typeToConvert, propertyEnumType, attribute.ClassTemplate, enumValue),
                string enumName => GetConcreteType(typeToConvert, propertyEnumType, attribute.ClassTemplate, enumName),
                _ => throw new InvalidOperationException($"The property type, {propertyValue.GetType().FullName}, was not valid for an enum and can not be used with the {GetType().FullName}.")
            };
        }

        #endregion

        #region Helpers

        private Type GetEnumPropertyType(Type typeToConvert, string propertyName)
        {
            return s_enumPropertyTypeLookup.GetOrAdd(typeToConvert, polymorphicType =>
            {
                var property = polymorphicType.GetProperties().SingleOrDefault(p =>
                {
                    return string.Equals(p.Name, propertyName, StringComparison.InvariantCultureIgnoreCase);
                });

                if (property == null)
                {
                    throw new InvalidOperationException($"Object of type {polymorphicType.FullName} is missing the required enum property {propertyName}.");
                }

                return property.PropertyType;
            });
        }

        private Type GetConcreteType(Type typeToConvert, Type enumPropertyType, string discriminatorTemplate, int enumValue)
        {
            var enumMap = GetEnumMap(enumPropertyType);
            if (!enumMap.TryGetValue(enumValue, out var enumName))
            {
                throw new InvalidOperationException($"The enum value, {enumValue}, was not a valid value for the enum of type {typeToConvert.FullName}.");
            }

            return DetermineConcreteType(typeToConvert, discriminatorTemplate, enumName);
        }

        private Type GetConcreteType(Type typeToConvert, Type enumPropertyType, string discriminatorTemplate, string enumName)
        {
            var enumMap = GetEnumMap(enumPropertyType);
            if (!enumMap.TryGetValue(enumName.ToLowerInvariant(), out _))
            {
                throw new InvalidOperationException($"The enum name, {enumName}, was not a valid name for the enum type {typeToConvert.FullName}.");
            }

            return DetermineConcreteType(typeToConvert, discriminatorTemplate, enumName);
        }

        private static TwoWayMap<int, string> GetEnumMap(Type enumType)
        {
            return s_enumMapLookup.GetOrAdd(enumType, type =>
            {
                var enumMap = new TwoWayMap<int, string>();
                foreach (var value in Enum.GetValues(enumType))
                {
                    enumMap.Add((int)value, Enum.GetName(enumType, value)!.ToLowerInvariant());
                }

                return enumMap;
            });
        }

        private static Type DetermineConcreteType(Type baseType, string classNameTemplate, string discriminatorValue)
        {
            var concreteClassName = string.Format(classNameTemplate, discriminatorValue);

            // TODO later on, it may become necessary to look outside of the baseType.Namespace for the concrete class, add that here
            return s_concreteTypeLookup.GetOrAdd(concreteClassName, type => baseType.Assembly.GetType(baseType.Namespace! + "." + concreteClassName!, false, true)!);
        }

        #endregion
    }
}

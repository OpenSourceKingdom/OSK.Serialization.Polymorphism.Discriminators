using OSK.Serialization.Polymorphism.Discriminators.Internal.Services;
using OSK.Serialization.Polymorphism.Discriminators.UnitTests.Helpers.Alternate;
using OSK.Serialization.Polymorphism.Discriminators.UnitTests.Helpers.Error;
using OSK.Serialization.Polymorphism.Discriminators.UnitTests.Helpers.Middle;
using OSK.Serialization.Polymorphism.Discriminators.UnitTests.Helpers.None;
using OSK.Serialization.Polymorphism.Discriminators.UnitTests.Helpers.Prefix;
using OSK.Serialization.Polymorphism.Discriminators.UnitTests.Helpers.Suffix;
using OSK.Serialization.Polymorphism.Ports;
using Xunit;


namespace OSK.Serialization.Polymorphism.Discriminators.UnitTests.Internal.Services
{
    public class PolymorphismEnumDiscriminatorStrategyTests
    {
        #region Variables

        private IPolymorphismStrategy _polymorphismTypeHandler;

        #endregion

        #region Constructors

        public PolymorphismEnumDiscriminatorStrategyTests()
        {
            _polymorphismTypeHandler = new PolymorphismEnumDiscriminatorStrategy();
        }

        #endregion

        #region GetConcreteType

        [Fact]
        public void GetConcreteType_NullAttribute_ReturnsOriginalType()
        {
            // Arrange
            var expectedType = typeof(NoDiscriminatorAnimal);

            // Act
            var concreteType = _polymorphismTypeHandler.GetConcreteType(null, expectedType, new object());

            // Assert
            Assert.Equal(expectedType, concreteType);
        }

        [Fact]
        public void GetConcreteType_NullTypeToConvert_ReturnsNull()
        {
            // Arrange/Act
            var concreteType = _polymorphismTypeHandler.GetConcreteType(new DiscriminatorAttribute(), null, new object());

            // Assert
            Assert.Null(concreteType);
        }

        [Fact]
        public void GetConcreteType_NullPolymorphismValue_ReturnsNull()
        {
            // Arrange
            var discriminator = new DiscriminatorAttribute();
            var typeToConvert = typeof(AlternateNamedAnimalDiscriminator);

            // Act
            var concreteType = _polymorphismTypeHandler.GetConcreteType(discriminator, typeToConvert, null);

            // Assert
            Assert.Null(concreteType);
        }

        [Fact]
        public void GetConcreteType_MissingPolymorphismProperty_ThrowsInvalidOperationException()
        {
            // Arrange
            var discriminator = new DiscriminatorAttribute();
            var typeToConvert = typeof(ErrorAnimal);

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => _polymorphismTypeHandler.GetConcreteType(discriminator, typeToConvert, "1"));
        }

        [Fact]
        public void GetConcreteType_InvalidEnumPolymorphismValue_ThrowsInvalidOperationException()
        {
            // Arrange
            var typeToConvert = typeof(AlternateNamedAnimalDiscriminator);
            var discriminator = PolymorphismAttribute.GetPolymorphismAttribute(typeToConvert);

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => _polymorphismTypeHandler.GetConcreteType(discriminator, typeToConvert, DateTime.Now));
        }

        [Fact]
        public void GetConcreteType_InvalidEnumIntValue_ThrowsInvalidOperationException()
        {
            // Arrange
            var typeToConvert = typeof(AlternateNamedAnimalDiscriminator);
            var discriminator = PolymorphismAttribute.GetPolymorphismAttribute(typeToConvert);

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => _polymorphismTypeHandler.GetConcreteType(discriminator, typeToConvert, 100));
        }

        [Fact]
        public void GetConcreteType_InvalidEnumStringValue_ThrowsInvalidOperationException()
        {
            // Arrange
            var typeToConvert = typeof(AlternateNamedAnimalDiscriminator);
            var discriminator = PolymorphismAttribute.GetPolymorphismAttribute(typeToConvert);

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => _polymorphismTypeHandler.GetConcreteType(discriminator, typeToConvert, "abcdefghijklmnopqrstuvwxyz"));
        }

        [Theory]
        [InlineData(typeof(AlternateNamedAnimalDiscriminator), AlternateAnimalType.AlphaAnimal, typeof(AlphaAnimal))]
        [InlineData(typeof(AlternateNamedAnimalDiscriminator), AlternateAnimalType.BetaAnimal, typeof(BetaAnimal))]
        [InlineData(typeof(MiddleReplacementAnimalDiscriminator), ReplacementAnimalType.Super, typeof(SpecialSuperAnimal))]
        [InlineData(typeof(MiddleReplacementAnimalDiscriminator), ReplacementAnimalType.Weak, typeof(SpecialWeakAnimal))]
        [InlineData(typeof(PrefixAnimalDiscriminator), PrefixAnimalType.PackAnimal, typeof(PackAnimalDto))]
        [InlineData(typeof(PrefixAnimalDiscriminator), PrefixAnimalType.LoneAnimal, typeof(LoneAnimalDto))]
        [InlineData(typeof(SuffixAnimalDiscriminator), SuffixAnimalType.DayCare, typeof(AnimalDayCare))]
        [InlineData(typeof(SuffixAnimalDiscriminator), SuffixAnimalType.Hospital, typeof(AnimalHospital))]
        public void GetConcreteType_DifferingClassTemplatesAndPropertyNames_ReturnsExpectedValues(Type typeToConvert, Enum enumValue, Type expectedType)
        {
            // Arrange
            var discriminator = PolymorphismAttribute.GetPolymorphismAttribute(typeToConvert);

            // Act
            var concreteIntType = _polymorphismTypeHandler.GetConcreteType(discriminator, typeToConvert, (int)Convert.ChangeType(enumValue, enumValue.GetTypeCode()));
            var concreteStringType = _polymorphismTypeHandler.GetConcreteType(discriminator, typeToConvert, enumValue.ToString());

            // Assert
            Assert.Equal(expectedType, concreteIntType);
            Assert.Equal(expectedType, concreteStringType);
        }

        #endregion

        #region GetPolymorphicPropertyName

        [Theory]
        [InlineData(null)]
        [InlineData(typeof(NoDiscriminatorAnimal))]
        public void GetPolymorphicPropertyName_ExpectedNullResponseTypes_ReturnsNull(Type type)
        {
            // Arrange/Act
            var propertyName = _polymorphismTypeHandler.GetPolymorphicPropertyName(type);

            // Assert
            Assert.Null(propertyName);
        }

        [Fact]
        public void GetPolymorphicPropertyName_ValidPolymorphismAttribte_ReturnsSpecifiedPolymorphismPropertyName()
        {
            // Arrange
            var typeToConvert = typeof(AlternateNamedAnimalDiscriminator);

            // Act
            var propertyName = _polymorphismTypeHandler.GetPolymorphicPropertyName(typeToConvert);

            // Assert
            Assert.Equal(AlternateNamedAnimalDiscriminator.PropertyName, propertyName);
        }

        #endregion
    }
}

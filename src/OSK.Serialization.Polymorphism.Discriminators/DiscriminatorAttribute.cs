using System;

namespace OSK.Serialization.Polymorphism.Discriminators
{
    public class DiscriminatorAttribute : PolymorphismAttribute
    {
        #region Variables

        public const string DefaultDiscriminator = "type";
        public const string DefaultClassNameTemplate = "{0}";

        public string ClassTemplate { get; }

        #endregion

        #region Constructors

        public DiscriminatorAttribute(string discriminator = DefaultDiscriminator,
            string classTemplate = DefaultClassNameTemplate)
            : base(discriminator)
        {
            ClassTemplate = string.IsNullOrWhiteSpace(classTemplate)
                ? throw new ArgumentNullException(nameof(classTemplate))
                : classTemplate;
        }

        #endregion
    }
}

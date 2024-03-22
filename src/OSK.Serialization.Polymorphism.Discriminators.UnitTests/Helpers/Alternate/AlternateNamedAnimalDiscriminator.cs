namespace OSK.Serialization.Polymorphism.Discriminators.UnitTests.Helpers.Alternate
{
    [Discriminator(PropertyName)]
    public abstract class AlternateNamedAnimalDiscriminator
    {
        public const string PropertyName = "discRimiNator";

        public AlternateAnimalType Discriminator { get; set; }
    }
}

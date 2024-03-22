namespace OSK.Serialization.Polymorphism.Discriminators.UnitTests.Helpers.Prefix
{
    [Discriminator(classTemplate: "{0}Dto")]
    public abstract class PrefixAnimalDiscriminator
    {
        public PrefixAnimalType Type { get; set; }
    }
}

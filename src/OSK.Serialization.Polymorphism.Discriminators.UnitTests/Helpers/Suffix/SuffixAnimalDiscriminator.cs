namespace OSK.Serialization.Polymorphism.Discriminators.UnitTests.Helpers.Suffix
{
    [Discriminator(discriminator: nameof(AnimalType), classTemplate: "Animal{0}")]
    public abstract class SuffixAnimalDiscriminator
    {
        public SuffixAnimalType AnimalType { get; set; }
    }
}

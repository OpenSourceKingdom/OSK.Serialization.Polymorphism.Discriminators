namespace OSK.Serialization.Polymorphism.Discriminators.UnitTests.Helpers.Middle
{
    [Discriminator(classTemplate: "Special{0}Animal")]
    public abstract class MiddleReplacementAnimalDiscriminator
    {
        public ReplacementAnimalType Type { get; set; }
    }
}

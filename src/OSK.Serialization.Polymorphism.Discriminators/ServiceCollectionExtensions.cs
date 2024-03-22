using Microsoft.Extensions.DependencyInjection;
using OSK.Serialization.Polymorphism.Discriminators.Internal.Services;

namespace OSK.Serialization.Polymorphism.Discriminators
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPolymorphismEnumDiscriminatorStrategy(this IServiceCollection services)
        {
            services.AddPolymorphicSerialization();
            services.AddPolymorphismStrategy<DiscriminatorAttribute, PolymorphismEnumDiscriminatorStrategy>();

            return services;
        }
    }
}

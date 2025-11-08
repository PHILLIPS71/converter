using MassTransit;

namespace Giantnodes.Infrastructure.MassTransit;

public static class Setup
{
    /// <summary>
    /// Configures MassTransit to use the platform specific filters and settings.
    /// </summary>
    /// <param name="configurator">The <see cref="IBusFactoryConfigurator" /> for the bus being configured</param>
    /// <param name="registration">The registration for this bus instance</param>
    public static void UsePlatform(
        this IBusFactoryConfigurator configurator,
        IRegistrationContext registration)
    {
        configurator.UseConsumeFilter(typeof(FluentValidationConsumeFilter<>), registration);
    }
}

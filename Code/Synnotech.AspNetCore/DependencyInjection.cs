using System;
using Light.GuardClauses;
using LightInject;
using LightInject.Microsoft.DependencyInjection;
using Microsoft.Extensions.Hosting;
#if NET6_0
using Microsoft.AspNetCore.Builder;
#endif

namespace Synnotech.AspNetCore;

/// <summary>
/// Provides members to add LightInject to your ASP.NET Core app.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Creates a <see cref="ServiceContainer"/> with the default Microsoft settings.
    /// </summary>
    public static ServiceContainer CreateContainer() => new (ContainerOptions.Default.WithMicrosoftSettings());

    /// <summary>
    /// Configures the host builder to use LightInject as the DI container.
    /// </summary>
    /// <param name="hostBuilder">The host builder that will be manipulated.</param>
    /// <param name="container">
    /// The DI container instance that should be used (optional).
    /// If no container is specified, a new instance with Microsoft settings will be used.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="hostBuilder"/> is null.</exception>
    public static IHostBuilder UseLightInject(this IHostBuilder hostBuilder, IServiceContainer? container = null)
    {
        hostBuilder.MustNotBeNull(nameof(hostBuilder));
        var factory = container == null ? new LightInjectServiceProviderFactory() : new LightInjectServiceProviderFactory(container);
        hostBuilder.UseServiceProviderFactory(factory);
        return hostBuilder;
    }

#if NET6_0
    /// <summary>
    /// Configures the web application builder to use LightInject as the DI container.
    /// </summary>
    /// <param name="webApplicationBuilder">The web app builder that will be manipulated.</param>
    /// <param name="container">
    /// The DI container instance that should be used (optional).
    /// If no container is specified, a new instance with Microsoft settings will be used.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="webApplicationBuilder"/> is null.</exception>
    public static WebApplicationBuilder UseLightInject(this WebApplicationBuilder webApplicationBuilder,
                                                       IServiceContainer? container = null)
    {
        webApplicationBuilder.MustNotBeNull(nameof(webApplicationBuilder));
        webApplicationBuilder.Host.UseLightInject(container);
        return webApplicationBuilder;
    }
#endif
}
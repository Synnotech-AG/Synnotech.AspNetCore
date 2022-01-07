using System;
using Light.GuardClauses;
using Microsoft.AspNetCore.Builder;

namespace Synnotech.AspNetCore.VersionValidation;

/// <summary>
/// Provides extension methods to add Version Validation to your ASP.NET Core app.
/// </summary>
public static class VersionValidationDependencyInjection
{
    /// <summary>
    /// Configures the host builder to use Version Validation.
    /// </summary>
    /// <param name="app">The application builder that will be manipulated.</param>
    /// <param name="configureOptions">The action that lets you configure the options used by the middleware.</param>
    /// <returns></returns>
    public static IApplicationBuilder UseVersionValidation(this IApplicationBuilder app, Action<VersionValidationOptions>? configureOptions = null)
    {
        app.MustNotBeNull();
        var options = new VersionValidationOptions();
        configureOptions?.Invoke(options);
        app.UseMiddleware<VersionValidationMiddleware>(options);
        return app;
    }
}
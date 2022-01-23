using System;
using Light.GuardClauses;
using Microsoft.AspNetCore.Builder;

namespace Synnotech.AspNetCore.VersionValidation;

/// <summary>
/// Provides extension methods to add version validation to your ASP.NET Core app.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Configures the host builder to use Version Validation.
    /// </summary>
    /// <param name="app">The application builder that will be manipulated.</param>
    /// <param name="options">The options to customize version validation.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="app"/> is null.</exception>
    public static IApplicationBuilder UseVersionValidation<T>(this IApplicationBuilder app, VersionValidationOptions<T> options) =>
        app.MustNotBeNull().UseMiddleware<VersionValidationMiddleware<T>>(options);
}
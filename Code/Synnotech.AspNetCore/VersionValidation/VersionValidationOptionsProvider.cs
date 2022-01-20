using System;

namespace Synnotech.AspNetCore.VersionValidation;

/// <summary>
/// Provides methods to build default options for version validation.
/// </summary>
public static class VersionValidationOptionsProvider
{
    /// <summary>
    /// Creates options using <see cref="Version"/> to validate versions. Validation algorithm is based on <see cref="VersionValidationMode"/>.
    /// </summary>
    /// <param name="appVersion">The application version.</param>
    /// <param name="mode">The version validation algorithm to compare the app version with the request version.</param>
    /// <returns>Returns the options.</returns>
    public static VersionValidationOptions<Version> CreateDefaultOptions(Version appVersion, VersionValidationMode mode) =>
        new(appVersion,
            VersionValidation.ResolveVersionValidationFunc(mode),
            version => Version.TryParse(version, out var parsedVersion) ? parsedVersion : null);
}
using System;
using Light.GuardClauses;

namespace Synnotech.AspNetCore.VersionValidation;

/// <summary>
/// Provides options for the version validation middleware.
/// </summary>
public sealed class VersionValidationOptions<T>
{
    /// <summary>
    /// Creates a new instance of <see cref="VersionValidationOptions{T}" />
    /// </summary>
    /// <param name="appVersion">The application version.</param>
    /// <param name="compareVersions">The delegate that is used to compare two versions with each other.</param>
    /// <param name="tryParseRequestVersion">The delegate that parses a string to the version type</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    public VersionValidationOptions(T appVersion,
                                    Func<VersionCompareData<T>, string?> compareVersions,
                                    Func<string, T?> tryParseRequestVersion)
    {
        AppVersion = appVersion.MustNotBeNullReference();
        CompareVersions = compareVersions.MustNotBeNull();
        TryParseRequestVersion = tryParseRequestVersion.MustNotBeNull();
    }

    /// <summary>
    /// Gets the application version.
    /// </summary>
    public T AppVersion { get; }

    /// <summary>
    /// Gets the delegate to compare the HTTP header version with the app version.
    /// Returns null if no errors, otherwise returns the error message.
    /// </summary>
    public Func<VersionCompareData<T>, string?> CompareVersions { get; }

    /// <summary>
    /// Gets the delegate to parse the request version to the required version type.
    /// </summary>
    public Func<string, T?> TryParseRequestVersion { get; }

    /// <summary>
    /// Gets or sets the HTTP header name that is expected to contain the request version.
    /// </summary>
    public string VersionHeaderName { get; set; } = "AppVersion";

    /// <summary>
    /// Gets or sets the version validation mode which specifies
    /// how the app version should be compared with the HTTP header version.
    /// Ignored if <see cref="CompareVersions" /> is set.
    /// </summary>
    public VersionValidationMode ValidationMode { get; set; } = VersionValidationMode.ExactMatch;

    /// <summary>
    /// Gets or sets a boolean which indicates if the middleware should return
    /// an HTTP 400 BadRequest response when a request does not contain the request version in the header
    /// named <see cref="VersionHeaderName" />.
    /// </summary>
    public bool IsValidationOptional { get; set; } = false;
    
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
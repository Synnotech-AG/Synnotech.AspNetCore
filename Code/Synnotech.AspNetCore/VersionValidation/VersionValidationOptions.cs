using System;

namespace Synnotech.AspNetCore.VersionValidation;

/// <summary>
/// Provides options for the version validation middleware.
/// </summary>
public sealed class VersionValidationOptions<T>
{
    /// <summary>
    /// Creates a new instance of <see cref="VersionValidationOptions{T}"/>
    /// </summary>
    /// <param name="appVersion">The application version.</param>
    /// <param name="compareVersions"></param>
    /// <param name="tryParseRequestVersion"></param>
    public VersionValidationOptions(T appVersion, Func<VersionCompareData<T>, string?> compareVersions, Func<string, T?> tryParseRequestVersion)
    {
        AppVersion = appVersion;
        CompareVersions = compareVersions;
        TryParseRequestVersion = tryParseRequestVersion;
    }

    /// <summary>
    /// Gets the application version.
    /// </summary>
    public T AppVersion { get; }

    /// <summary>
    /// Gets the func to compare the http header version with the app version.
    /// Returns null if no errors, otherwise returns the error message.
    /// </summary>
    public Func<VersionCompareData<T>, string?> CompareVersions { get; }

    /// <summary>
    /// Gets the func to parse the request version to the required version type.
    /// </summary>
    public Func<string, T?> TryParseRequestVersion { get; }

    /// <summary>
    /// Gets or sets the http header name that is expected to contain the request version.
    /// </summary>
    public string VersionHeaderName { get; set; } = "AppVersion";

    /// <summary>
    /// Gets the version validation mode which specifies how the app version should be compared with the http header version. Ignored if <see cref="CompareVersions"/> is set.
    /// </summary>
    public VersionValidationMode ValidationMode { get; set; } = VersionValidationMode.ExactMatch;

    /// <summary>
    /// Gets or sets a boolean which indicates if the middleware should return a BadRequest when a request does not contain the request version in the header named <see cref="VersionHeaderName"/>.
    /// </summary>
    public bool IsValidationOptional { get; set; } = false;
}
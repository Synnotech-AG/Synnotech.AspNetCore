using System;

namespace Synnotech.AspNetCore.VersionValidation;

/// <summary>
/// Provides options for the version validation middleware.
/// </summary>
public sealed class VersionValidationOptions
{
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
    public bool AllowUnknownVersionRequests { get; set; } = true;

    /// <summary>
    /// Gets or sets the func to resolve the application version that should be compared to the http header version. If null the version is determined by the entry assembly product version.
    /// </summary>
    public Func<string>? GetAppVersion { get; set; }

    /// <summary>
    /// Gets or sets the func to compare the http header version with the app version which is used instead of validation based on <see cref="ValidationMode"/>.
    /// </summary>
    public Func<VersionCompareData, bool>? CompareVersions { get; set; }
}
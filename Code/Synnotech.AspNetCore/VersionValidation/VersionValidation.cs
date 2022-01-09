using System;

namespace Synnotech.AspNetCore.VersionValidation;

/// <summary>
/// Provides methods to validate a request version against the application version.
/// </summary>
public static class VersionValidation
{
    /// <summary>
    /// Resolves a func to validate versions based on <see cref="VersionValidationMode"/>.
    /// </summary>
    /// <param name="mode">The version validation mode.</param>
    /// <returns>Returns a func that validates version compare data with the provided validation mode.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static Func<VersionCompareData, string?> ResolveVersionValidationFunc(VersionValidationMode mode) =>
        mode switch
        {
            VersionValidationMode.ExactMatch => ValidateExactMatch,
            VersionValidationMode.AllowOlder => ValidateAllowOlder,
            VersionValidationMode.AllowOlderOrEqual => ValidateAllowOlderOrEqual,
            VersionValidationMode.AllowNewer => ValidateAllowNewer,
            VersionValidationMode.AllowNewerOrEqual => ValidateAllowNewerOrEqual,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, "Mode not supported")
        };

    /// <summary>
    /// Request version has to match the app version.
    /// </summary>
    /// <param name="data">The versions data to compare.</param>
    /// <returns>Returns null if valid, otherwise the error message</returns>
    public static string? ValidateExactMatch(VersionCompareData data)
    {
        var requestVersion = new Version(data.RequestVersion);
        var appVersion = new Version(data.AppVersion);
        return requestVersion != appVersion ? "Request version is not equal to app version" : null;
    }

    /// <summary>
    /// Request version has to be older or equal to the app version.
    /// </summary>
    /// <param name="data">The versions data to compare.</param>
    /// <returns>Returns null if valid, otherwise the error message</returns>
    public static string? ValidateAllowOlderOrEqual(VersionCompareData data)
    {
        var requestVersion = new Version(data.RequestVersion);
        var appVersion = new Version(data.AppVersion);
        return requestVersion > appVersion ? "Request version is not lower than or equal to app version" : null;
    }

    /// <summary>
    /// Request version has to be newer than or equal to the app version.
    /// </summary>
    /// <param name="data">The versions data to compare.</param>
    /// <returns>Returns null if valid, otherwise the error message</returns>
    public static string? ValidateAllowNewerOrEqual(VersionCompareData data)
    {
        var requestVersion = new Version(data.RequestVersion);
        var appVersion = new Version(data.AppVersion);
        return requestVersion < appVersion ? "Request version is not newer than or equal to app version" : null;
    }

    /// <summary>
    /// Request version has to be older the app version.
    /// </summary>
    /// <param name="data">The versions data to compare.</param>
    /// <returns>Returns null if valid, otherwise the error message</returns>
    public static string? ValidateAllowOlder(VersionCompareData data)
    {
        var requestVersion = new Version(data.RequestVersion);
        var appVersion = new Version(data.AppVersion);
        return requestVersion >= appVersion ? "Request version is not lower than app version" : null;
    }

    /// <summary>
    /// Request version has to be newer than the app version.
    /// </summary>
    /// <param name="data">The versions data to compare.</param>
    /// <returns>Returns null if valid, otherwise the error message</returns>
    public static string? ValidateAllowNewer(VersionCompareData data)
    {
        var requestVersion = new Version(data.RequestVersion);
        var appVersion = new Version(data.AppVersion);
        return requestVersion <= appVersion ? "Request version is not newer than app version" : null;
    }
}
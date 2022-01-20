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
    public static Func<VersionCompareData<Version>, string?> ResolveVersionValidationFunc(VersionValidationMode mode) =>
        mode switch
        {
            VersionValidationMode.ExactMatch => data => data.RequestVersion != data.AppVersion ? "Request version is not equal to app version" : null,
            VersionValidationMode.AllowOlder => data => data.RequestVersion >= data.AppVersion ? "Request version is not lower than app version" : null,
            VersionValidationMode.AllowOlderOrEqual => data => data.RequestVersion > data.AppVersion ? "Request version is not lower than or equal to app version" : null,
            VersionValidationMode.AllowNewer => data => data.RequestVersion <= data.AppVersion ? "Request version is not newer than app version" : null,
            VersionValidationMode.AllowNewerOrEqual => data => data.RequestVersion < data.AppVersion ? "Request version is not newer than or equal to app version" : null,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, "Mode not supported")
        };
   
}
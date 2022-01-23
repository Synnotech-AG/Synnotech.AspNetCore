namespace Synnotech.AspNetCore.VersionValidation;

/// <summary>
/// Represents a structure containing data to compare the app version with a request version.
/// </summary>
/// <param name="AppVersion">The version of the app.</param>
/// <param name="RequestVersion">The version of the request.</param>
public readonly record struct VersionCompareData<T>(T AppVersion, T RequestVersion);
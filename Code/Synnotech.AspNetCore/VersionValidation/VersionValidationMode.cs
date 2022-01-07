namespace Synnotech.AspNetCore.VersionValidation;

/// <summary>
/// The enum that describes how two versions should be compared with.
/// </summary>
public enum VersionValidationMode
{
    /// <summary>
    /// Input version must be equal to compare version.
    /// </summary>
    ExactMatch,

    /// <summary>
    /// Input version must be lower than compare version.
    /// </summary>
    AllowOlder,

    /// <summary>
    /// Input version must be lower than or equal to compare version.
    /// </summary>
    AllowOlderOrEqual,

    /// <summary>
    /// Input version must be newer than compare version.
    /// </summary>
    AllowNewer,

    /// <summary>
    /// Input version must be newer than or equal to compare version.
    /// </summary>
    AllowNewerOrEqual
}
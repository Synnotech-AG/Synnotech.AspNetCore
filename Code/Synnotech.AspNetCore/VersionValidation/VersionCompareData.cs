namespace Synnotech.AspNetCore.VersionValidation;

/// <summary>
/// Struct containing data to compare the app version with a request version.
/// </summary>
public readonly struct VersionCompareData<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VersionCompareData{T}"/> struct.
    /// </summary>
    /// <param name="appVersion">The application version.</param>
    /// <param name="requestVersion">The request version.</param>
    public VersionCompareData(T appVersion, T requestVersion)
    {
        AppVersion = appVersion;
        RequestVersion = requestVersion;
    }

    /// <summary>
    /// Gets the application version.
    /// </summary>
    public T AppVersion { get; }

    /// <summary>
    /// Gets the request version.
    /// </summary>
    public T RequestVersion { get; }
}
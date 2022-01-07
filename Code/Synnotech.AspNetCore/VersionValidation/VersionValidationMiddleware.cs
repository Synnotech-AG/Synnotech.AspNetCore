using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Synnotech.AspNetCore.VersionValidation
{
    /// <summary>
    /// Middleware that compares the app version to the request version which is read from a http header.
    /// Produces 400 if app version header is missing and options are configured to disallow such requests.
    /// Produces 400 if request version and app version validation based on validation mode fails.
    /// </summary>
    public sealed class VersionValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly VersionValidationOptions _options;

#pragma warning disable CS1591
        public VersionValidationMiddleware(RequestDelegate next, VersionValidationOptions options)
#pragma warning restore CS1591
        {
            _next = next;
            _options = options;
        }

#pragma warning disable CS1591
        public async Task InvokeAsync(HttpContext context)
#pragma warning restore CS1591
        {
            // Retrieve request version based on header
            if (!context.Request.Headers.TryGetValue(_options.VersionHeaderName, out var requestVersionHeader))
            {
                if (!_options.AllowUnknownVersionRequests)
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("AppVersion Header is missing");
                    return;
                }
            }

            if (requestVersionHeader.Count > 0)
            {
                var requestVersion = requestVersionHeader![0];
                var appVersion = _options.GetAppVersion != null ? _options.GetAppVersion() : TryGetProductVersion();

                if (appVersion != null)
                {
                    var validationError = ValidateVersions(appVersion, requestVersion);
                    if (validationError != null)
                    {
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync("Version validation problem: " + validationError);
                        return;
                    }
                }
            }

            await _next.Invoke(context);
        }

        private string? ValidateVersions(string appVersionString, string requestVersionString)
        {
            // Execute custom validation logic if set
            if (_options.CompareVersions != null)
            {
                var data = new VersionCompareData(appVersionString, requestVersionString);
                var success = _options.CompareVersions(data);
                return success ? null : "Custom validation of request version and app version failed";
            }


            // Validate versions based on validation mode
            var requestVersion = new Version(requestVersionString);
            var appVersion = new Version(appVersionString);

            switch (_options.ValidationMode)
            {
                case VersionValidationMode.ExactMatch:
                    if (requestVersion != appVersion)
                    {
                        return "Request version is not equal to app version";
                    }
                    break;
                case VersionValidationMode.AllowOlderOrEqual:
                    if (requestVersion >= appVersion)
                    {
                        return "Request version is not lower than or equal to app version";
                    }
                    break;
                case VersionValidationMode.AllowNewerOrEqual:
                    if (requestVersion <= appVersion)
                    {
                        return "Request version is not newer than or equal to app version";
                    }
                    break;
                case VersionValidationMode.AllowOlder:
                    if (requestVersion > appVersion)
                    {
                        return "Request version is not lower than app version";
                    }
                    break;
                case VersionValidationMode.AllowNewer:
                    if (requestVersion < appVersion)
                    {
                        return "Request version is not newer than app version";
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }

            return null;
        }

        private static string? TryGetProductVersion()
        {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly == null)
                return null;

            var versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            return versionInfo.ProductVersion;
        }
    }
}

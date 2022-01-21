using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Synnotech.AspNetCore.VersionValidation
{
    /// <summary>
    /// Middleware that compares the app version to the request version which is read from an http header.
    /// Produces 400 if app version header is missing and options are configured to disallow such requests.
    /// Produces 400 if request version validation against app version fails.
    /// </summary>
    public sealed class VersionValidationMiddleware<T>
    {
        private readonly RequestDelegate _next;
        private readonly VersionValidationOptions<T> _options;

#pragma warning disable CS1591
        public VersionValidationMiddleware(RequestDelegate next, VersionValidationOptions<T> options)
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
                if (!_options.IsValidationOptional)
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("AppVersion Header is missing");
                    return;
                }
            }

            if (requestVersionHeader.Count > 0)
            {
                var requestVersion = _options.TryParseRequestVersion(requestVersionHeader![0]);
                if (requestVersion == null)
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Version validation problem: Could parse request version");
                    return;
                }

                var compareData = new VersionCompareData<T>(_options.AppVersion, requestVersion);
                var error = _options.CompareVersions(compareData);
                if (error != null)
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Version validation problem: " + error);
                    return;
                }
            }

            await _next.Invoke(context);
        }
    }
}

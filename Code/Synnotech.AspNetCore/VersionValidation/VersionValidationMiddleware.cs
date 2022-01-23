using System;
using System.Threading.Tasks;
using Light.GuardClauses;
using Microsoft.AspNetCore.Http;

namespace Synnotech.AspNetCore.VersionValidation
{
    /// <summary>
    /// Represents an ASP.NET Core middleware that compares the app version to the request version
    /// which is read from an http header.
    /// Produces 400 if app version header is missing and options are configured to disallow such requests.
    /// Produces 400 if request version validation against app version fails.
    /// </summary>
    public sealed class VersionValidationMiddleware<T>
    {
        private readonly RequestDelegate _next;
        private readonly VersionValidationOptions<T> _options;

        /// <summary>
        /// Initializes a new instance of <see cref="VersionValidationMiddleware{T}" />.
        /// </summary>
        /// <param name="next">The delegate that represents the next middleware in the pipeline.</param>
        /// <param name="options">The options for this middleware.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="next" /> or <paramref name="options" /> are null.</exception>
        public VersionValidationMiddleware(RequestDelegate next, VersionValidationOptions<T> options)
        {
            _next = next.MustNotBeNull();
            _options = options.MustNotBeNull();
        }

        /// <summary>
        /// Executes this middleware. Normally, this method is called by ASP.NET Core.
        /// </summary>
        public async Task InvokeAsync(HttpContext context)
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
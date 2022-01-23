using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Synnotech.AspNetCore.VersionValidation;
using Xunit;

namespace Synnotech.AspNetCore.Tests.VersionValidation
{
    public static class VersionValidationTests
    {
        private const string DefaultVersionHeaderName = "AppVersion";

        [Theory]
        [InlineData(VersionValidationMode.ExactMatch, "1.0.0", "0.9.0", false, 400)]
        [InlineData(VersionValidationMode.ExactMatch, "1.0.0", "1.0.0", true, 200)]
        [InlineData(VersionValidationMode.ExactMatch, "1.0.0", "2.0.0", false, 400)]
        [InlineData(VersionValidationMode.AllowOlder, "1.0.0", "0.9.0", true, 200)]
        [InlineData(VersionValidationMode.AllowOlder, "1.0.0", "1.0.0", false, 400)]
        [InlineData(VersionValidationMode.AllowOlder, "1.0.0", "2.0.0", false, 400)]
        [InlineData(VersionValidationMode.AllowOlderOrEqual, "1.0.0", "0.9.0", true, 200)]
        [InlineData(VersionValidationMode.AllowOlderOrEqual, "1.0.0", "1.0.0", true, 200)]
        [InlineData(VersionValidationMode.AllowOlderOrEqual, "1.0.0", "2.0.0", false, 400)]
        [InlineData(VersionValidationMode.AllowNewer, "1.0.0", "0.9.0", false, 400)]
        [InlineData(VersionValidationMode.AllowNewer, "1.0.0", "1.0.0", false, 400)]
        [InlineData(VersionValidationMode.AllowNewer, "1.0.0", "2.0.0", true, 200)]
        [InlineData(VersionValidationMode.AllowNewerOrEqual, "1.0.0", "0.9.0", false, 400)]
        [InlineData(VersionValidationMode.AllowNewerOrEqual, "1.0.0", "1.0.0", true, 200)]
        [InlineData(VersionValidationMode.AllowNewerOrEqual, "1.0.0", "2.0.0", true, 200)]
        public static async Task VersionValidation_ModeAlgorithmShouldWork(VersionValidationMode mode, string appVersion, string requestVersion, bool shouldRequestBeProcessed, int expectedStatusCode)
        {
            var options = VersionValidationOptions<Version>.CreateDefaultOptions(Version.Parse(appVersion), mode);
            options.IsValidationOptional = false;

            var (middleware, receiver) = CreateVersionValidationMiddleware(options);
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[DefaultVersionHeaderName] = requestVersion;

            await middleware.InvokeAsync(httpContext);

            receiver.IsReceived.Should().Be(shouldRequestBeProcessed);
            httpContext.Response.StatusCode.Should().Be(expectedStatusCode);
        }

        [Theory]
        [InlineData(true, false, true, 200)]
        [InlineData(true, true, true, 200)]
        [InlineData(false, false, false, 400)]
        [InlineData(false, true, true, 200)]
        public static async Task VersionValidation_CanBeOptional(bool isValidationOptional, bool isRequestVersionSet, bool shouldRequestBeProcessed, int expectedStatusCode)
        {
            var options = VersionValidationOptions<Version>.CreateDefaultOptions(Version.Parse("1.0.0"), VersionValidationMode.ExactMatch);
            options.IsValidationOptional = isValidationOptional;

            var (middleware, receiver) = CreateVersionValidationMiddleware(options);
            var httpContext = new DefaultHttpContext();
            if (isRequestVersionSet)
            {
                httpContext.Request.Headers[DefaultVersionHeaderName] = "1.0.0";
            }

            await middleware.InvokeAsync(httpContext);

            receiver.IsReceived.Should().Be(shouldRequestBeProcessed);
            httpContext.Response.StatusCode.Should().Be(expectedStatusCode);
        }

        [Theory]
        [InlineData("Foo", "Foo", true, 200)]
        [InlineData("Foo", "Bar", false, 400)]
        public static async Task VersionValidation_CanHeaderNameBeDifferent(string middlewareHeaderName, string requestHeaderName, bool shouldRequestBeProcessed, int expectedStatusCode)
        {
            var options = VersionValidationOptions<Version>.CreateDefaultOptions(Version.Parse("1.0.0"), VersionValidationMode.ExactMatch);
            options.IsValidationOptional = false;
            options.VersionHeaderName = middlewareHeaderName;

            var (middleware, receiver) = CreateVersionValidationMiddleware(options);
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[requestHeaderName] = "1.0.0";

            await middleware.InvokeAsync(httpContext);

            receiver.IsReceived.Should().Be(shouldRequestBeProcessed);
            httpContext.Response.StatusCode.Should().Be(expectedStatusCode);
        }

        [Fact]
        public static async Task VersionValidation_CanHaveCustomVersionValidation()
        {
            var options = new VersionValidationOptions<string>("1.0.0", _ => null, v => v)
            {
                IsValidationOptional = false
            };

            var (middleware, receiver) = CreateVersionValidationMiddleware(options);
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[DefaultVersionHeaderName] = "2.0.0";

            await middleware.InvokeAsync(httpContext);

            receiver.IsReceived.Should().Be(true);
            httpContext.Response.StatusCode.Should().Be(200);
        }

        private static (VersionValidationMiddleware<T>, RequestDelegateReceiver) CreateVersionValidationMiddleware<T>(VersionValidationOptions<T> options)
        {
            var receiver = new RequestDelegateReceiver();
            return (new VersionValidationMiddleware<T>(receiver.ReceiveAsync, options), receiver);
        }
    }
}
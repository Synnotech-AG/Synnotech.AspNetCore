using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Synnotech.AspNetCore.Tests.VersionValidation;

public sealed class RequestDelegateReceiver
{
    public HttpContext? ReceivedHttpContext { get; private set; }

    public bool IsReceived => ReceivedHttpContext != null;

    public Task ReceiveAsync(HttpContext httpContext)
    {
        ReceivedHttpContext = httpContext;
        return Task.CompletedTask;
    }
}
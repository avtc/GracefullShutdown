using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GracefullShutdown
{
    public class GracefullShutdownMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly GracefullShutdownOptions _options;
        private readonly ILogger<GracefullShutdownMiddleware> _logger;
        private static int _requestsInProgress;
        private static bool _stopRequest = false;

        public GracefullShutdownMiddleware(RequestDelegate next, GracefullShutdownOptions options, IApplicationLifetime applicationLifetime, ILogger<GracefullShutdownMiddleware> logger)
        {
            _next = next;
            _options = options;
            _logger = logger;
            applicationLifetime.ApplicationStopping.Register(OnApplicationStopping);
            applicationLifetime.ApplicationStopped.Register(OnApplicationStopped);
        }

        public async Task Invoke(HttpContext context)
        {
            if (_stopRequest)
            {
                var response = context.Response;
                if (_options.Redirect)
                { 
                    response.StatusCode = 308;
                    response.Headers.Add("Location", $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}");
                }
                else
                    response.StatusCode = 500;
                return;
            }

            Interlocked.Increment(ref _requestsInProgress);
            try
            {
                await _next.Invoke(context);
            }
            finally
            {
                Interlocked.Decrement(ref _requestsInProgress);
            }
        }

        private void OnApplicationStopping()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:MM:ss")} Application stopping, requests in progress: {_requestsInProgress}");
            _stopRequest = true;
            DateTime shutdownStarted = DateTime.UtcNow;
            while (_requestsInProgress > 0 && DateTime.UtcNow < shutdownStarted.Add(_options.ShutdownTimeout))
            {
                Thread.Sleep(1000);
                _logger.LogInformation($"{DateTime.Now.ToString("HH:MM:ss")} Application stopping, requests in progress: {_requestsInProgress}");
            }
        }

        private void OnApplicationStopped()
        {
            if (_requestsInProgress > 0)
                _logger.LogCritical($"{DateTime.Now.ToString("HH:MM:ss")} Application stopped, requests in progress: {_requestsInProgress}");
        }

    }
}

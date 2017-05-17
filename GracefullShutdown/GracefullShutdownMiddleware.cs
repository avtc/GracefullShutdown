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
        private readonly GracefullShutdownState _state;

        public GracefullShutdownMiddleware(
            RequestDelegate next, 
            GracefullShutdownOptions options, 
            IApplicationLifetime applicationLifetime, 
            ILogger<GracefullShutdownMiddleware> logger,
            GracefullShutdownState state
        )
        {
            if (applicationLifetime == null) throw new ArgumentNullException(nameof(applicationLifetime));
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _state = state ?? throw new ArgumentNullException(nameof(state));
            applicationLifetime.ApplicationStopping.Register(OnApplicationStopping);
            applicationLifetime.ApplicationStopped.Register(OnApplicationStopped);
        }

        public async Task Invoke(HttpContext context)
        {
            if (_state.StopRequested)
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

            _state.NotifyRequestStarted();
            try
            {
                await _next.Invoke(context);
            }
            finally
            {
                _state.NotifyRequestFinished();
            }
        }

        private void OnApplicationStopping()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:MM:ss")} Application stopping, requests in progress: {_state.RequestsInProgress}");
            _state.NotifyStopRequested();
            DateTime shutdownStarted = DateTime.UtcNow;
            while (_state.RequestsInProgress > 0 && DateTime.UtcNow < shutdownStarted.Add(_options.ShutdownTimeout))
            {
                Thread.Sleep(1000);
                _logger.LogInformation($"{DateTime.Now.ToString("HH:MM:ss")} Application stopping, requests in progress: {_state.RequestsInProgress}");
            }
        }

        private void OnApplicationStopped()
        {
            if (_state.RequestsInProgress > 0)
                _logger.LogCritical($"{DateTime.Now.ToString("HH:MM:ss")} Application stopped, requests in progress: {_state.RequestsInProgress}");
        }

        
    }
}

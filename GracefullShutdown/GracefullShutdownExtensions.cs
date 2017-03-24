using Microsoft.AspNetCore.Builder;
using System;

namespace GracefullShutdown
{
    public static class GracefullShutdownExtensions
    {
        public static IApplicationBuilder UseGracefullShutdown(this IApplicationBuilder builder, Action<GracefullShutdownOptions> configuration = null)
        {
            var options = new GracefullShutdownOptions();
            configuration?.Invoke(options);
            return builder.UseMiddleware<GracefullShutdownMiddleware>(options);
        }
    }
}

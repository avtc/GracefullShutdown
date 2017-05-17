using Microsoft.AspNetCore.Builder;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace GracefullShutdown
{
    public static class GracefullShutdownExtensions
    {
        public static IApplicationBuilder UseGracefullShutdown(this IApplicationBuilder builder, 
            Action<GracefullShutdownOptions> configuration = null)
        {
            var options = builder.ApplicationServices.GetService<GracefullShutdownOptions>();
            configuration?.Invoke(options);
            return builder.UseMiddleware<GracefullShutdownMiddleware>(options);
        }

        public static IServiceCollection AddGracefullShutdown(this IServiceCollection services)
        {
            var state = new GracefullShutdownState();
            services.AddSingleton(state);
            services.AddSingleton<IRequestsCountProvider>(state);

            var options = new GracefullShutdownOptions();
            services.AddSingleton(options);

            return services;
        }
    }
}

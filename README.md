# GracefullShutdown
Asp.net core middleware, allows to wait for ongoing requests to finish.

Sample usage:
```csharp
public class Startup
{
  //...
  public void Configure(IApplicationBuilder app)
  {
    app.UseGracefullShutdown(options => {
      options.Redirect = true; 
      // Redirect: false - incoming requests will get 500 code after shutdown initiation
      //         : true  - incoming requests will be redirected with 308 code, and same url
      //                   for load balancer case
      options.ShutdownTimeout = TimeSpan.FromSeconds(20); 
      // default timeout is 10 seconds
    });

    app.UseMvc();
  }
  //...
}
```

Shutdown can be initiated using:
- IApplicationLifetime.StopApplication()
- Ctrl+C for self-hosted Kestrel server
- SIGINT or SIGTERM signals for self-hosted Kestrel server in linux / docker

After shutdown is initiated, ongoing requests will be processed until timeout reached, new incoming requests will get 500 or 308 code.
using System;

namespace GracefullShutdown
{
    public class GracefullShutdownOptions
    {
        /// <summary>
        /// false - incoming requests will get 500 code after shutdown initiation
        /// true  - incoming requests will be redirected with 308 code, and same url
        /// </summary>
        public bool Redirect { get; set; } = false;
        public TimeSpan ShutdownTimeout { get; set; } = TimeSpan.FromSeconds(10);
    }
}

using System;

namespace GracefullShutdown
{
    public class GracefullShutdownOptions
    {
        public bool Redirect { get; set; } = false;
        public TimeSpan ShutdownTimeout { get; set; } = TimeSpan.FromSeconds(10);
    }
}

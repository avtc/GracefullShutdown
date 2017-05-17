using System;
using System.Threading;

namespace GracefullShutdown
{
    public class GracefullShutdownOptions
    {
        private bool _redirect;
        /// <summary>
        /// false - incoming requests will get 500 code after shutdown initiation
        /// true  - incoming requests will be redirected with 308 code, and same url
        /// </summary>
        public bool Redirect {
            get { return Volatile.Read(ref _redirect); }
            set { Volatile.Write(ref _redirect, value); }
        }

        private long _shutdownTimeout = TimeSpan.FromSeconds(10).Ticks;
        public TimeSpan ShutdownTimeout {
            get { return TimeSpan.FromTicks(Volatile.Read(ref _shutdownTimeout)); }
            set { Volatile.Write(ref _shutdownTimeout, value.Ticks); }
        }
    }
}

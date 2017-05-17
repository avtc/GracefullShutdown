using System.Threading;

namespace GracefullShutdown
{
    public class GracefullShutdownState: IRequestsCountProvider
    {
        private long _requestsInProgress;
        private long _requestsProcessed;
        private bool _stopRequested;

        public long RequestsInProgress => Volatile.Read(ref _requestsInProgress);
        public long RequestsProcessed => Volatile.Read(ref _requestsProcessed);
        public bool StopRequested => Volatile.Read(ref _stopRequested);
        public void NotifyRequestStarted()
        {
            Interlocked.Increment(ref _requestsInProgress);
        }
        public void NotifyRequestFinished()
        {
            Interlocked.Decrement(ref _requestsInProgress);
            Interlocked.Increment(ref _requestsProcessed);
        }
        public void NotifyStopRequested()
        {
            Volatile.Write(ref _stopRequested, true);
        }
    }
}

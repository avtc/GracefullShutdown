namespace GracefullShutdown
{
    public interface IRequestsCountProvider
    {
        long RequestsInProgress { get; }
        long RequestsProcessed { get; }
    }
}

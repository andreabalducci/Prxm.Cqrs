namespace Proximo.Cqrs.Core.Support
{
    public interface IDebugLogger
    {
        void Log(string message);

        void Error(string message);
    }
}

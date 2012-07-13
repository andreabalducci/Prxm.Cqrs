namespace Proximo.Cqrs.Core.Commanding
{
    public interface ICommandQueue
    {
        void Enqueue<T>(T command) where T : class, ICommand;
    }
}
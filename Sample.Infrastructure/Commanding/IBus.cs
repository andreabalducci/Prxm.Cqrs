namespace Sample.Infrastructure.Commanding
{
    public interface IBus
    {
        void Send<T>(T command) where T : class, ICommand;
    }
}
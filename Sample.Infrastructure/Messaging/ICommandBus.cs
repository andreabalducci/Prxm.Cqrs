using Proximo.Cqrs.Core.Commanding;

namespace Sample.Infrastructure.Messaging
{
	/// <summary>
	/// todo: will be used later to plugin different types of busses, actual implementation uses Rhino.ServiceBus
	/// </summary>
    public interface ICommandBus
    {
        void Send<T>(T command) where T : class, ICommand;
    }
}
using Proximo.Cqrs.Core.Commanding;

namespace Proximo.Cqrs.Client.Commanding
{
	/// <summary>
	/// todo: will be used later to plugin different types of busses, actual implementation uses Rhino.ServiceBus
	/// </summary>
    public interface ICommandSender
    {
        void Send<T>(T command) where T : class, ICommand;
    }
}
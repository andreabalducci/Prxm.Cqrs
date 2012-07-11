using Proximo.Cqrs.Core.Commanding;

namespace Proximo.Cqrs.Server.Commanding
{
    public interface ICommandHandler<T> where T: class, ICommand
    {
        void Handle(T command);
    }
}

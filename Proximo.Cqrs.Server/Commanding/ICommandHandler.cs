using Proximo.Cqrs.Core.Commanding;

namespace Proximo.Cqrs.Server.Commanding
{
    /// <summary>
    /// Marker interface for a generic command handler.
    /// </summary>
    public interface ICommandHandler { 
    
    }

    /// <summary>
    /// Typed command handler interface
    /// </summary>
    /// <typeparam name="T">Command type</typeparam>
    public interface ICommandHandler<T> : ICommandHandler where T : class, ICommand
    {
        void Handle(T command);
    }
}

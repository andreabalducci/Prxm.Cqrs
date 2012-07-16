using System;

namespace Proximo.Cqrs.Core.Commanding
{
    public interface ICommand
    {
        Guid Id { get; }
    }

    /// <summary>
    /// Convenience class to avoid implementing some basic command properties in 
    /// all concrete classes
    /// </summary>
    public abstract class CommandBase : ICommand 
    {
        public Guid Id { get; protected set; }
    }
}

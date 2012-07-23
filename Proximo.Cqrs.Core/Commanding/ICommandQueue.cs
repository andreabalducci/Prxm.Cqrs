using System;
namespace Proximo.Cqrs.Core.Commanding
{
    public interface ICommandQueue
    {
        void Enqueue<T>(T command) where T : class, ICommand;

        void Enqueue<T>(T command, TimeSpan delay) where T : class, ICommand;

        void Enqueue<T>(T command, DateTime datetime) where T : class, ICommand;
    }
}
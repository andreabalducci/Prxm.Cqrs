using System;

namespace Proximo.Cqrs.Core.Commanding
{
    public interface ICommand
    {
        Guid Id { get; }
    }
}

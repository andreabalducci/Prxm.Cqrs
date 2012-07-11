using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Proximo.Cqrs.Server.Commanding
{
    public interface ICommandHandlerFactory
    {
        object CreateHandler(Type commandType);
        void ReleaseHandler(object handler);
    }
}

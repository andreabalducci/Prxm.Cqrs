using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Core.Commanding;

namespace Proximo.Cqrs.Server.Commanding
{
    public interface ICommandRouter
    {
        void RouteToHandler(ICommand command);
    }
}

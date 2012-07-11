using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sample.Infrastructure.Commanding
{
    public interface ICommandHandler<T> where T: class, ICommand
    {
        void Handle(T command);
    }
}

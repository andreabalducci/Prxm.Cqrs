using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sample.Infrastructure.Commanding
{
    public interface ICommand
    {
        Guid Id { get; }
    }
}

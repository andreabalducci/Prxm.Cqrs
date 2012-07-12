using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonDomain.Core;

namespace Sample.Domain
{
    public static class AggregateExtensions
    {
        public static bool HasValidId(this AggregateBase aggregate)
        {
            return aggregate.Id != Guid.Empty;
        }
    }
}

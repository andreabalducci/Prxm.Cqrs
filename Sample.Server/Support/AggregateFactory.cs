using System;
using CommonDomain;
using CommonDomain.Persistence;

namespace Sample.Server.Support
{
    public class AggregateFactory : IConstructAggregates
    {
        public IAggregate Build(Type type, Guid id, IMemento snapshot)
        {
			var a = Activator.CreateInstance(type) as IAggregate;
            return a;
        }
    }
}
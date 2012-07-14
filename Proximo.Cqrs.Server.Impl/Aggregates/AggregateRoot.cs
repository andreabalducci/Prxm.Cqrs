using CommonDomain;
using CommonDomain.Core;
using Proximo.Cqrs.Server.Aggregates;
using Proximo.Cqrs.Server.Eventing;

namespace Proximo.Cqrs.Server.Impl.Aggregates
{
    public abstract class AggregateRoot : AggregateBase
    {
        protected AggregateRoot()
        {
            this.RegisteredRoutes = new AutoVersioningConventionEventRouter(this);
        }
    }

    public class AutoVersioningConventionEventRouter : ConventionEventRouter
    {
        private IAggregate _source;
        public AutoVersioningConventionEventRouter(IAggregate aggregateRoot):base(true, aggregateRoot)
        {
            _source = aggregateRoot;
        }

        public override void Dispatch(object eventMessage)
        {
            base.Dispatch(eventMessage);
            
            var evt = eventMessage as IPleaseVersionThisDomainEventAfterInternalStateChange;
            if (evt != null)
            {
                evt.TakeThis(new AggregateVersion(_source.Id, _source.Version));
            }
        }
    }
}

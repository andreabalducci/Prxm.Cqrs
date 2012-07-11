using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventStore;
using EventStore.Dispatcher;

namespace Proximo.Cqrs.Server.Eventing
{
    public class CommitToEventsDispatcher : IDispatchCommits
    {
        private readonly IDomainEventRouter _domainEventRouter;

        public CommitToEventsDispatcher(IDomainEventRouter domainEventRouter)
        {
            _domainEventRouter = domainEventRouter;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public void Dispatch(Commit commit)
        {
            foreach (var eventMessage in commit.Events)
            {
                _domainEventRouter.Dispatch(eventMessage.Body);
            }
        }
    }
}

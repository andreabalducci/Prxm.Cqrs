using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventStore;
using EventStore.Dispatcher;

namespace Sample.Server.Support
{
    public class CommitDispatcher : IDispatchCommits
    {
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // no op
        }

        public void Dispatch(Commit commit)
        {
            Console.WriteLine();
            Console.WriteLine("[commit {0}]", commit.CommitId);
            foreach (var eventMessage in commit.Events)
            {
                Console.WriteLine("{0} - {1}",  eventMessage.GetType(), eventMessage.Body);
            }
            Console.WriteLine("[/commit {0}]", commit.CommitId);
            Console.WriteLine();
        }
    }
}

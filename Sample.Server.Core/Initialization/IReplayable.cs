using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sample.Server.Core.Initialization
{
    /// <summary>
    /// An handler that implements this interface supports replay of the events.
    /// </summary>
    public interface IReplayable
    {
        /// <summary>
        /// This method is called at bootstrap time to understand if the handlers
        /// needs to replay domain events.
        /// </summary>
        /// <returns></returns>
        Boolean ShouldReplay();

        /// <summary>
        /// This is called before the replay takes place
        /// </summary>
        void StartReplay();

        /// <summary>
        /// this is called after replay is finished.
        /// </summary>
        void EndReplay();
    }
}

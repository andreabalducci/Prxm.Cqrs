using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sample.QueryModel.System
{
    public class CommandStatus
    {
        public Guid CommandId { get; protected set; }
        public DateTime CommittedAt { get; protected set; }
    }
}

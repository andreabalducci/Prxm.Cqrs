﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sample.Server.Support
{
    /// <summary>
    /// this is the class we want to store with <see cref="IRawEventStore"/> to 
    /// capture all events that were committed by EventStore to use for replay.
    /// </summary>
    public class PersistedDomainEvent
    {
        public PersistedDomainEvent() 
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }

        public DateTime Timestamp { get; set; }

        public Int32 CommitSequence { get; set; }

        public String EventName { get; set; }

        public Object DomainEvent { get; set; }
    }
}

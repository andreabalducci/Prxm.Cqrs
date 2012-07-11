﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Core.Commanding;

namespace Sample.Infrastructure.Messaging
{
    public class CommandEnvelope : IMessage
    {
        public ICommand Command { get; set; }
    }
}

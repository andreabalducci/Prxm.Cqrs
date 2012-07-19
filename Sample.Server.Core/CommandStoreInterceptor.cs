using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;
using Proximo.Cqrs.Core.Commanding;

namespace Sample.Server.Core
{
    public class CommandStoreInterceptor : IInterceptor
    {
        private IRawCommandStore _commandStore;

        public CommandStoreInterceptor(IRawCommandStore commandStore)
        {
            _commandStore = commandStore;
        }

        public void Intercept(IInvocation invocation)
        {

            ICommand cmd = invocation.Arguments[0] as ICommand;
            ExecutedCommand executedCommand;
            if (cmd == null)
            {
               
                invocation.Proceed();
            }
            else
            {
                //this is a method that accepts a command invoker, should be intercepted
                 executedCommand = new ExecutedCommand()
                {
                    Command = cmd,
                    Id = cmd.Id,
                };
                try
                {
                    invocation.Proceed();
                    _commandStore.Store(executedCommand);
                }
                catch (Exception ex)
                {
                    executedCommand.IsSuccess = false;
                    executedCommand.Error = ex.ToString();
                    _commandStore.Store(executedCommand);
                    throw;
                }

            }


        }
    }
}

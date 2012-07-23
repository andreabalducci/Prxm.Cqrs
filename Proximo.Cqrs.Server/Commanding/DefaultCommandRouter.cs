using System.Reflection;
using Proximo.Cqrs.Core.Commanding;
using Proximo.Cqrs.Core.Support;

namespace Proximo.Cqrs.Server.Commanding
{
    /// <summary>
    /// this is the default router for commands, it simply execute in process the
    /// command given an handler.
    /// </summary>
    public class DefaultCommandRouter : ICommandRouter
    {
        private ICommandHandlerCatalog _commandHandlerCatalog;
        
        private ILogger _logger;

        public DefaultCommandRouter(ICommandHandlerCatalog commandHandlerCatalog, ILogger logger)
        {
            _commandHandlerCatalog = commandHandlerCatalog;
            _logger = logger;
        }

        public void RouteToHandler(ICommand command)
        {
            //optype set in logger context information about the logical operation that the system is executing
            //is used to group log messages togheter and to correlate child log to a logical operation.
            _logger.SetOpType("command", command.GetType().FullName + " Id:" + command.Id);
            
            _logger.Info("[queue] processing command " + command.ToString());

            var commandType = command.GetType();
            
            //get the executor function from the catalog, and then simply execute the command.
            var commandinvoker = _commandHandlerCatalog.GetExecutorFor(commandType);
            try
            {
                commandinvoker.Invoke(command);
                _logger.Info("[queue] command handled " + command.ToString());
            }
            catch (System.Exception ex)
            {
                //TODO log or do something better instead of retrhowing exception
                _logger.Error("[queue] Command error " + ex.Message, ex);
                throw;
            }
            finally 
            {
                _logger.RemoveOpType();
            }
            
        }
    }
}
using System;
using Castle.MicroKernel;
using Proximo.Cqrs.Server.Commanding;

namespace Sample.Server.Support
{
    public class CastleCommandHandlerFactory : ICommandHandlerFactory
    {
        private IKernel _kernel;

        public CastleCommandHandlerFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public object CreateHandler(Type commandType)
        {
            return _kernel.Resolve(commandType);
        }

        public void ReleaseHandler(object handler)
        {
            _kernel.ReleaseComponent(handler);
        }
    }
}
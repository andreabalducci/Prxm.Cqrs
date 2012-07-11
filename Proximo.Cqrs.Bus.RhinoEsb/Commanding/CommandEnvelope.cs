using Proximo.Cqrs.Core.Commanding;

namespace Proximo.Cqrs.Bus.RhinoEsb.Commanding
{
    public class CommandEnvelope 
    {
        public ICommand Command { get; set; }
    }
}

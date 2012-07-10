using Castle.Windsor;
using Rhino.ServiceBus.Castle;

namespace Sample.Server
{
    public class BootStrapper : CastleBootStrapper
    {
        public static IWindsorContainer GlobalContainer { get; set; }

        public BootStrapper()
            : base(BootStrapper.GlobalContainer)
        {
        }
    }
}
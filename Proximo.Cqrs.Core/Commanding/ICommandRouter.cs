namespace Proximo.Cqrs.Core.Commanding
{
    public interface ICommandRouter
    {
        void RouteToHandler(ICommand command);
    }
}

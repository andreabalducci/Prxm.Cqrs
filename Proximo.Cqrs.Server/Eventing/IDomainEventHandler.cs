namespace Proximo.Cqrs.Server.Eventing
{
	/// <summary>
	/// Domain events can be dispatched or sent again through the wire
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IDomainEventHandler<T> where T : class
	{
		void Handle(T @event);
	}
}

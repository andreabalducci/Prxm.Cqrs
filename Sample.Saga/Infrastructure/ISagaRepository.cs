using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sample.Saga.Infrastructure
{
	/// <summary>
	/// a generic interface to load and write sagas
	/// </summary>
	/// <typeparam name="T">Type of the SagaState class</typeparam>
	/// <typeparam name="TId">The type of the id.</typeparam>
	public interface ISagaRepository<T, TId> where T : SagaState<TId>, new()
	{
		/// <summary>
		/// loads the curret execution state of a saga
		/// </summary>
		/// <param name="params">Key,Value pairs that are the parameters used to uniquely identify the saga</param>
		/// <returns></returns>
		T Load(IDictionary<string, object> @params);

		/// <summary>
		/// save the saga state
		/// </summary>
		/// <param name="state"></param>
		void Save(T state);

		/// <summary>
		/// The saga ended and can be discarded form the database
		/// </summary>
		/// <param name="state"></param>
		void Remove(T state);
	}
}

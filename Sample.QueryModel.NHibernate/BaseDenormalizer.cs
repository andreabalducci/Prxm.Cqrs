using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Server.Eventing;
using NHibernate;

namespace Sample.QueryModel.NHibernate
{
    /// <summary>
    /// Base class for denormalizing into QueryModel.
    /// </summary>
    public class BaseDenormalizer : IDomainEventHandler
    {
        protected void ExecuteInSession(Action<ISession> act)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (session.BeginTransaction()) { 
                
                    //TODO: logging, exceptionhandling.
                    act(session);
                    session.Transaction.Commit();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        protected T GetById<T>(Object id) 
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.Get<T>(id);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        protected void SaveOrUpdate(Object obj)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (session.BeginTransaction())
                {
                    session.SaveOrUpdate(obj);
                    session.Transaction.Commit();
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using System.Reflection;

namespace Sample.QueryModel.NHibernate
{

    public class NHibernateHelper
    {
        private static ISessionFactory _sessionFactory;
        private static Configuration cfg;

        static NHibernateHelper()
        {
            try
            {
                cfg = new Configuration();
                cfg.Configure("NHibernateQueryModelConfiguration.xml");

                var mapper = new ConventionModelMapper();
                mapper.IsEntity((t, declared) => t.Namespace.StartsWith("Sample.QueryModel"));

                mapper.AfterMapClass += (inspector, type, classCustomizer) =>
                {
                    classCustomizer.Lazy(false);
                };
                var mapping = mapper.CompileMappingFor(
                    Assembly.Load("Sample.QueryModel").GetExportedTypes());

                cfg.AddDeserializedMapping(mapping, "AutoModel");
                _sessionFactory = cfg.BuildSessionFactory();
                //To be sure that the database is always aligned to the very same version
                UpdateDatabase();
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
         
        }

        static void mapper_AfterMapClass(IModelInspector modelInspector, Type type, IClassAttributesMapper classCustomizer)
        {
            throw new NotImplementedException();
        }

        public static void UpdateDatabase()
        {

            var export = new SchemaUpdate(cfg);
            export.Execute(false, true);
        }

        public static ISession OpenSession()
        {
            return _sessionFactory.OpenSession();
        }

        public static IStatelessSession OpenStatelessSession()
        {
            return _sessionFactory.OpenStatelessSession();
        }
    }
}

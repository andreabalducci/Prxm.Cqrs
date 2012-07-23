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
                //mapper.IsEntity((t, declared) => t.Namespace.StartsWith("Sample.QueryModel") || );

                mapper.AfterMapClass += (inspector, type, classCustomizer) =>
                {
                    classCustomizer.Lazy(false);
                    //classCustomizer.Id(m => m.Generator(new GuidGeneratorDef()));
                };
                var mapping = mapper.CompileMappingFor(
                    Assembly.Load("Sample.QueryModel").GetExportedTypes()
                    .Union(new Type[] {typeof(Version)}));
                var allmapping = mapping.AsString();

                cfg.AddDeserializedMapping(mapping, "AutoModel");
                _sessionFactory = cfg.BuildSessionFactory();
 
            }
            catch (Exception ex)
            {
                throw ex;
            }
         
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

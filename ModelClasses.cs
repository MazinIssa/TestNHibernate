using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibernateTest.NHBNTypes
{
    public class Person
    {
        public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }
        public virtual DateTime BirthDate { get; set; }
        [Cascade]
        public virtual IList<Item> Items { get; set; }
    }

    public class Item
    {
        public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }
        public virtual int Price { get; set; }
    }

    public class DbConfig : FluentNHibernate.Automapping.DefaultAutomappingConfiguration
    {
        public override bool ShouldMap(Type type)
        {
            //return type.Namespace == "NHibernateTest.NHBNTypes";
            return
                type == typeof(Person)
                || type == typeof(Item); //because of compiler generated classes.
        }

    }

    public class CascadeConvention : IHasManyConvention, IReferenceConvention
    {
        public void Apply(FluentNHibernate.Conventions.Instances.IOneToManyCollectionInstance instance)
        {
            var attrs = instance.Member.GetCustomAttributes(typeof(CascadeAttribute), false);

            if (attrs != null && attrs.Length != 0)
                instance.Cascade.All();
        }

        public void Apply(FluentNHibernate.Conventions.Instances.IManyToOneInstance instance)
        {
            var attrs = instance.Property.MemberInfo.GetCustomAttributes(typeof(CascadeAttribute), false);

            if (attrs != null && attrs.Length != 0)
                instance.Cascade.All();
        }
    }

    public class CascadeAttribute : Attribute { }

    public static class nhbn
    {
        internal static NHibernate.ISessionFactory sessionFactory = null;
        public static void does()
        {
            var connString = "Data Source=.\\SQLExpress;Initial Catalog=TestNHibernate;Integrated Security=SSPI;Trusted_Connection=Yes";
            DbConfig d = new DbConfig();
            var atpm = FluentNHibernate.Automapping.AutoMap
                    .AssemblyOf<Item>(d)
                    .Conventions.Add(typeof(CascadeConvention));

            sessionFactory = Fluently.Configure()
               .Database(MsSqlConfiguration.MsSql2008.ConnectionString(connString))
               .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(false, true))
               .Mappings(m => m.AutoMappings.Add(atpm))
               .BuildSessionFactory();
        }
    }
}

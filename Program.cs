using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using NHibernateTest.NHBNTypes;

namespace NHibernateTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //does does configure NHibernate. Excellent choice of a method name.
            nhbn.does();

            var session = nhbn.sessionFactory.OpenSession();

            //SaveDataTest(session);
            //GetDataTest(session);
            //UpdateExistingDataUsingUnAttachedObject(session);
            TestListStringSave(session);

            session.Close();
        }

        private static void SaveDataTest(NHibernate.ISession session)
        {
            Person p = new Person() { Name = "lksjflkdsjf" };
            Item i = new Item() { Name = "Existentialism", Price = 15 };
            Item ii = new Item() { Name = "Artificial Intelligence", Price = 1 };
            p.Items = new List<Item>();
            p.Items.Add(i);

            var transaction = session.BeginTransaction();

            session.Save(p);

            transaction.Commit();
        }

        private static void GetDataTest(NHibernate.ISession session)
        {


            var transaction = session.BeginTransaction();

            var results1 = session.QueryOver<Person>().Where(x => x.Name == "Mazen").List();
            var person = session.Get<Person>(new Guid("5A1E5ADC-9DC5-46B3-9AF6-A2F500B44CCF"));
            person.Name = "Mazen";
            person.BirthDate = DateTime.Now;
            session.Save(person);

            transaction.Commit();
        }

        private static void UpdateExistingDataUsingUnAttachedObject(NHibernate.ISession session)
        {
            Person p = new Person()
            {
                Name = "Mazen likes NHibernate",
                BirthDate = new DateTime(1989, 5, 31),
                Id = new Guid("5A1E5ADC-9DC5-46B3-9AF6-A2F500B44CCF")
            };

            var transaction = session.BeginTransaction();

            //session.Save(p); //this ignored that the key is set to an existent object, and created a new row in the table!
            session.SaveOrUpdate(p); //worked as expected. NHibernate is my ORM of choice from now on.

            transaction.Commit();
        }

        private static void TestListStringSave(NHibernate.ISession session)
        {
            Item i = new Item() { Name = "Washing Machine", Price = 300, Tags = new List<string>() { "Rabbit", "Jump", "Over" } };
            var transaction = session.BeginTransaction();

            session.Save(i);

            transaction.Commit();
        }
    }
}

using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Inheritance.Single.Bug
{
    [Audited]
    public class ParentBase
    {
        public virtual int Id { get; set; }
        public virtual ISet<Child> Children { get; set; }
    }

    [Audited]
    public class ParentDerived : ParentBase
    { }

    [Audited]
    public class Child
    {
        public virtual int Id { get; set; }
        public virtual ParentBase ParentBase { get; set; }
    }



    public class BugTest : TestBase
    {
        private int id;

        public BugTest(string strategyType)
            : base(strategyType)
        {
        }

        protected override void Initialize()
        {
            id = 1;

            using (var tx = Session.BeginTransaction())
            {
                var entity = new ParentDerived() { Id = id };
                Session.Save(entity);
                Session.Flush();

                tx.Commit();
            }
            var q = Cfg.GetClassMapping(typeof(ParentBase));
            var w = Cfg.GetClassMapping(typeof(ParentDerived));
            var e = Cfg.GetClassMapping(typeof(Child));
            var c = Cfg.GetCollectionMapping(typeof(ParentBase).FullName + "." + "Children");
        }

        [Test]
        public void AddChildToParent()
        {
            using (var tx = Session.BeginTransaction())
            {
                var entity = Session.Get<ParentDerived>(id);

                var item = new Child() { Id = 10 };
                entity.Children.Add(item);

                Session.Save(item);
                tx.Commit();
            }
        }

        [Test]
        public void SetParentInChild()
        {
            using (var tx = Session.BeginTransaction())
            {
                var entity = Session.Get<ParentDerived>(id);

                var item = new Child() { Id = 10 };
                item.ParentBase = entity;

                Session.Save(item);
                Assert.DoesNotThrow(() => tx.Commit());
            }
        }

    }
}
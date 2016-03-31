using System.Linq;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.BiDirOneToOneWithOneSideNotAudited
{
    //[Ignore("Not yet fixed - NHE-150")]
    public class BiDirOneToOneWithOneSideNotAuditedTests : TestBase
    {
        public BiDirOneToOneWithOneSideNotAuditedTests(string strategyType)
            : base(strategyType)
        {
        }

        protected override void Initialize()
        {
            var parent = new Parent { Child = new Child { Str = "sometext" } };
            using (var tx = Session.BeginTransaction())
            {
                Session.Save(parent);
                Session.Save(parent.Child);
                tx.Commit();
            }
        }

        [Test]
        public void QueryHistoryShouldNotThrow()
        {
            Assert.DoesNotThrow(() =>
                {
                    AuditReader().CreateQuery().ForRevisionsOf<Parent>().Results().ToList();
                    AuditReader().CreateQuery().ForHistoryOf<Parent, DefaultRevisionEntity>().Results().ToList();
                });
        }
    }
}
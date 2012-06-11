using System.IO;
using NeosIT.DB_Migrator.DBMigration;
using NUnit.Framework;

namespace NeosIT.DB_Migrator.Test.DBMigration
{
    [TestFixture]
    public class GuardTest
    {
        private Guard _guard;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _guard = new Guard();
        }

        [Test]
        public void TestIsMigrationAllowed()
        {
            var current = new Version {Major = "20120307", Minor = "001",};
            var file = new Version {Major = "20120308", Minor = "0001",};

            _guard.Suffix = null;

            Assert.IsTrue(_guard.IsMigrationAllowed(new FileInfo("./Fixtures/MySQL/Flat/20000101_001.sql"), current,
                                                    file));

            _guard.Suffix = ".sql";

            Assert.IsTrue(_guard.IsMigrationAllowed(new FileInfo("./Fixtures/MySQL/Flat/20000101_001.sql"), current,
                                                    file));
            Assert.IsFalse(_guard.IsMigrationAllowed(new FileInfo("./Fixtures/MySQL/Flat/20000101_002.ignored"), current,
                                                     file));
        }
    }
}
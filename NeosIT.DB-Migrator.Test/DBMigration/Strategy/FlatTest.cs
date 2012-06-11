using System.Collections.Generic;
using System.IO;
using NeosIT.DB_Migrator.DBMigration;
using NeosIT.DB_Migrator.DBMigration.Strategy;
using NUnit.Framework;

namespace NeosIT.DB_Migrator.Test.DBMigration.Strategy
{
    [TestFixture]
    public class FlatTest
    {
        private IStrategy _strategy;
        private Version _version;
        private Guard _guard;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _strategy = new Flat();
            _version = new Version {Major = "20120307", Minor = "001",};
            _guard = new Guard();
        }

        [Test]
        public void TestFindUnappliedMigrationsSince()
        {
            Dictionary<Version, SqlFileInfo> r = _strategy.FindUnappliedMigrationsSince(_version,
                                                                                        new SqlDirInfo { DirectoryInfo = new DirectoryInfo(
                                                                                            "./Fixtures/MySQL/Flat/"),},
                                                                                        _guard);
            IList<long> neededVersions = new List<long> {210001010004, 210001010001, 210001010002, 210001010005,};

            foreach (Version k in r.Keys)
            {
                Assert.IsTrue(neededVersions.Contains(k.GetVersion()));
                neededVersions.Remove(k.GetVersion());
            }

            Assert.AreEqual(0, neededVersions.Count);
        }
    }
}
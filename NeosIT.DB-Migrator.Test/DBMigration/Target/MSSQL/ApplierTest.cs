using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeosIT.DB_Migrator.DBMigration.Target;
using NeosIT.DB_Migrator.DBMigration.Parsers.MSSQL;
using System.Xml;
using System.Xml.XPath;
using System.Data.SqlClient;
using NUnit.Framework;
using System.IO;
using NeosIT.DB_Migrator.DBMigration.Target.MSSQL;
using NeosIT.DB_Migrator.DBMigration.Strategy;
using NeosIT.DB_Migrator.DBMigration;

namespace NeosIT.DB_Migrator.Test.DBMigration.Target.MSSQL
{
    public class ApplierMockTest : NeosIT.DB_Migrator.DBMigration.Target.MSSQL.Applier
    {
        public override void Commit()
        {
            // Ignore writing to file
            AppendCommitTransaction();

            Sw.Dispose();
        }
    }

    [TestFixture]
    public class ApplierTest
    {
        private IStrategy _strategy;
        private NeosIT.DB_Migrator.DBMigration.Version _version;
        private Guard _guard;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _strategy = new Flat();
            _version = new NeosIT.DB_Migrator.DBMigration.Version { Major = "20120101", Minor = "001", };
            _guard = new Guard();
        }

        [Test]
        public void TestUsingOfGo()
        {
            Dictionary<NeosIT.DB_Migrator.DBMigration.Version, SqlFileInfo> r = _strategy.FindUnappliedMigrationsSince(_version,
                                                                                        new SqlDirInfo
                                                                                        {
                                                                                            DirectoryInfo = new DirectoryInfo(
                                                                                                "./Fixtures/SimpleMigrations/"),
                                                                                        },
                                                                                        _guard);
            Assert.AreEqual(2, r.Count);

            ApplierMockTest applier = new ApplierMockTest();
            applier.Begin();
            applier.Prepare(r);
            applier.Commit();
            string[] content = File.ReadAllLines(applier.Filename);

            Assert.NotNull(content);
            Assert.True(content.Length > 4);
            Assert.AreEqual("GO", content[content.Length - 2]);
            applier.Cleanup();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using NeosIT.DB_Migrator.DBMigration;
using NeosIT.DB_Migrator.DBMigration.Strategy;
using NUnit.Framework;
using Version = NeosIT.DB_Migrator.DBMigration.Version;

namespace NeosIT.DB_Migrator.Test.DBMigration
{
    [TestFixture]
    public class MigratorTest
    {
        private Migrator _migrator;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _migrator = new Migrator();
            _migrator.Guard = new Guard();
        }

        [Test]
        public void TestCreateDirElementHandelsCustomParameters()
        {
            string opts = "./fixtures" + _migrator.SeparatorOpts + "latest" + _migrator.SeparatorOpts + "yes";
            SqlDirInfo r = _migrator.CreateDirElement(opts);

            Assert.IsTrue(r.LatestOnly);
            Assert.IsTrue(r.SqlInsertMigration);
        }

        [Test]
        public void TestCreateDirElementHandlesInvalidDir()
        {
            string opts = "invaliddir";

            try
            {
                SqlDirInfo r = _migrator.CreateDirElement(opts);
                Assert.IsTrue(false);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("is not valid"));
            }

            //Assert.Throws(typeof(FileNotFoundException), () => _migrator.CreateDirElement(opts));
        }

        [Test]
        public void TestCreateDirElementHandlesParameterDefaults()
        {
            string opts = "./Fixtures" + _migrator.SeparatorOpts + "bla" + _migrator.SeparatorOpts + "no";
            SqlDirInfo r = _migrator.CreateDirElement(opts);

            Assert.IsTrue(false == r.LatestOnly);
            Assert.IsTrue(null == r.Files);
            Assert.IsTrue(false == r.SqlInsertMigration);
        }

        [Test]
        public void TestGetSqlStacktraceWithoutMagicTag()
        {
            string sep = Environment.NewLine;
            SqlStacktrace r = _migrator.GetSqlStacktrace(new List<string> {"hello", "world", "iam", "evil", "jared"}, 3,
                                                         2, 0);

            Assert.AreEqual(new List<string> {"world", "iam", "evil"}, r.Lines);
            Assert.IsNull(r.File);
        }

        [Test]
        public void TestMergeMigrationsFromDirectoriesCorrectOrderLatestOnly()
        {
            IList<SqlDirInfo> pathdef = new List<SqlDirInfo>
                                            {
                                                _migrator.CreateDirElement("./Fixtures/MergeDirectories/Unittest"),
                                                _migrator.CreateDirElement("./Fixtures/MergeDirectories/Coredata"),
                                                _migrator.CreateDirElement(
                                                    "./Fixtures/MergeDirectories/Migrations,latest,true"),
                                            };
            _migrator.Strategy = new Flat();
            Dictionary<Version, SqlFileInfo> r = _migrator.MergeMigrationsFromDirectories(pathdef, new Version());

            Assert.AreEqual(4, r.Count);
            Assert.IsFalse(r.Keys.Any(x => x.GetVersion() == 201201010001));
            Assert.IsFalse(r.Keys.Any(x => x.GetVersion() == 201201010002));
            Assert.IsFalse(r.Keys.Any(x => x.GetVersion() == 201201040001));

            var v = r.Keys.Single(x => x.GetVersion() == 201206060001);
            Assert.IsTrue(r[v].FileInfo.Name.EndsWith("conflict.sql"));
        }

        [Test]
        public void TestMergeMigrationsFromDirectory()
        {
            IList<SqlDirInfo> pathdef = new List<SqlDirInfo>
                                            {
                                                _migrator.CreateDirElement("./Fixtures/MergeDirectories/Unittest"),
                                                _migrator.CreateDirElement("./Fixtures/MergeDirectories/Coredata"),
                                                _migrator.CreateDirElement("./Fixtures/MergeDirectories/Migrations"),
                                            };
            _migrator.Strategy = new Flat();
            Dictionary<Version, SqlFileInfo> r = _migrator.MergeMigrationsFromDirectories(pathdef, new Version());

            Assert.AreEqual(7, r.Count);
            var v = r.Keys.Single(x => x.GetVersion() == 201206060001);
            Assert.IsTrue(r[v].FileInfo.Name.EndsWith("conflict.sql"));
        }
    }
}
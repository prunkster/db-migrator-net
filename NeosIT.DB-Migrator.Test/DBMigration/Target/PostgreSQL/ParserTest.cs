using NeosIT.DB_Migrator.DBMigration;
using NeosIT.DB_Migrator.DBMigration.Parsers;
using NeosIT.DB_Migrator.DBMigration.Parsers.PostgreSQL;
using NeosIT.DB_Migrator.DBMigration.Strategy;
using NeosIT.DB_Migrator.DBMigration.Target.PostgreSQL;
using NUnit.Framework;

namespace NeosIT.DB_Migrator.Test.DBMigration.Target.PostgreSQL
{
    [TestFixture]
    public class ParserTest
    {
        private AbstractParser _parser;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _parser = new Parser();
        }

        [Test]
        public void TestParseCustomSettings()
        {
            Migrator r =
                _parser.Parse(
                    new[]
                        {
                            "--username=user", "--database=database", "--command=postgresql-custom-command",
                            "--password=custom-password", "--host=remote-host",
                            "--args=--custom-postgresql-args=bla", "--suffix=.bla", "--strategy=hierarchial",
                            "c:/temp,latest,true;", "--target=postgresql"
                        },
                    new Migrator());

            Assert.AreEqual("c:/temp" + r.SeparatorOpts + "latest" + r.SeparatorOpts + "true;", r.Directories);
            Assert.AreEqual("user", r.DbInterface.Executor.Username);
            Assert.AreEqual("custom-password", r.DbInterface.Executor.Password);
            Assert.AreEqual("postgresql-custom-command", r.DbInterface.Executor.Command);
            Assert.AreEqual("database", r.DbInterface.Executor.Database);
            Assert.IsInstanceOf<Hierarchial>(r.Strategy);
            Assert.AreEqual("--custom-postgresql-args=bla", r.DbInterface.Executor.Args);
            Assert.IsInstanceOf<Guard>(r.Guard);
            Assert.AreEqual(".bla", r.Guard.Suffix);
        }

        [Test]
        public void TestParseDefaultSettings()
        {
            Migrator r = _parser.Parse(new[] { "--username=user", "--database=database", "--target=postgresql" },
                                       new Migrator());

            Assert.AreEqual("." + r.SeparatorOpts + "all" + r.SeparatorOpts + "false", r.Directories);
            Assert.AreEqual("user", r.DbInterface.Executor.Username);
            Assert.AreEqual("", r.DbInterface.Executor.Password);
            Assert.AreEqual("psql", r.DbInterface.Executor.Command);
            Assert.AreEqual("database", r.DbInterface.Executor.Database);
            Assert.IsInstanceOf<Flat>(r.Strategy);
            Assert.AreEqual("", r.DbInterface.Executor.Args);
            Assert.IsInstanceOf<Guard>(r.Guard);
            Assert.AreEqual(".sql", r.Guard.Suffix);
        }
    }
}
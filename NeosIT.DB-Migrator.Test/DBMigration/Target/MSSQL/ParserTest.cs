using NeosIT.DB_Migrator.DBMigration;
using NeosIT.DB_Migrator.DBMigration.Parsers;
using NeosIT.DB_Migrator.DBMigration.Parsers.MSSQL;
using NeosIT.DB_Migrator.DBMigration.Strategy;
using NeosIT.DB_Migrator.DBMigration.Target.MSSQL;
using NUnit.Framework;
using System.IO;

namespace NeosIT.DB_Migrator.Test.DBMigration.Target.MSSQL
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
                            "--username=user", "--database=database", "--command=mssql-custom-command",
                            "--password=custom-password", "--host=remote-host",
                            "--args=--custom-mssql-args=bla", "--suffix=.bla", "--strategy=hierarchial",
                            "c:/temp,latest,true;", "--target=mssql"
                        },
                    new Migrator());

            Assert.AreEqual("c:/temp" + r.SeparatorOpts + "latest" + r.SeparatorOpts + "true;", r.Directories);
            Assert.AreEqual("user", r.DbInterface.Executor.Username);
            Assert.AreEqual("custom-password", r.DbInterface.Executor.Password);
            Assert.AreEqual("mssql-custom-command", r.DbInterface.Executor.Command);
            Assert.AreEqual("database", r.DbInterface.Executor.Database);
            Assert.IsInstanceOf<Hierarchial>(r.Strategy);
            Assert.AreEqual("--custom-mssql-args=bla", r.DbInterface.Executor.Args);
            Assert.IsInstanceOf<Guard>(r.Guard);
            Assert.AreEqual(".bla", r.Guard.Suffix);
        }

        [Test]
        public void TestParseDefaultSettings()
        {
            Migrator r = _parser.Parse(new[] { "--username=user", "--database=database", "--password=password", "--target=mssql" },
                                       new Migrator());

            Assert.AreEqual("." + r.SeparatorOpts + "all" + r.SeparatorOpts + "false", r.Directories);
            Assert.AreEqual("user", r.DbInterface.Executor.Username);
            Assert.AreEqual("password", r.DbInterface.Executor.Password);
            Assert.AreEqual("osql", r.DbInterface.Executor.Command);
            Assert.AreEqual("database", r.DbInterface.Executor.Database);
            Assert.IsInstanceOf<Flat>(r.Strategy);
            Assert.AreEqual("", r.DbInterface.Executor.Args);
            Assert.IsInstanceOf<Guard>(r.Guard);
            Assert.AreEqual(".sql", r.Guard.Suffix);
        }

        [Test]
        public void TestParseXmlConfigurationFile()
        {
            string path = new DirectoryInfo("./Fixtures/MSSQL/ConfigWithUsernamePassword.xml").FullName;
            Migrator r = _parser.Parse(new[] { "--username=overwrite", "--password=overwrite", "--config='" + path + "'", "--target=mssql" }, new Migrator());

            Assert.AreEqual("fixture2User", r.DbInterface.Executor.Username);
            Assert.AreEqual("fixture2Password", r.DbInterface.Executor.Password);
            Assert.AreEqual("fixture2Host", r.DbInterface.Executor.Host);
            Assert.AreEqual("fixture2Db", r.DbInterface.Executor.Database);
        }

        [Test]
        public void TestParseXpathExpression()
        {
            string path = new DirectoryInfo("./Fixtures/MSSQL/ConfigWithUnityInjection.xml").FullName;
            Migrator r = _parser.Parse(new[] { "--username=overwrite", 
                "--password=overwrite", 
                "--config='" + path + "'",
                "--xpath='/configuration/unity/container/register[@type=\"IDalFactory\"]/constructor/param/value/@value'",
                "--target=mssql" }, new Migrator());

            Assert.AreEqual("fixture3User", r.DbInterface.Executor.Username);
            Assert.AreEqual("fixture3Password", r.DbInterface.Executor.Password);
            Assert.AreEqual("fixture3Host", r.DbInterface.Executor.Host);
            Assert.AreEqual("fixture3Db", r.DbInterface.Executor.Database);
        }

        [Test]
        public void TestParseXpathExressionWithNamespaces()
        {
            string path = new DirectoryInfo("./Fixtures/MSSQL/ConfigWithUnityInjectionAndNamespaces.xml").FullName;
            Migrator r = _parser.Parse(new[] { "--username=overwrite", 
                "--password=overwrite", 
                "--config='" + path + "'",
                "--namespaces='ns1=http://schemas.microsoft.com/practices/2010/unity'",
                "--xpath='/configuration/ns1:unity/ns1:container/ns1:register[@type=\"IDalFactory\"]/ns1:constructor/ns1:param/ns1:value/@value'",
                "--target=mssql" }, new Migrator());

            Assert.AreEqual("fixture4User", r.DbInterface.Executor.Username);
            Assert.AreEqual("fixture4Password", r.DbInterface.Executor.Password);
            Assert.AreEqual("fixture4Host", r.DbInterface.Executor.Host);
            Assert.AreEqual("fixture4Db", r.DbInterface.Executor.Database);
        }
    }
}
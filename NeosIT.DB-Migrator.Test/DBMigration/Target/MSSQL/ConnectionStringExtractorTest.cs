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

namespace NeosIT.DB_Migrator.Test.DBMigration.Target.MSSQL
{
    [TestFixture]
    public class ConnectionStringExtractorTest
    {
        private IExecutor _executor;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _executor = new Executor();
        }

        [Test]
        public void TestDefaultConnectionString()
        {
            string fixture = new DirectoryInfo("./Fixtures/MSSQL/ConfigWithIntegratedSecurity.xml").FullName;
            XPathDocument doc = new XPathDocument(fixture);
            var nav = doc.CreateNavigator();
            ConnectionStringExtractor cse = new ConnectionStringExtractor(nav, new XmlNamespaceManager(nav.NameTable));
            _executor.Username = "bla";
            _executor.Password = "blub";
            _executor.Database = "blip";

            cse.LoadConnectionStringInto(@"/configuration/connectionStrings/add[1]/@connectionString",  _executor);
            Assert.AreEqual("fixture1Host", _executor.Host);
            Assert.AreEqual("fixture1Db", _executor.Database);
            // SSPI
            Assert.AreEqual("", _executor.Username);
            Assert.AreEqual("", _executor.Password);
        }
    }
}

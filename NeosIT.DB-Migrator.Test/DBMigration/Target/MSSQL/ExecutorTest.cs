using System.Collections.Generic;
using NeosIT.DB_Migrator.DBMigration.Target;
using NeosIT.DB_Migrator.DBMigration.Target.MSSQL;
using NUnit.Framework;

namespace NeosIT.DB_Migrator.Test.DBMigration.Target.MSSQL
{
    [TestFixture]
    public class ExecutorTest
    {
        private IExecutor _executor;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _executor = new Executor();
        }

        [Test]
        public void TestBuildExecCommandDefaultOpts()
        {
            IList<string> r = _executor.BuildExecCommand("cmd");

            Assert.IsTrue(r.Contains("-S localhost"));
            Assert.IsTrue(r.Contains("-U Administrator"));
            Assert.IsTrue(r.Contains("-Q \"cmd\""));
        }

        [Test]
        public void TestBuildExecCommandSetCustomOpts()
        {
            _executor.Username = "username";
            _executor.Host = "host";
            _executor.Password = "password";
            _executor.Args = "--ssl-enabled=true";
            IList<string> r = _executor.BuildExecCommand("cmd", true);

            Assert.IsTrue(r.Contains("-S host"));
            Assert.IsTrue(r.Contains("-U username"));
            Assert.IsTrue(r.Contains("-P password"));
            Assert.IsTrue(r.Contains("--ssl-enabled=true"));
            Assert.IsTrue(r.Contains("-V 10"));
            Assert.IsTrue(r.Contains("-Q \"cmd\""));
        }
    }
}
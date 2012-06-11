using System.Collections.Generic;
using NeosIT.DB_Migrator.DBMigration.Target;
using NeosIT.DB_Migrator.DBMigration.Target.PostgreSQL;
using NUnit.Framework;

namespace NeosIT.DB_Migrator.Test.DBMigration.Target.PostgreSQL
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

            Assert.IsTrue(r.Contains("--host=localhost"));
            Assert.IsTrue(r.Contains("--username=postgres"));
            /*Assert.IsTrue(r.Contains("--no-align"));
            Assert.IsTrue(r.Contains("-t"));*/
            Assert.IsTrue(r.Contains("--command=\"cmd\""));
        }

        [Test]
        public void TestBuildExecCommandSetCustomOpts()
        {
            _executor.Username = "username";
            _executor.Host = "host";
            _executor.Password = "password";
            _executor.Args = "--ssl-enabled=true";
            IList<string> r = _executor.BuildExecCommand("cmd", true);

            Assert.IsTrue(r.Contains("--host=host"));
            Assert.IsTrue(r.Contains("--username=username"));
            //Assert.IsTrue(r.Contains("-P password"));
            Assert.IsTrue(r.Contains("--ssl-enabled=true"));
            Assert.IsTrue(r.Contains("--command=\"cmd\""));
        }
    }
}
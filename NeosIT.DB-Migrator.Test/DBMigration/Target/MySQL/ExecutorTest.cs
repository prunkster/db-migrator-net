using System.Collections.Generic;
using NeosIT.DB_Migrator.DBMigration.Target;
using NeosIT.DB_Migrator.DBMigration.Target.MySQL;
using NUnit.Framework;

namespace NeosIT.DB_Migrator.Test.DBMigration.Target.MySQL
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
        public void BuildExecCommandDefaultOpts()
        {
            IList<string> r = _executor.BuildExecCommand("cmd");

            Assert.IsTrue(r.Contains("--host=localhost"));
            Assert.IsTrue(r.Contains("--password="));
            Assert.IsTrue(r.Contains("--user=root"));
            Assert.IsTrue(r.Contains("--vertical"));
            Assert.IsTrue(r.Contains("--execute=cmd"));
        }

        [Test]
        public void BuildExecCommandSetCustomOpts()
        {
            _executor.Username = "username";
            _executor.Host = "host";
            _executor.Password = "password";
            _executor.Args = "--ssl-enabled=true";
            IList<string> r = _executor.BuildExecCommand("cmd", true);

            Assert.IsTrue(r.Contains("--host=host"));
            Assert.IsTrue(r.Contains("--password=password"));
            Assert.IsTrue(r.Contains("--user=username"));
            Assert.IsTrue(r.Contains("--ssl-enabled=true"));
            Assert.IsTrue(r.Contains("--vertical"));
            Assert.IsTrue(r.Contains("--verbose"));
            Assert.IsTrue(r.Contains("--execute=cmd"));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace NeosIT.DB_Migrator.Test.DBMigration.Target.MSSQL
{
    public class DbInterfaceTest
    {
        [Test]
        public void TestRegExp()
        {
            var s = " 20120820 001      \r";
            Match match = Regex.Match(s, @"\s*(\d*)\s*(\d*)\s*");

            Assert.AreEqual("20120820", match.Groups[1].ToString());
            Assert.AreEqual("001", match.Groups[2].ToString());
        }
    }
}

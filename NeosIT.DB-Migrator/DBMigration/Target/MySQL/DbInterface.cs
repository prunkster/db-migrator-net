using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NeosIT.DB_Migrator.DBMigration.Target.MySQL
{
    public class DbInterface : IDbInterface
    {
        private Log log = new Log();
        private const string SqlMajorCol = "major";
        private const string SqlMinorCol = "minor";

        private const string SqlLatestMigration =
            "SELECT " + SqlMajorCol + ", " + SqlMinorCol + " FROM migrations ORDER BY " + SqlMajorCol + " DESC, " +
            SqlMinorCol + " DESC LIMIT 1";

        private const string SqlCreateMigration =
            "CREATE TABLE migrations(id INT NOT NULL auto_increment, installed_on TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP, " +
            SqlMajorCol + " char(8), " + SqlMinorCol + " char(8), filename longtext, PRIMARY KEY(id))";

        #region IDbInterface Members

        public IExecutor Executor { get; set; }

        public Version FindLatestMigration()
        {
            try
            {
                IList<string> lines = Executor.ExecCommand(SqlLatestMigration).Split(new[] {'\n',});
                string major = "0";
                string minor = "0";

                if (lines.Count >= 3)
                {
                    Match majorMatch = Regex.Match(lines[1], SqlMajorCol + @": (\d*)");
                    Match minorMatch = Regex.Match(lines[2], SqlMinorCol + @": (\d*)");

                    if (!majorMatch.Success || !minorMatch.Success)
                    {
                        throw new Exception("Could not filter major/minor version from returned SQL statement");
                    }

                    major = majorMatch.Groups[1].Value;
                    minor = minorMatch.Groups[1].Value;
                }

                var r = new Version {Major = major, Minor = minor,};

                return r;
            }
            catch (Exception e)
            {
                log.Error(String.Format("Could not retrieve latest revision from database: {0}", e.Message));

                if (Regex.Match(e.Message, ".*migrations.*doesn.t.*exist").Success)
                {
                    log.Warn("Migrations table does not exist... creating");

                    try
                    {
                        Executor.ExecCommand(SqlCreateMigration);
                        log.Success("migrations table successfully created :-)");
                    }
                    catch (Exception eCreate)
                    {
                        throw new Exception(string.Format("Could not create migrations table: {0}", eCreate.Message));
                    }

                    return FindLatestMigration();
                }

                throw new Exception(e.Message);
            }
        }

        #endregion
    }
}
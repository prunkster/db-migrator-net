using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NeosIT.DB_Migrator.DBMigration.Target.PostgreSQL
{
    public class FilterException : Exception
    {
    }

    public class DbInterface : IDbInterface
    {
        private const string SqlMajorCol = "major";
        private const string SqlMinorCol = "minor";

        private const string SqlLatestMigration =
            "SELECT " + SqlMajorCol + ", " + SqlMinorCol + " FROM migrations ORDER BY " + SqlMajorCol + " DESC, " +
            SqlMinorCol + " DESC LIMIT 1";

        private const string SqlCreateMigration =
            "CREATE TABLE migrations(id SERIAL NOT NULL PRIMARY KEY, installed_on TIMESTAMP NOT NULL DEFAULT NOW(), " +
            SqlMajorCol + " varchar(8), " + SqlMinorCol + " varchar(8), filename text)";

        #region IDbInterface Members

        public IExecutor Executor { get; set; }

        public Version FindLatestMigration()
        {
            try
            {
                IList<string> lines = Executor.ExecCommand(SqlLatestMigration).Split(new[] {'\n',});
                string major = "0";
                string minor = "0";

                if (lines.Count >= 4)
                {
                    if (!Regex.Match(lines[0], @"\s+" + SqlMajorCol + @"\s+\|\s+" + SqlMinorCol + @"\s+").Success)
                    {
                        throw new FilterException();
                    }

                    Match match = Regex.Match(lines[2], @"\s+(\d*)\s+\|\s+(\d*)\s*");
                    if (match.Success)
                    {
                        major = match.Groups[1].Value;
                        minor = match.Groups[2].Value;
                    }
                }

                var r = new Version {Major = major, Minor = minor,};

                return r;
            }
            catch (Exception e)
            {
                Console.WriteLine("[error] could not retrieve latest revision from database: {0}", e.Message);

                if (e is FilterException)
                {
                    throw new Exception("Could not filter output");
                }

                if (Regex.Match(e.Message.Split(new[] {'\n'})[0], "(.*)relation(.*)does not exist").Success)
                {
                    Console.WriteLine("[create] migrations table does not exist... creating");

                    try
                    {
                        Executor.ExecCommand(SqlCreateMigration);
                        Console.WriteLine("[create] migrations table successfully created");
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
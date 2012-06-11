using System;
using Appature.Common;

namespace NeosIT.DB_Migrator.DBMigration.Target.MSSQL
{
    public class Parser : AbstractParser
    {
        private bool _useWindowsAuth;

        public override CommandParser CreateCustomOpts(CommandParser cli)
        {
            cli.Argument("u", "username",
                         "a valid username for Microsoft SQL server. If none is set, a trusted connection (osql parameter -E) is used. If you specify a username, you need to add a password!",
                         "database-username",
                         CommandArgumentFlags.TakesParameter, (p, v) => Options.Add("username", v));

            cli.Argument("c", "command", "Path to osql if not in environment path", "command",
                         CommandArgumentFlags.TakesParameter, (p, v) => Options.Add("command", v));

            cli.Argument("d", "database", "database - can be left if your MSSQL user has a default database set",
                         "database",
                         CommandArgumentFlags.TakesParameter, (p, v) => Options.Add("database", v));

            cli.Argument("w", "winauth", "use Windows authentication - no user/password must be set",
                         "use-windows-auth", CommandArgumentFlags.None,
                         (p, v) => { _useWindowsAuth = true; });

            return cli;
        }

        public override Migrator SetCustomOptionsForMigrator(Migrator migrator)
        {
            if (_useWindowsAuth)
            {
                migrator.DbInterface.Executor.Username = string.Empty;
                migrator.DbInterface.Executor.Password = string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(migrator.DbInterface.Executor.Username) &&
                string.IsNullOrWhiteSpace(migrator.DbInterface.Executor.Password))
            {
                throw new Exception("You must apply a password for your database username!");
            }

            return migrator;
        }

        public override Migrator InitMigrator(Migrator migrator)
        {
            return Factory.Create("mssql", migrator);
        }
    }
}
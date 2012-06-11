using CommandLine;

namespace NeosIT.DB_Migrator.DBMigration.Options.MSSQL
{
    public class Options : AbstractOptions
    {
        [Option("u", "username", HelpText = "A valid username for Microsoft SQL server. If none is set, a trusted connection (osql parameter -E) is used. If you specify a username, you need to add a password!")]
        public override string Username { get; set; }

        [Option("c", "command", HelpText = "Path to osql if not in environment path")]
        public override string Command { get; set; }

        [Option("d", "database", HelpText = "Database - can be left if your MSSQL user has a default database set")]
        public override string Database { get; set; }

        [Option("w", "winauth", HelpText = "Use Windows authentication - no user/password must be set")]
        public virtual bool WinAuth { get; set; }
    }
}

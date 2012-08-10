using CommandLine;

namespace NeosIT.DB_Migrator.DBMigration.Options.MySQL
{
    public class Options : DefaultOptions
    {
        [Option("u", "username", HelpText = "A valid MySQL username (required)", Required = true)]
        public override string Username { get; set; }

        [Option("c", "command", HelpText = "Path to mysql if not in environment path")]
        public override string Command { get; set; }

        [Option("d", "database", HelpText = "MySQL database (required)")]
        public override string Database { get; set; }
    }
}

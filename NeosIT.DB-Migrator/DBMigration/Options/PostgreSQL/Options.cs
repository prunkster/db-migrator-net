using CommandLine;

namespace NeosIT.DB_Migrator.DBMigration.Options.PostgreSQL
{
    public class Options : DefaultOptions
    {
        [Option("p", "password", HelpText = "Password will be passed via environment variable PGPASSWORD; this could be security issue on some systems")]
        public override string Password { get; set; }
    }
}

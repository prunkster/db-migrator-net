using Appature.Common;

namespace NeosIT.DB_Migrator.DBMigration.Target.MySQL
{
    public class Parser : AbstractParser
    {
        public override CommandParser CreateCustomOpts(CommandParser cli)
        {
            cli.Argument("u", "username", "a valid MySQL username (required)", "database-username",
                         CommandArgumentFlags.TakesParameter, (p, v) => Options.Add("username", v));

            cli.Argument("c", "command", "Path to mysql if not in environment path", "command",
                         CommandArgumentFlags.TakesParameter, (p, v) => Options.Add("command", v));

            cli.Argument("d", "database", "MySQL database (required)", "database",
                         CommandArgumentFlags.TakesParameter, (p, v) => Options.Add("database", v));

            return cli;
        }

        public override Migrator InitMigrator(Migrator migrator)
        {
            return Factory.Create("mysql", migrator);
        }
    }
}
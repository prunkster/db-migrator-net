using Appature.Common;

namespace NeosIT.DB_Migrator.DBMigration.Target.PostgreSQL
{
    public class Parser : AbstractParser
    {
        public override CommandParser CreateCustomOpts(CommandParser cli)
        {
            cli.Argument("p", "password",
                         "Password will be passed via environment variable PGPASSWORD; this could be security issue on some systems",
                         "database-password",
                         CommandArgumentFlags.TakesParameter, (p, v) => Options.Add("password", v));

            return cli;
        }

        public override Migrator InitMigrator(Migrator migrator)
        {
            return Factory.Create("postgresql", migrator);
        }
    }
}
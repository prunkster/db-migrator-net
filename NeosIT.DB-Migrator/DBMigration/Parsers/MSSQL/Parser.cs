using System;

namespace NeosIT.DB_Migrator.DBMigration.Parsers.MSSQL
{
    public class Parser : AbstractParser
    {
        public override Migrator SetCustomOptionsForMigrator(Migrator migrator)
        {
            if (((Options.MSSQL.Options) CurrentOptions).WinAuth)
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
            return Target.Factory.Create("mssql", migrator);
        }
    }
}
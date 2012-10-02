namespace NeosIT.DB_Migrator.DBMigration.Target.MSSQL
{
    public class Applier : Target.Applier
    {
        public override void AppendBeginTransaction()
        {
            Sw.WriteLine("BEGIN TRANSACTION;");
        }

        public override void AppendCommitTransaction()
        {
            Sw.WriteLine("COMMIT;");
        }

        public override void AfterMigrationFile(Version version, SqlFileInfo file)
        {
            // GO is needed for applying multiple DDL statements in one transaction
            Sw.WriteLine("GO");

            base.AfterMigrationFile(version, file);
            Sw.WriteLine("GO");
        }
    }
}
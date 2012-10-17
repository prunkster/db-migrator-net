namespace NeosIT.DB_Migrator.DBMigration.Target.MSSQL
{
    public class Applier : Target.Applier
    {
        public override void AppendBeginTransaction()
        {
            streamWriter.WriteLine("BEGIN TRANSACTION;");
            streamWriter.WriteLine("GO");
        }

        public override void AppendCommitTransaction()
        {
            streamWriter.WriteLine("COMMIT;");
        }

        public override void AfterMigrationFile(Version version, SqlFileInfo file)
        {
            // GO is needed for applying multiple DDL statements in one transaction
            streamWriter.WriteLine("GO");

            base.AfterMigrationFile(version, file);
            streamWriter.WriteLine("GO");
        }
    }
}
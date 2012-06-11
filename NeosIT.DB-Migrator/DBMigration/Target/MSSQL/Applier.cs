namespace NeosIT.DB_Migrator.DBMigration.Target.MSSQL
{
    public class Applier : Target.Applier
    {
        public new void AppendBeginTransaction()
        {
            Sw.WriteLine("BEGIN TRANSACTION;");
        }

        public new void AppendCommitTransaction()
        {
            Sw.WriteLine("COMMIT;");
        }
    }
}
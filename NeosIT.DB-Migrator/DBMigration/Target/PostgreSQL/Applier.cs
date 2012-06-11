namespace NeosIT.DB_Migrator.DBMigration.Target.PostgreSQL
{
    public class Applier : Target.Applier
    {
        public new void AppendBeginTransaction()
        {
            Sw.WriteLine("BEGIN;");
        }

        public new void AppendCommitTransaction()
        {
            Sw.WriteLine("COMMIT;");
        }
    }
}
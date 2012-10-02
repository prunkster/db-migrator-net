namespace NeosIT.DB_Migrator.DBMigration.Target.PostgreSQL
{
    public class Applier : Target.Applier
    {
        public override void AppendBeginTransaction()
        {
            Sw.WriteLine("BEGIN;");
        }

        public override void AppendCommitTransaction()
        {
            Sw.WriteLine("COMMIT;");
        }
    }
}
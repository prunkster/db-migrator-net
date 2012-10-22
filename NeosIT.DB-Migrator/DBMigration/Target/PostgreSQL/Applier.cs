namespace NeosIT.DB_Migrator.DBMigration.Target.PostgreSQL
{
    public class Applier : Target.Applier
    {
        public override void AppendBeginTransaction()
        {
            streamWriter.WriteLine("BEGIN;");
        }

        public override void AppendCommitTransaction()
        {
            streamWriter.WriteLine("COMMIT;");
        }
    }
}
namespace NeosIT.DB_Migrator.DBMigration.Target.MySQL
{
    public class Applier : Target.Applier
    {
        public override void AppendBeginTransaction()
        {
            streamWriter.WriteLine("SET autocommit=0;");
            streamWriter.WriteLine("START TRANSACTION;");
        }

        public override void AppendCommitTransaction()
        {
            streamWriter.WriteLine("COMMIT;");
        }
    }
}
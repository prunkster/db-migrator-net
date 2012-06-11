namespace NeosIT.DB_Migrator.DBMigration.Target.MySQL
{
    public class Applier : Target.Applier
    {
        public new void AppendBeginTransaction()
        {
            Sw.WriteLine("SET autocommit=0;");
            Sw.WriteLine("START TRANSACTION;");
        }

        public new void AppendCommitTransaction()
        {
            Sw.WriteLine("COMMIT;");
        }
    }
}
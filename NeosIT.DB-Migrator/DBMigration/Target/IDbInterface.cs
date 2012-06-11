namespace NeosIT.DB_Migrator.DBMigration.Target
{
    public interface IDbInterface
    {
        IExecutor Executor { get; set; }
        Version FindLatestMigration();
    }
}
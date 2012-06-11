namespace NeosIT.DB_Migrator.DBMigration.Parsers.MySQL
{
    public class Parser : AbstractParser
    {
        public override Migrator InitMigrator(Migrator migrator)
        {
            return Target.Factory.Create("mysql", migrator);
        }
    }
}
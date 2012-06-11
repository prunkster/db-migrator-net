namespace NeosIT.DB_Migrator.DBMigration.Parsers.PostgreSQL
{
    public class Parser : AbstractParser
    {
        public override Migrator InitMigrator(Migrator migrator)
        {
            return Target.Factory.Create("postgresql", migrator);
        }
    }
}
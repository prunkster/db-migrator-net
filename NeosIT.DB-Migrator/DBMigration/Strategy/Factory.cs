namespace NeosIT.DB_Migrator.DBMigration.Strategy
{
    public class Factory
    {
        public static IStrategy Create(string strategy)
        {
            switch (strategy)
            {
                case "hierarchial":
                    return new Hierarchial();
                default:
                    return new Flat();
            }
        }
    }
}
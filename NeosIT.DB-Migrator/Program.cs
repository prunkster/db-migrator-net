using System;
using NeosIT.DB_Migrator.DBMigration;
using Factory = NeosIT.DB_Migrator.DBMigration.Parsers.Factory;
using NeosIT.DB_Migrator.DBMigration.Parsers;
using NeosIT.DB_Migrator.DBMigration.Options;

namespace NeosIT.DB_Migrator
{
    class Program
    {
        static void Main(string[] args)
        {
            Migrator migrator = null;
            AbstractParser parser = null;

            try
            {
                parser = Factory.Create(args);
            }
            catch (Exception ex)
            {
                Console.Write(new DefaultOptions().Help());
                Console.WriteLine("Could not continue: " + ex.Message);
                Environment.Exit(1);
            }

            try
            {
                migrator = parser.Parse(args, new Migrator());
            }
            catch (Exception ex)
            {
                Console.Write(parser.CurrentOptions.Help());
                Console.WriteLine("Error: " + ex.Message);
                Environment.Exit(1);
            }

            if (migrator != null)
            {
                migrator.Run();
            }
        }
    }
}

using System;
using NeosIT.DB_Migrator.DBMigration;
using Factory = NeosIT.DB_Migrator.DBMigration.Parsers.Factory;

namespace NeosIT.DB_Migrator
{
    class Program
    {
        static void Main(string[] args)
        {
            Migrator migrator = Factory.Create(args).Parse(args, new Migrator());
            migrator.Run();
            Console.ReadLine();
        }
    }
}

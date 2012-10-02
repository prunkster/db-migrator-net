using System;
using NeosIT.DB_Migrator.DBMigration.Target.MySQL;

namespace NeosIT.DB_Migrator.DBMigration.Target
{
    public class Factory
    {
        public static Migrator Create(string targetName, Migrator migrator)
        {
            migrator.Guard = new Guard();

            switch (targetName)
            {
                case "mysql":
                    migrator.DbInterface = new DbInterface();
                    migrator.DbInterface.Executor = new Executor();
                    migrator.Applier = new MySQL.Applier();
                    break;
                case "mssql":
                    migrator.DbInterface = new MSSQL.DbInterface();
                    migrator.DbInterface.Executor = new MSSQL.Executor();
                    migrator.Applier = new MSSQL.Applier();
                    break;
                case "postgresql":
                    migrator.DbInterface = new PostgreSQL.DbInterface();
                    migrator.DbInterface.Executor = new PostgreSQL.Executor();
                    migrator.Applier = new PostgreSQL.Applier();
                    break;
                default:
                    throw new Exception("Target name " + targetName + " is not valid");
            }

            return migrator;
        }
    }
}
using System;
using System.Linq;
using NeosIT.DB_Migrator.DBMigration.Options;

namespace NeosIT.DB_Migrator.DBMigration.Parsers
{
    public abstract class AbstractParser
    {
        public DefaultOptions CurrentOptions { get; protected set; }

        public Migrator Parse(string[] args, Migrator migrator)
        {
            CurrentOptions = Options.Factory.Create(args);

            migrator = InitMigrator(migrator);

            SetDefaultOptionsForMigrator(migrator);
            SetCustomOptionsForMigrator(migrator);

            migrator.Strategy = Strategy.Factory.Create(CurrentOptions.Strategy);
            

            return migrator;
        }

        public virtual Migrator InitMigrator(Migrator migrator)
        {
            throw new Exception("InitMigrator() must be overwritten!");
        }

        public virtual string GetAdditionalHeader()
        {
            return "None";
        }

        public Migrator SetDefaultOptionsForMigrator(Migrator migrator)
        {
            // if (options.ini)
            // ini_parser.file = new File(options.ini)
            // if (options.section)
            // ini_parser.use_section = options.section

            if (!string.IsNullOrEmpty(CurrentOptions.Suffix))
                migrator.Guard.Suffix = CurrentOptions.Suffix;

            if (!string.IsNullOrEmpty(CurrentOptions.Host))
                migrator.DbInterface.Executor.Host = CurrentOptions.Host;

            if (!string.IsNullOrEmpty(CurrentOptions.Password))
                migrator.DbInterface.Executor.Password = CurrentOptions.Password;
            
            if (!string.IsNullOrEmpty(CurrentOptions.Database))
                migrator.DbInterface.Executor.Database = CurrentOptions.Database;

            if (!string.IsNullOrEmpty(CurrentOptions.Command))
                migrator.DbInterface.Executor.Command = CurrentOptions.Command;
            
            if (!string.IsNullOrEmpty(CurrentOptions.Args))
                migrator.DbInterface.Executor.Args = CurrentOptions.Args;

            if (!string.IsNullOrEmpty(CurrentOptions.Username))
                migrator.DbInterface.Executor.Username = CurrentOptions.Username;

            if (CurrentOptions.Directories.Count > 0)
                migrator.Directories = CurrentOptions.Directories.Aggregate((x, y) => x + migrator.SeparatorPath + y);

            migrator.OnlySimulate = CurrentOptions.OnlySimulate;
            migrator.KeepTemporaryFile = CurrentOptions.KeepTemporaryFile;

            if (!string.IsNullOrEmpty(CurrentOptions.AppliedVersion))
            {
                migrator.ReferenceVersion = new Version(CurrentOptions.AppliedVersion);
            }

            return migrator;
        }

        public virtual Migrator SetCustomOptionsForMigrator(Migrator migrator)
        {
            return migrator;
        }
    }
}
using System;
using System.Collections.Generic;
using Appature.Common;
using NeosIT.DB_Migrator.DBMigration.Strategy;

namespace NeosIT.DB_Migrator.DBMigration
{
    public abstract class AbstractParser
    {
        protected Dictionary<string, string> Options;
        private bool _showHelp;

        public Migrator Parse(string[] args, Migrator migrator)
        {
            Options = new Dictionary<string, string>();
            var cli = new CommandParser();

            cli = CreateDefaultOpts(cli);
            cli = CreateCustomOpts(cli);

            cli.Parse(args);

            if (cli.UnknownCommands.Count > 0)
            {
                foreach (string unknown in cli.UnknownCommands)
                {
                    Console.WriteLine("Invalid command: " + unknown);
                }

                Console.WriteLine(cli.GetHelp());
                Environment.Exit(0);
            }
            else if (cli.MissingRequiredCommands.Count > 0)
            {
                foreach (string missing in cli.MissingRequiredCommands)
                {
                    Console.WriteLine("ERROR: Missing argument: " + missing);
                }

                Console.WriteLine(cli.GetHelp());
                Environment.Exit(0);
            }
            else if (_showHelp)
            {
                Console.WriteLine(cli.GetHelp());
                Environment.Exit(0);
            }

            migrator = InitMigrator(migrator);

            SetDefaultOptionsForMigrator(migrator);
            SetCustomOptionsForMigrator(migrator);

            string strategy = string.Empty;
            if (Options.ContainsKey("strategy"))
            {
                strategy = Options["strategy"];
            }

            migrator.Strategy = Factory.Create(strategy);

            return migrator;
        }

        private CommandParser CreateDefaultOpts(CommandParser cli)
        {
            cli.Argument("u", "username", "database username", "database-username",
                         CommandArgumentFlags.TakesParameter, (p, v) => Options.Add("username", v));

            cli.Argument("p", "password", "database password", "database-password",
                         CommandArgumentFlags.TakesParameter, (p, v) => Options.Add("password", v));

            cli.Argument("c", "command", "Path to osql if not in environment path", "command",
                         CommandArgumentFlags.TakesParameter, (p, v) => Options.Add("command", v));

            cli.Argument("h", "host", "Host", "database-host", CommandArgumentFlags.TakesParameter,
                         (p, v) => Options.Add("host", v));

            cli.Argument("d", "database", "database - can be left if default database for user is set", "database",
                         CommandArgumentFlags.TakesParameter, (p, v) => Options.Add("database", v));

            cli.Argument("a", "args", "Additional arguments which will added to osql command", "args",
                         CommandArgumentFlags.TakesParameter, (p, v) => Options.Add("args", v));

            cli.Argument("S", "suffix",
                         "Use only files with this suffix as migration scripts and ignore other resources (default: .sql)",
                         "migration-suffix",
                         CommandArgumentFlags.TakesParameter, (p, v) => Options.Add("suffix", v));

            cli.Argument("s", "strategy",
                         "can be \"flat\" or \"hierarchial\". flat means, that all scripts must be available inside this directory in form of yyyymmdd[-|_]<migration-number>-<name>.suffix. \"hierarchial\" means a directory structure in form of <major>\\<minor>\\<migration-number>[-|_]<name>.suffix",
                         "migration-strategy",
                         CommandArgumentFlags.TakesParameter, (p, v) => Options.Add("strategy", v));

            cli.Argument("i", "ini", "an .ini file from which to read all relevant settings",
                         "INI configuration file", CommandArgumentFlags.TakesParameter,
                         (p, v) => Options.Add("ini", v));

            cli.Argument("x", "section", "name of the section in which to look for", "INI section name",
                         CommandArgumentFlags.TakesParameter, (p, v) => Options.Add("section", v));

            cli.Argument("?", "help", "Help", "Help", CommandArgumentFlags.None, (p, v) => { _showHelp = true; });

            return cli;
        }

        public virtual Migrator InitMigrator(Migrator migrator)
        {
            throw new Exception("InitMigrator() must be overwritten!");
        }

        public virtual string GetAdditionalHeader()
        {
            return "None";
        }

        public virtual CommandParser CreateCustomOpts(CommandParser cli)
        {
            return cli;
        }

        public Migrator SetDefaultOptionsForMigrator(Migrator migrator)
        {
            // if (options.ini)
            // ini_parser.file = new File(options.ini)
            // if (options.section)
            // ini_parser.use_section = options.section

            if (Options.ContainsKey("suffix"))
            {
                migrator.Guard.Suffix = Options["suffix"];
            }

            if (Options.ContainsKey("host"))
            {
                migrator.DbInterface.Executor.Host = Options["host"];
            }

            if (Options.ContainsKey("password"))
            {
                migrator.DbInterface.Executor.Password = Options["password"];
            }

            if (Options.ContainsKey("database"))
            {
                migrator.DbInterface.Executor.Database = Options["database"];
            }

            if (Options.ContainsKey("command"))
            {
                migrator.DbInterface.Executor.Command = Options["command"];
            }

            if (Options.ContainsKey("args"))
            {
                migrator.DbInterface.Executor.Args = Options["args"];
            }

            if (Options.ContainsKey("username"))
            {
                migrator.DbInterface.Executor.Username = Options["username"];
            }

            return migrator;
        }

        public virtual Migrator SetCustomOptionsForMigrator(Migrator migrator)
        {
            return migrator;
        }
    }
}
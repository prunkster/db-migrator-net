using System;
using CommandLine;

namespace NeosIT.DB_Migrator.DBMigration.Options
{
    public class Factory
    {
        public static AbstractOptions Create(string[] args)
        {
            string target = string.Empty;
            int i = 0;

            foreach (string str in args)
            {
                if (str.StartsWith("-t"))
                {
                    if (str.Length > 2)
                    {
                        target = str.Substring(2);
                        if (target.StartsWith("=") || target.StartsWith(":"))
                        {
                            target = target.Substring(1);
                        }
                    }
                    else
                    {
                        if (args.Length > i)
                        {
                            target = args[i + 1];
                        }
                    }
                }


                if (str.StartsWith("--target"))
                {
                    target = str.Substring(9);
                }

                i++;
            }
            
            AbstractOptions options;
            CommandLineParser parser = new CommandLineParser();
            switch (target)
            {
                case "mssql":
                    options = new MSSQL.Options();
                    if (parser.ParseArguments(args, options))
                        return options;
                    throw new Exception("Could not parse arguments.");
                case "mysql":
                    options = new MySQL.Options();
                    if (parser.ParseArguments(args, options))
                        return options;
                    throw new Exception("Could not parse arguments.");
                case "postgresql":
                    options = new PostgreSQL.Options();
                    if (parser.ParseArguments(args, options))
                        return options;
                    throw new Exception("Could not parse arguments.");
                default:
                    throw new Exception(string.Format("Target {0} is not valid.", target));
            }
        }
    }
}

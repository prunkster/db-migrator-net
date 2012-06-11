using System;

namespace NeosIT.DB_Migrator.DBMigration.Parsers
{
    public class Factory
    {
        public static AbstractParser Create(string[] args)
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
            
            switch(target)
            {
                case "mssql":
                    return new MSSQL.Parser();
                case "mysql":
                    return new MySQL.Parser();
                case "postgresql":
                    return new PostgreSQL.Parser();
                default:
                    throw new Exception(string.Format("Target {0} is not valid.", target));
            }
        }
    }
}

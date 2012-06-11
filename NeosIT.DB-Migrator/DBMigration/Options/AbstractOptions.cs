using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace NeosIT.DB_Migrator.DBMigration.Options
{
    public class AbstractOptions
    {
        [Option("u", "username", HelpText = "Database username")]
        public virtual string Username { get; set; }

        [Option("p", "password", HelpText = "Database password")]
        public virtual string Password { get; set; }

        [Option("c", "command", HelpText = "Path to osql if not in environment path")]
        public virtual string Command { get; set; }

        [Option("h", "host", HelpText = "Host")]
        public virtual string Host { get; set; }

        [Option("d", "database", HelpText = "Database - can be left empty if default database for user is set")]
        public virtual string Database { get; set; }

        [Option("a", "args", HelpText = "Additional arguments which will be added to osql command")]
        public virtual string Args { get; set; }

        [Option("S", "suffix", HelpText = "Use only files with this suffix as migration scripts and ignore other resources (default: .sql)")]
        public virtual string Suffix { get; set; }

        [Option("s", "strategy", HelpText = "can be \"flat\" or \"hierarchial\". flat means, that all scripts must be available inside this directory in form of yyyymmdd[-|_]<migration-number>-<name>.suffix. \"hierarchial\" means a directory structure in form of <major>\\<minor>\\<migration-number>[-|_]<name>.suffix")]
        public virtual string Strategy { get; set; }

        [Option("i", "ini", HelpText = "An .ini file from which to read all relevant settings")]
        public virtual string IniFile { get; set; }

        [Option("x", "section", HelpText = "Name of the section in which to look for")]
        public virtual string IniSection { get; set; }

        [Option("t", "target", HelpText = "Target SQL system (can be MSSQL, MySQL or PostgreSQL", Required = true)]
        public string Target { get; set; }

        [ValueList(typeof(List<string>), MaximumElements = 1)]
        public IList<string> Directories { get; set; }

        [HelpOption]
        public string Help()
        {
            var help = new HelpText
            {
                Heading = new HeadingInfo("<>", "<>"),
                Copyright = new CopyrightInfo("<>", 2012),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true,
            };
            help.AddPreOptionsLine("<>");
            help.AddPreOptionsLine("Usage: app -pSomeone");
            help.AddOptions(this);
            return help;
        }
    }
}

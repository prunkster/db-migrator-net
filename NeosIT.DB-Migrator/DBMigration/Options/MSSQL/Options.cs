using CommandLine;

namespace NeosIT.DB_Migrator.DBMigration.Options.MSSQL
{
    public class Options : DefaultOptions
    {
        [Option("u", "username", HelpText = "A valid username for Microsoft SQL server. If none is set, a trusted connection (osql parameter -E) is used. If you specify a username, you need to add a password!")]
        public override string Username { get; set; }

        [Option("c", "command", HelpText = "Path to osql if not in environment path")]
        public override string Command { get; set; }

        [Option("d", "database", HelpText = "Database - can be left if your MSSQL user has a default database set")]
        public override string Database { get; set; }

        [Option("w", "winauth", HelpText = "Use Windows authentication - no user/password must be set")]
        public virtual bool WinAuth { get; set; }

        [Option("C", "config", HelpText = "Path to Web.config/App.config/*.config to read connection settings from. Using this overwrites every other database centric configuration (winauth, database, username)")]
        public virtual string XmlConfiguration { get; set; }

        [Option("X", "xpath", HelpText = "Use this in conjunction with 'config' to define where to look for the connection string. Default is '/configuration/connectionStrings/add[1]/@connectionString' (first connection string)", DefaultValue = "/configuration/connectionStrings/add[1]/@connectionString")]
        public virtual string XpathExpression { get; set; }

        [Option("N", "namespaces", HelpText = "Define namespaces with semicolon separated: 'ns1=http://domain/namespace;ns2=http://domain/namespace2'. Make sure you are referencing the nodes in your XPath expression with namespaces suffixes!")]
        public virtual string XmlNamespaces { get; set; }
    }
}

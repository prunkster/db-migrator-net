using System;
using System.Xml;
using System.Xml.XPath;
using NeosIT.DB_Migrator.DBMigration.Target;
using System.Data.SqlClient;

namespace NeosIT.DB_Migrator.DBMigration.Parsers.MSSQL
{
    public class Parser : AbstractParser
    {
        public override Migrator SetCustomOptionsForMigrator(Migrator migrator)
        {
            if (((Options.MSSQL.Options) CurrentOptions).WinAuth)
            {
                migrator.DbInterface.Executor.Username = string.Empty;
                migrator.DbInterface.Executor.Password = string.Empty;
            }

            // Load configuration
            if (!string.IsNullOrWhiteSpace(((Options.MSSQL.Options)CurrentOptions).XmlConfiguration))
            {
                String configurationPath = ((Options.MSSQL.Options)CurrentOptions).XmlConfiguration;
                String xpathExpression = ((Options.MSSQL.Options)CurrentOptions).XpathExpression.Trim('"').Trim('\'');

                var doc = new XPathDocument(configurationPath.Trim('"').Trim('\''));
                var nav = doc.CreateNavigator();
                var ns = new XmlNamespaceManager(nav.NameTable);

                // add namespaces if needed
                if (!string.IsNullOrWhiteSpace(((Options.MSSQL.Options)CurrentOptions).XmlNamespaces))
                {
                    var namespaceOption = ((Options.MSSQL.Options)CurrentOptions).XmlNamespaces.Trim('"').Trim('\'');
                    string[] namespaces = namespaceOption.Split(';');

                    foreach (string userDefinedNamespace in namespaces) {
                        string[] parts = userDefinedNamespace.Split('=');
                        ns.AddNamespace(parts[0], parts[1]);
                    }
                }

                ConnectionStringExtractor cse = new ConnectionStringExtractor(nav, ns);
                cse.LoadConnectionStringInto(xpathExpression, migrator.DbInterface.Executor);
            }

            if (!string.IsNullOrWhiteSpace(migrator.DbInterface.Executor.Username) &&
                string.IsNullOrWhiteSpace(migrator.DbInterface.Executor.Password))
            {
                throw new Exception("You must apply a password for your database username!");
            }


            return migrator;
        }

        public override Migrator InitMigrator(Migrator migrator)
        {
            return Target.Factory.Create("mssql", migrator);
        }
    }
}
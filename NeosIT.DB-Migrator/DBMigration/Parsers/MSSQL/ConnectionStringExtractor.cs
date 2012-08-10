using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Data.SqlClient;
using NeosIT.DB_Migrator.DBMigration.Target;

namespace NeosIT.DB_Migrator.DBMigration.Parsers.MSSQL
{
    public class ConnectionStringExtractor
    {
        private XPathNavigator navigator;
        private XmlNamespaceManager namespaceManager;

        public ConnectionStringExtractor(XPathNavigator navigator, XmlNamespaceManager namespaceManager)
        {
            this.navigator = navigator;
            this.namespaceManager = namespaceManager;
        }

        public SqlConnectionStringBuilder Extract(string xpathExpression)
        {
            XPathNodeIterator iterator = navigator.Select(xpathExpression, namespaceManager);
            iterator.MoveNext();
            string connectionString = iterator.Current.Value;

            return new SqlConnectionStringBuilder(connectionString);
        }

        public void LoadConnectionStringInto(string xpathExpression, IExecutor executor)
        {
            var r = Extract(xpathExpression);

            if (r.IntegratedSecurity)
            {
                executor.Username = string.Empty;
                executor.Password = string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(r.DataSource))
            {
                executor.Host = r.DataSource;
            }

            if (!string.IsNullOrWhiteSpace(r.UserID))
            {
                executor.Username = r.UserID;
            }

            if (!string.IsNullOrWhiteSpace(r.Password))
            {
                executor.Password = r.Password;
            }

            if (!string.IsNullOrWhiteSpace(r.InitialCatalog))
            {
                executor.Database = r.InitialCatalog;
            }
        }
    }
}

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

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception(String.Format("Could not get any connection string at XPath expression {0}. You must use single-quotes in your XPath arguments.", xpathExpression));
            }

            try
            {
                return new SqlConnectionStringBuilder(connectionString);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Failed to interpret connectionString '{0}'. This is not a valid connectionString format: {1}", connectionString, ex.Message));
            }
        }

        public void LoadConnectionStringInto(string xpathExpression, IExecutor executor)
        {
            var r = Extract(xpathExpression);

            if (r.IntegratedSecurity)
            {
                executor.Username = string.Empty;
                executor.Password = string.Empty;
            }

            if (!string.IsNullOrEmpty(r.DataSource))
            {
                executor.Host = r.DataSource;
            }

            if (!string.IsNullOrEmpty(r.UserID))
            {
                executor.Username = r.UserID;
            }

            if (!string.IsNullOrEmpty(r.Password))
            {
                executor.Password = r.Password;
            }

            if (!string.IsNullOrEmpty(r.InitialCatalog))
            {
                executor.Database = r.InitialCatalog;
            }
        }
    }
}

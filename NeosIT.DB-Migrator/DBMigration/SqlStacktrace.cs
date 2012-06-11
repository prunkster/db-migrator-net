using System.Collections.Generic;

namespace NeosIT.DB_Migrator.DBMigration
{
    public class SqlStacktrace
    {
        public IList<string> Lines { get; set; }
        public SqlFileInfo File { get; set; }
        public int BeginLine { get; set; }
    }
}
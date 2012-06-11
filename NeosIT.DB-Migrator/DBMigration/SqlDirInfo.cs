using System.Collections.Generic;
using System.IO;

namespace NeosIT.DB_Migrator.DBMigration
{
    public class SqlDirInfo
    {
        public DirectoryInfo DirectoryInfo { get; set; }
        public bool LatestOnly { get; set; }
        public bool SqlInsertMigration { get; set; }
        public IList<SqlFileInfo> Files { get; set; }
    }
}
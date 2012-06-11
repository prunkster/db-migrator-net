using System.Collections.Generic;
using System.IO;

namespace NeosIT.DB_Migrator.DBMigration.Strategy
{
    public class Hierarchial : IStrategy
    {
        #region IStrategy Members

        public Dictionary<Version, SqlFileInfo> FindUnappliedMigrationsSince(Version version, SqlDirInfo dir,
                                                                             Guard guard = null)
        {
            var r = new Dictionary<Version, SqlFileInfo>();

            foreach (DirectoryInfo majorDir in dir.DirectoryInfo.GetDirectories())
            {
            }

            return r;
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace NeosIT.DB_Migrator.DBMigration.Strategy
{
    public class Flat : IStrategy
    {
        private Log log = new Log();
        #region IStrategy Members

        public Dictionary<Version, SqlFileInfo> FindUnappliedMigrationsSince(Version version, SqlDirInfo dir,
                                                                             Guard guard = null)
        {
            var r = new Dictionary<Version, SqlFileInfo>();

            foreach (FileInfo file in dir.DirectoryInfo.GetFiles())
            {
                string name = file.Name;

                Version fileVersion = new Version(name);

                if (fileVersion.GetVersion() > 0)
                {
                    if (guard.IsMigrationAllowed(file, version, fileVersion))
                    {
                        r.Add(fileVersion, new SqlFileInfo {FileInfo = file, SqlInsertMigration = dir.SqlInsertMigration});
                        log.Debug(String.Format("{0} is a potential candidate for migration", name), "migration");
                    }
                }
            }

            return r;
        }

        #endregion
    }
}
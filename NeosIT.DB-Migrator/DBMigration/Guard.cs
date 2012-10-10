using System;
using System.IO;

namespace NeosIT.DB_Migrator.DBMigration
{
    public class Guard
    {
        private string _suffix = ".sql";

        public string Suffix
        {
            get { return _suffix; }
            set { _suffix = value; }
        }

        public bool IsMigrationAllowed(FileInfo file, Version currentVersion, Version fileVersion)
        {
            if (null == file || !file.Exists)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(_suffix))
            {
                if (!file.Extension.Equals(_suffix, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return fileVersion.IsHigherThan(currentVersion);
        }
    }
}
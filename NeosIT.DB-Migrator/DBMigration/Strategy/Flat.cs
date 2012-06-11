using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace NeosIT.DB_Migrator.DBMigration.Strategy
{
    public class Flat : IStrategy
    {
        #region IStrategy Members

        public Dictionary<Version, SqlFileInfo> FindUnappliedMigrationsSince(Version version, SqlDirInfo dir,
                                                                             Guard guard = null)
        {
            var r = new Dictionary<Version, SqlFileInfo>();

            foreach (FileInfo file in dir.DirectoryInfo.GetFiles())
            {
                string name = file.Name;

                Match matchMajor = Regex.Match(name, @"(\d*)([-|_])");
                Match matchMinor = Regex.Match(name, @"([-|_])(\d*)");

                if (matchMajor.Success && matchMinor.Success)
                {
                    string major = matchMajor.Groups[1].Value;
                    string minor = matchMinor.Groups[2].Value;

                    var fileVersion = new Version {Minor = minor, Major = major,};

                    if (guard.IsMigrationAllowed(file, version, fileVersion))
                    {
                        r.Add(fileVersion, new SqlFileInfo {FileInfo = file, SqlInsertMigration = dir.SqlInsertMigration});
                        Console.WriteLine("[migration] {0} is a potential candidate for migration", name);
                    }
                }
            }

            return r;
        }

        #endregion
    }
}
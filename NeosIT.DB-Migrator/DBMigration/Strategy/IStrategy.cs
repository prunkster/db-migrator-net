using System.Collections.Generic;

namespace NeosIT.DB_Migrator.DBMigration.Strategy
{
    public interface IStrategy
    {
        Dictionary<Version, SqlFileInfo> FindUnappliedMigrationsSince(Version version, SqlDirInfo dir,
                                                                      Guard guard = null);
    }
}
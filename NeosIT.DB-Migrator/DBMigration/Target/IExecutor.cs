using System.Collections.Generic;

namespace NeosIT.DB_Migrator.DBMigration.Target
{
    public interface IExecutor
    {
        string Host { get; set; }
        string Database { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        string Command { get; set; }
        string Args { get; set; }
        string Exec(string cmdArgs);
        string ExecCommand(string command);
        IList<string> BuildExecCommand(string cmd, bool verbose = false);
        string ExecFile(string path, bool verbose = false);
    }
}
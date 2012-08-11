using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using NeosIT.DB_Migrator.DBMigration.Strategy;
using NeosIT.DB_Migrator.DBMigration.Target;

namespace NeosIT.DB_Migrator.DBMigration
{
    public class Migrator
    {
        private static string _separatorPath = ";";
        private static string _separatorOpts = ",";
        private string _directories = "." + _separatorOpts + "all" + _separatorOpts + "false";
        public Guard Guard { get; set; }
        public IDbInterface DbInterface { get; set; }
        public IStrategy Strategy { get; set; }
        public Applier Applier { get; set; }

        public string SeparatorPath
        {
            get { return _separatorPath; }
            set { _separatorPath = value; }
        }

        public string SeparatorOpts
        {
            get { return _separatorOpts; }
            set { _separatorOpts = value; }
        }

        public string Directories
        {
            get { return _directories; }
            set { _directories = value; }
        }

        public void Run()
        {
            Console.WriteLine("[migration] working dir: {0}", Environment.CurrentDirectory);

            IList<string> dirs = _directories.Split(new[] {_separatorPath,}, StringSplitOptions.None);
            Version version = DbInterface.FindLatestMigration();
            Version useVersion = version;

            IList<SqlDirInfo> stack = new List<SqlDirInfo>();

            foreach (string pathdef in dirs)
            {
                stack.Add(CreateDirElement(pathdef));
            }

            Dictionary<Version, SqlFileInfo> mergedMigrations = MergeMigrationsFromDirectories(stack, version);

            if (0 == mergedMigrations.Count)
            {
                Console.WriteLine("[migration] no migrations available - project is up-to-date :-)");
                return;
            }

            Applier.Begin();
            Applier.Prepare(mergedMigrations);

            try
            {
                Applier.Commit();
                Applier.Cleanup();
                Console.WriteLine("[migration] {0} migrations applied. Project is now up-to-date :-)",
                                  Applier.TotalMigrations);
            }
            catch (Exception e)
            {
                Console.WriteLine("[error] Migration failed: {0}", e.Message);

                if (DbInterface.Executor.HasMethod("GetLinenumberOfError"))
                {
                    MethodInfo method = DbInterface.Executor.GetMethod("GetLinenumberOfError");
                    var errorInLine = (int) method.Invoke(this, new object[] {e.Message,});

                    SqlStacktrace stacktrace = GetSqlStacktrace(File.ReadAllLines(Applier.Filename), (errorInLine - 1),
                                                                5, 2);

                    Console.WriteLine("[debug] error occured somewhere in file: {0}", stacktrace.File.FileInfo.FullName);
                    Console.WriteLine("... lines of aggregated SQL script ...");

                    int curLine = stacktrace.BeginLine;

                    foreach (string item in stacktrace.Lines)
                    {
                        if (curLine == errorInLine)
                        {
                        }

                        Console.WriteLine("\t{0}\t{1}", curLine, item);
                    }

                    Console.WriteLine();
                }

                Console.WriteLine("[error] SQL-script has not been deleted for debugging purposes ({0})",
                                  Applier.Filename);
                Environment.ExitCode = 1;
            }
        }

        public SqlStacktrace GetSqlStacktrace(IList<string> lines, int idx, int linesBefore, int linesAfter)
        {
            IList<string> output = new List<string>();
            int endLine = ((idx + linesAfter) > (lines.Count - 1)) ? (lines.Count - 1) : (idx + linesAfter);
            int beginLine = ((endLine - linesBefore) < 0) ? 0 : (endLine - linesBefore);
            string referenceFile = string.Empty;
            string sep = Environment.NewLine;

            while (endLine >= 0)
            {
                string currentLine = lines[endLine];

                if (endLine >= beginLine)
                {
                    output.Insert(0, currentLine);
                }

                Match matcher = Regex.Match(currentLine, "db-migrator:FILE:(.*)");
                if (matcher.Success)
                {
                    referenceFile = matcher.Groups[1].Value;
                }

                if ((endLine <= beginLine) && matcher.Success)
                {
                    break;
                }

                endLine--;
            }

            return new SqlStacktrace
                       {
                           BeginLine = beginLine,
                           File =
                               string.IsNullOrWhiteSpace(referenceFile)
                                   ? null
                                   : new SqlFileInfo {FileInfo = new FileInfo(referenceFile),},
                           Lines = output,
                       };
        }

        public Dictionary<Version, SqlFileInfo> MergeMigrationsFromDirectories(IList<SqlDirInfo> directories,
                                                                               Version version)
        {
            var r = new Dictionary<Version, SqlFileInfo>();

            foreach (SqlDirInfo sqlDirInfo in directories)
            {
                Console.WriteLine("[merging] Searching directory '{0}'", sqlDirInfo.DirectoryInfo.FullName);

                // if latest file should be used, the latest migration inside the database is not relevant
                Version useVersion = sqlDirInfo.LatestOnly ? new Version() : version;

                Dictionary<Version, SqlFileInfo> candidates =
                    Strategy.FindUnappliedMigrationsSince(useVersion, sqlDirInfo, Guard).OrderBy(
                        x => x.Key.GetVersion()).ToDictionary(x => x.Key, x => x.Value);

                var versions = new Dictionary<Version, SqlFileInfo>();

                if (sqlDirInfo.LatestOnly)
                {
                    Console.WriteLine("[migration] only the latest migration will be applied");
                    //def latest_version = ks.pop()

                    //versions[latest_version] = candidates[latest_version]
                }
                else
                {
                    versions = candidates;
                }

                foreach (var kvp in versions)
                {
                    if (r.ContainsKey(kvp.Key))
                    {
                        Console.WriteLine(
                            "[merging] You have a duplicate in both directories: {0} <-> {1}. First one will be used.",
                            r[kvp.Key].FileInfo.FullName, kvp.Value.FileInfo.FullName);
                    }
                    else
                    {
                        r.Add(kvp.Key, kvp.Value);
                    }
                }
            }

            return r;
        }


        public SqlDirInfo CreateDirElement(string pathdef)
        {
            string[] opts = pathdef.Split(new[] {_separatorOpts,}, StringSplitOptions.None);
            DirectoryInfo dir = LocateMigrationDir(opts[0]);

            bool latestOnly = false;
            bool sqlInsertMigration = false;

            if (opts.Length >= 2)
            {
                if ("latest" == opts[1])
                {
                    latestOnly = true;
                }

                if (opts.Length >= 3)
                {
                    string[] array = {"1", "true", "enabled", "on", "yes", "y"};
                    if (array.Contains(opts[2]))
                    {
                        sqlInsertMigration = true;
                    }
                }
            }

            return new SqlDirInfo
                       {
                           DirectoryInfo = dir,
                           Files = null,
                           LatestOnly = latestOnly,
                           SqlInsertMigration = sqlInsertMigration,
                       };
        }

        public DirectoryInfo LocateMigrationDir(string dir)
        {
            if (!Directory.Exists(dir))
            {
                throw new Exception(string.Format("The path {0} is not valid", dir));
            }

            return new DirectoryInfo(dir);
        }
    }
}
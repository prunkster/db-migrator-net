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
        private Log log = new Log();
        private static string _separatorPath = ";";
        private static string _separatorOpts = ",";
        private string _directories = "." + _separatorOpts + "all" + _separatorOpts + "false";
        public Guard Guard { get; set; }
        public IDbInterface DbInterface { get; set; }
        public IStrategy Strategy { get; set; }
        public Applier Applier { get; set; }
        public bool OnlySimulate { get; set; }
        public bool KeepTemporaryFile { get; set; }
        public Version ReferenceVersion { get; set; }

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
            if (OnlySimulate)
            {
                log.Warn("Only simulating the migration process. No migration will be applied!", "migration");
            }

            log.Debug(String.Format("current working dir: {0}", Environment.CurrentDirectory), "migration");

            IList<string> dirs = _directories.Split(new[] {_separatorPath,}, StringSplitOptions.None);

            if (ReferenceVersion == null)
            {
                ReferenceVersion = DbInterface.FindLatestMigration();
            }

            log.Info(String.Format("current installed migration: {0}", ReferenceVersion.ToString()), "migration");

            Version useVersion = ReferenceVersion;

            IList<SqlDirInfo> stack = new List<SqlDirInfo>();

            foreach (string pathdef in dirs)
            {
                stack.Add(CreateDirElement(pathdef));
            }

            Dictionary<Version, SqlFileInfo> mergedMigrations = MergeMigrationsFromDirectories(stack, ReferenceVersion);

            if (0 == mergedMigrations.Count)
            {
                log.Success(String.Format("no migrations available - project is up-to-date :-)"), "migration");
                return;
            }

            Applier.Begin();
            Applier.Prepare(mergedMigrations);

            bool removeFile = !KeepTemporaryFile;

            try
            {
                Applier.Commit();

                if (OnlySimulate)
                {
                    log.Success(String.Format("{0} migrations would be applied - if not running a simulation", Applier.TotalMigrations), "migration");
                }
                else
                {
                    Console.WriteLine(DbInterface.Executor.ExecFile(Applier.Filename));
                    log.Success(String.Format("{0} migrations applied. Project is now up-to-date :-)",
                                      Applier.TotalMigrations), "migration");
                }
            }
            catch (Exception e)
            {
                log.Error(String.Format("Migration failed: {0}", e.Message), "migration");

                if (DbInterface.Executor.HasMethod("GetLinenumberOfError"))
                {
                    MethodInfo method = DbInterface.Executor.GetMethod("GetLinenumberOfError");
                    var errorInLine = (int) method.Invoke(this, new object[] {e.Message,});

                    SqlStacktrace stacktrace = GetSqlStacktrace(File.ReadAllLines(Applier.Filename), (errorInLine - 1),
                                                                5, 2);

                    log.Debug(String.Format("error occured somewhere in file: {0}", stacktrace.File.FileInfo.FullName));
                    log.Debug("... lines of aggregated SQL script ...");

                    int curLine = stacktrace.BeginLine;

                    foreach (string item in stacktrace.Lines)
                    {
                        log.Debug(String.Format("\t{0}\t{1}", curLine, item));
                    }

                    log.Debug("");
                }

                removeFile = false;

                Environment.ExitCode = 1;
            }

            if (removeFile)
            {
                Applier.Cleanup();
            }
            else
            {
                log.Error(String.Format("SQL-script has not been deleted for debugging purposes ({0})",
                                  Applier.Filename));
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
                               string.IsNullOrEmpty(referenceFile)
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
                log.Info(String.Format("Searching directory '{0}'", sqlDirInfo.DirectoryInfo.FullName), "merging");

                // if latest file should be used, the latest migration inside the database is not relevant
                Version useVersion = sqlDirInfo.LatestOnly ? new Version() : version;

                Dictionary<Version, SqlFileInfo> candidates =
                    Strategy.FindUnappliedMigrationsSince(useVersion, sqlDirInfo, Guard).OrderBy(
                        x => x.Key.GetVersion()).ToDictionary(x => x.Key, x => x.Value);

                var versions = new Dictionary<Version, SqlFileInfo>();

                if (sqlDirInfo.LatestOnly)
                {
                    log.Info("only the latest migration will be applied", "merging");
                    Version latestVersionInStack = candidates.Keys.Last();
                    versions.Add(latestVersionInStack, candidates[latestVersionInStack]);
                }
                else
                {
                    versions = candidates;
                }

                foreach (var kvp in versions)
                {
                    if (r.ContainsKey(kvp.Key))
                    {
                        log.Warn(String.Format("You have a duplicate in both directories: {0} <-> {1}. First one will be used.",
                            r[kvp.Key].FileInfo.FullName, kvp.Value.FileInfo.FullName), "merging");
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
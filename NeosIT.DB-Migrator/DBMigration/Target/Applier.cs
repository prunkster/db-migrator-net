using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NeosIT.DB_Migrator.DBMigration.Target
{
    public abstract class Applier
    {
        protected Log log = new Log();

        protected StreamWriter Sw;
        public string Filename { get; protected set; }
        public int TotalMigrations { get; protected set; }
        public IDbInterface DbInterface { get; set; }

        public void Begin()
        {
            try
            {
                Filename = new FileInfo("migration_" + Path.GetRandomFileName() + ".sql").FullName;
                //Filename = "migration_" + Path.GetRandomFileName() + ".sql";

                Sw = File.CreateText(Filename);
                TotalMigrations = 0;
                log.Info(String.Format("output will be written to {0}", Filename), "migration");

                Sw.WriteLine("-- migration file was created at {0}", DateTime.Now);
                Sw.WriteLine(
                    "-- all migrations are concated to one big transaction so that a consistent state will be reached after finishing the migration");
                AppendBeginTransaction();
            }
            catch (Exception e)
            {
                log.Error(String.Format("Sorry, but a temporary file could not be created: {0}", e.Message));
            }
        }

        public bool Prepare(Dictionary<Version, SqlFileInfo> unappliedMigrations)
        {
            int size = unappliedMigrations.Count;
            unappliedMigrations = unappliedMigrations.OrderBy(x => x.Key.GetVersion()).ToDictionary(x => x.Key,
                                                                                                    x => x.Value);

            foreach (Version key in unappliedMigrations.Keys)
            {
                ++TotalMigrations;
                SqlFileInfo file = unappliedMigrations[key];

                log.Info(String.Format("{0} / {1} {2} scheduled for applying", TotalMigrations, size,
                                  file.FileInfo.Name), "migration");

                string[] content = File.ReadAllLines(file.FileInfo.FullName);
                Sw.WriteLine("-- db-migrator:FILE: {0}", file.FileInfo.Name);

                foreach (string line in content)
                {
                    Sw.WriteLine(line);
                }

                if (file.SqlInsertMigration)
                {
                    Sw.WriteLine("INSERT INTO migrations (major, minor, filename) VALUES('{0}', '{1}', '{2}');",
                                 key.Major, key.Minor, file.FileInfo.Name);
                }
            }

            return true;
        }

        public void Commit()
        {
            AppendCommitTransaction();

            Sw.Dispose();

            Console.WriteLine(DbInterface.Executor.ExecFile(Filename));
        }

        public void Cleanup()
        {
            if (File.Exists(Filename))
            {
                File.Delete(Filename);
            }

            log.Info("Temporary file containing all statements deleted", "cleanup");
        }

        public void AppendBeginTransaction()
        {
        }

        public void AppendCommitTransaction()
        {
        }
    }
}
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

        /// <summary>
        /// Initialies the output file
        /// </summary>
        public virtual void Begin()
        {
            try
            {
                Filename = new FileInfo("migration_" + Path.GetRandomFileName() + ".sql").FullName;

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
        
        /// <summary>
        /// Iterates over every unapplied migration and writes them to the target file
        /// </summary>
        /// <param name="unappliedMigrations"></param>
        /// <returns></returns>
        public virtual bool Prepare(Dictionary<Version, SqlFileInfo> unappliedMigrations)
        {
            int size = unappliedMigrations.Count;
            unappliedMigrations = unappliedMigrations.OrderBy(x => x.Key.GetVersion()).ToDictionary(x => x.Key,
                                                                                                    x => x.Value);

            foreach (Version version in unappliedMigrations.Keys)
            {
                ++TotalMigrations;
                SqlFileInfo file = unappliedMigrations[version];

                log.Info(String.Format("{0} / {1} {2} scheduled for applying", TotalMigrations, size,
                                  file.FileInfo.Name), "migration");

                string[] content = File.ReadAllLines(file.FileInfo.FullName);
                BeforeMigrationFile(version, file);

                foreach (string line in content)
                {
                    Sw.WriteLine(line);
                }

                AfterMigrationFile(version, file);

            }

            return true;
        }

        /// <summary>
        /// Is executed before the content of the migration file is copied to output file
        /// </summary>
        /// <param name="version"></param>
        /// <param name="file"></param>
        public virtual void BeforeMigrationFile(Version version, SqlFileInfo file)
        {
            Sw.WriteLine("-- db-migrator:FILE: {0}", file.FileInfo.Name);
        }

        /// <summary>
        /// Is executed after the content of the migration file has been copied to output file
        /// </summary>
        /// <param name="version"></param>
        /// <param name="file"></param>
        public virtual void AfterMigrationFile(Version version, SqlFileInfo file)
        {
            if (file.SqlInsertMigration)
            {
                Sw.WriteLine("INSERT INTO migrations (major, minor, filename) VALUES('{0}', '{1}', '{2}');",
                             version.Major, version.Minor, file.FileInfo.Name);
            }
        }

        /// <summary>
        /// Is executed after copying every migration file to output file
        /// </summary>
        public virtual void Commit()
        {
            AppendCommitTransaction();

            Sw.Dispose();
        }

        /// <summary>
        /// Removes output file
        /// </summary>
        public virtual void Cleanup()
        {
            if (File.Exists(Filename))
            {
                File.Delete(Filename);
            }

            log.Info("Temporary file containing all statements deleted", "cleanup");
        }

        public virtual void AppendBeginTransaction()
        {
        }

        public virtual void AppendCommitTransaction()
        {
        }
    }
}
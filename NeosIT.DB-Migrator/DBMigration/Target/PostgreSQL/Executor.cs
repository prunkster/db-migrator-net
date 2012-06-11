using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NeosIT.DB_Migrator.DBMigration.Target.PostgreSQL
{
    public class Executor : IExecutor
    {
        private string _args = "";
        private string _command = "psql";
        private string _database = "";
        private string _host = "localhost";
        private string _password = "";
        private string _username = "postgres";

        #region IExecutor Members

        public string Command
        {
            get { return _command; }
            set { _command = value; }
        }

        public string Host
        {
            get { return _host; }
            set { _host = value; }
        }

        public string Database
        {
            get { return _database; }
            set { _database = value; }
        }

        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        public string Args
        {
            get { return _args; }
            set { _args = value; }
        }

        public string Exec(string cmdArgs)
        {
            Console.WriteLine("[command] executing {0} {1}", _command, cmdArgs);

            var proc = new Process();
            proc.StartInfo = new ProcessStartInfo(Command);
            proc.StartInfo.Arguments = cmdArgs;
            proc.StartInfo.EnvironmentVariables.Add("PGPASSWORD", Password);

            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardError = true;
            //proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.RedirectStandardOutput = true;

            proc.Start();

            string text = proc.StandardOutput.ReadToEnd();
            string err = proc.StandardError.ReadToEnd();

            proc.WaitForExit(15);

            if (0 != proc.ExitCode)
            {
                throw !string.IsNullOrWhiteSpace(err)
                          ? new Exception(err)
                          : new Exception(
                                "Command did not exit normal but although did not return any error text. Is the executed command correct? Normal text stream follows:\n" +
                                text);
            }

            return text;
        }

        public IList<string> BuildExecCommand(string cmd, bool verbose = false)
        {
            IList<string> r = BuildExecCommandDefault(verbose);
            r.Add("--command=\"" + cmd + "\"");
            return r;
        }

        public string ExecFile(string path, bool verbose = false)
        {
            IList<string> r = BuildExecCommandDefault(verbose);
            r.Add("--file=" + path);

            return Exec(r.Aggregate((x, y) => x + " " + y));
        }

        public string ExecCommand(string command)
        {
            return Exec(BuildExecCommand(command).Aggregate((x, y) => x + " " + y));
        }

        #endregion

        public IList<string> BuildExecCommandDefault(bool verbose = false)
        {
            IList<string> r = new List<string>();
            r.Add("-F;");

            if (!string.IsNullOrWhiteSpace(Host))
            {
                r.Add("--host=" + Host);
            }

            if (!string.IsNullOrWhiteSpace(Username))
            {
                r.Add("--username=" + Username);
            }

            if (!string.IsNullOrWhiteSpace(Password))
            {
                r.Add("-w");
            }

            if (!string.IsNullOrWhiteSpace(Args))
            {
                r.Add(Args);
            }

            if (!string.IsNullOrWhiteSpace(Database))
            {
                r.Add("--dbname=" + Database);
            }

            if (!string.IsNullOrWhiteSpace(Args))
            {
                r.Add(Args);
            }

            //r.Add("--no-align");
            //r.Add("-t");

            return r;
        }
    }
}
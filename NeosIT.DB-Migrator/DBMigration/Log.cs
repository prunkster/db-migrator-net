using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeosIT.DB_Migrator.DBMigration
{
    /// <summary>
    /// a *really* simple logging class
    /// </summary>
    public class Log
    {
        public void Debug(string _s, params string[] components)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Write(_s, components);
        }

        public void Warn(string _s, params string[] components)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Write(_s, components);
        }

        public void Error(string _s, params string[] components)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Write(_s, components);
        }

        public void Info(string _s, params string[] components)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Write(_s, components);
        }

        public void Success(string _s, params string[] components)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Write(_s, components);
        }

        public void Write(string _s, params string[] components)
        {
            /// TODO component
            Console.WriteLine(_s);
        }
    }
}

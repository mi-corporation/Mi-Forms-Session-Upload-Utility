using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiCo.MiForms.CC
{
    class Logger
    {
        private string path;

        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        /// <summary>
        /// Initializes a Logger instance
        /// </summary>
        /// <param name="path">The path for the log file.</param>
        public Logger(string path)
        {
            this.path = path;
        }

        /// <summary>
        /// Adds a string to the log file.
        /// </summary>
        /// <param name="text">The string to write.</param>
        public void add(string text)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(path, true);
            file.WriteLine("[" + DateTime.Now.ToString() + "] " + text);
            file.Close();
        }

        /// <summary>
        /// Adds a string to the log file and writes it to the console.
        /// </summary>
        /// <param name="text">The string to write.</param>
        public void echo(string text)
        {
            Console.WriteLine(text);
            System.IO.StreamWriter file = new System.IO.StreamWriter(path, true);
            file.WriteLine("[" + DateTime.Now.ToString() + "] " + text);
            file.Close();
        }
    }
}

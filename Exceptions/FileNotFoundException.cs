using System;
using System.Collections.Generic;
using System.Text;

namespace Stacker
{
    class FileNotFoundException : Exception
    {
        string message { get; }
        public FileNotFoundException(string file) : base()
        {
            message = ($"File {file} not found.");
        }

        public override string ToString()
        {
            return message;
        }
    }
}

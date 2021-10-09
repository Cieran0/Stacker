using System;
using static Stacker.Program;

namespace Stacker
{
    class UnkownStringException : Exception
    {
        string message { get; }
        public UnkownStringException(string s) : base()
        {
            message = ($"Unknown string \"{s}\".");
        }

        public override string ToString()
        {
            return message;
        }

    }
}
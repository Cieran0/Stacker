using System;
using static Stacker.Program;

namespace Stacker
{
    class InvalidCharacterException : Exception
    {
        string message { get; }
        public InvalidCharacterException(char expected, char given, string statement) : base()
        {
            message = ($"Invalid character \'{given}\' after {statement}. Expected \'{expected}\'");
        }

        public override string ToString()
        {
            return message;
        }

    }
}

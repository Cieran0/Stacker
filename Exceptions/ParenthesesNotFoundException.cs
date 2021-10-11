using System;
using static Stacker.Program;

namespace Stacker
{
    class ParenthesesNotFoundException : Exception
    {
        string message { get; }
        public ParenthesesNotFoundException(char parenthesesType, string statement) : base()
        {
            message = ($"Parenthese \'{parenthesesType}\' not found for {statement}.");
        }

        public override string ToString()
        {
            return message;
        }
    }
}

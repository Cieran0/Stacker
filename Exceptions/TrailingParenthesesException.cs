using System;
using static Stacker.Program;

namespace Stacker
{
    class TrailingParenthesesException : Exception
    {
        string message { get; }
        public TrailingParenthesesException(char parenthesesType, COMMANDS statement) : base()
        {
            message = ($"Trailing parentheses \'{parenthesesType}\' in {statement} statement.");
        }

        public override string ToString()
        {
            return message;
        }

    }
}

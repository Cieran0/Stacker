using System;
using System.Collections.Generic;
using System.Text;

namespace Stacker
{
    class StackException : Exception
    {
        string message { get; }
        public StackException(bool overflow) : base()
        {
            if (overflow) message = ($"Stack overflow.");
            else message = ($"Stack underflow.");
        }

        public override string ToString()
        {
            return message;
        }
    }
}

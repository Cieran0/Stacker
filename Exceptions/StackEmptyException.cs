using System;
using System.Collections.Generic;
using System.Text;

namespace Stacker
{
    class StackEmptyException : Exception
    {
        string message { get; }
        public StackEmptyException() : base()
        {
            message = ($"Stack is empty.");
        }

        public override string ToString()
        {
            return message;
        }
    }
}

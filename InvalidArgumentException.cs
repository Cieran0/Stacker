using System;
using System.Collections.Generic;
using System.Text;

namespace Stacker
{
    class InvalidArgumentException : Exception
    {
        string message { get; }
        public InvalidArgumentException(object argument, string commandOrBlock) : base()
        {
            message = ($"{argument} is an invalid argument for {commandOrBlock}");
        }

        public override string ToString()
        {
            return message;
        }
    
    }
}

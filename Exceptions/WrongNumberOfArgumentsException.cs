using System;
using System.Collections.Generic;
using System.Text;

namespace Stacker
{
    class WrongNumberOfArgumentsException : Exception
    {
        string message { get; }
        public WrongNumberOfArgumentsException(int lowerRange, int upperRange, int numberGot, string commandOrBlock) : base() 
        {
            string temps = $"between {lowerRange} and {upperRange}";
            if (lowerRange == upperRange) { temps = $"{lowerRange}"; }
            message = ($"Wrong number of arguments given for {commandOrBlock}. Expected {temps} but was given {numberGot}");
        }

        public override string ToString() 
        {
            return message;
        }

    }
}

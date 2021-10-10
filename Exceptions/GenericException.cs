using System;
using System.Collections.Generic;
using System.Text;

namespace Stacker
{
    class GenericException : Exception
    {
        private string message { get; }
        public GenericException(string message) : base() 
        {
            this.message = message;
        }

        public override string ToString()
        {
            return message;
        }
    }
}

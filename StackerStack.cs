using System;
using System.Collections.Generic;
using System.Text;

namespace Stacker
{
    class StackerStack<T> : Stack<T>
    {
        public StackerStack() : base() { }
        public new T Pop() 
        {
            if (base.Count <= 0) { throw new StackEmptyException(); }
            return base.Pop();
        }  

    }
}

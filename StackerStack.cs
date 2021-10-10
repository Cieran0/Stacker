using System;
using System.Collections.Generic;
using System.Text;

namespace Stacker
{
    class StackerStack<T> : Stack<T>
    {
        public StackerStack() : base() { }

        private const short MAX_STACK_SIZE = short.MaxValue;  

        public new T Pop() 
        {
            if (base.Count <= 0) { throw new StackException(false); }
            return base.Pop();
        }

        public new void Push(T item) 
        {
            if (this.Count == MAX_STACK_SIZE) throw new StackException(true);
            base.Push(item);
        }

    }
}

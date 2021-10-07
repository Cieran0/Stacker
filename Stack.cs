using System;

namespace Stacker
{
    class Stack<T>
    {
        public const short MAX_STACK_SIZE = short.MaxValue;
        private T[] STACKARRAY = new T[MAX_STACK_SIZE];
        public ushort Length = 0;

        public Stack() { }

        public void Push(T value) 
        {
            Array.Copy(STACKARRAY, 0, STACKARRAY, 1, MAX_STACK_SIZE - 1);
            STACKARRAY[0] = value;
            Length++;
        }

        public void Push(T[] values) 
        {
            for (int i = 0; i < values.Length; i++) 
            {
                Array.Copy(STACKARRAY, 0, STACKARRAY, 1, MAX_STACK_SIZE - 1);
                STACKARRAY[0] = values[i];
                Length++;
            }
        }

        public T Pop() 
        {
            T temp = STACKARRAY[0];
            Array.Copy(STACKARRAY, 1, STACKARRAY, 0, MAX_STACK_SIZE - 1);
            Length--;
            return temp;
        }

    }

}

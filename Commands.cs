using System;
using static Stacker.Program;

namespace Stacker
{
    class Commands
    {
        public static void PUSH(string[] args)
        {
            CheckArgs(2,2, args);
            switch (args[0])
            {
                case "%i":
                    PushByteArray(ShortToBytes(short.Parse(args[1])));
                    break;
                case "%c":
                    stack.Push((byte)args[1][0]);
                    break;
                case "%s":
                    pushString(args[1]);
                    break;
                default:
                    throw argumentException;
            }
        }

        public static void MATHS(string[] args) 
        {
            int[] numbs = new int[2];
            int finalNum = 0;

            CheckArgs(1,1, args);
            for(int i =0; i < 2; i++) numbs[i] = BytesToShort(new byte[] { stack.Pop(), stack.Pop() });

            switch (args[0]) 
            {
                case "add":
                    finalNum = numbs[0] + numbs[1];
                    break;
                case "min":
                    finalNum = numbs[1] - numbs[0];
                    break;
                case "mul":
                    finalNum = numbs[1] * numbs[0];
                    break;
                case "div":
                    finalNum = numbs[1] / numbs[0];
                    break;
                case "pow":
                    finalNum = (int)Math.Pow(numbs[1], numbs[0]);
                    break;
            }

            PushByteArray(ShortToBytes((short)finalNum));

        }

        private static void CheckArgs(int min, int max, string[] args) { if (args.Length > max || args.Length < min) { throw argumentException; } }

        public static void DUP(string[] args) 
        { 
            CheckArgs(0,1, args);
            if (args.Length == 0) 
            {
                byte x = stack.Pop();
                stack.Push(x); stack.Push(x); 
            }
            else
            {
                byte[] x = new byte[int.Parse(args[0])];
                for (int i = x.Length - 1; i >= 0; i--)
                {
                    x[i] = stack.Pop();

                }
                PushByteArray(x);
                PushByteArray(x);
            }
        }

        public static void POP(string[] args) { CheckArgs(0,0, args); stack.Pop(); }

        public static void PRINT(string[] args) 
        {
            CheckArgs(1,1, args);
            string WhatToPrint = "";
            switch (args[0]) 
            {
                case "%i":
                    WhatToPrint = BytesToShort(new byte[] { stack.Pop(), stack.Pop() }).ToString();
                    break;
                case "%c":
                    WhatToPrint = ((char)stack.Pop()).ToString();
                    break;
                case "%s":
                    WhatToPrint = getString();
                    break;
                default:
                    throw argumentException;
            }
            Console.Write(WhatToPrint);
        
        }

        public static void LOOP(string[] args, Token[] tokens) 
        {
            CheckArgs(1,1, args);
            int size = int.Parse(args[0]);
            if (size < 0) { throw argumentException; }
            if (size == 0) while (true) Interpret(tokens);
            else for (int i = 0; i < size; i++) Interpret(tokens);
        }

        private static void pushString(string s) 
        {
            s = s.Replace(@"\n", "\n");
            stack.Push(0);
            for (int i = s.Length - 1; i >= 0; i--) { stack.Push((byte)s[i]); }
        }

        private static string getString()
        {
            byte temp;
            string s = "";
            int len = stack.Count;
            for (int i = 0; i < len; i++) 
            {
                temp = stack.Pop();
                if (temp != 0) { s += (char)temp; }
                else { break; }
            }
            return s;
        }

        private static short BytesToShort(byte[] eight) 
        {
            return (short)(eight[0] << 8 | eight[1]);
        }

        private static byte[] ShortToBytes(short sixteen)
        {
            return new byte[] { (byte)(sixteen >> 0), (byte)(sixteen >> 8) };
        }

        private static void PushByteArray(byte[] bytes) { foreach (byte b in bytes) stack.Push(b); }
    }
}

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
                case "%b":
                    stack.Push(byte.Parse(args[1]));
                    break;
                case "%i":
                    PushByteArray(ShortToBytes(short.Parse(args[1])));
                    break;
                case "%c":
                    stack.Push((byte)args[1][0]);
                    break;
                case "%s":
                    PushString(args[1]);
                    break;
                default:
                    throw argumentException;
            }
        }

        public static void MATHS(string[] args) 
        {
            int[] numbs = new int[2];
            int finalNum = 0;
            int loc = 0;
            bool eightBit = false;

            CheckArgs(1,2, args);

            if (args.Length == 2)
            {
                loc = 1;
                switch (args[0])
                {
                    case "%b":
                    case "%c":
                        numbs = new int[] { stack.Pop(), stack.Pop() };
                        eightBit = true;
                        break;
                    case "%i":
                        numbs = new int[] { PopShort(), PopShort() };
                        break;
                }
            }
            else { numbs = new int[] { PopShort(),PopShort() }; }

            switch (args[loc]) 
            {
                case "add":
                    finalNum = numbs[1] + numbs[0];
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

            if (!eightBit) {
                PushByteArray(ShortToBytes((short)finalNum));
            }
            else 
            {
                stack.Push((byte)finalNum);
            }

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
                case "%b":
                    WhatToPrint = stack.Pop().ToString();
                    break;
                case "%i":
                    WhatToPrint = PopShort().ToString();
                    break;
                case "%c":
                    WhatToPrint = ((char)stack.Pop()).ToString();
                    break;
                case "%s":
                    WhatToPrint = GetString();
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

        public static void MEM(string[] args)
        {
            CheckArgs(1, 2, args);
            string op = args[0];
            int length = 1;
            if (args.Length > 1) length = int.Parse(args[1]);

            switch (op)
            {
                case "get":
                    GetMem(length);
                    break;
                case "set":
                    SetMem(length);
                    break;
                default:
                    throw argumentException;
            }

        }

        public static void INC(string[] args) { ModifyTop(args, 1); }

        public static void DEC(string[] args) { ModifyTop(args, -1); }

        private static void ModifyTop(string[] args, int i) 
        {
            CheckArgs(0, 1, args);
            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "%b":
                    case "%c":
                        stack.Push((byte)(stack.Pop() + i));
                        break;
                    case "%i":
                        PushByteArray(ShortToBytes((short)(PopShort() + i)));
                        break;
                }
            }
            else PushByteArray(ShortToBytes((short)(PopShort() + i)));
        }

        private static void GetMem(int length) 
        {
            short pointer = PopShort();
            for (int i = pointer + (length - 1); i >= pointer; i--) stack.Push(MEMORY[i]);
        }

        private static void SetMem(int length) 
        {
            short pointer = PopShort();
            for (int i = pointer; i < pointer + length; i++) { MEMORY[i] = stack.Pop(); }
            PushByteArray(ShortToBytes(pointer));
        }

        private static void PushString(string s) 
        {
            s = s.Replace(@"\n", "\n");
            stack.Push(0);
            for (int i = s.Length - 1; i >= 0; i--) { stack.Push((byte)s[i]); }
        }

        private static string GetString()
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

        private static short PopShort() { return BytesToShort(new byte[] { stack.Pop(), stack.Pop() }); }

        private static byte[] ShortToBytes(short sixteen)
        {
            return new byte[] { (byte)(sixteen >> 0), (byte)(sixteen >> 8) };
        }

        private static void PushByteArray(byte[] bytes) { foreach (byte b in bytes) stack.Push(b); }
    }
}

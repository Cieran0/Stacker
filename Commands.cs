using System;
using static Stacker.Program;
using static Stacker.ExecutionEngine;

namespace Stacker
{
    class Commands
    {
        //COMMANDS
        public static void PUSH(string[] args)
        {
            string name = "push";
            CheckArgs(2, 2, args, name);
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
                    throw new InvalidArgumentException(args[0],name);
            }
        }

        public static void MATHS(string[] args)
        {
            string name = "maths";
            int[] numbs = new int[2];
            int finalNum = 0;
            int loc = 0;
            bool eightBit = false;

            CheckArgs(1, 2, args,name);

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
            else { numbs = new int[] { PopShort(), PopShort() }; }

            switch (args[loc])
            {
                case "+":
                    finalNum = numbs[1] + numbs[0];
                    break;
                case "-":
                    finalNum = numbs[1] - numbs[0];
                    break;
                case "*":
                    finalNum = numbs[1] * numbs[0];
                    break;
                case "/":
                    finalNum = numbs[1] / numbs[0];
                    break;
                case "%":
                    finalNum = numbs[1] % numbs[0];
                    break;
            }

            if (!eightBit)
            {
                PushByteArray(ShortToBytes((short)finalNum));
            }
            else
            {
                stack.Push((byte)finalNum);
            }

        }

        public static void DUP(string[] args)
        {
            string name = "dup";
            CheckArgs(0, 1, args,name);
            if (args.Length == 0)
            {
                byte x = stack.Pop();
                stack.Push(x); stack.Push(x);
            }
            else
            {
                int size = IFSGFSEPI(args[0],name);
                byte[] x = new byte[size];
                for (int i = x.Length - 1; i >= 0; i--) x[i] = stack.Pop();
                PushByteArray(x);
                PushByteArray(x);
            }
        }

        public static void POP(string[] args)
        {
            string name = "pop";
            CheckArgs(0, 1, args, name);
            if (args.Length > 0) { int size = IFSGFSEPI(args[0],name); for (int i = 0; i < size; i++) stack.Pop(); }
            else stack.Pop();
        }

        public static void PRINT(string[] args)
        {
            string name = "print";
            CheckArgs(1, 1, args, name);
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
                    WhatToPrint = args[0];
                    break;
            }
            Console.Write(WhatToPrint);

        }

        public static void MEM(string[] args)
        {
            string name = "mem";
            CheckArgs(1, 2, args, name);
            string op = args[0];
            int length = 1;
            if (args.Length > 1) length = IFSGFSEPI(args[1],name);

            switch (op)
            {
                case "clear":
                    if (args.Length > 1) throw new WrongNumberOfArgumentsException(1,1,args.Length,name); 
                    MEMORY = new byte[MAX_MEM];
                    break;
                case "get":
                    GetMem(length);
                    break;
                case "set":
                    SetMem(length);
                    break;
                case "setp":
                    SetMem(length,true);
                    break;
                default:
                    throw new InvalidArgumentException(op,name);
            }

        }

        public static void INC(string[] args) { string name = "inc"; CheckArgs(0, 1, args, name); ModifyTop(args, 1); }

        public static void DEC(string[] args) { string name = "dec";  CheckArgs(0, 1, args, name); ModifyTop(args, -1); }

        public static void INPUT(string[] args)
        {
            string name = "input";
            CheckArgs(1, 1, args,name);
            switch (args[0])
            {
                case "%b":
                    stack.Push(byte.Parse(Console.ReadLine()));
                    break;
                case "%i":
                    PushByteArray(ShortToBytes(short.Parse(Console.ReadLine())));
                    break;
                case "%c":
                    stack.Push((byte)(Console.ReadLine()[0]));
                    break;
                case "%s":
                    PushString(Console.ReadLine());
                    break;
            }
        }

        public static void EXIT(string[] args) { string name = "exit";  CheckArgs(0, 0, args,name); Environment.Exit(0); }

        public static void ESCAPE(string[] args) { string name = "escape"; CheckArgs(0, 0, args,name); Escaping = true; }

        public static void RUN(string[] args) { string name = "run"; CheckArgs(1, 1, args,name); Interpret(Tokeniser.Tokenise(ReadInFile(args[0]))); }

        public static void DUMP(string[] args) { string name = "dump"; CheckArgs(0, 0, args,name); stack.Clear(); }

        public static void SWAP(string[] args)
        {
            string name = "swap";
            CheckArgs(0, 1, args, name);
            int size = 1;
            if (args.Length > 0) { size = IFSGFSEPI(args[0],name); }
            byte[,] toSwap = new byte[2, size];
            for (int i = 0; i < 2; i++) for (int j = 0; j < size; j++) { toSwap[i, j] = stack.Pop(); }
            for (int i = 0; i < 2; i++) for (int j = size - 1; j >= 0; j--) { stack.Push(toSwap[i, j]); }
        }

        //BLOCKS
        public static void LOOP(string[] args, Token[] tokens)
        {
            string name = "LOOP";
            CheckArgs(1, 1, args,name);
            int size = IFSGFSEPI(args[0],name);
            if (size < 0) { throw new InvalidArgumentException(size,name); }
            if (size == 0) while (true) { if (Escaping) { Escaping = false; break; } Interpret(tokens); }
            else for (int i = 0; i < size; i++) { if (Escaping) { Escaping = false; break; } Interpret(tokens); }
        }

        public static void IF(string[] args, Token[] tokens) { string name = "IF"; CheckArgs(3, 3, args, name); Conditional(args, tokens, name); }

        public static void ELIF(string[] args, Token[] tokens) { string name = "ELIF"; CheckArgs(3, 3, args, name); Conditional(args, tokens, name); }

        public static void ELSE(string[] args, Token[] tokens) { string name = "ELSE";  CheckArgs(0, 0, args, name); SkippingElses = true; Interpret(tokens); }

        //Useful functions

        private static void CheckArgs(int min, int max, string[] args, string name) { if (args.Length > max || args.Length < min) { throw new WrongNumberOfArgumentsException(min, max, args.Length, name); } }

        private static void Conditional(string[] args, Token[] tokens, string name) 
        {
            int[] num = new int[2]; num[1] = IFSGFSEPI(args[2],name); num[0] = IFSGFSEPI(args[0],name);
            bool condition = false;
            switch (args[1])
            {
                case "==": condition = (num[0] == num[1]); break;
                case "!=": condition = (num[0] != num[1]); break;
                case ">=": condition = (num[0] >= num[1]); break;
                case "<=": condition = (num[0] <= num[1]); break;
                case ">": condition = (num[0] > num[1]); break;
                case "<": condition = (num[0] < num[1]); break;
            }
            if (condition) { SkippingElses = true; Interpret(tokens); } else SkippingElses = false;
        }

        //If From Stack Get From Stack Else Parse Integer
        private static int IFSGFSEPI(string arg, string name) 
        {
            int value = 0;
            switch (arg) 
            {
                case "stk8":
                    return stack.Pop();
                case "stk":
                    return PopShort();
            }
            if (!int.TryParse(arg, out value)) { throw new InvalidArgumentException(arg, name); }
            return value;
        }

        private static void ModifyTop(string[] args, int i)
        {
            
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
            CheckPointer(pointer);
            for (int i = pointer + (length - 1); i >= pointer; i--) stack.Push(MEMORY[i]);
        }

        private static void SetMem(int length, bool pushPointer = false)
        {
            short pointer = PopShort();
            CheckPointer(pointer);
            for (int i = pointer; i < pointer + length; i++) { MEMORY[i] = stack.Pop(); }
            if(pushPointer)PushByteArray(ShortToBytes(pointer));
        }

        private static void CheckPointer(short pointer) { if (pointer < 0 || pointer > MAX_MEM - 1) throw new GenericException($"Memory pointer must be between {0} and {MAX_MEM - 1}, {pointer} is not."); }

        private static void PushString(string s)
        {
            s = s.Replace(@"\n", "\n");
            stack.Push(0);
            for (int i = s.Length - 1; i >= 0; i--) { stack.Push((byte)s[i]); }
            PushByteArray(ShortToBytes((short)s.Length));
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
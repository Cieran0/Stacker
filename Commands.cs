using System;
using static Stacker.Program;

namespace Stacker
{
    class Commands
    {
        public static void PUSH(string[] args)
        {
            CheckArgs(2, args);
            switch (args[0])
            {
                case "u8":
                    stack.Push(byte.Parse(args[1]));
                    break;
                case "s8":
                    stack.Push((byte)sbyte.Parse(args[1]));
                    break;
                case "u16":
                    stack.Push(ShortToBytes(ushort.Parse(args[1])));
                    break;
                case "s16":
                    stack.Push(ShortToBytes(short.Parse(args[1])));
                    break;
                case "asc":
                    stack.Push((byte)args[1][0]);
                    break;
                case "str":
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

            CheckArgs(2, args);
            switch (args[0])
            {
                case "u8":
                    numbs[0] = (int)(stack.Pop());
                    numbs[1] = (int)(stack.Pop());       
                    break;          
                case "s8":          
                    numbs[0] = (int)(((sbyte)stack.Pop()));
                    numbs[1] = (int)(((sbyte)stack.Pop()));    
                    break;          
                case "u16":         
                    numbs[0] = (int)(BytesToUShort(new byte[] { stack.Pop(), stack.Pop() }));
                    numbs[1] = (int)(BytesToUShort(new byte[] { stack.Pop(), stack.Pop() }));      
                    break;          
                case "s16":         
                    numbs[0] = (int)(BytesToShort(new byte[] { stack.Pop(), stack.Pop() }));
                    numbs[1] = (int)(BytesToShort(new byte[] { stack.Pop(), stack.Pop() }));
                    break;
                default:
                    throw argumentException;
            }
            
            switch (args[1]) 
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

            switch (args[0])
            {
                case "u8":
                    stack.Push((byte)finalNum);
                    break;
                case "s8":
                    stack.Push((byte)((sbyte)finalNum));
                    break;
                case "u16":
                    stack.Push(ShortToBytes((ushort)finalNum));
                    break;
                case "s16":
                    stack.Push(ShortToBytes((short)finalNum));
                    break;
                default:
                    throw argumentException;
            }

        }

        private static void CheckArgs(int count, string[] args) { if (args.Length != count) { throw argumentException; } }

        public static void DUP(string[] args) 
        { 
            CheckArgs(1, args);
            byte[] x = new byte[int.Parse(args[0])];
            for (int i = x.Length-1; i >= 0; i--) 
            {
                x[i] = stack.Pop();
                
            }
            stack.Push(x);
            stack.Push(x);
        }

        public static void POP(string[] args) { CheckArgs(0, args); stack.Pop(); }

        public static void PRINT(string[] args) 
        {
            CheckArgs(1, args);
            string WhatToPrint = "";
            switch (args[0]) 
            {
                case "u8":
                    WhatToPrint = stack.Pop().ToString();
                    break;
                case "s8":
                    WhatToPrint = ((sbyte)stack.Pop()).ToString();
                    break;
                case "u16":
                    WhatToPrint = BytesToUShort(new byte[] { stack.Pop(), stack.Pop() }).ToString();
                    break;
                case "s16":
                    WhatToPrint = BytesToShort(new byte[] { stack.Pop(), stack.Pop() }).ToString();
                    break;
                case "asc":
                    WhatToPrint = ((char)stack.Pop()).ToString();
                    break;
                case "str":
                    WhatToPrint = getString();
                    break;
                default:
                    throw argumentException;
            }
            Console.Write(WhatToPrint);
        
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
            int len = stack.Length;
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

        private static ushort BytesToUShort(byte[] eight)
        {
            return (ushort)(eight[0] << 8 | eight[1]);
        }

        private static byte[] ShortToBytes(short sixteen)
        {
            return new byte[] { (byte)(sixteen >> 0), (byte)(sixteen >> 8) };
        }

        private static byte[] ShortToBytes(ushort sixteen)
        {
            return new byte[] { (byte)(sixteen >> 0), (byte)(sixteen >> 8) };
        }
    }
}

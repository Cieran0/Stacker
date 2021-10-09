using System;
using System.Collections.Generic;

using static Stacker.Command;
using static Stacker.ExecutionEngine;
using static Stacker.Tokeniser;

namespace Stacker
{
    class Program
    {
        const string VER = "0.1.0\n";
        const short MAX_MEM = 32001;

        public static byte[] MEMORY = new byte[MAX_MEM];

        public static bool SkippingElses = true;
        public static bool Escaping = false;

        public static Command[] commands;
        public static Stack<byte> stack = new Stack<byte>();
        public static NotImplementedException notImplemented = new NotImplementedException();
        public static ArgumentException argumentException = new ArgumentException();

        public enum COMMANDS
        {
            //COMMANDS
            push, print, pop, dup, maths, mem, inc, dec, swap, exit, input, escape, run, dump,
            //BLOCKS
            LOOP, IF, ELSE, ELIF
        }

        static void Main(string[] args)
        {
            string input = "";
            commands = new Command[Enum.GetNames(typeof(COMMANDS)).Length];
            for (byte i = 0; i < commands.Length; i++) { commands[i] = new Command(i); }
            Console.Write("Stacker v");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(VER);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(">>> ");
            while (true)
            {
                SkippingElses = Escaping = false;
                Console.ForegroundColor = ConsoleColor.White;
                input = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.Blue;
                if (input.StartsWith('\"') && input.EndsWith('\"')) { Interpret(Tokenise(ReadInFile(input.Substring(1, input.Length-2)))) ; }
                else Interpret(Tokenise(input));
                Console.ForegroundColor = ConsoleColor.Red;
                if (Console.CursorLeft != Console.WindowLeft) Console.Write('\n'); 
                Console.Write(">>> ");
            }

        }

        public static string ReadInFile(string path)
        {
            string formated = "";
            string[] lines = System.IO.File.ReadAllLines(path);
            foreach (string line in lines)
            {
                formated += " ";
                if (line.Contains("//")) formated += line.Split("//")[0];
                else formated += line;
            }
            return formated;
        }

        public enum TokenType
        {
            COMMAND,
            ARGUMENT,
            BLOCK
        }
        public struct Token
        {
            public TokenType type;
            public string Svalue;
            public int index;
            public Token[] Tvalue;
        }

    }
}
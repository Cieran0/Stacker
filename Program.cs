using System;
using System.Collections.Generic;
using Colour = System.ConsoleColor;
using static Stacker.ExecutionEngine;
using static Stacker.Tokeniser;

namespace Stacker
{
    class Program
    {
        public const string VER = "0.1.0\n";
        public const short MAX_MEM = 32001;

        public static byte[] MEMORY = new byte[MAX_MEM];

        public static bool SkippingElses = true;
        public static bool Escaping = false;

        public static Command[] commands = new Command[Enum.GetNames(typeof(COMMANDS)).Length];
        public static Stack<byte> stack = new Stack<byte>();
        public static NotImplementedException notImplemented = new NotImplementedException();

        public enum COMMANDS
        {
            //COMMANDS
            push, print, pop, dup, maths, mem, inc, dec, swap, exit, input, escape, run, dump,
            //BLOCKS
            LOOP, IF, ELSE, ELIF
        }

        private static void InitCommands() { for (byte i = 0; i < commands.Length; i++) { commands[i] = new Command(i); } }

        private static void PrintVer() { Console.Write("Stacker v"); Console.ForegroundColor = Colour.Green; Console.Write(VER); }

        static void Main(string[] args)
        {
            InitCommands();
            PrintVer();
            Console.ForegroundColor = Colour.Red;
            Console.Write(">>> ");
            string input = "";
            while (true)
            {
                SkippingElses = Escaping = false;
                Console.ForegroundColor = Colour.White;
                input = Console.ReadLine();
                Console.ForegroundColor = Colour.Blue;
                try
                {
                    Interpret(Tokenise(input));
                }
                catch (Exception ex) {
                    MEMORY = new byte[MAX_MEM];
                    Console.ForegroundColor = Colour.Red;
                    Console.WriteLine("Error: {0}", ex);
                }
                Console.ForegroundColor = Colour.Red;
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
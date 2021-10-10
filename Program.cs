﻿using System;
using System.Collections.Generic;
using Colour = System.ConsoleColor;
using static Stacker.ExecutionEngine;
using static Stacker.Parser;
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

        public static List<string> keywords = new List<string>();
        public static List<Function> functions = new List<Function>();
        public static Command[] commands = new Command[Enum.GetNames(typeof(COMMANDS)).Length];
        public static StackerStack<byte> stack = new StackerStack<byte>();
        public static NotImplementedException notImplemented = new NotImplementedException();

        public enum COMMANDS
        {
            //COMMANDS
            push, print, pop, dup, maths, mem, inc, dec, swap, exit, input, escape, run, dump,
            //BLOCKS
            LOOP, IF, ELSE, ELIF
        }

        private static void Init() 
        { 
            for (byte i = 0; i < commands.Length; i++) commands[i] = new Command(i);
            foreach (string s in Enum.GetNames(typeof(COMMANDS))) keywords.Add(s);
            keywords.Add("define");
        }

        private static void PrintVer() { Console.Write("Stacker v"); Console.ForegroundColor = Colour.Green; Console.Write(VER); }

        static void Main(string[] args)
        {
            Init();
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
                    if(ex is StackException) stack.Clear();
                    Console.ForegroundColor = Colour.Red;
                    Console.WriteLine("Error: {0}", ex);
                }
                Console.ForegroundColor = Colour.Red;
                if (Console.CursorLeft != Console.WindowLeft) Console.Write('\n'); 
                Console.Write(">>> ");
            }

        }

        public enum TokenType
        {
            COMMAND,
            ARGUMENT,
            BLOCK,
            FUNCTIONCALL
        }
        public struct Token
        {
            public TokenType type;
            public string Svalue;
            public COMMANDS index;
            public Token[] Tvalue;
        }

    }
}
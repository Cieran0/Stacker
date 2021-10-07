﻿using System;
using System.Collections.Generic;

namespace Stacker
{
    class Program
    {
        public static byte[] MEMORY = new byte[short.MaxValue];

        static string[] commandNames = { "push", "print", "pop", "dup", "maths", "mem", "inc", "dec" };
        static string[] blockNames = { "loop" };

        public static Command[] commands;
        public static Stack<byte> stack = new Stack<byte>();
        public static NotImplementedException notImplemented = new NotImplementedException();
        public static ArgumentException argumentException = new ArgumentException();

        static void Main(string[] args)
        {
            commands = new Command[commandNames.Length + blockNames.Length];
            for (byte i = 0; i < commands.Length; i++) { commands[i] = new Command(i); }
            Console.Write(">>> ");
            while (true)
            {
                Interpret(Tokenise(Console.ReadLine()));
                Console.Write("\n>>> ");
            }

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

        static Token NewToken(TokenType type, string val)
        {
            Token placeholder = new Token();
            placeholder.type = type;
            placeholder.Svalue = val;
            return placeholder;
        }

        static Token NewToken(TokenType type, int index)
        {
            Token placeholder = new Token();
            placeholder.type = type;
            placeholder.index = index;
            return placeholder;
        }

        static Token NewToken(TokenType type, int index, Token[] tokens)
        {
            Token placeholder = new Token();
            placeholder.type = type;
            placeholder.index = index;
            placeholder.Tvalue = tokens;
            return placeholder;
        }

        static Token[] Tokenise(string input)
        {
            //Console.WriteLine(input);
            //System.Diagnostics.Stopwatch SW = new System.Diagnostics.Stopwatch();
            //SW.Start();
            List<Token> tokens = new List<Token>();
            string s = "";

            for (int i = 0; i < input.Length; i++)
            {
                if (!char.IsWhiteSpace(input[i])) { s += input[i]; }
                for (int q = 0; q < commandNames.Length; q++) if (s == commandNames[q]) { AddCommandToken(ref tokens, q, input, ref i); s = ""; };
                for (int q = 0; q < blockNames.Length; q++) if (s == blockNames[q]) { AddBlockToken(ref tokens, q, input, ref i); s = ""; };
            }
            //SW.Stop();
            //Console.WriteLine("Parsed {0} Tokens in {1}ms", tokens.Count, SW.ElapsedMilliseconds);
            return tokens.ToArray();
        }

        private static void AddBlockToken(ref List<Token> tokens, int index, string input, ref int i)
        {
            tokens.Add(NewToken(TokenType.BLOCK, index));
            string ss = "";
            int pos = tokens.Count - 1;
            int j = 0;
            int k = 0;
            int open = 0;
            bool CanLeave = false;

            while (input[i + j] != ')')
            {
                if (input[i + j] == '\"')
                {
                    k = 1;
                    while (input[i + j + k] != '\"') { ss += input[i + j + k]; k++; }
                    tokens.Add(NewToken(TokenType.ARGUMENT, ss));
                    ss = ""; j += k;
                }
                else if (input[i + j] == ',' && ss != "")
                {
                    tokens.Add(NewToken(TokenType.ARGUMENT, ss));
                    ss = ""; j += k;
                }
                else if ((byte)input[i + j] >= 48 && (byte)input[i + j] <= 57)
                {
                    ss += input[i + j];
                }
                j++;
            }
            if (ss != "") tokens.Add(NewToken(TokenType.ARGUMENT, ss));
            i += j;
            j = 1; k = 0;

            while (input[i + j] != '{') if (!char.IsWhiteSpace(input[i + j])) throw argumentException; else { j++; }
            j++;
            while (!CanLeave)
            {
                if (input[i + j + k] == '}' && open == 0) { CanLeave = true; }
                else if (input[i + j + k] == '{') { open++; }
                else if (input[i + j + k] == '}') { open--; }
                k++;
            }
            tokens[pos] = NewToken(TokenType.BLOCK, index, Tokenise(input.Substring((i + j), k)));
            i = i + j + k;
        }

        static void AddCommandToken(ref List<Token> tokens, int index, string input, ref int i)
        {
            tokens.Add(NewToken(TokenType.COMMAND, index));
            string ss = "";
            int j = 0;
            int k = 0;
            while (input[i + j] != ')')
            {
                if (input[i + j] == '\"')
                {
                    k = 1;
                    while (input[i + j + k] != '\"') { ss += input[i + j + k]; k++; }
                    tokens.Add(NewToken(TokenType.ARGUMENT, ss));
                    ss = ""; j += k;
                }
                else if (input[i + j] == ',' && ss != "")
                {
                    tokens.Add(NewToken(TokenType.ARGUMENT, ss));
                    ss = ""; j += k;
                }
                else if ((byte)input[i + j] >= 48 && (byte)input[i + j] <= 57)
                {
                    ss += input[i + j];
                }
                j++;
            }
            if (ss != "")
            {
                tokens.Add(NewToken(TokenType.ARGUMENT, ss));
            }
            ss = "";
            i += j;
        }

        public static void Interpret(Token[] tokens)
        {

            string[] args;
            int argCounter = 0;
            int j = 0;
            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i].type != TokenType.ARGUMENT)
                {
                    j = 1;
                    argCounter = 0;
                    if (i + j < tokens.Length)
                    {
                        while (tokens[i + j].type == TokenType.ARGUMENT)
                        {
                            argCounter++;

                            j++;
                            if (i + j >= tokens.Length) { break; }
                        }
                    }
                    args = new string[argCounter];
                    for (int k = i + 1; k < i + j; k++) { args[k - (i + 1)] = tokens[k].Svalue; }
                    if (tokens[i].type == TokenType.COMMAND)
                    {
                        commands[tokens[i].index].Execute(args);
                    }
                    else
                    {
                        commands[tokens[i].index + commandNames.Length].Execute(args, tokens[i].Tvalue);
                    }
                }
            }
        }
    }
}
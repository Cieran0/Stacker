﻿using System.Collections.Generic;
using System;
using static Stacker.Program;

namespace Stacker
{
    class Function
    {

        public string name { get; }
        
        public Token[] tokens { get; }

        public string[] args { get; }

        public static Dictionary<string, Function> functionDict = new Dictionary<string, Function>();

        public Function(string name, Token[] tokens, string[] args) 
        {
            if (keywords.Contains(name)) throw new GenericException($"Keyword {name} is already defined");
            this.name = name;
            this.tokens = tokens;
            this.args = args;
            functionDict.Add(name, this);
            keywords.Add(name);
            Console.WriteLine($"Loaded Function: {name}");
        }

        public void Execute(string[] args) 
        {
            if (args.Length != this.args.Length) throw new WrongNumberOfArgumentsException(this.args.Length, this.args.Length, args.Length, name);
            for (int i = 0; i < args.Length; ++i) 
            {
                for (int j = 0; j < tokens.Length; j++) 
                {
                    if (tokens[j].type == TokenType.ARGUMENT && tokens[j].Svalue == this.args[i]) 
                    {
                        tokens[j].Svalue = args[i];
                    }
                }
            }
            ExecutionEngine.Interpret(tokens);
            
        }
    }
}

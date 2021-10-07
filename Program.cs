using System;

namespace Stacker
{
    class Program
    {

        static string[] commandNames = {"push", "print", "pop", "dup", "maths"};

        public static Command[] commands;
        public static Stack<byte> stack = new Stack<byte>();
        public static NotImplementedException notImplemented = new NotImplementedException();
        public static ArgumentException argumentException = new ArgumentException();

        static void Main(string[] args)
        {
            commands = new Command[commandNames.Length];
            for (byte i = 0; i < commands.Length; i++) { commands[i] = new Command(i); }
            while (true) 
            {
                Interpret(Console.ReadLine());
            }
        }

        enum TokenType 
        { 
            COMMAND,
            ARGUMENT
        }

        struct Token 
        {
            public int type;
            public string value;

        }

        static Token NewToken(TokenType type, string val) 
        {
            Token placeholder = new Token();
            placeholder.type = (int)type;
            placeholder.value = val;
            return placeholder;
        }

        static Token[] Tokenise(string input) 
        {
            System.Collections.Generic.List<Token> tokens = new System.Collections.Generic.List<Token>();
            string s = "";

            for (int i = 0; i < input.Length; i++) 
            {
                if (!char.IsWhiteSpace(input[i])) { s += input[i]; }
                for (int q = 0; q < commandNames.Length; q++) 
                {
                    if (s == commandNames[q]) { UpdateTokenList(ref tokens, q.ToString(), input, ref i); s = "";}
                }

            }

            return tokens.ToArray();
        }

        static void UpdateTokenList(ref System.Collections.Generic.List<Token> tokens, string val, string input, ref int i) 
        {
            tokens.Add(NewToken(TokenType.COMMAND, val));
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

        static void Interpret(string input) 
        {
            Token[] tokens = Tokenise(input);
            string[] args;
            int argCounter = 0;
            int j = 0;
            for (int i = 0; i < tokens.Length; i++) 
            {
                if (tokens[i].type == (int)TokenType.COMMAND) 
                {
                    j = 1;
                    argCounter = 0;
                    if (i + j < tokens.Length)
                    {
                        while (tokens[i + j].type == (int)TokenType.ARGUMENT)
                        {
                            argCounter++;

                            j++;
                            if (i + j >= tokens.Length) { break; }
                        }
                    }
                    args = new string[argCounter];
                    for (int k = i + 1; k < i + j; k++) { args[k - (i + 1)] = tokens[k].value; }
                    commands[int.Parse(tokens[i].value)].Execute(args);
                }
            }
        }
    }
}

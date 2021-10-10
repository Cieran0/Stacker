using static Stacker.Program;
using static Stacker.Function;

namespace Stacker
{
    class ExecutionEngine
    {
        public static void Interpret(Token[] tokens)
        {
            string[] args;
            int argCounter = 0;
            int j = 0;
            for (int i = 0; i < tokens.Length; i++)
            {
                if (Escaping) break;
                
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
                        commands[(int)tokens[i].index].Execute(args);
                    }
                    else if (tokens[i].type == TokenType.BLOCK)
                    {
                        if (!(tokens[i].index == COMMANDS.ELSE || tokens[i].index == COMMANDS.ELIF)) SkippingElses = false;

                        if (!SkippingElses) commands[(int)tokens[i].index].Execute(args, tokens[i].Tvalue);
                    }
                    else if (tokens[i].type == TokenType.FUNCTIONCALL) 
                    {
                        functionDict[tokens[i].Svalue].Execute(args);
                    }
                }
            }
        }
    }
}

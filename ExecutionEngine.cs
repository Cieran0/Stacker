using static Stacker.Program;

namespace Stacker
{
    class ExecutionEngine
    {
        public static void Interpret(Token[] tokens)
        {
            bool skip = false;
            string[] args;
            int argCounter = 0;
            int j = 0;
            for (int i = 0; i < tokens.Length; i++)
            {
                skip = false;
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
                    else if (tokens[i].type == TokenType.BLOCK)
                    {
                        if (tokens[i].index == (int)COMMANDS.ELSE || tokens[i].index == (int)COMMANDS.ELIF)
                        {
                            if (SkipElses) skip = true;
                        }
                        else
                        {
                            SkipElses = false;
                        }
                        if (!skip) commands[tokens[i].index].Execute(args, tokens[i].Tvalue);
                    }
                }
            }
        }
    }
}

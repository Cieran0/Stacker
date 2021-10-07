using static Stacker.Program;
using static Stacker.Commands;
using System;

namespace Stacker
{
    class Command
    {
        public enum COMMANDS 
        {
            push, 
            print, 
            pop,
            dup,
            maths,
            //BLOCKS
            loop
        }

        public COMMANDS INDEX { get; }

        public Command(byte INDEX) 
        {
            this.INDEX = (COMMANDS)INDEX;
        }

        public void Execute(string[] args) 
        {
            switch (INDEX)
            {
                case COMMANDS.push:
                    PUSH(args);
                    break;
                case COMMANDS.print:
                    PRINT(args);
                    break;
                case COMMANDS.pop:
                    POP(args);
                    break;
                case COMMANDS.dup:
                    DUP(args);
                    break;
                case COMMANDS.maths:
                    MATHS(args);
                    break;
            }
        }

        public void Execute(string[] args, Token[] tokens) 
        {
            switch (INDEX) 
            { 
                case COMMANDS.loop:
                    LOOP(args, tokens);
                    break;
            }
        }
    }
}

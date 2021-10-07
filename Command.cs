using static Stacker.Program;
using static Stacker.Commands;
using System.Collections.Generic;
using System;

namespace Stacker
{
    class Command
    {
        public enum COMMANDS
        {
            //COMMANDS
            push, print, pop, dup, maths, mem, inc, dec,
            //BLOCKS
            loop
        }

        static Dictionary<COMMANDS, Action<string[]>> commandDict = new Dictionary<COMMANDS, Action<string[]>>()
        {
            { COMMANDS.push, PUSH}, { COMMANDS.print, PRINT}, { COMMANDS.pop, POP}, { COMMANDS.dup, DUP}, { COMMANDS.maths, MATHS}, { COMMANDS.mem, MEM}, { COMMANDS.inc, INC}, { COMMANDS.dec, DEC},
        };

        static Dictionary<COMMANDS, Action<string[], Token[]>> blockDict = new Dictionary<COMMANDS, Action<string[], Token[]>>()
        {
            { COMMANDS.loop, LOOP}
        };

        public COMMANDS INDEX { get; }

        public Command(byte INDEX) => this.INDEX = (COMMANDS)INDEX;

        public void Execute(string[] args) => commandDict[INDEX].Invoke(args);

        public void Execute(string[] args, Token[] tokens) => blockDict[INDEX].Invoke(args, tokens);
    }
}
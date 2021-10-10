using System;
using static Stacker.Function;
using static Stacker.Program;

namespace Stacker
{
    class Parser
    {
        public static string ReadInFile(string path)
        {
            if (!System.IO.File.Exists(path)) throw new FileNotFoundException(path);
            string formated = "";
            string[] lines = System.IO.File.ReadAllLines(path);
            foreach (string line in lines)
            {
                formated += " ";
                if (line.Contains("//")) formated += line.Split("//")[0];
                else formated += line;
            }
            ExtractFunctions(ref formated);
            return formated;
        }

        public static void ExtractFunctions(ref string formated) 
        {
            if (!formated.Contains("define")) { return; }
            string name = "";
            string rawArgs = "";
            string rawCommands = "";
            string[] args;
            string temp = "";

            int start = formated.IndexOf("define");
            string subString = formated.Substring(start + 6);
            if (!char.IsWhiteSpace(subString[0])) throw new GenericException("after define have one whitespace");
            int i = 1;
            
            while (subString[i] != '(') 
            {
                if (!char.IsLetter(subString[i])) throw new GenericException("something something function names can only contain letters");
                name += subString[i];
                i++;
            }
            i++;

            while (subString[i] != ')') {  rawArgs += subString[i]; i++; }

            
            while (subString[i] != '{') { i++; }
            i++;

            
            while (subString[i] != '}') { rawCommands += subString[i]; i++; }
            i++;
            i += start + 6;
            if (start != 0) { temp = formated.Substring(0, start); }
            if (i <= formated.Length-1) { formated = formated.Substring(i, formated.Length - i); } else formated = "";
            
            formated = temp + formated;
            
            args = rawArgs.Split(',');
            for (int j = 0; j < args.Length; j++) 
            {
                if (!args[j].StartsWith('\"') && !args[j].EndsWith('\"')) { throw new GenericException("something something variables in functions must be strings"); }
                else { args[j] = args[j].Substring(1, args[j].Length - 2); }
            }

            functions.Add(new Function(name,Tokeniser.Tokenise(rawCommands),args));
                
           ExtractFunctions(ref formated);

        }

    }
}

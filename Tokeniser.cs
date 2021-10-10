using static Stacker.Program;
using System;
using System.Collections.Generic;

namespace Stacker
{
    class Tokeniser
    {
        private static Token NewToken(TokenType type, string val)
        {
            Token placeholder = new Token();
            placeholder.type = type;
            placeholder.Svalue = val;
            return placeholder;
        }

        private static Token NewToken(TokenType type, COMMANDS index, Token[] tokens = null)
        {
            Token placeholder = new Token();
            placeholder.type = type;
            placeholder.index = index;
            if (tokens != null) placeholder.Tvalue = tokens;
            return placeholder;
        }

        public static Token[] Tokenise(string input)
        {
            List<Token> tokens = new List<Token>();
            string CurrentStr = "";
            string TrimmedStr = "";
            bool knownStr = false;

            for (int i = 0; i < input.Length; i++)
            {
                CurrentStr += input[i];
                if (input[i] == '(') 
                {
                    TrimmedStr = CurrentStr.Trim();
                    knownStr = false;
                    for (int currentKeyword = 0; currentKeyword < commands.Length; currentKeyword++)
                    {
                        if (TrimmedStr == ((COMMANDS)currentKeyword).ToString()+'(')
                        {
                            if (IsBlock((COMMANDS)currentKeyword)) { i--; AddBlockToken(ref tokens, (COMMANDS)currentKeyword, input, ref i); CurrentStr = "";}
                            else { i--; AddCommandToken(ref tokens, (COMMANDS)currentKeyword, input, ref i); CurrentStr = ""; }
                            knownStr = true;
                        }
                    }
                    if (!knownStr) throw new UnkownStringException(CurrentStr);
                }
            }
            if (!string.IsNullOrWhiteSpace(CurrentStr)) {
                string[] keywords = Enum.GetNames(typeof(COMMANDS));
                for (int i = 0; i < keywords.Length; i++) 
                { 
                    if (CurrentStr == keywords[i]) throw new InvalidCharacterException('(', ' ', (COMMANDS)i); 
                }
                throw new UnkownStringException(CurrentStr); 
            }
            return tokens.ToArray();
        }

        private static bool IsBlock(COMMANDS index) => char.IsUpper((index).ToString()[0]);

        private static void AddBlockToken(ref List<Token> tokens, COMMANDS index, string input, ref int i)
        {
            tokens.Add(NewToken(TokenType.BLOCK, index));
            int blockTokenPos = tokens.Count - 1;
            int j, k, parenthesesOpened;
            j = k = parenthesesOpened = 0;
            bool CanLeave = false;
            AddArgumentTokens(ref tokens, index, input, ref i, ref j);
            j = 1;

            if (i + j >= input.Length) { throw new ParenthesesNotFoundException('{', index); }

            while (input[i + j] != '{')
            {
                if (!char.IsWhiteSpace(input[i + j]))
                {
                    throw new InvalidCharacterException('{', input[i + j], index);
                }
                else
                {
                    j++; if (i + j >= input.Length) { throw new InvalidCharacterException('{', input[i + j], index); }
                }
            }
            j++;
            if (i + j + k >= input.Length) throw new TrailingParenthesesException('{', index);
            while (!CanLeave)
            {
                if (input[i + j + k] == '}' && parenthesesOpened == 0) { CanLeave = true; }
                else if (input[i + j + k] == '{') { parenthesesOpened++; }
                else if (input[i + j + k] == '}') { parenthesesOpened--; }
                k++;
                if (i + j + k >= input.Length && !CanLeave) { throw new TrailingParenthesesException('{', index); }
            }
            tokens[blockTokenPos] = NewToken(TokenType.BLOCK, index, Tokenise(input.Substring((i + j), k-1)));
            i = i + j + k;
        }

        private static void AddCommandToken(ref List<Token> tokens, COMMANDS index, string input, ref int i)
        {
            int j = 0;
            tokens.Add(NewToken(TokenType.COMMAND, index));
            AddArgumentTokens(ref tokens, index, input, ref i, ref j);
        }

        private static void AddArgumentTokens(ref List<Token> tokens, COMMANDS index, string input, ref int i, ref int j) 
        {
            string rawArgumentData = "";
            j = 1;
            if (i + j >= input.Length) { throw new ParenthesesNotFoundException('(', index); }
            else if (input[i + j] != '(') { throw new InvalidCharacterException('(', input[i + j], index); }
            while (input[i + j] != ')')
            {
                rawArgumentData += input[i + j];
                j++;
                if (i + j >= input.Length) throw new TrailingParenthesesException('(', index);
            }
            foreach (string arg in ProcessArgumentData(rawArgumentData.Substring(1), index)) 
            {
                tokens.Add(NewToken(TokenType.ARGUMENT, arg));
            }
            i += j;
        }

        private static string[] ProcessArgumentData(string rawArgumentData, COMMANDS index) 
        {
            if (rawArgumentData.EndsWith(',')) { throw new GenericException($"Unexpected ',' at the end of arugments in {index.ToString()}"); }
            bool shouldBreak = false;
            int i = 0;
            int j = 0;
            List<string> args = new List<string>();
            while (i < rawArgumentData.Length) 
            {
                if (rawArgumentData[i] == ',') { i++; }
                else if (char.IsNumber(rawArgumentData[i]) || rawArgumentData[i] == '-')
                {
                    j = 0;
                    if (rawArgumentData[i] == '-') 
                    {
                        j++;
                        if (i + j >= rawArgumentData.Length) throw new GenericException($"'-' in {index.ToString()} should be followed directly by an integer");
                        if (!char.IsNumber(rawArgumentData[i+j])) throw new GenericException($"'-' in {index.ToString()} should be followed directly by an integer");
                    }
                    while (char.IsNumber(rawArgumentData[i + j]))
                    {
                        j++;
                        if (i + j >= rawArgumentData.Length) { args.Add(rawArgumentData.Substring(i, j)); shouldBreak = true; break; }
                    }
                    if (shouldBreak) { break; }
                    if (rawArgumentData[i + j] != ',') { throw new GenericException($"{rawArgumentData.Substring(i, j)} in {index.ToString()} is an invalid integer, make sure this number does not end with whitespace."); }
                    args.Add(rawArgumentData.Substring(i, j));
                    i++;
                }
                else if (rawArgumentData[i] != '\"')
                {
                    throw new GenericException($"String argument in {index.ToString()} must be inclosed in \'\"\', \'{rawArgumentData[i]}\' is not \'\"\'");
                }
                else if (rawArgumentData[i] == '\"')
                {
                    j = 1;
                    if (i + j >= rawArgumentData.Length) { throw new TrailingParenthesesException('\"', index); }
                    while (rawArgumentData[i + j] != '\"')
                    {
                        j++;
                        if (i + j >= rawArgumentData.Length) { throw new TrailingParenthesesException('\"', index); }
                    }
                    if (i + j + 1 < rawArgumentData.Length)
                    {
                        j += 1;
                        if (rawArgumentData[i + j] != ',')
                        {
                            throw new GenericException($"Arguments in ${index.ToString()} must be seperated by ',' \'{rawArgumentData[i + j]}\' is not ','.");
                        }
                        args.Add(rawArgumentData.Substring(i + 1, j - 2));
                    }
                    else { args.Add(rawArgumentData.Substring(i + 1, j - 1)); j += 1; }

                    i += j;
                }
                else throw new GenericException($"Unreachable code executed in {"Tokeniser.ProcessArgumentData"}.");
            }
            return args.ToArray(); ;
        }
    }
}

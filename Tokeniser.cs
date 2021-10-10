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
                            if (IsBlock(currentKeyword)) { i--; AddBlockToken(ref tokens, (COMMANDS)currentKeyword, input, ref i); CurrentStr = "";}
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

        private static bool IsBlock(int index) => char.IsUpper((index).ToString()[0]);

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
            string ss = "";
            j = 1;
            int k = 0;
            if (i + j >= input.Length) { throw new ParenthesesNotFoundException('(', index); }
            else if (input[i + j] != '(') { throw new InvalidCharacterException('(', input[i + j], index); }
            while (input[i + j] != ')')
            {
                if (input[i + j] == '\"')
                {
                    k = 1;
                    while (input[i + j + k] != '\"') { ss += input[i + j + k]; k++; if (i + j + k >= input.Length) throw new TrailingParenthesesException('\"', index); }
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
                if (i + j >= input.Length) throw new TrailingParenthesesException('(', index);
            }
            if (ss != "") tokens.Add(NewToken(TokenType.ARGUMENT, ss));
            ss = "";
            i += j;
        }
    }
}

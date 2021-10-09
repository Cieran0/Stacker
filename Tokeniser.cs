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

        private static Token NewToken(TokenType type, int index, Token[] tokens = null)
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
            string s = "";
            string SuperS = "";
            bool vaild = false;

            for (int i = 0; i < input.Length; i++)
            {
                s += input[i];
                if (input[i] == '(') 
                {
                    SuperS = s.Trim();
                    vaild = false;
                    for (int q = 0; q < commands.Length; q++)
                    {
                        if (SuperS == ((COMMANDS)q).ToString()+'(')
                        {
                            if (IsBlock(q)) { i--; AddBlockToken(ref tokens, q, input, ref i); s = "";}
                            else { i--; AddCommandToken(ref tokens, q, input, ref i); s = ""; }
                            vaild = true;
                        }
                    }
                    if (!vaild) throw new UnkownStringException(s);
                }
            }
            if (!string.IsNullOrWhiteSpace(s)) throw new UnkownStringException(s);
            return tokens.ToArray();
        }

        private static bool IsBlock(int index) => char.IsUpper(((COMMANDS)index).ToString()[0]);

        private static void AddBlockToken(ref List<Token> tokens, int index, string input, ref int i)
        {
            tokens.Add(NewToken(TokenType.BLOCK, index));
            string ss = "";
            int pos = tokens.Count - 1;
            int j = 1;
            int k = 0;
            int open = 0;
            bool CanLeave = false;
            if (i + j >= input.Length) { throw new ParenthesesNotFoundException('(', (COMMANDS)index); }
            else if (input[i + j] != '(') { throw new InvalidCharacterException('(', input[i + j], (COMMANDS)index); }
            while (input[i + j] != ')')
            {
                if (input[i + j] == '\"')
                {
                    k = 1;
                    while (input[i + j + k] != '\"') { ss += input[i + j + k]; k++; if (i + j + k >= input.Length) throw new TrailingParenthesesException('\"', (COMMANDS)index); }
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
                if (i + j >= input.Length) throw new TrailingParenthesesException('(', (COMMANDS)index);
            }
            if (ss != "") tokens.Add(NewToken(TokenType.ARGUMENT, ss));
            i += j;
            j = 1; k = 0;

            if (i + j >= input.Length) { throw new ParenthesesNotFoundException('{', (COMMANDS)index); }

            while (input[i + j] != '{')
            {
                if (!char.IsWhiteSpace(input[i + j]))
                {
                    throw new InvalidCharacterException('{', input[i + j], (COMMANDS)index);
                }
                else
                {
                    j++; if (i + j >= input.Length) { throw new InvalidCharacterException('{', input[i + j], (COMMANDS)index); }
                }
            }
            j++;
            while (!CanLeave)
            {
                if (input[i + j + k] == '}' && open == 0) { CanLeave = true; }
                else if (input[i + j + k] == '{') { open++; }
                else if (input[i + j + k] == '}') { open--; }
                k++;
                if (i + j + k >= input.Length && !CanLeave) { throw new TrailingParenthesesException('{', (COMMANDS)index); }
            }
            tokens[pos] = NewToken(TokenType.BLOCK, index, Tokenise(input.Substring((i + j), k-1)));
            i = i + j + k;
        }

        private static void AddCommandToken(ref List<Token> tokens, int index, string input, ref int i)
        {
            tokens.Add(NewToken(TokenType.COMMAND, index));
            string ss = "";
            int j = 1;
            int k = 0;
            if (i + j >= input.Length) { throw new ParenthesesNotFoundException('(', (COMMANDS)index); }
            else if (input[i + j] != '(') { throw new InvalidCharacterException('(', input[i + j], (COMMANDS)index); }
            while (input[i + j] != ')')
            {
                if (input[i + j] == '\"')
                {
                    k = 1;
                    while (input[i + j + k] != '\"') { ss += input[i + j + k]; k++; if (i + j + k >= input.Length) throw new TrailingParenthesesException('\"', (COMMANDS)index); }
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
                if (i + j >= input.Length) throw new TrailingParenthesesException('(', (COMMANDS)index);
            }
            if (ss != "")
            {
                tokens.Add(NewToken(TokenType.ARGUMENT, ss));
            }
            ss = "";
            i += j;
        }
    }
}

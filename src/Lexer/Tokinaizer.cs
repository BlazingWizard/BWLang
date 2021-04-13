using BW.Common;
using BW.Common.Exceptions;

using System.Collections.Generic;
using System.Linq;

namespace BW.Lexer
{
    public class Tokinaizer
    {
        private string _programmText;
        public Tokinaizer(string programmText)
        {
            _programmText = programmText.Replace(" ", "");
            _programmText = _programmText.Replace("\n", "");
            _programmText = _programmText.Replace("\r", "");
            _programmText = _programmText.Replace("\t", "");
            _programmText = _programmText + "$";
        }

        public List<Token> Tokinaize()
        {
            var tokens = new List<Token>();
            var terminalWords = new TerminalWordsList();

            for (int i = 1; i < _programmText.Length; i++)
            {
                string tokenValue = _programmText[i - 1].ToString();
                while (terminalWords.GetSuccesTerminals(tokenValue).Count != 0)
                {
                    if (i == _programmText.Length)
                    {
                        break;
                    }

                    tokenValue += _programmText[i];
                    i++;
                }

                // Удаляем последний символ
                if (i < _programmText.Length + 1)
                {
                    i--;
                    tokenValue = tokenValue.Remove(tokenValue.Length - 1);
                }

                var terminalWord = terminalWords.GetSuccesTerminals(tokenValue).FirstOrDefault();
                if (terminalWord != null)
                {
                    tokens.Add(new Token(terminalWord.Word, tokenValue));
                }
                else
                {
                    throw new InvalidTokenException($"Unexepted secuence {tokenValue}");
                }
            }

            return tokens;
        }
    }
}

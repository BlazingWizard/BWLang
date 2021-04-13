using BW.Common;

using System.Text.RegularExpressions;

namespace BW.Lexer
{
    public class Terminal
    {
        public TerminalWords Word { get; set; }

        public Regex Regex  { get; set; }

        public int Priority { get; set; }

        public Terminal(TerminalWords terminalWord, string regex, int prioty)
        {
            Word = terminalWord;
            Regex = new Regex(regex, RegexOptions.Compiled);
            Priority = prioty;
        }
    }
}

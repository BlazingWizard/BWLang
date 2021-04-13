using BW.Common;

using System.Collections.Generic;
using System.Linq;

namespace BW.Lexer
{
    public class TerminalWordsList
    {
        public List<Terminal> _terminalWordsList;

        public TerminalWordsList()
        {
            _terminalWordsList = new List<Terminal>()
            {
                new Terminal(TerminalWords.VAR, @"^[a-z]+$", 1),
                new Terminal(TerminalWords.DIGIT, @"^[0-9]+$", 1),
                new Terminal(TerminalWords.K_IF, @"^if$", 2),
                new Terminal(TerminalWords.A_OP, @"^=$", 1),
                new Terminal(TerminalWords.OP, @"^(\+|-|\*|\/)$",1),
                new Terminal(TerminalWords.L_B, @"^\($",1),
                new Terminal(TerminalWords.R_B, @"^\)$",1),
                new Terminal(TerminalWords.L_CB, @"^\{$",1),
                new Terminal(TerminalWords.R_CB, @"^\}$",1),
                new Terminal(TerminalWords.K_WHILE, @"^while$", 2),
                new Terminal(TerminalWords.EOL, @"^;$", 1),
                new Terminal(TerminalWords.K_ELSE, @"^else$", 2),
                new Terminal(TerminalWords.L_OP, @"^(==|!=|>|<)$", 1),
                new Terminal(TerminalWords.K_PRINT, @"^print$", 2),
                new Terminal(TerminalWords.K_DOT, @"^\.$", 1),
                new Terminal(TerminalWords.K_NEW, @"^new!$", 2),
                new Terminal(TerminalWords.K_FUNCTION, @"^function!$", 2),
                new Terminal(TerminalWords.K_THREAD, @"^thread\($", 2),
                new Terminal(TerminalWords.THREAD_OP, @"^:$", 2)
            };
        }
        
        public List<Terminal> GetSuccesTerminals(string word)
        {
            return _terminalWordsList.Where(x => x.Regex.IsMatch(word)).OrderByDescending(x => x.Priority).ToList();
        }
    }
}

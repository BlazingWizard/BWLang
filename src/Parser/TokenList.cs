using BW.Common;

using System.Collections.Generic;

namespace BW.SyntaxAnalayzer
{
    class TokenList
    {
        private List<Token> _tokens;

        public int SavedTokenID { get; private set; }

        public int CurrentTokenID { get; private set; }

        public Token CurrentToken => CurrentTokenID < _tokens.Count ? _tokens[CurrentTokenID] : null;

        public bool EOF => _tokens.Count == CurrentTokenID;
        
        public Token this [int TokenID]
        {
            get
            {
                return TokenID < _tokens.Count ? _tokens[TokenID] : null;
            }
        }

        public TokenList(List<Token> tokens)
        {
            _tokens = tokens;
        }

        public void Next()
        {
            CurrentTokenID++;
        }

        public void Rollback()
        {
            CurrentTokenID = SavedTokenID;
        }

        public void Commit()
        {
            SavedTokenID = CurrentTokenID;
        }

        internal void Prev()
        {
            CurrentTokenID--;
        }
    }
}

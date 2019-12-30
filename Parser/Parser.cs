using BW.Common;
using BW.Common.Exceptions;
using System;
using System.Collections.Generic;

namespace BW.SyntaxAnalayzer
{
    public class Parser
    {
        public Dictionary<string, List<PolisElement>> FunctionTable;

        public List<PolisElement> Polis;

        private TokenList _tokenList;

        private PolisCreator _polisCreator;

        public Parser(List<Token> tokenList)
        {
            _tokenList = new TokenList(tokenList);
            Polis = new List<PolisElement>();
            FunctionTable = new Dictionary<string, List<PolisElement>>();
            _polisCreator = new PolisCreator(_tokenList, Polis, FunctionTable);
        }

        public List<PolisElement> CreatePolis()
        {
            Lang();
            return Polis;
        }

        private void Lang()
        {
            while (!_tokenList.EOF)
            {
                bool isCorrect = Expr();
                _tokenList.Commit();
                if (!isCorrect)
                {
                    throw new ParseException();
                }
            }
        }

        private bool Expr()
        {
            if (Assigment())
            {
                _tokenList.Next();
                return true;
            }
            else
            {
                _tokenList.Rollback();
            }

            if (If())
            {
                _tokenList.Commit();
                _tokenList.Next();
                return true;
            }
            else
            {
                _tokenList.Rollback();
            }

            if (While())
            {
                _tokenList.Commit();
                _tokenList.Next();
                return true;
            }
            else
            {
                _tokenList.Rollback();
            }

            if (Print())
            {
                _tokenList.Commit();
                _tokenList.Next();
                return true;
            }
            else
            {
                _tokenList.Rollback();
            }

            if (MethodCall())
            {
                _polisCreator.CreateAsigmentPolis();
                _tokenList.Next();
                if (_tokenList.CurrentToken.Lexemma == TerminalWords.EOL)
                {
                    _tokenList.Commit();
                    _tokenList.Next();
                    return true;
                }
                else
                {
                    _tokenList.Rollback();
                }
                
            }
            else
            {
                _tokenList.Rollback();
            }

            if (FunctionDelcaration())
            {
                _tokenList.Commit();
                _tokenList.Next();
                return true;
            }
            else
            {
                _tokenList.Rollback();
            }

            return false;
        }

        private bool FunctionDelcaration()
        {
            if(_tokenList.CurrentToken.Lexemma != TerminalWords.K_FUNCTION)
            {
                return false;
            }
            _tokenList.Next();


            if (_tokenList.CurrentToken.Lexemma != TerminalWords.VAR)
            {
                return false;
            }
            _polisCreator.AddFunctionStart(_tokenList.CurrentToken.Value);
            _tokenList.Next();

            if (!Args())
            {
                return false;
            }
            _tokenList.Next();

            if (!Body())
            {
                return false;
            }
            _polisCreator.CreateFunctionPolis();

            return true;
        }

        private bool Args()
        {
            if(_tokenList.CurrentToken.Lexemma != TerminalWords.L_B)
            {
                return false;
            }
            _tokenList.Next();

            if (_tokenList.CurrentToken.Lexemma != TerminalWords.VAR)
            {
                return false;
            }
            _polisCreator.AddArgPolis(_tokenList.CurrentToken.Value);
            _tokenList.Next();

            if (_tokenList.CurrentToken.Lexemma != TerminalWords.R_B)
            {
                return false;
            }

            return true;
        }

        private bool Print()
        {
            if (_tokenList.CurrentToken.Lexemma != TerminalWords.K_PRINT)
            {
                return false;
            }
            _tokenList.Next();

            if (_tokenList.CurrentToken.Lexemma != TerminalWords.EOL)
            {
                return false;
            }
            _polisCreator.CreatePrintPolis();
            return true;
        }

        private bool While()
        {
            if (_tokenList.CurrentToken.Lexemma != TerminalWords.K_WHILE)
            {
                return false;
            }
            var startPos = Polis.Count;
            _tokenList.Next();
            _tokenList.Commit();


            if (!Head())
            {
                return false;
            }
            _polisCreator.AddNegativeGo();
            _tokenList.Commit();
            _tokenList.Next();

            if (!Body())
            {
                return false;
            }
            _polisCreator.AddDirectGo(startPos);
            _polisCreator.ReplaceTmpAddress();

            return true;
        }

        private bool If()
        {
            if (_tokenList.CurrentToken.Lexemma != TerminalWords.K_IF)
            {
                return false;
            }
            _tokenList.Next();
            _tokenList.Commit();


            if (!Head())
            {
                return false;
            }
            _polisCreator.AddNegativeGo();
            _tokenList.Next();

            if (!Body())
            {
                return false;
            }

            _tokenList.Next();


            if (_tokenList.CurrentToken?.Lexemma != TerminalWords.K_ELSE)
            {
                _polisCreator.ReplaceTmpAddress();
                _tokenList.Prev();
                return true;
            }
            else
            {
                _polisCreator.ReplaceTmpAddress(2);
                _polisCreator.AddDirectGo();
                _tokenList.Next();
                if (!Body())
                {
                    return false;
                }
                _polisCreator.ReplaceTmpAddress();
            }

            return true;
        }

        private bool Head()
        {
            if (_tokenList.CurrentToken.Lexemma != TerminalWords.L_B)
            {
                return false;
            }
            _tokenList.Next();

            if (!LogicalExpr())
            {
                return false;
            }
            _tokenList.Next();

            if (_tokenList.CurrentToken.Lexemma != TerminalWords.R_B)
            {
                return false;
            }
            _polisCreator.CreateLogicPolis();
            return true;
        }

        private bool LogicalExpr()
        {
            if (MethodCall())
            {
                return true;
            }
            else
            {
                _tokenList.Rollback();
                _tokenList.Next();
            }

            if (!SingleValue())
            {
                return false;
            }
            _tokenList.Next();

            if (_tokenList.CurrentToken.Lexemma != TerminalWords.L_OP)
            {
                return false;
            }
            _tokenList.Next();

            if (!SingleValue())
            {
                return false;
            }
            return true;
        }

        private bool Body()
        {
            if (_tokenList.CurrentToken.Lexemma != TerminalWords.L_CB)
            {
                return false;
            }
            _tokenList.Next();
            _tokenList.Commit();

            if (!Expr())
            {
                return false;
            }
            _tokenList.Commit();

            while (Expr())
            {
                _tokenList.Commit();
            }

            if (_tokenList.CurrentToken.Lexemma != TerminalWords.R_CB)
            {
                return false;
            }

            return true;
        }

        private bool Assigment()
        {
            if (_tokenList.CurrentToken?.Lexemma != TerminalWords.VAR)
            {
                return false;
            }
            _tokenList.Next();

            if (_tokenList.CurrentToken.Lexemma != TerminalWords.A_OP)
            {
                return false;
            }
            _tokenList.Next();

            if (!AssigmentStm())
            {
                return false;
            }
            _polisCreator.CreateAsigmentPolis();
            _tokenList.Next();

            return _tokenList.CurrentToken.Lexemma == TerminalWords.EOL;
        }

        private bool AssigmentStm()
        {
            if (FunctionCall())
            {
                return true;
            }
            else
            {
                _tokenList.Rollback();
                // Костыль
                _tokenList.Next();
                _tokenList.Next();
            }

            if (NewObject())
            {
                return true;
            }
            else
            {
                _tokenList.Rollback();
                // Костыль
                _tokenList.Next();
                _tokenList.Next();
            }

            if (MethodCall())
            {
                return true;
            }
            else
            {
                _tokenList.Rollback();
                // Костыль
                _tokenList.Next();
                _tokenList.Next();
            }

            if (ValueStm())
            {
                return true;
            }

            return false;
        }

        private bool FunctionCall()
        {
            if (_tokenList.CurrentToken.Lexemma != TerminalWords.VAR)
            {
                return false;
            }
            _tokenList.Next();

            if (_tokenList.CurrentToken.Lexemma != TerminalWords.L_B)
            {
                return false;
            }
            _tokenList.Next();

            if (!SingleValue())
            {
                return false;
            }
            _tokenList.Next();

            if (_tokenList.CurrentToken.Lexemma != TerminalWords.R_B)
            {
                return false;
            }

            return true;
        }

        private bool MethodCall()
        {
            if (_tokenList.CurrentToken.Lexemma != TerminalWords.VAR)
            {
                return false;
            }
            _tokenList.Next();

            if (_tokenList.CurrentToken.Lexemma != TerminalWords.K_DOT)
            {
                return false;
            }
            _tokenList.Next();

            if (_tokenList.CurrentToken.Lexemma != TerminalWords.VAR)
            {
                return false;
            }
            _tokenList.Next();

            if (_tokenList.CurrentToken.Lexemma != TerminalWords.L_B)
            {
                return false;
            }
            _tokenList.Next();

            if (!SingleValue())
            {
                return false;
            }
            _tokenList.Next();

            if (_tokenList.CurrentToken.Lexemma != TerminalWords.R_B)
            {
                return false;
            }

            return true;
        }

        private bool NewObject()
        {
            if (_tokenList.CurrentToken.Lexemma != TerminalWords.K_NEW)
            {
                return false;
            }
            _tokenList.Next();

            if(_tokenList.CurrentToken.Lexemma != TerminalWords.VAR)
            {
                return false;
            }
            _tokenList.Next();

            if (_tokenList.CurrentToken.Lexemma != TerminalWords.L_B)
            {
                return false;
            }
            _tokenList.Next();

            if (_tokenList.CurrentToken.Lexemma != TerminalWords.R_B)
            {
                return false;
            }

            return true;
        }

        private bool ValueStm()
        {
            if (!Value())
            {
                return false;
            }
            _tokenList.Next();

            while (true)
            {
                if (_tokenList.CurrentToken?.Lexemma != TerminalWords.OP)
                {
                    _tokenList.Prev();
                    break;
                }
                _tokenList.Next();

                if (!Value())
                {
                    return false;
                }
                _tokenList.Next();
            }

            return true;
        }

        private bool Value()
        {
            if (SingleValue())
            {
                return true;
            }

            if (_tokenList.CurrentToken.Lexemma != TerminalWords.L_B)
            {
                return false;
            }
            _tokenList.Next();

            if (!ValueStm())
            {
                return false;
            }
            _tokenList.Next();

            if (_tokenList.CurrentToken.Lexemma != TerminalWords.R_B)
            {
                return false;
            }
            return true;
        }

        private bool SingleValue()
        {
            if (_tokenList.CurrentToken.Lexemma != TerminalWords.DIGIT && _tokenList.CurrentToken.Lexemma != TerminalWords.VAR)
            {
                return false;
            }
            return true;
        }
    }
}

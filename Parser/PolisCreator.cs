using BW.Common;
using System;
using System.Collections.Generic;

namespace BW.SyntaxAnalayzer
{
    internal class PolisCreator
    {
        private TokenList _tokenList;
        private List<PolisElement> _polis;
        private Dictionary<string, List<PolisElement>> _functionTable;

        private const string _tmpAddres = "ForReplace";

        public PolisCreator(TokenList tokenList, List<PolisElement> polis, Dictionary<string, List<PolisElement>> functionTable)
        {
            _tokenList = tokenList;
            _polis = polis;
            _functionTable = functionTable;
        }

        public void CreateAsigmentPolis()
        {
            var opPriority = new Dictionary<string, int>
            {
                { "(", 0},
                { ")", 0},
                { "=", 1},
                { "*", 3},
                { "/", 3},
                { "-", 2},
                { "+", 2},
                { ".", 2 },
                { "new!", 2 },
                { "function" , 2 }
            };
            CreateStatmenPolis(opPriority);
        }

        public void CreateLogicPolis()
        {
            var opPriority = new Dictionary<string, int>
            {
                { "(", 0},
                { ")", 0},
                { "==", 1},
                { "!=", 1},
                { "<", 1},
                { ">", 1},
                { ".", 1 }
            };

            CreateStatmenPolis(opPriority);
        }

        private void CreateStatmenPolis(Dictionary<string, int> opPriority)
        {
            var opStack = new Stack<Token>();
            for (int i = _tokenList.SavedTokenID; i <= _tokenList.CurrentTokenID; i++)
            {
                var token = _tokenList[i];

                if (token == null)
                {
                    break;
                }

                // Обработка потока (костыль)
                if (token.Lexemma == TerminalWords.K_THREAD)
                {
                    i += 3;
                    continue;
                }

                var tokenValue = token.Value.Contains(":") && token.Lexemma == TerminalWords.VAR ? token.Value.Split(':')[0] : token.Value;
                if (_functionTable.ContainsKey(tokenValue))
                {
                    tokenValue = "function";
                }

                // Если операнд
                if (!opPriority.ContainsKey(tokenValue))
                {
                    _polis.Add(new PolisElement(token));
                }
                else
                {
                    // Если оператор открывающая скобка
                    if (token.Value == "(")
                    {
                        opStack.Push(token);
                    }
                    else
                    {
                        // Если оператор закрывающая скобка
                        if (token.Value == ")")
                        {
                            // Перенести все элементы из стека, которые не являются закрывающей скобкой в выходную последовательность
                            while (opStack.Count != 0 && opStack.Peek().Value != "(")
                            {
                                _polis.Add(new PolisElement(opStack.Pop()));
                            }
                            // Убрать открывающую скобку из стека
                            opStack.Pop();
                        }
                        else
                        {
                            // Оставшиеся операторы
                            while (opStack.Count != 0 && opPriority[tokenValue] <= opPriority[opStack.Peek().Value])
                            {
                                _polis.Add(new PolisElement(opStack.Pop()));
                            }
                            opStack.Push(token);
                        }
                    }
                }
            }

            // Перенести все элемент из стека в входную последовательность.
            while (opStack.Count != 0)
            {
                _polis.Add(new PolisElement(opStack.Pop()));
            }
        }

        internal void CreatePrintPolis()
        {
            _polis.Add(new PolisElement(PolisElementType.Print, "print"));
        }

        public void AddNegativeGo(int? goAddress = null)
        {
            _polis.Add(new PolisElement(PolisElementType.NegativeGo, "!"));
            AddGoAddres(goAddress);
        }

        public void AddDirectGo(int? goAddress = null)
        {
            _polis.Add(new PolisElement(PolisElementType.DirectGo, "#"));
            AddGoAddres(goAddress);
        }

        internal void AddArgPolis(string argName)
        {
            _polis.Add(new PolisElement(PolisElementType.Var, argName));
            _polis.Add(new PolisElement(PolisElementType.FunctionArg, ""));
            _polis.Add(new PolisElement(PolisElementType.Assigment, "="));
        }

        public void AddGoAddres(int? goAddress)
        {
            if (goAddress != null)
            {
                _polis.Add(new PolisElement(PolisElementType.GoAddres, goAddress.ToString()));
            }
            else
            {
                _polis.Add(new PolisElement(PolisElementType.GoAddres, _tmpAddres));
            }
        }

        public void ReplaceTmpAddress(int offset = 0)
        {
            for (int i = _polis.Count - 1; i > 0; i--)
            {
                if (_polis[i].Type == PolisElementType.GoAddres && _polis[i].Value == _tmpAddres)
                {
                    _polis[i].Value = (_polis.Count + offset).ToString();
                    return;
                }
            }
        }


        public void AddFunctionStart(string name)
        {
            _polis.Add(new PolisElement(PolisElementType.FunctionStart, name));
        }

        public void CreateFunctionPolis()
        {
            var startIndex = _polis.FindIndex(x => x.Type == PolisElementType.FunctionStart);
            var functionName = _polis[startIndex].Value;

            var f_polis = new List<PolisElement>();
            for (var i = startIndex + 1; i < _polis.Count; i++)
            {
                f_polis.Add(_polis[i]);
            }

            _functionTable.Add(functionName, f_polis);
            _polis.RemoveRange(startIndex, _polis.Count - startIndex);  
        }
    }
}
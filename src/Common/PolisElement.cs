using System;

namespace BW.Common
{
    public class PolisElement
    {
        public PolisElementType Type { get; set; }

        public string Value { get; set; }

        public PolisElement(Token token)
        {
            Type = GetPolisType(token.Lexemma);
            Value = token.Value;
        }

        public PolisElement(PolisElementType type, string value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return ToString(true);
        }

        public string ToString(bool returnType)
        {
            return returnType ? $"{Type}:{Value}" : Value;
        }

        private PolisElementType GetPolisType(TerminalWords lexemma)
        {
            PolisElementType type;

            switch (lexemma)
            {
                case TerminalWords.VAR:
                    type = PolisElementType.Var;
                    break;
                case TerminalWords.OP:
                    type = PolisElementType.ArethmOp;
                    break;
                case TerminalWords.A_OP:
                    type = PolisElementType.Assigment;
                    break;
                case TerminalWords.L_OP:
                    type = PolisElementType.LogicOp;
                    break;
                case TerminalWords.DIGIT:
                    type = PolisElementType.Double;
                    break;
                case TerminalWords.K_NEW:
                    type = PolisElementType.OpNew;
                    break;
                case TerminalWords.K_DOT:
                    type = PolisElementType.MethodCall;
                    break;
                default:
                    throw new NotImplementedException();
            }
            return type;
        }


    }
}
